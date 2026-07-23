namespace TicTacToeMonogame.Core;

public readonly record struct GameState(Board Board, Cell Turn, Mode Mode)
{
    public static GameState New() => new(Board.Empty(), Cell.X, Mode.InGame);
}
public static class Interpreter
{
    // Mode decides what a command *means* — this is the Core-side interpretation.
    public static GameState Apply(GameState state, Command command) => state.Mode switch
    {
        Mode.InGame => InGame(state, command),
        _ => state,          // StartMenu / EndGame handling comes later
    };

    private static GameState InGame(GameState state, Command command) => command switch
    {
        Command.Place(var row, var col) => AfterPlace(state, row, col),
        _ => state,
    };

    private static GameState AfterPlace(GameState state, int row, int col)
    {
        var outcome = state.Board.Place(row, col, state.Turn);   // Board decides
        return outcome.Result switch                             // Interpreter reacts
        {
            MoveResult.Continues => state with { Board = outcome.Board, Turn = state.Turn.Other() },
            MoveResult.Won or MoveResult.Drawn => state with { Board = outcome.Board, Mode = Mode.EndGame },
            _ => state,            // Rejected → unchanged
        };
    }
}
