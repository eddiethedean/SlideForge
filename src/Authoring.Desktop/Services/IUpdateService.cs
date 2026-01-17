using System;
using System.Threading.Tasks;

namespace Authoring.Desktop.Services;

/// <summary>
/// Service for checking and installing application updates from GitHub Releases.
/// </summary>
public interface IUpdateService
{
    /// <summary>
    /// Gets the current application version.
    /// </summary>
    string CurrentVersion { get; }

    /// <summary>
    /// Checks for available updates from GitHub Releases.
    /// </summary>
    /// <returns>Update information if a newer version is available, null otherwise.</returns>
    Task<UpdateInfo?> CheckForUpdatesAsync();

    /// <summary>
    /// Downloads the update package for the current platform.
    /// </summary>
    /// <param name="updateInfo">The update information.</param>
    /// <param name="progressCallback">Optional callback for download progress (0.0 to 1.0).</param>
    /// <returns>The path to the downloaded update file.</returns>
    Task<string> DownloadUpdateAsync(UpdateInfo updateInfo, Action<double>? progressCallback = null);

    /// <summary>
    /// Installs the downloaded update and restarts the application.
    /// </summary>
    /// <param name="updateFilePath">Path to the downloaded update file.</param>
    Task InstallAndRestartAsync(string updateFilePath);
}

/// <summary>
/// Information about an available update.
/// </summary>
public class UpdateInfo
{
    public string Version { get; set; } = string.Empty;
    public string ReleaseNotes { get; set; } = string.Empty;
    public string DownloadUrl { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime PublishedAt { get; set; }
}
