# SlideForge Development Roadmap

This document outlines the development phases and milestones for SlideForge, an open-source e-learning authoring tool.

## Current Status: Phase 1 Complete ✅

**Phase 1: Core Data Model & JSON Schema** - ✅ **COMPLETE**

### ✅ Completed Work

- ✅ Project structure and solution files
- ✅ Avalonia UI framework setup
- ✅ MVVM architecture foundation
- ✅ Complete domain model implementation
- ✅ JSON serialization with polymorphic type support
- ✅ Comprehensive validation system
- ✅ Comprehensive test suite (85 tests, 97.2% line coverage)

---

## Phase 1: Core Data Model & JSON Schema ✅

**Goal**: Establish the foundational data structures that represent e-learning projects.

**Status**: ✅ **COMPLETE**

### 1.1 Domain Model (Authoring.Core) ✅
- ✅ `Project` class with metadata (name, version, author)
- ✅ `Slide` class with properties (id, title, dimensions)
- ✅ `Layer` class for slide layers
- ✅ `SlideObject` base class with polymorphic serialization
  - ✅ `TextObject` (text content, font, styling)
  - ✅ `ImageObject` (source path, dimensions, position)
  - ✅ `ButtonObject` (text, styling, enabled state)
- ✅ `Timeline` class (start time, duration)
- ✅ `Trigger` class (condition, actions)
  - ✅ Trigger types: `OnClick`, `OnTimelineStart`
- ✅ `Variable` class (name, type: bool/number/string, value)
- ✅ `Action` classes (navigate to slide, set variable, show/hide layer)

### 1.2 JSON Serialization ✅
- ✅ Implement serialization/deserialization (System.Text.Json)
- ✅ Polymorphic type support using JsonDerivedType attributes
- ✅ Custom converters for object value handling
- ✅ Unit tests for serialization round-trips

### 1.3 Validation ✅
- ✅ Data validation rules (referential integrity, required fields)
- ✅ ProjectValidator with comprehensive rule checking
- ✅ Validation tests covering all rules and edge cases

### Testing ✅
- ✅ 85 comprehensive unit tests
- ✅ 97.2% line coverage, 94.06% branch coverage
- ✅ Tests for models, serialization, validation, and integration scenarios

**Acceptance Criteria Met**: ✅ Can create a `Project` instance in code, serialize to JSON, and deserialize back without data loss.

---

## Phase 2: Minimal Desktop Editor (MVP)

**Goal**: Create a basic visual editor where users can create projects, add slides, and place objects.

### 2.1 Project Management UI
- [ ] New project dialog
- [ ] Open/Save project file dialogs
- [ ] Project settings panel
- [ ] Recent projects list

### 2.2 Slide Management
- [ ] Slide list/navigation panel
- [ ] Add/delete/duplicate slides
- [ ] Slide properties panel (title, dimensions)

### 2.3 Canvas View
- [ ] Canvas control for slide editing
- [ ] Zoom controls
- [ ] Grid/ruler guides
- [ ] Selection handling (click, drag selection box)

### 2.4 Object Creation & Editing
- [ ] Toolbar with object creation tools (text, image, button)
- [ ] Drag-to-place objects on canvas
- [ ] Object selection and property editing
- [ ] Move/resize objects (drag handles)
- [ ] Object properties panel (position, size, styling)

### 2.5 Basic Layer Support
- [ ] Layer panel/list
- [ ] Show/hide layers
- [ ] Layer reordering

### 2.6 Timeline Panel (Basic)
- [ ] Timeline view (horizontal bar)
- [ ] Set object start time and duration
- [ ] Visual timeline markers

**Acceptance Criteria**: Users can create a project, add slides, place text/images/buttons, adjust their properties, and save the project as JSON.

---

## Phase 3: Triggers & Variables System

**Goal**: Implement the trigger-based interaction model.

### 3.1 Variables Management
- [ ] Variables panel
- [ ] Create/edit/delete variables (boolean, number, string)
- [ ] Variable value initialization

### 3.2 Trigger Editor
- [ ] Trigger panel/list for selected object
- [ ] Add trigger dialog
- [ ] Trigger condition builder
  - [ ] On click (for buttons/objects)
  - [ ] On timeline start (for slides/objects)
- [ ] Action builder
  - [ ] Navigate to slide
  - [ ] Set variable
  - [ ] Show/hide layer
  - [ ] Jump to timeline point

### 3.3 Validation & Warnings
- [ ] Validate trigger references (variables, slides, layers exist)
- [ ] Warn on missing references

**Acceptance Criteria**: Users can create variables, add triggers to objects, and configure actions like navigation and variable manipulation.

---

## Phase 4: HTML/JavaScript Runtime Player

**Goal**: Generate a functional web-based player that can execute projects.

### 4.1 Player Core (Authoring.Player)
- [ ] Player HTML template
- [ ] CSS styling system (basic responsive layout)
- [ ] JavaScript runtime engine
  - [ ] Project JSON loader
  - [ ] Slide renderer
  - [ ] Object renderer (text, images, buttons)
  - [ ] Layer manager (show/hide, stacking)
  - [ ] Timeline engine (play/pause, time-based events)

