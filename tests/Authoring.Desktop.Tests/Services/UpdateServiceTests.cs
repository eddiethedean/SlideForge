using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Authoring.Desktop.Services;
using Moq;
using Moq.Protected;
using Xunit;

namespace Authoring.Desktop.Tests.Services;

public class UpdateServiceTests : IDisposable
{
    private readonly Mock<HttpMessageHandler> _mockHttpHandler;
    private readonly HttpClient _httpClient;
    private readonly UpdateService _updateService;
    private readonly string _tempDirectory;

    public UpdateServiceTests()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object);
        _updateService = new UpdateService(_httpClient);
        _tempDirectory = Path.Combine(Path.GetTempPath(), $"SlideForgeUpdateTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDirectory);
    }

    public void Dispose()
    {
        _httpClient?.Dispose();
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, true);
        }
    }

    [Fact]
    public void CurrentVersion_ReturnsValidVersion()
    {
        // Act
        var version = _updateService.CurrentVersion;

        // Assert
        Assert.NotNull(version);
        Assert.NotEmpty(version);
        // Version should be in format like "0.5.1" or "0.5.0"
        Assert.Matches(@"^\d+\.\d+\.\d+$", version);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_NewerVersionAvailable_ReturnsUpdateInfo()
    {
        // Arrange
        var currentVersion = _updateService.CurrentVersion;
        var newerVersion = IncrementVersion(currentVersion);
        var releaseJson = CreateGitHubReleaseJson(newerVersion, "Test release notes");

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(releaseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        // Assert
        Assert.NotNull(updateInfo);
        Assert.Equal(newerVersion, updateInfo.Version);
        Assert.Equal("Test release notes", updateInfo.ReleaseNotes);
        Assert.Contains("SlideForge-", updateInfo.DownloadUrl);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_NoNewerVersion_ReturnsNull()
    {
        // Arrange
        var currentVersion = _updateService.CurrentVersion;
        var olderVersion = DecrementVersion(currentVersion);
        var releaseJson = CreateGitHubReleaseJson(olderVersion, "Old release");

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(releaseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        // Assert
        Assert.Null(updateInfo);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_NetworkError_ReturnsNull()
    {
        // Arrange
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("Network error"));

        // Act
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        // Assert
        Assert.Null(updateInfo);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_InvalidJson_ReturnsNull()
    {
        // Arrange
        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
            });

        // Act
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        // Assert
        Assert.Null(updateInfo);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_NoAssets_ReturnsNull()
    {
        // Arrange
        var releaseJson = CreateGitHubReleaseJsonWithoutAssets("1.0.0");

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(releaseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        // Assert
        Assert.Null(updateInfo);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_TagWithVPrefix_StripsPrefix()
    {
        // Arrange
        var newerVersion = IncrementVersion(_updateService.CurrentVersion);
        var releaseJson = CreateGitHubReleaseJson($"v{newerVersion}", "Release with v prefix");

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(releaseJson, Encoding.UTF8, "application/json")
            });

        // Act
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        // Assert
        Assert.NotNull(updateInfo);
        Assert.Equal(newerVersion, updateInfo.Version);
        Assert.DoesNotContain("v", updateInfo.Version);
    }

    [Fact]
    public async Task DownloadUpdateAsync_ValidUrl_DownloadsFile()
    {
        // Arrange
        var testContent = "test zip content";
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/SlideForge-test.zip"
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(testContent)
            });

        // Act
        var filePath = await _updateService.DownloadUpdateAsync(updateInfo);

        // Assert
        Assert.NotNull(filePath);
        Assert.True(File.Exists(filePath));
        var content = await File.ReadAllTextAsync(filePath);
        Assert.Equal(testContent, content);
    }

    [Fact]
    public async Task DownloadUpdateAsync_CallsProgressCallback()
    {
        // Arrange
        var testContent = "test content";
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/SlideForge-test.zip"
        };

        var progressValues = new System.Collections.Generic.List<double>();

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(Encoding.UTF8.GetBytes(testContent))
                {
                    Headers = { { "Content-Length", testContent.Length.ToString() } }
                }
            });

        // Act
        await _updateService.DownloadUpdateAsync(updateInfo, progress =>
        {
            progressValues.Add(progress);
        });

        // Assert
        Assert.NotEmpty(progressValues);
        Assert.Contains(1.0, progressValues);
    }

    [Fact]
    public async Task DownloadUpdateAsync_HttpError_ThrowsException()
    {
        // Arrange
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/SlideForge-test.zip"
        };

        _mockHttpHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => 
            _updateService.DownloadUpdateAsync(updateInfo));
    }

    [Theory]
    [InlineData("0.5.0", "0.5.1", true)]
    [InlineData("0.5.0", "0.6.0", true)]
    [InlineData("0.5.0", "1.0.0", true)]
    [InlineData("0.5.1", "0.5.0", false)]
    [InlineData("1.0.0", "0.5.0", false)]
    [InlineData("1.0.0", "1.0.0", false)]
    [InlineData("0.5.0", "0.5.0", false)]
    public void IsNewerVersion_VersionComparison_ReturnsCorrectResult(
        string currentVersion, string checkVersion, bool expected)
    {
        // Arrange
        var currentVersionParts = currentVersion.Split('.').Select(int.Parse).ToArray();
        var checkVersionParts = checkVersion.Split('.').Select(int.Parse).ToArray();

        // Act
        bool result = IsNewerVersion(checkVersionParts, currentVersionParts);

        // Assert
        Assert.Equal(expected, result);
    }

    private bool IsNewerVersion(int[] version1, int[] version2)
    {
        for (int i = 0; i < Math.Max(version1.Length, version2.Length); i++)
        {
            var v1Part = i < version1.Length ? version1[i] : 0;
            var v2Part = i < version2.Length ? version2[i] : 0;

            if (v1Part > v2Part) return true;
            if (v1Part < v2Part) return false;
        }
        return false;
    }

    private string CreateGitHubReleaseJson(string version, string body)
    {
        var tagName = version.StartsWith("v") ? version : $"v{version}";
        var platform = GetCurrentPlatformAsset();
        
        return $$"""
        {
          "tag_name": "{{tagName}}",
          "body": "{{body}}",
          "published_at": "2024-01-01T00:00:00Z",
          "assets": [
            {
              "browser_download_url": "https://github.com/eddiethedean/SlideForge/releases/download/{{tagName}}/SlideForge-{{platform}}-v{{version}}.{{GetCurrentPlatformExtension()}}",
              "size": 50000000
            }
          ]
        }
        """;
    }

    private string CreateGitHubReleaseJsonWithoutAssets(string version)
    {
        var tagName = $"v{version}";
        return $$"""
        {
          "tag_name": "{{tagName}}",
          "body": "Release without assets",
          "published_at": "2024-01-01T00:00:00Z",
          "assets": []
        }
        """;
    }

    private string IncrementVersion(string version)
    {
        var parts = version.Split('.').Select(int.Parse).ToArray();
        parts[2]++; // Increment patch version
        return string.Join(".", parts);
    }

    private string DecrementVersion(string version)
    {
        var parts = version.Split('.').Select(int.Parse).ToArray();
        if (parts[2] > 0)
        {
            parts[2]--; // Decrement patch version
        }
        else if (parts[1] > 0)
        {
            parts[1]--;
            parts[2] = 99; // Set to a reasonable default
        }
        else if (parts[0] > 0)
        {
            parts[0]--;
            parts[1] = 99;
            parts[2] = 99;
        }
        return string.Join(".", parts);
    }

    private string GetCurrentPlatformAsset()
    {
        var arch = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
        var isArm64 = arch == System.Runtime.InteropServices.Architecture.Arm64;
        
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return isArm64 ? "windows-arm64" : "windows-x64";
        }
        else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.OSX))
        {
            return isArm64 ? "macos-arm64" : "macos-x64";
        }
        else
        {
            return isArm64 ? "linux-arm64" : "linux-x64";
        }
    }

    private string GetCurrentPlatformExtension()
    {
        if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.Windows))
        {
            return "zip";
        }
        else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(
            System.Runtime.InteropServices.OSPlatform.OSX))
        {
            return "zip";
        }
        else
        {
            return "tar.gz";
        }
    }
}
