# macOS Installation Guide

## Quick Installation

1. **Download** the latest `SlideForge-macos-arm64.zip` (Apple Silicon) or `SlideForge-macos-x64.zip` (Intel Mac)
2. **Extract** the ZIP file (double-click in Finder)
3. **Drag `SlideForge.app` to Applications folder** (optional - you can run from anywhere)
4. **Launch** by double-clicking `SlideForge.app`
   - The app will automatically remove Gatekeeper quarantine on first run
   - If macOS blocks it, right-click `SlideForge.app` → Open → Click "Open"

## Gatekeeper (Security) Note

macOS Gatekeeper blocks unsigned executables downloaded from the internet. After extracting the ZIP, you must remove the quarantine attribute once.

**Option 1 - Terminal (Recommended):**
```bash
cd /path/to/extracted/folder
xattr -d com.apple.quarantine Authoring.Desktop.command
```

**Option 2 - Finder:**
1. Right-click `Authoring.Desktop.command`
2. Select "Open"
3. Click "Open" in the security dialog
4. Future launches will work normally

## Alternative: Run from Terminal

If you prefer to run from Terminal:

```bash
cd /path/to/extracted/folder
xattr -d com.apple.quarantine Authoring.Desktop.command
./Authoring.Desktop
```

## Troubleshooting

**"Apple could not verify..." error:**
- This is normal for unsigned applications
- Run: `xattr -d com.apple.quarantine Authoring.Desktop.command` in Terminal
- Or use the Finder method (right-click → Open)

**"Permission denied":**
- Run: `chmod +x Authoring.Desktop.command`
- Run: `chmod +x Authoring.Desktop`

**Want to skip Gatekeeper permanently (not recommended):**
```bash
sudo spctl --master-disable
```
This disables Gatekeeper system-wide. Use with caution.
