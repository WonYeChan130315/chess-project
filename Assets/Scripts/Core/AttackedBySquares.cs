using System.Collections.Generic;
using System.Linq;

public static class AttackedBySquares {
    public static bool IsAttackedTargetSquare(int targetIndex) {
        foreach(int attackedSquare in GetAttackedSquares())
            if(attackedSquare == targetIndex)
                return true;

        return false;
    }

    public static List<int> GetAttackedSquares() {
        int[] squares = Board.squares;
        List<int> attackedSquares = new();

        for(int i = 0; i < squares.Length; i++)
            if(i != Piece.None)
                attackedSquares.AddRange(PieceMovement.MakeMove(i));

        List<int> returnValue = attackedSquares.Distinct().ToList();
        return returnValue;
    }
}