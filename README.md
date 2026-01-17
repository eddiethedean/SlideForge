# SlideForge

An open-source, extensible e-learning authoring tool that replicates the core workflow of Articulate Storyline while remaining transparent and developer-friendly.

## Project Goals

- Desktop-first visual authoring experience
- Trigger-based interaction model
- HTML/JavaScript runtime output
- SCORM-compatible exports
- Open data formats and extensible architecture

## Architecture

The project is organized into four main components:

- **Authoring.Core** – Pure C# domain model (slides, layers, triggers, variables) ✅
- **Authoring.Desktop** – Visual editor application (Avalonia-based) ✅ (MVP with Triggers & Variables UI)
- **Authoring.Player** – HTML/JS runtime player ✅
- **Authoring.Export** – SCORM and HTML exporters 📋

**Status Legend:** ✅ Complete | 🚧 In Progress | 📋 Planned

## Technology Stack

- **Language**: C# (.NET 10.0)
- **Desktop UI**: Avalonia (cross-platform)
- **Serialization**: JSON
- **Runtime**: HTML, CSS, JavaScript
- **Packaging**: SCORM 1.2 (initial), xAPI (future)

## Getting Started

### Prerequisites

- .NET SDK 10.0 or later
- An IDE (Visual Studio, Rider, or VS Code)

### Building the Solution

```bash
dotnet build
```

### Running the Desktop Application

```bash
dotnet run --project src/Authoring.Desktop/Authoring.Desktop.csproj
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test categories
dotnet test --filter "Category=Integration"
dotnet test --filter "Category=UI"
```

Test coverage is tracked and reported using Coverlet. Current coverage:
- **Line Coverage**: 97.2%+
- **Branch Coverage**: 94%+
- **Total Tests**: 407+ across 40+ test files (.NET) + 47+ JavaScript tests (Player)
- **Test Categories**: Unit, Integration, UI, Property-Based, Performance, JavaScript Runtime

See [TESTING.md](tests/TESTING.md) for comprehensive testing guidelines and best practices.

## MVP Feature Set

- Slide-based project structure with easy slide management (toolbar, menu, panel)
- Objects: text, images, buttons
- **Interactive object dragging** - Click and drag objects to reposition on the slide
- Simple timeline (start time + duration)
- Variables management UI (create, edit, delete boolean/number/string variables)
- Triggers editor UI (add triggers to objects with on click/on timeline start)
- Actions builder (navigate to slide, set variable, show/hide layer)
- Validation warnings for broken references
- Real-time validation feedback
- Menu system with Edit, View, and Help menus
- Keyboard shortcuts support (with shortcuts reference dialog)

## Data Model

Projects are stored as JSON files describing slides, objects, timelines, triggers, and variables. The editor operates on this model, and exporters translate it into runnable output.

- Project → Slides → Layers → Objects
- Triggers evaluated at runtime
- Variables stored in a global state container

## License

This project will be licensed under MIT or Apache 2.0 (to be determined).

## Current Status

**Phase 1 Complete ✅** - Core data model, JSON serialization, and validation are implemented with comprehensive test coverage.

- ✅ Complete domain model (Project, Slide, Layer, SlideObject hierarchy, Variable, Trigger, Timeline, Actions)
- ✅ JSON serialization with polymorphic type support
- ✅ Comprehensive validation system
- ✅ 125+ core tests with extensive coverage (97%+ line coverage, 94%+ branch coverage)

**Phase 2 Complete ✅** - Minimal Desktop Editor (MVP) is implemented and functional.

- ✅ Project management (New, Open, Save, Save As)
- ✅ Slide management (Add via toolbar/menu/panel, Delete, Duplicate)
- ✅ Layer management (Add, Delete, Show/Hide)
- ✅ Canvas-based visual editing
- ✅ Object creation and editing (Text, Image, Button)
- ✅ **Interactive object dragging** with bounds checking and coordinate conversion
- ✅ Property panels for slide and object configuration
- ✅ Basic timeline support
- ✅ Menu system (File, Edit, View, Help)
- ✅ Comprehensive test suite for desktop components (282+ tests including 24 dragging tests)

**Phase 3 Complete ✅** - Triggers & Variables System UI is fully implemented.

- ✅ Variables panel UI with full CRUD operations
- ✅ Variable dialog for creating/editing variables (Boolean, Number, String types)
- ✅ Triggers section in Properties panel
- ✅ Trigger dialog for adding/editing triggers (OnClick, OnTimelineStart)
- ✅ Action dialog for configuring actions (NavigateToSlide, SetVariable, ShowLayer, HideLayer)
- ✅ Real-time validation warnings display
- ✅ Reference validation (variables, slides, layers)
- ✅ 104+ robust tests covering edge cases, validation, and complex workflows
- ✅ 407+ total tests (125 Core + 282 Desktop) across 40+ test files

**Phase 4 Complete ✅** - HTML/JavaScript Runtime Player is fully implemented and functional.

- ✅ Complete JavaScript runtime engine (ProjectLoader, VariableSystem, LayerManager, ObjectRenderer, SlideRenderer, TimelineEngine, TriggerEvaluator, ActionExecutor, PlayerController, PlayerUIController)
- ✅ HTML/CSS player interface with responsive design
- ✅ Slide navigation (prev/next buttons)
- ✅ Play/pause timeline controls
- ✅ Progress indicator
- ✅ Full support for all object types (Text, Image, Button)
- ✅ Variable system with type validation and change events
- ✅ Trigger system (OnClick, OnTimelineStart)
- ✅ Action execution (NavigateToSlide, SetVariable, ShowLayer, HideLayer)
- ✅ Timeline playback with object visibility control
- ✅ Comprehensive JavaScript test suite (47+ tests)
- ✅ Test project samples for manual testing
- ✅ Browser compatibility verified

**Recent Improvements:**
- ✅ Code refactoring: Service extraction (SlideManagementService, ObjectManagementService)
- ✅ Improved architecture: Better separation of concerns
- ✅ All warnings resolved: 0 warnings, 0 errors
- ✅ .NET 10.0 upgrade completed

**Next:** Phase 5 - Export System (SCORM and HTML exporters)

See the [ROADMAP.md](ROADMAP.md) for detailed progress and upcoming milestones.

## Contributing

Contributions are welcome! This project is in early development. See the [project plan](Open_Source_Storyline_Alternative_Plan.md) for more details on the roadmap and architecture.
