# SlideForge Player Testing Guide

This document provides a comprehensive testing checklist for the SlideForge HTML/JavaScript Runtime Player.

## Quick Start

1. Start a local web server in the `src/Authoring.Player/www/` directory
2. Place a test project JSON file in the same directory (or adjust path)
3. Open the player in a browser: `index.html?project=simple-project.json`

### Using Python HTTP Server (recommended)

```bash
cd src/Authoring.Player/www
python3 -m http.server 8000
```

Then open: `http://localhost:8000/index.html?project=../tests/TestData/PlayerProjects/simple-project.json`

### Using Node.js HTTP Server

```bash
cd src/Authoring.Player/www
npx http-server -p 8000
```

## Test Projects

Test projects are located in `tests/TestData/PlayerProjects/`:

1. **simple-project.json** - Basic slide with text object
2. **navigation-project.json** - Multiple slides with navigation buttons
3. **variables-project.json** - Variable manipulation (Boolean, Number, String)
4. **layers-project.json** - Layer show/hide actions
5. **timeline-project.json** - Timeline playback and OnTimelineStart triggers
6. **comprehensive-project.json** - All features combined

## Manual Testing Checklist

### Core Functionality

#### Project Loading
- [ ] Player loads and displays loading spinner
- [ ] Project JSON loads successfully
- [ ] Error message displays if project file is missing
- [ ] Error message displays if project JSON is invalid
- [ ] Error message displays if project structure is invalid

#### Slide Rendering
- [ ] Slide renders at correct dimensions (1920x1080)
- [ ] Slide viewport scales correctly on different screen sizes
- [ ] Slide title displays correctly in player controls
- [ ] Slide progress indicator shows correct current/total (e.g., "1 / 3")

#### Object Rendering
- [ ] Text objects render with correct content
- [ ] Text objects render with correct font family
- [ ] Text objects render with correct font size
- [ ] Text objects render with correct color
- [ ] Text objects render at correct position (x, y)
- [ ] Text objects render with correct dimensions (width, height)

- [ ] Image objects render from sourcePath
- [ ] Image objects respect maintainAspectRatio property
- [ ] Image objects render at correct position
- [ ] Image objects render with correct dimensions
- [ ] Missing images display as broken image icon (acceptable)

- [ ] Button objects render with correct label text
- [ ] Button objects render at correct position
- [ ] Button objects render with correct dimensions
- [ ] Enabled buttons are clickable
- [ ] Disabled buttons are not clickable and appear grayed out
- [ ] Button hover states work correctly

#### Visibility
- [ ] Objects with `visible: true` are displayed
- [ ] Objects with `visible: false` are hidden
- [ ] Objects without visible property default to visible

### Navigation

#### Slide Navigation
- [ ] Previous button navigates to previous slide
- [ ] Previous button is disabled on first slide
- [ ] Next button navigates to next slide
- [ ] Next button is disabled on last slide
- [ ] Navigation updates slide content correctly
- [ ] Navigation updates slide title in controls
- [ ] Navigation updates progress indicator
- [ ] Navigation resets timeline on new slide

#### Button Navigation Actions
- [ ] OnClick trigger fires when button is clicked
- [ ] NavigateToSlide action changes current slide
- [ ] Multiple actions on same trigger execute sequentially
- [ ] Navigation works from any slide to any other slide

### Variables

#### Variable Initialization
- [ ] Variables initialize with default values from project
- [ ] Boolean variables initialize correctly (true/false)
- [ ] Number variables initialize correctly (numeric values)
- [ ] String variables initialize correctly (text values)

#### Variable Manipulation
- [ ] SetVariable action updates variable value
- [ ] Boolean variable can be set to true
- [ ] Boolean variable can be set to false
- [ ] Number variable can be set to numeric values
- [ ] String variable can be set to text values
- [ ] Multiple SetVariable actions in same trigger work correctly
- [ ] Variable changes persist across slide navigation

#### Variable Type Validation
- [ ] Setting non-numeric value to Number variable is rejected (console warning)
- [ ] Boolean conversion works correctly (truthy/falsy)
- [ ] String conversion works correctly (any value to string)

### Layers

#### Layer Visibility
- [ ] Layers with `visible: true` show their objects
- [ ] Layers with `visible: false` hide their objects
- [ ] Layer visibility applies to all objects in that layer

#### Layer Actions
- [ ] ShowLayer action makes layer visible
- [ ] HideLayer action makes layer hidden
- [ ] Layer visibility updates immediately in DOM
- [ ] Layer actions work from any trigger
- [ ] Layer actions work when triggered from timeline

### Timeline

