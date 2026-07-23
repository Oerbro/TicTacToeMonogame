# TicTacToe — MonoGame (DesktopGL, desktop-only, portfolio project)

## Design philosophy — READ FIRST

This codebase is written in a **functional / category-theory-influenced** style. The
goal is excellent code design, not adherence to any one paradigm.

- **Functional is not the opposite of OOP here.** Use classes, but favour the
  functional virtues: immutability, pure functions, values over side effects,
  total functions (make illegal states unrepresentable), composition over mutation.
- Prefer expressions to statements; prefer returning a value to mutating in place.
- Model outcomes as data — discriminated-union-style results / enums / records —
  rather than out-params, sentinel values, or exceptions for control flow.
- Types carry meaning: prefer a named type or enum over a bare `bool`/`int` when it
  encodes a domain concept. `None`-first enums so `default` is the empty/zero state.
- Keep the code terse and dense in the way functional code is — but never at the
  cost of clarity. When suggesting code, match this register.

Do not push imperative/mutation-first idioms. When there's a functional and an
imperative way to express something, show the functional one.

## Architecture boundary (hard rule)

**No file under `Core/` may reference MonoGame.** No `using Microsoft.Xna.Framework`
(or `.Graphics` / `.Input`). That includes the sneaky ones: `Vector2`, `Point`,
`Rectangle`, `Color`, `Keys`, `GameTime` are ALL MonoGame types.

- Core talks in `(int row, int col)` and its own types — never pixels.
- Dependencies point one way: `Game1` → `Input`/`Render` → `Core`. Core references nothing outward.
- The test: could `Core` run unchanged in a console app? If not, the boundary leaked.
- Core **decides/determines** (rules, win detection, transitions); adapters **react**
  (render, call `Exit()`). Renderer pulls state each frame — it is never pushed to.

## Namespaces (convention)

Deliberately flat — exactly **two**, mirroring the one boundary that matters:

- Everything under `Core/**` → `TicTacToeMonogame.Core` (no deeper nesting, regardless
  of subfolder — `World/`, `StateMachine/`, `Commands/` all land here).
- Adapters (`Input/`, `Render/`) and `Game1` → `TicTacToeMonogame`.

Folders still organise files; namespaces do not mirror the folder tree beyond this.
The point: an adapter reaching into Core writes `using TicTacToeMonogame.Core;`
(expected); a `Core` file needing `using TicTacToeMonogame;` to see `Renderer` is a
visible boundary smell. Prefer file-scoped namespaces.

## Stack

- MonoGame.Framework.DesktopGL 3.8.* (currently 3.8.5) — supports .NET 8+ incl. .NET 10.
- Targets net10.0 → C# 14 by default (extension members, etc.).
- Deliberately single-project, DesktopGL, desktop-only. Not multi-platform by choice.
