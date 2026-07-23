namespace TicTacToeMonogame.Core;

// The domain vocabulary. A command carries a *fact* (what the player did),
// never a *decision* (where to go) — the Interpreter decides meaning per Mode.
public abstract record Command
{
    private Command() { }

    public sealed record Place(int Row, int Col) : Command;  // a board cell was clicked (adapter resolved pixel → cell)
    public sealed record Navigate(int Delta)     : Command;  // move a menu cursor by ±1
    public sealed record Confirm                 : Command;  // Enter — activate current selection
    public sealed record Cancel                  : Command;  // Esc — back / pause
}
