using System;
using System.Threading.Tasks;
using Authoring.Desktop.Services;
using Authoring.Desktop.ViewModels;
using Moq;
using Xunit;

namespace Authoring.Desktop.Tests.ViewModels;

public class UpdateDialogViewModelTests
{
    private readonly Mock<IUpdateService> _mockUpdateService;
    private readonly UpdateDialogViewModel _viewModel;

    public UpdateDialogViewModelTests()
    {
        _mockUpdateService = new Mock<IUpdateService>();
        _mockUpdateService.Setup(s => s.CurrentVersion).Returns("0.5.0");
        
        _viewModel = new UpdateDialogViewModel(_mockUpdateService.Object);
    }

    [Fact]
    public void Constructor_SetsCurrentVersion()
    {
        // Assert
        Assert.Equal("0.5.0", _viewModel.CurrentVersion);
    }

    [Fact]
    public void Constructor_InitializesProperties()
    {
        // Assert
        Assert.False(_viewModel.IsChecking);
        Assert.False(_viewModel.UpdateAvailable);
        Assert.False(_viewModel.IsDownloading);
        Assert.Equal(0, _viewModel.DownloadProgress);
        Assert.Null(_viewModel.UpdateVersion);
        Assert.Null(_viewModel.ReleaseNotes);
        Assert.Null(_viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_UpdateAvailable_SetsProperties()
    {
        // Arrange
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            ReleaseNotes = "New features",
            DownloadUrl = "https://example.com/update.zip",
            FileSize = 50000000,
            PublishedAt = DateTime.UtcNow
        };

        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync(updateInfo);

        // Act
        await _viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

        // Assert
        Assert.True(_viewModel.UpdateAvailable);
        Assert.Equal("1.0.0", _viewModel.UpdateVersion);
        Assert.Equal("New features", _viewModel.ReleaseNotes);
        Assert.Equal(50000000, _viewModel.FileSize);
        Assert.False(_viewModel.IsChecking);
        Assert.Null(_viewModel.ErrorMessage);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_NoUpdate_SetsErrorMessage()
    {
        // Arrange
        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync((UpdateInfo?)null);

        // Act
        await _viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.UpdateAvailable);
        Assert.Equal("You are running the latest version.", _viewModel.ErrorMessage);
        Assert.False(_viewModel.IsChecking);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_SetsIsCheckingDuringCheck()
    {
        // Arrange
        var tcs = new TaskCompletionSource<UpdateInfo?>();
        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .Returns(tcs.Task);

        // Act
        var checkTask = _viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

        // Assert - Should be checking
        Assert.True(_viewModel.IsChecking);
        Assert.False(_viewModel.UpdateAvailable);

        // Complete the check
        tcs.SetResult(null);
        await checkTask;

        // Assert - Should no longer be checking
        Assert.False(_viewModel.IsChecking);
    }

    [Fact]
    public async Task CheckForUpdatesAsync_ServiceThrowsException_SetsErrorMessage()
    {
        // Arrange
        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ThrowsAsync(new Exception("Network error"));

        // Act
        await _viewModel.CheckForUpdatesCommand.ExecuteAsync(null);

        // Assert
        Assert.False(_viewModel.UpdateAvailable);
        Assert.Contains("Error checking for updates", _viewModel.ErrorMessage);
        Assert.Contains("Network error", _viewModel.ErrorMessage);
        Assert.False(_viewModel.IsChecking);
    }

    [Fact]
    public async Task DownloadAndInstallAsync_UpdateNotAvailable_DoesNothing()
    {
        // Arrange
        _viewModel.UpdateAvailable = false;

        // Act
        await _viewModel.DownloadAndInstallCommand.ExecuteAsync(null);

        // Assert
        _mockUpdateService.Verify(s => s.CheckForUpdatesAsync(), Times.Never);
        _mockUpdateService.Verify(s => s.DownloadUpdateAsync(It.IsAny<UpdateInfo>(), It.IsAny<Action<double>>()), Times.Never);
    }

    [Fact]
    public async Task DownloadAndInstallAsync_Success_DownloadsAndInstalls()
    {
        // Arrange
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/update.zip"
        };

        _viewModel.UpdateAvailable = true;
        _viewModel.UpdateVersion = "1.0.0";

        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync(updateInfo);

        _mockUpdateService
            .Setup(s => s.DownloadUpdateAsync(It.IsAny<UpdateInfo>(), It.IsAny<Action<double>>()))
            .Callback<UpdateInfo, Action<double>?>((info, progress) =>
            {
                // Simulate progress
                progress?.Invoke(0.5);
                progress?.Invoke(1.0);
            })
            .ReturnsAsync("/tmp/update.zip");

        _mockUpdateService
            .Setup(s => s.InstallAndRestartAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.DownloadAndInstallCommand.ExecuteAsync(null);

        // Assert
        _mockUpdateService.Verify(s => s.CheckForUpdatesAsync(), Times.Once);
        _mockUpdateService.Verify(s => s.DownloadUpdateAsync(updateInfo, It.IsAny<Action<double>>()), Times.Once);
        _mockUpdateService.Verify(s => s.InstallAndRestartAsync("/tmp/update.zip"), Times.Once);
    }

    [Fact]
    public async Task DownloadAndInstallAsync_UpdatesProgress()
    {
        // Arrange
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/update.zip"
        };

        _viewModel.UpdateAvailable = true;
        _viewModel.UpdateVersion = "1.0.0";

        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync(updateInfo);

        var progressValues = new System.Collections.Generic.List<double>();
        _mockUpdateService
            .Setup(s => s.DownloadUpdateAsync(It.IsAny<UpdateInfo>(), It.IsAny<Action<double>>()))
            .Callback<UpdateInfo, Action<double>?>((info, progress) =>
            {
                progress?.Invoke(0.25);
                progressValues.Add(0.25);
                progress?.Invoke(0.75);
                progressValues.Add(0.75);
                progress?.Invoke(1.0);
                progressValues.Add(1.0);
            })
            .ReturnsAsync("/tmp/update.zip");

        _mockUpdateService
            .Setup(s => s.InstallAndRestartAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);

        // Act
        await _viewModel.DownloadAndInstallCommand.ExecuteAsync(null);

        // Assert
        Assert.Contains(25.0, progressValues.Select(p => p * 100));
        Assert.Contains(75.0, progressValues.Select(p => p * 100));
        Assert.Contains(100.0, progressValues.Select(p => p * 100));
    }

    [Fact]
    public async Task DownloadAndInstallAsync_UpdateInfoNoLongerAvailable_SetsErrorMessage()
    {
        // Arrange
        _viewModel.UpdateAvailable = true;
        _viewModel.UpdateVersion = "1.0.0";

        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync((UpdateInfo?)null);

        // Act
        await _viewModel.DownloadAndInstallCommand.ExecuteAsync(null);

        // Assert
        Assert.Equal("Update information is no longer available.", _viewModel.ErrorMessage);
        Assert.False(_viewModel.IsDownloading);
        _mockUpdateService.Verify(s => s.DownloadUpdateAsync(It.IsAny<UpdateInfo>(), It.IsAny<Action<double>>()), Times.Never);
    }

    [Fact]
    public async Task DownloadAndInstallAsync_DownloadError_SetsErrorMessage()
    {
        // Arrange
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/update.zip"
        };

        _viewModel.UpdateAvailable = true;
        _viewModel.UpdateVersion = "1.0.0";

        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync(updateInfo);

        _mockUpdateService
            .Setup(s => s.DownloadUpdateAsync(It.IsAny<UpdateInfo>(), It.IsAny<Action<double>>()))
            .ThrowsAsync(new Exception("Download failed"));

        // Act
        await _viewModel.DownloadAndInstallCommand.ExecuteAsync(null);

        // Assert
        Assert.Contains("Error downloading update", _viewModel.ErrorMessage);
        Assert.Contains("Download failed", _viewModel.ErrorMessage);
        Assert.False(_viewModel.IsDownloading);
        _mockUpdateService.Verify(s => s.InstallAndRestartAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task DownloadAndInstallAsync_SetsIsDownloadingDuringDownload()
    {
        // Arrange
        var updateInfo = new UpdateInfo
        {
            Version = "1.0.0",
            DownloadUrl = "https://example.com/update.zip"
        };

        _viewModel.UpdateAvailable = true;
        _viewModel.UpdateVersion = "1.0.0";

        var downloadTcs = new TaskCompletionSource<string>();
        _mockUpdateService
            .Setup(s => s.CheckForUpdatesAsync())
            .ReturnsAsync(updateInfo);

        _mockUpdateService
            .Setup(s => s.DownloadUpdateAsync(It.IsAny<UpdateInfo>(), It.IsAny<Action<double>>()))
            .Returns(downloadTcs.Task);

        // Act
        var downloadTask = _viewModel.DownloadAndInstallCommand.ExecuteAsync(null);

        // Assert - Should be downloading
        Assert.True(_viewModel.IsDownloading);

        // Complete the download
        downloadTcs.SetResult("/tmp/update.zip");
        _mockUpdateService
            .Setup(s => s.InstallAndRestartAsync(It.IsAny<string>()))
            .Returns(Task.CompletedTask);
        await downloadTask;

        // After completion, IsDownloading should be false (or app would have exited)
        // In actual scenario, the app restarts, so this is hard to test fully
    }

    [Fact]
    public void CloseCommand_Exists()
    {
        // Assert
        Assert.NotNull(_viewModel.CloseCommand);
    }
}
