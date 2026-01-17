using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Threading.Tasks;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service implementation for checking and installing application updates from GitHub Releases.
/// </summary>
public class UpdateService : IUpdateService
{
    private const string GitHubRepoOwner = "eddiethedean";
    private const string GitHubRepoName = "SlideForge";
    private const string GitHubReleasesApi = $"https://api.github.com/repos/{GitHubRepoOwner}/{GitHubRepoName}/releases/latest";
    
    private readonly HttpClient _httpClient;

    public string CurrentVersion { get; }

    public UpdateService() : this(new HttpClient())
    {
    }

    // Constructor for testing - allows injection of HttpClient
    public UpdateService(HttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "SlideForge-Updater");
        
        // Get version from assembly or default to 0.5.0
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        CurrentVersion = version != null ? $"{version.Major}.{version.Minor}.{version.Build}" : "0.5.0";
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        try
        {
            var response = await _httpClient.GetStringAsync(GitHubReleasesApi);
            var release = JsonSerializer.Deserialize<GitHubRelease>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (release == null || string.IsNullOrEmpty(release.TagName))
                return null;

            // Remove 'v' prefix from tag if present
            var latestVersion = release.TagName.TrimStart('v');
            
            if (!IsNewerVersion(latestVersion, CurrentVersion))
                return null;

            // Find the download URL for the current platform
            var downloadUrl = GetDownloadUrlForCurrentPlatform(release.Assets);
            if (string.IsNullOrEmpty(downloadUrl))
                return null;

            var asset = release.Assets?.FirstOrDefault(a => a.BrowserDownloadUrl == downloadUrl);
            
            return new UpdateInfo
            {
                Version = latestVersion,
                ReleaseNotes = release.Body ?? string.Empty,
                DownloadUrl = downloadUrl,
                FileSize = asset?.Size ?? 0,
                PublishedAt = release.PublishedAt
            };
        }
        catch (Exception)
        {
            // Silently fail - network issues shouldn't break the app
            return null;
        }
    }

    public async Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, Action<double>? progressCallback = null)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), "SlideForgeUpdate");
        Directory.CreateDirectory(tempDir);
        
        var fileName = Path.GetFileName(new Uri(updateInfo.DownloadUrl).LocalPath);
        var filePath = Path.Combine(tempDir, fileName);

        using var response = await _httpClient.GetAsync(updateInfo.DownloadUrl, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();

        var totalBytes = response.Content.Headers.ContentLength ?? 0;
        var downloadedBytes = 0L;

        await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var contentStream = await response.Content.ReadAsStreamAsync();
        
        var buffer = new byte[8192];
        int bytesRead;

        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            await fileStream.WriteAsync(buffer, 0, bytesRead);
            downloadedBytes += bytesRead;
            
            if (totalBytes > 0 && progressCallback != null)
            {
                var progress = (double)downloadedBytes / totalBytes;
                progressCallback(progress);
            }
        }

        return filePath;
    }

    public async Task InstallAndRestartAsync(string updateFilePath)
    {
        var appDirectory = AppContext.BaseDirectory;
        var updateScript = CreateUpdateScript(updateFilePath, appDirectory);
        
        // Execute the update script (platform-specific)
        var processInfo = new ProcessStartInfo
        {
            FileName = updateScript,
            UseShellExecute = true,
            CreateNoWindow = false
        };

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            processInfo.FileName = "cmd.exe";
            processInfo.Arguments = $"/c \"{updateScript}\"";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            processInfo.FileName = "/bin/bash";
            processInfo.Arguments = $"\"{updateScript}\"";
            processInfo.Arguments = $"chmod +x \"{updateScript}\" && \"{updateScript}\"";
        }

        Process.Start(processInfo);
        
        // Give the script a moment to start, then exit
        await Task.Delay(1000);
        Environment.Exit(0);
    }

    private string CreateUpdateScript(string updateFilePath, string appDirectory)
    {
        var scriptPath = Path.Combine(Path.GetTempPath(), "SlideForgeUpdate", 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "update.bat" : "update.sh");
        
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.WriteAllText(scriptPath, $@"
@echo off
timeout /t 2 /nobreak >nul
""{Path.Combine(appDirectory, "Authoring.Desktop.exe")}"" --update ""{updateFilePath}"" ""{appDirectory}""
");
        }
        else
        {
            File.WriteAllText(scriptPath, $@"#!/bin/bash
sleep 2
cd ""{appDirectory}""
if [ -f ""{updateFilePath}"" ]; then
    if [[ ""{updateFilePath}"" == *.zip ]]; then
        unzip -o ""{updateFilePath}"" -d ""{appDirectory}""
    elif [[ ""{updateFilePath}"" == *.tar.gz ]]; then
        tar -xzf ""{updateFilePath}"" -C ""{appDirectory}""
    fi
fi
""{Path.Combine(appDirectory, "Authoring.Desktop")}"" &
");
            // Make script executable
            Process.Start(new ProcessStartInfo
            {
                FileName = "chmod",
                Arguments = $"+x \"{scriptPath}\"",
                UseShellExecute = false
            })?.WaitForExit();
        }

        return scriptPath;
    }

    private bool IsNewerVersion(string version1, string version2)
    {
        var v1Parts = version1.Split('.').Select(int.Parse).ToArray();
        var v2Parts = version2.Split('.').Select(int.Parse).ToArray();

        for (int i = 0; i < Math.Max(v1Parts.Length, v2Parts.Length); i++)
        {
            var v1Part = i < v1Parts.Length ? v1Parts[i] : 0;
            var v2Part = i < v2Parts.Length ? v2Parts[i] : 0;

            if (v1Part > v2Part) return true;
            if (v1Part < v2Part) return false;
        }

        return false;
    }

    private string? GetDownloadUrlForCurrentPlatform(GitHubAsset[]? assets)
    {
        if (assets == null || assets.Length == 0)
            return null;

        string platform;
        string extension;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            platform = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 
                ? "windows-arm64" 
                : "windows-x64";
            extension = ".zip";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            platform = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 
                ? "macos-arm64" 
                : "macos-x64";
            extension = ".zip";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            platform = RuntimeInformation.ProcessArchitecture == Architecture.Arm64 
                ? "linux-arm64" 
                : "linux-x64";
            extension = ".tar.gz";
        }
        else
        {
            return null;
        }

        var fileName = $"SlideForge-{platform}";
        return assets.FirstOrDefault(a => 
            a.BrowserDownloadUrl.Contains(fileName) && 
            a.BrowserDownloadUrl.EndsWith(extension))?.BrowserDownloadUrl;
    }

    private class GitHubRelease
    {
        [System.Text.Json.Serialization.JsonPropertyName("tag_name")]
        public string TagName { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("assets")]
        public GitHubAsset[]? Assets { get; set; }
    }

    private class GitHubAsset
    {
        [System.Text.Json.Serialization.JsonPropertyName("browser_download_url")]
        public string BrowserDownloadUrl { get; set; } = string.Empty;
        
        [System.Text.Json.Serialization.JsonPropertyName("size")]
        public long Size { get; set; }
    }
}
