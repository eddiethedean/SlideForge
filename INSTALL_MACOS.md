# macOS Installation Guide

## Quick Installation

1. **Download** the latest `SlideForge-macos-arm64.zip` (Apple Silicon) or `SlideForge-macos-x64.zip` (Intel Mac)
2. **Extract** the ZIP file (double-click in Finder)
3. **Remove Gatekeeper quarantine** (required before first launch):
   ```bash
   cd ~/Downloads  # or wherever you extracted the ZIP
   xattr -dr com.apple.quarantine SlideForge-arm64-SlideForge.app
   # For Intel Macs: xattr -dr com.apple.quarantine SlideForge-x64-SlideForge.app
   ```
4. **Drag `SlideForge.app` to Applications folder** (optional - you can run from anywhere)
5. **Launch** by double-clicking `SlideForge.app`

## Gatekeeper (Security) Note

macOS Gatekeeper blocks unsigned executables downloaded from the internet. After extracting the ZIP, you must remove the quarantine attribute once.

**Option 1 - Terminal (Recommended):**
```bash
cd /path/to/extracted/folder
# For Apple Silicon:
xattr -dr com.apple.quarantine SlideForge-arm64-SlideForge.app
# For Intel:
xattr -dr com.apple.quarantine SlideForge-x64-SlideForge.app
# Then double-click SlideForge.app
```

**Option 2 - Finder (Bypass Gatekeeper):**
1. Right-click `SlideForge.app`
2. Select "Open"
3. Click "Open" in the security dialog
4. Future launches will work normally

## Alternative: Run from Terminal

If you prefer to run from Terminal:

```bash
cd /path/to/extracted/folder
xattr -dr com.apple.quarantine SlideForge-arm64-SlideForge.app
open SlideForge-arm64-SlideForge.app
```

## Troubleshooting

**"Apple could not verify..." error:**
- This is normal for unsigned applications
- Run: `xattr -dr com.apple.quarantine SlideForge-arm64-SlideForge.app` in Terminal
- The `-dr` flags recursively remove quarantine from the entire .app bundle
- Or use the Finder method (right-click → Open → Click "Open")

**"Permission denied":**
- Run: `chmod +x Authoring.Desktop.command`
- Run: `chmod +x Authoring.Desktop`

**Want to skip Gatekeeper permanently (not recommended):**
```bash
sudo spctl --master-disable
```
This disables Gatekeeper system-wide. Use with caution.
