using System.Collections.Generic;
using UnityEngine;

public static class AttackedBySquares {
    public static List<int> whiteAttackedSquare = new();
    public static List<int> blackAttackedSquare = new();

    public static void UpdateCheck() {
        int color = GameManager.currentOrder;
        if(color == Piece.White)
            whiteAttackedSquare.Clear();
        else
            blackAttackedSquare.Clear();

        for(int square = 0; square < Board.squares.Length; square++) {
            if(!Piece.IsEqualColor(Board.squares[square], color)) {
                if(color == Piece.White)
                    whiteAttackedSquare.AddRange(PieceMovement.MakeMove(square, true));
                else
                    blackAttackedSquare.AddRange(PieceMovement.MakeMove(square, true));
            }
        }
    }

    public static bool IsCheck() {
        UpdateCheck();

        int color = GameManager.currentOrder;
        int kingIndex = color == Piece.White ? GameManager.whiteKingIndex : GameManager.blackKingIndex;
        List<int> attackedSquares = color == Piece.White ? whiteAttackedSquare : blackAttackedSquare;

        foreach(int square in attackedSquares)
            if(square == kingIndex)
                return true;
        
        return false;
    }
}