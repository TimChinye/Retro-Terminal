# Retro Terminal - C# Architecture Showcase

https://github.com/user-attachments/assets/b69bbe53-a96f-4cfc-9d39-3f69022fa51d

![Project Logo](https://raw.githubusercontent.com/TimChinye/Retro-Terminal/refs/heads/main/RetroTerminal%20Logo.png) 

This project is a complete re-engineering of a first-year university assignment. The original task was to create a set of simple algorithms (Trinary conversion, ISBN validation, School Roster) wrapped in a console menu.

While the original submission received a high grade for its creativity (simulating a retro PC monitor), the code was monolithic and difficult to maintain. **This repository represents a professional refactoring of that project.**

The goal was a strict technical challenge: **preserve the exact, unique user experience (1:1 behavior parity) while completely replacing the underlying architecture with industry-standard patterns.**

## The Challenge

The original assignment required:
1.  **Menu System:** Iterative selection without recursion.
2.  **Trinary Converter:** Convert Base-3 strings to Base-10.
3.  **School Roster:** An in-memory database of students.
4.  **ISBN Verifier:** Logic to validate ISBN-10 checksums.

**The Constraints for Refactoring:**
*   The application must retain the "Atari SM124" ASCII art aesthetic.
*   The "School Roster" must accept the specific conversational flow (Chatbot style), not a standard CLI interface.
*   Animations (boot-up, calculation steps) must be preserved.
*   The code must be testable, modular, and dependency-injected.

## Technical Highlights

### 1. Custom Display Compositing Engine
Instead of simple `Console.WriteLine` calls, I engineered a `DisplayService`. This service acts as a graphics engine for the console:
*   Maintains a virtual screen buffer string representing the "Monitor."
*   Performs **coordinate-based string splicing** to inject text into the monitor frame without redrawing the whole border.
*   Implements a custom **scrolling viewport** logic to handle the School Roster conversation within the fixed-height display boundaries.
*   Handles word-wrapping and text padding dynamically based on the monitor's configurable width.

### 2. Separation of Concerns (SoC) & Clean Architecture
The original "God Class" (`Computer`) was dismantled into a layered architecture:
*   **`RetroTerminal.Logic`:** A pure class library containing the business rules (Math, Regex validation, Student entities). It has **zero** dependencies on the Console UI.
*   **`RetroTerminal.App`:** The presentation layer. It handles user input (`ConsoleKeyInfo`) and orchestration but delegates the actual work to the Logic layer and the rendering to the Display service.
*   **`RetroTerminal.Logic.Tests`:** A comprehensive xUnit test suite ensuring the core logic remains accurate during the refactor.

### 3. Dependency Injection (DI)
The application uses a Composition Root pattern in `Program.cs`. Services (`SchoolRoster`, `TrinaryConverter`, `DisplayService`) are injected into the Application classes. This allows for easy swapping of implementations and creates a highly testable structure.

## Application Modules

### Trinary Converter
Converts base-3 inputs to decimals.
*   **Refactor Note:** The original logic used complex LINQ one-liners. This was replaced with readable, maintainable arithmetic logic while preserving the "step-by-step" calculation animation in the UI layer.

### School Roster (The Conversational UI)
A database of students that interacts via a "Head Teacher" chatbot.
*   **Refactor Note:** This was the most complex module to refactor. The original used deeply nested `if/else` and `goto` statements to manage the conversation state. The new version uses a state-managed loop within `SchoolRosterApp`, cleanly separating the *dialogue flow* from the *data storage*.

### ISBN-10 Verifier
Validates book codes using the modulus 11 algorithm.
*   **Refactor Note:** Logic was extracted to `IsbnVerifier.cs` and heavily unit tested to ensure edge cases (like 'X' check digits) are handled correctly.

## Project Structure

```
.
├── src/                        # THE NEW CODEBASE
│   ├── RetroTerminal.App/      # Console UI & Application Flow
│   └── RetroTerminal.Logic/    # Pure Business Logic (No UI)
├── tests/
│   └── RetroTerminal.Logic.Tests/ # xUnit Test Suite for the new logic
└── archive/                    # THE ORIGINAL SUBMISSION
    ├── OldApp/                 # The original monolithic application
    └── OldTests/               # The original test project
```

## How to Run

### 1. Running the Refactored Application
This is the main, modern version of the project running on .NET 8.

```bash
dotnet run --project src/RetroTerminal.App
```

### 2. Running the Modern Unit Tests
These tests verify the logic in `RetroTerminal.Logic` using xUnit. They cover the Trinary Converter, ISBN Verifier, and School Roster logic.

```bash
dotnet test tests/RetroTerminal.Logic.Tests
```

### 3. Accessing the Archives (Comparison)
The original submission files are preserved in the `archive/` folder. You can run them to compare the internal code structure against the user experience (which remains identical).

**To run the original monolithic app:**
```bash
dotnet run --project archive/OldApp
```

**To run the original test suite:**
```bash
dotnet test archive/OldTests
```

## Feedback & Improvements
This project originally received feedback regarding "conciseness" and "readability" due to the excessive use of `goto` and "clever" code. This refactor directly addresses that feedback by prioritizing:
*   **Readability:** Standard C# conventions and clear variable naming.
*   **Maintainability:** Breaking code into small, single-purpose classes.
*   **Stability:** Removing `goto` in favor of standard control flow loops (`while`, `switch`).