#### Timeline Playback
- [ ] Play button starts timeline playback
- [ ] Pause button stops timeline playback
- [ ] Play/Pause button text updates correctly
- [ ] Timeline resets when navigating to new slide
- [ ] Timeline time progresses correctly during playback

#### Object Timeline Visibility
- [ ] Objects without timeline are always visible
- [ ] Objects appear at their startTime
- [ ] Objects disappear after startTime + duration
- [ ] Objects remain hidden before startTime
- [ ] Timeline visibility updates during playback

#### OnTimelineStart Triggers
- [ ] OnTimelineStart trigger fires when object becomes visible
- [ ] OnTimelineStart trigger fires only once per object visibility
- [ ] OnTimelineStart trigger fires when timeline plays
- [ ] OnTimelineStart actions execute correctly
- [ ] OnTimelineStart can trigger navigation, variables, layer actions

### Player UI Controls

#### Control Buttons
- [ ] Previous button is styled correctly
- [ ] Next button is styled correctly
- [ ] Play/Pause button is styled correctly
- [ ] Buttons show disabled state correctly
- [ ] Buttons respond to hover states
- [ ] Buttons respond to click events

#### Progress Display
- [ ] Slide title displays current slide title
- [ ] Slide progress shows "current / total" format
- [ ] Progress updates when navigating slides

#### Responsive Design
- [ ] Player scales correctly on smaller screens
- [ ] Slide viewport maintains aspect ratio
- [ ] Controls remain accessible on mobile devices

## Browser Compatibility Testing

Test the player in the following browsers:

### Chrome/Chromium
- [ ] Chrome (latest)
- [ ] Edge (latest)
- [ ] Opera (latest)

### Firefox
- [ ] Firefox (latest)

### Safari
- [ ] Safari (latest)
- [ ] Safari on iOS (if available)

### Test Points per Browser
1. Project loading
2. Object rendering (text, image, button)
3. Navigation between slides
4. Button click triggers
5. Variable manipulation
6. Layer show/hide
7. Timeline playback
8. OnTimelineStart triggers

## Edge Cases and Error Scenarios

### Missing Data
- [ ] Project with no slides (should show error or empty state)
- [ ] Slide with no layers (should render empty slide)
- [ ] Layer with no objects (should render empty layer)
- [ ] Object with missing properties (should use defaults)

### Invalid References
- [ ] NavigateToSlide action with non-existent slide ID (should be ignored or show warning)
- [ ] SetVariable action with non-existent variable ID (should show console warning)
- [ ] ShowLayer/HideLayer action with non-existent layer ID (should be ignored)

### Timeline Edge Cases
- [ ] Object with startTime = 0
- [ ] Object with very short duration (0.1 seconds)
- [ ] Object with very long duration (100 seconds)
- [ ] Multiple objects appearing at same time
- [ ] Timeline plays past all object durations

### Empty States
- [ ] Project with empty variables array
- [ ] Slide with empty objects array
- [ ] Trigger with empty actions array (should be allowed but do nothing)

## Performance Testing

- [ ] Player loads quickly (< 2 seconds for small projects)
- [ ] Slide rendering is smooth (no visible lag)
- [ ] Timeline playback is smooth (60fps or near)
- [ ] Multiple rapid button clicks don't cause issues
- [ ] Navigation between slides is instant
- [ ] Large projects (10+ slides, 100+ objects) perform acceptably

## Accessibility Testing (Future Enhancement)

These should be noted but are not critical for MVP:

- [ ] Keyboard navigation (Tab to focus buttons, Enter to activate)
- [ ] Screen reader compatibility
- [ ] ARIA labels on interactive elements
- [ ] High contrast mode support

## Automated Testing Notes

For future automated testing:

- Use headless browser (Puppeteer, Playwright)
- Test JSON loading and parsing
- Test DOM element creation
- Test event handlers
- Test variable state changes
- Test timeline calculations

## Known Limitations (MVP)

- No keyboard navigation
- No fullscreen mode
- No asset bundling (images must be accessible via URL)
- No progress saving/resume
- No analytics tracking
- Limited error recovery
- No validation of action references (silent failures)

## Reporting Issues

When reporting bugs, include:

1. Browser and version
2. Test project used
3. Steps to reproduce
4. Expected behavior
5. Actual behavior
6. Console errors (if any)
7. Screenshots (if applicable)

## Test Results Template

```
Date: [Date]
Tester: [Name]
Browser: [Browser/Version]

Core Functionality: [Pass/Fail/Notes]
Navigation: [Pass/Fail/Notes]
Variables: [Pass/Fail/Notes]
Layers: [Pass/Fail/Notes]
Timeline: [Pass/Fail/Notes]
Player UI: [Pass/Fail/Notes]

Issues Found:
- [Issue description]
- [Issue description]
```