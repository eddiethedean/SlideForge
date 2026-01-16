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
- **Authoring.Desktop** – Visual editor application (Avalonia-based) 🚧
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
dotnet test
```

Test coverage is tracked and reported using Coverlet. Current coverage:
- **Line Coverage**: 97.2%
- **Branch Coverage**: 94.06%

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
- ✅ 85 unit tests with 97.2% line coverage

**Next:** Phase 2 - Minimal Desktop Editor (MVP)

See the [ROADMAP.md](ROADMAP.md) for detailed progress and upcoming milestones.

## Contributing

Contributions are welcome! This project is in early development. See the [project plan](Open_Source_Storyline_Alternative_Plan.md) for more details on the roadmap and architecture.
