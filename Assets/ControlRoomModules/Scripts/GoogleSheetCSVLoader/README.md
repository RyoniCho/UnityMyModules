# Google Sheet CSV Loader

A pipeline for loading game data (Stats, Dialogue, Items, etc.) from Google Sheets via CSV export.

## Workflow

1.  **Google Sheets**: Manage your game data in a Google Sheet.
2.  **Export**: Export sheets as `.csv` files.
3.  **Import**: Place `.csv` files in `Resources/Data/CSV` (or configured path).
4.  **Load**: The module parses CSVs into C# Objects (`TableData`) at runtime.

## Core Classes

*   **`TableDataLoader`**: Reads CSV text assets.
*   **`TableManager`**: Central access point for all loaded data tables.
*   **`TableData<T>`**: Base class for a row of data.

## Adding New Data Types

1.  Create a data class inheriting from `TableData` (e.g., `TEnemyData`).
2.  Create a matching loader/builder class if necessary.
3.  Register the new type in `TableManager`.
