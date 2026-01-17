using System;
using System.Threading.Tasks;
using Authoring.Desktop.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Authoring.Desktop.ViewModels;

public partial class UpdateDialogViewModel : ViewModelBase
{
    private readonly IUpdateService _updateService;

    [ObservableProperty]
    private bool isChecking;

    [ObservableProperty]
    private bool updateAvailable;

    [ObservableProperty]
    private bool isDownloading;

    [ObservableProperty]
    private double downloadProgress;

    [ObservableProperty]
    private string? updateVersion;

    [ObservableProperty]
    private string? releaseNotes;

    [ObservableProperty]
    private string? errorMessage;

    [ObservableProperty]
    private long? fileSize;

    public UpdateDialogViewModel(IUpdateService updateService)
    {
        _updateService = updateService ?? throw new ArgumentNullException(nameof(updateService));
        CurrentVersion = _updateService.CurrentVersion;
    }

    public string CurrentVersion { get; }

    [RelayCommand]
    private async Task CheckForUpdatesAsync()
    {
        IsChecking = true;
        ErrorMessage = null;
        UpdateAvailable = false;

        try
        {
            var updateInfo = await _updateService.CheckForUpdatesAsync();
            
            if (updateInfo != null)
            {
                UpdateAvailable = true;
                UpdateVersion = updateInfo.Version;
                ReleaseNotes = updateInfo.ReleaseNotes;
                FileSize = updateInfo.FileSize;
            }
            else
            {
                ErrorMessage = "You are running the latest version.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error checking for updates: {ex.Message}";
        }
        finally
        {
            IsChecking = false;
        }
    }

    [RelayCommand]
    private async Task DownloadAndInstallAsync()
    {
        if (!UpdateAvailable || string.IsNullOrEmpty(UpdateVersion))
            return;

        IsDownloading = true;
        ErrorMessage = null;
        DownloadProgress = 0;

        try
        {
            var updateInfo = await _updateService.CheckForUpdatesAsync();
            if (updateInfo == null)
            {
                ErrorMessage = "Update information is no longer available.";
                IsDownloading = false;
                return;
            }

            var filePath = await _updateService.DownloadUpdateAsync(updateInfo, progress =>
            {
                DownloadProgress = progress * 100;
            });

            await _updateService.InstallAndRestartAsync(filePath);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error downloading update: {ex.Message}";
            IsDownloading = false;
        }
    }

    [RelayCommand]
    private void Close()
    {
        // This will be handled by the dialog
    }
}
