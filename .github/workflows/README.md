# GitHub Actions Workflows

This directory contains GitHub Actions workflows for automated builds and releases.

## Release Workflow

The `release.yml` workflow automatically builds and publishes SlideForge for Windows, macOS, and Linux when:

1. **Automatic Release**: A tag starting with `v` is pushed (e.g., `v0.5.0`)
2. **Manual Release**: The workflow is manually triggered from the Actions tab

### How to Create a Release

#### Option 1: Automatic Release (Recommended)

1. Create and push a tag:
   ```bash
   git tag v0.5.0
   git push origin v0.5.0
   ```

2. The workflow will automatically:
   - Build the application for Windows (x64, ARM64), macOS (x64 Intel, ARM64 Apple Silicon), and Linux (x64, ARM64)
   - Run all tests
   - Create platform-specific packages
   - Create a GitHub Release with download links

#### Option 2: Manual Release

1. Go to the **Actions** tab in GitHub
2. Select **Build and Release** workflow
3. Click **Run workflow**
4. Enter the version number (e.g., `0.5.0`)
5. Click **Run workflow**

### Release Artifacts

Each release includes builds for all architectures:

#### Windows
- **x64 (Intel/AMD)**: `SlideForge-windows-x64-vX.X.X.zip`
- **ARM64**: `SlideForge-windows-arm64-vX.X.X.zip`
  - Extract and run `Authoring.Desktop.exe`

#### macOS
- **x64 (Intel)**: `SlideForge-macos-x64-vX.X.X.zip`
- **ARM64 (Apple Silicon)**: `SlideForge-macos-arm64-vX.X.X.zip`
  - Extract the ZIP file
  - Open Terminal and navigate to the extracted folder
  - Run: `chmod +x Authoring.Desktop && ./Authoring.Desktop`
  - Note: Future releases will have execute permissions set automatically

#### Linux
- **x64 (Intel/AMD)**: `SlideForge-linux-x64-vX.X.X.tar.gz`
- **ARM64**: `SlideForge-linux-arm64-vX.X.X.tar.gz`
  - Extract and run `./Authoring.Desktop`

### Build Details

- **.NET Version**: 10.0.x
- **Build Configuration**: Release
- **Self-Contained**: Yes (includes .NET runtime)
- **Single File**: Yes (all dependencies bundled)
- **Compression**: Enabled for smaller file sizes

### Requirements

- GitHub repository with Actions enabled
- Write permissions for releases (automatically available for repository owners)
