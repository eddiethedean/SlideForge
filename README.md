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
- **Authoring.Desktop** – Visual editor application (Avalonia-based) ✅ (MVP)
- **Authoring.Player** – Generated HTML/JS runtime 📋
- **Authoring.Export** – SCORM and HTML exporters 📋

**Status Legend:** ✅ Complete | 🚧 In Progress | 📋 Planned

## Technology Stack

- **Language**: C# (.NET 9.0)
- **Desktop UI**: Avalonia (cross-platform)
- **Serialization**: JSON
- **Runtime**: HTML, CSS, JavaScript
- **Packaging**: SCORM 1.2 (initial), xAPI (future)

## Getting Started

### Prerequisites

- .NET SDK 9.0 or later
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
- **Total Tests**: 152+ across 34 test files
- **Test Categories**: Unit, Integration, UI, Property-Based, Performance

See [TESTING.md](tests/TESTING.md) for comprehensive testing guidelines and best practices.

## MVP Feature Set

- Slide-based project structure
- Objects: text, images, buttons
- Simple timeline (start time + duration)
- Triggers: on click, on timeline start
- Variables: boolean, number, string
- Slide navigation actions

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
- ✅ 152+ tests across 34 test files with extensive coverage

**Phase 2 Complete ✅** - Minimal Desktop Editor (MVP) is implemented and functional.

- ✅ Project management (New, Open, Save, Save As)
- ✅ Slide management (Add, Delete, Duplicate)
- ✅ Layer management (Add, Delete, Show/Hide)
- ✅ Canvas-based visual editing
- ✅ Object creation and editing (Text, Image, Button)
- ✅ Property panels for slide and object configuration
- ✅ Basic timeline support
- ✅ Comprehensive test suite for desktop components

**Next:** Phase 3 - Triggers & Variables System UI

See the [ROADMAP.md](ROADMAP.md) for detailed progress and upcoming milestones.

## Contributing

Contributions are welcome! This project is in early development. See the [project plan](Open_Source_Storyline_Alternative_Plan.md) for more details on the roadmap and architecture.
