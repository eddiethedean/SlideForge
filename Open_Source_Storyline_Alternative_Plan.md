
# Open Source Storyline Alternative – Project Plan

## 1. Project Goals
The primary goal is to create a free, open, extensible e-learning authoring tool that replicates the core workflow of Articulate Storyline while remaining transparent and developer-friendly.

- Desktop-first visual authoring experience
- Trigger-based interaction model
- HTML/JavaScript runtime output
- SCORM-compatible exports
- Open data formats and extensible architecture

## 2. Non-Goals
- Perfect feature parity with Articulate Storyline
- Cloud-based LMS or hosting platform
- Proprietary or locked-down project formats

## 3. Target Audience
- Instructional designers avoiding expensive licenses
- Developers building data-driven or interactive training
- Organizations needing long-term content ownership
- Open-source contributors interested in authoring tools

## 4. Technology Stack
- Language: C# (.NET)
- Desktop UI: Avalonia (cross-platform) or WPF (Windows-first)
- Serialization: JSON
- Runtime: HTML, CSS, JavaScript
- Packaging: SCORM 1.2 (initial), xAPI (future)

## 5. High-Level Architecture
- **Authoring.Core** – Pure C# domain model (slides, layers, triggers, variables)
- **Authoring.Desktop** – Visual editor application
- **Authoring.Player** – Generated HTML/JS runtime
- **Authoring.Export** – SCORM and HTML exporters

## 6. MVP Feature Set
- Slide-based project structure
- Objects: text, images, buttons
- Simple timeline (start time + duration)
- Triggers: on click, on timeline start
- Variables: boolean, number, string
- Slide navigation actions

## 7. Data Model Overview
Projects are stored as JSON files describing slides, objects, timelines, triggers, and variables. The editor operates on this model, and exporters translate it into runnable output.

- Project → Slides → Layers → Objects
- Triggers evaluated at runtime
- Variables stored in a global state container

## 8. Runtime Player Responsibilities
- Load project definition
- Render slide content
- Execute timeline events
- Evaluate trigger conditions
- Apply actions and navigation

## 9. Export Strategy
- Generate static HTML, CSS, and JS assets
- Wrap output in SCORM 1.2 manifest
- Ensure LMS-compatible launch behavior
- Allow standalone web export

## 10. Open Source Strategy
- License: MIT or Apache 2.0
- Public roadmap and contribution guidelines
- Modular repositories for easier contribution
- Strong documentation and examples

## 11. Development Phases
1. Core data model and JSON schema
2. Minimal desktop editor
3. HTML/JS runtime player
4. SCORM exporter
5. UX polish and community feedback

## 12. Risks and Mitigations
- UI complexity → start minimal and iterate
- Trigger logic scope creep → constrain MVP rules
- Low adoption → focus on documentation and demos

## 13. Success Criteria
- Users can build and export interactive modules
- Generated content runs reliably in an LMS
- Community contributors can extend the platform
