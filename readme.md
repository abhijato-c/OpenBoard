# OpenBoard

A simple chess interface built with Unity. It uses a bitboard-based backend(identical to that of my chess engine, Jimbo) for move validation and supports UCI engine integration for playing against the computer.

Download from [itch](https://m-8000.itch.io/openboard)

## Features

* **UCI Engine Support:** Connect your favorite engines (like Stockfish) to play against.
* **Timed Play:** Fully functional chess clocks with support for increments.
* **Export Options:** Copy FEN to clipboard, or save the PGN file.
* **Move History:** Scrub through the game history to review previous positions, or replay from that state.

## How to Use

### Playing a Game

By default, the game starts in Pass and Play mode. Click a piece to see highlighted legal moves, then click a target square to complete the move.

### Connecting an Engine

OpenBoard app is designed to communicate with UCI-compatible engines. Ensure your engine executable is in an accessible directory, and from the Engines tab, add an engine, and select the file location of the executable. Set any name you want to it. Engine configuration is saved even when the app closes. All added engines are available in the Opponent selector in the new game tab.

### Exporting Data

Open the export window for:

* **FEN:** A single-line representing the current board state.
* **PGN:** The full list of moves in standard format.

## Technical Details

* **Backend:** The bitboard code from Jimbo, translated to C#.
* **UI:** Uses unity.