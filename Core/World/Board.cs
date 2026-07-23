using System.Collections.Immutable;

namespace TicTacToeMonogame.Core;

// A cell is element for the boolean lattice 𝔹² = (ℤ/2)².
// None = (false,false).
// Contradiction = (true,true).
// X = (true,false).
// O = (false,true).
public readonly record struct Cell(bool HasX, bool HasO)
{
    public static readonly Cell None = default;          // (false, false) = ⊥
    public static readonly Cell X = new(true, false);
    public static readonly Cell O = new(false, true);
    public static readonly Cell Contradiction = new(true, true); // ⊤

    public static Cell operator |(Cell a, Cell b) => new(a.HasX | b.HasX, a.HasO | b.HasO);
    public static Cell operator &(Cell a, Cell b) => new(a.HasX && b.HasX, a.HasO && b.HasO); 

    public bool IsContradiction => HasX && HasO;

    public Cell Other() => this == X ? O : X;   // flip the turn: X ↔ O
}

public enum MoveResult { Rejected, Continues, Won, Drawn }
public readonly record struct MoveOutcome(MoveResult Result, Board Board);

public class Board
{
    public const int Size = 3;
    private readonly Cell[,] _board;

    private Board(Cell[,] board) => _board = board;
    
    public Cell At(int row, int col) => _board[row, col];

    public static Board Empty() => new(new Cell[Size, Size]);

    public MoveOutcome Place(int row, int col, Cell mark)
    {
        if (At(row, col) != Cell.None)                       // occupied → reject, board unchanged
            return new(MoveResult.Rejected, this);

        var next = (Cell[,])_board.Clone();
        next[row, col] = mark;                               // cell was None, so None | mark = mark
        var board = new Board(next);

        return new(                                          // check victory right here
            board.Victory() ? MoveResult.Won
          : board.IsFull() ? MoveResult.Drawn
          : MoveResult.Continues,
            board);
    }

    private bool IsFull()
    {
        foreach (var cell in _board)   // foreach over a Cell[,] walks every cell
            if (cell == Cell.None) return false;
        return true;
    }

    public bool Victory()
    {
        for (int i = 0; i < Size; i++)
        {
            var row = Cell.Contradiction;   // ⊤ is the identity of & (like starting a product at 1)
            var col = Cell.Contradiction;
            for (int j = 0; j < Size; j++)
            {
                row &= _board[i, j];        // multiply across row i
                col &= _board[j, i];        // multiply down column i
            }
            if (row.HasX || row.HasO) return true;
            if (col.HasX || col.HasO) return true;
        }

        var diag = Cell.Contradiction;
        var anti = Cell.Contradiction;
        for (int i = 0; i < Size; i++)
        {
            diag &= _board[i, i];
            anti &= _board[i, Size - 1 - i];
        }
        if (diag.HasX || diag.HasO) return true;
        if (anti.HasX || anti.HasO) return true;

        return false;   // no line completed
    }
}