### 4.2 Variable System (Runtime)
- [ ] Global variable state container
- [ ] Variable read/write operations
- [ ] Variable change events

### 4.3 Trigger System (Runtime)
- [ ] Trigger evaluator
- [ ] Event listeners (click, timeline events)
- [ ] Action executor (navigation, variable changes, layer toggles)

### 4.4 Player UI
- [ ] Slide navigation controls (prev/next)
- [ ] Play/pause button
- [ ] Progress indicator
- [ ] Basic styling/theme

### 4.5 Testing
- [ ] Test project samples
- [ ] Manual testing checklist
- [ ] Browser compatibility testing (Chrome, Firefox, Safari, Edge)

**Acceptance Criteria**: Can export a project, open the HTML in a browser, navigate slides, click buttons, and see triggers execute correctly.

---

## Phase 5: Export System

**Goal**: Generate distributable SCORM packages and standalone web exports.

### 5.1 HTML Export (Authoring.Export)
- [ ] Export project to static HTML/CSS/JS
- [ ] Asset bundling (images, fonts)
- [ ] Minification/optimization options
- [ ] Standalone web export (no SCORM wrapper)

### 5.2 SCORM 1.2 Exporter
- [ ] SCORM manifest generator (imsmanifest.xml)
- [ ] API wrapper for LMS communication
- [ ] Basic completion tracking (completed/not completed)
- [ ] SCORM package ZIP creator
- [ ] SCORM validation/testing tools

### 5.3 Export UI
- [ ] Export dialog in desktop app
- [ ] Export format selection (HTML vs SCORM)
- [ ] Export settings (output path, options)
- [ ] Export progress/status

### 5.4 Testing & Validation
- [ ] Test SCORM packages in sample LMS (Moodle, Canvas, etc.)
- [ ] Validate manifest structure
- [ ] Test completion tracking

**Acceptance Criteria**: Can export a project as a SCORM 1.2 package, upload it to an LMS, launch it successfully, and track completion.

---

## Phase 6: MVP Polish & Documentation

**Goal**: Refine the MVP experience and prepare for early adopters.

### 6.1 UX Improvements
- [ ] Keyboard shortcuts
- [ ] Undo/redo system
- [ ] Copy/paste objects between slides
- [ ] Better error messages and user feedback
- [ ] Loading states and progress indicators

### 6.2 Documentation
- [ ] User guide (getting started, basic workflows)
- [ ] Developer documentation (architecture, extending)
- [ ] Project file format documentation
- [ ] API documentation for runtime player
- [ ] Video tutorials (basic usage)

### 6.3 Sample Projects
- [ ] Example projects demonstrating features
- [ ] Template library (blank, quiz, interactive)
- [ ] Showcase gallery

### 6.4 Testing & Bug Fixes
- [ ] Comprehensive testing of all MVP features
- [ ] Bug triage and fixes
- [ ] Performance optimization

**Acceptance Criteria**: A new user can download SlideForge, follow documentation, create a simple interactive module, and export it successfully without significant issues.

---

## Phase 7: Enhanced Features (Post-MVP)

**Future enhancements beyond the MVP scope:**

### 7.1 Advanced Objects
- [ ] Shapes (rectangles, circles, polygons)
- [ ] Video player object
- [ ] Audio player object
- [ ] Interactive quiz questions (multiple choice, true/false)

### 7.2 Advanced Triggers
- [ ] Timeline events (on complete, on pause)
- [ ] Variable conditions (if variable equals X)
- [ ] Multiple conditions (AND/OR logic)
- [ ] Time-based triggers (after X seconds)

### 7.3 Advanced Timeline
- [ ] Keyframe animations
- [ ] Object animations (fade, slide, scale)
- [ ] Timeline scrubbing in editor

### 7.4 Collaboration Features
- [ ] Project sharing/export formats
- [ ] Comments/annotations
- [ ] Version history (future)

### 7.5 Advanced Export
- [ ] xAPI (Tin Can API) support
- [ ] Custom HTML templates
- [ ] Advanced SCORM features (score tracking, suspend/resume)
- [ ] Mobile-responsive output

### 7.6 Developer Features
- [ ] Plugin system
- [ ] Custom object types
- [ ] Scripting support (JavaScript in triggers)
- [ ] REST API for runtime player

---

## Contributing to the Roadmap

This roadmap is a living document. Priorities may shift based on:
- Community feedback
- Technical constraints discovered during development
- User needs and use cases

See [Contributing Guidelines](CONTRIBUTING.md) (when available) for how to get involved.

---

## Version History

- **v0.1** ✅ - Foundation and project setup
- **v0.2** ✅ - Phase 1: Core data model (COMPLETE)
- **v0.3** 🚧 - Phase 2: Basic editor UI (NEXT)
- **v1.0** 📋 - MVP complete (Phases 1-6)
