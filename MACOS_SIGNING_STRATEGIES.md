# macOS Trust & Code Signing Strategies

This document outlines different strategies to get macOS to trust SlideForge and eliminate Gatekeeper warnings.

## Strategy Comparison

| Strategy | Cost | Effort | Gatekeeper Warnings | User Experience |
|----------|------|--------|---------------------|-----------------|
| **Current (.command wrapper)** | Free | Low | ⚠️ Shows warnings | ✅ Works after manual step |
| **Proper .app Bundle** | Free | Medium | ⚠️ Still shows warnings | ✅✅ Better UX |
| **Code Signing (Developer ID)** | $99/year | Medium | ✅✅ No warnings | ✅✅✅ Best UX |
| **Code Signing + Notarization** | $99/year | High | ✅✅✅ No warnings + Apple verified | ✅✅✅ Best UX |

---

## Option 1: Create Proper .app Bundle (Free, Recommended for Now)

Even without code signing, creating a proper `.app` bundle improves user experience and can be signed later.

### Benefits:
- Better macOS integration (appears in Applications folder properly)
- Shows up in Dock correctly
- Can be signed later without repackaging
- Users can drag to Applications folder

### Implementation:
Create a proper macOS `.app` bundle structure during CI/CD build.

**Structure:**
```
SlideForge.app/
├── Contents/
│   ├── Info.plist
│   ├── MacOS/
│   │   └── Authoring.Desktop (executable)
│   └── Resources/
│       └── (resources, icons, etc.)
```

---

## Option 2: Code Signing with Developer ID ($99/year)

This is the **proper, official way** to distribute macOS applications outside the App Store.

### Requirements:
1. **Apple Developer Account** ($99/year)
2. **Developer ID Application Certificate**
3. **App-specific password** for notarization

### Process:
1. **Sign the application:**
   ```bash
   codesign --sign "Developer ID Application: Your Name" \
            --options runtime \
            --timestamp \
            SlideForge.app
   ```

2. **Verify signing:**
   ```bash
   codesign --verify --deep --strict --verbose=2 SlideForge.app
   spctl --assess --verbose SlideForge.app
   ```

### Benefits:
- ✅ No Gatekeeper warnings
- ✅ Shows as "Identified Developer" in System Preferences
- ✅ Professional appearance
- ✅ Required for auto-updates via Sparkle

---

## Option 3: Code Signing + Notarization ($99/year)

This is the **gold standard** and fully eliminates all security warnings.

### Process:
1. **Sign** (same as Option 2)
2. **Notarize** with Apple:
   ```bash
   xcrun notarytool submit SlideForge.app \
     --apple-id "your@email.com" \
     --team-id "YOUR_TEAM_ID" \
     --password "app-specific-password" \
     --wait
   ```

3. **Staple** the notarization ticket:
   ```bash
   xcrun stapler staple SlideForge.app
   ```

### Benefits:
- ✅✅✅ Zero security warnings
- ✅✅✅ Apple-verified (shows verified checkmark)
- ✅✅✅ Most trusted by users
- ✅✅✅ Required for Sparkle auto-updates

---

## Recommended Approach (Hybrid)

### Phase 1: Now (Free)
1. Create proper `.app` bundle structure
2. Improve UX (drag to Applications, proper icon, etc.)
3. Document code signing for future

### Phase 2: Later (If Budget Allows)
1. Get Apple Developer account ($99/year)
2. Set up code signing in CI/CD
3. Add notarization
4. Update auto-update system to use Sparkle

---

## Implementation Steps

### Step 1: Create .app Bundle (Can Do Now)

We can modify the CI/CD workflow to create a proper `.app` bundle:

1. Create `Info.plist` with app metadata
2. Structure files in `.app/Contents/MacOS/` format
3. Add app icon (optional)
4. Package as `.app` instead of raw executable

This improves UX immediately, even without signing.

### Step 2: Add Code Signing (When Ready)

1. Add secrets to GitHub Actions:
   - `APPLE_CERTIFICATE_BASE64` - Base64 encoded certificate
   - `APPLE_CERTIFICATE_PASSWORD` - Certificate password
   - `APPLE_ID` - Apple ID email
   - `APPLE_TEAM_ID` - Team ID
   - `APPLE_APP_SPECIFIC_PASSWORD` - App-specific password

2. Update workflow to:
   - Import certificate
   - Sign the `.app` bundle
   - Notarize (optional but recommended)
   - Staple (if notarized)

---

## Open Source Considerations

Many open-source projects use one of these approaches:

1. **Document the manual bypass** (current approach)
   - Pros: Free, immediate
   - Cons: User friction

2. **Accept Apple Developer cost**
   - Pros: Best UX, professional
   - Cons: Annual cost

3. **Community-funded signing**
   - Pros: Free for maintainers
   - Cons: Requires funding/sponsorship

4. **Provide .app bundle without signing**
   - Pros: Better UX, can sign later
   - Cons: Still shows warnings

---

## Next Steps

I recommend:

1. **Short-term**: Create proper `.app` bundle structure (improves UX immediately)
2. **Document**: Code signing process for contributors/users who want to self-sign
3. **Future**: Consider Apple Developer account if project grows

Would you like me to:
- ✅ Implement the `.app` bundle creation?
- ✅ Set up code signing workflow (requires Apple Developer account)?
- ✅ Both?
