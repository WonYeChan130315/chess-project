using System.Linq;
using UnityEngine;

public static class Board
{
    public static int[] squares = Enumerable.Repeat(Piece.None, 64).ToArray();

    public static Vector2 GetVectorPosition(int file, int rank) {
        return new Vector2(file, rank) - new Vector2(3.5f, 3.5f);
    }

    public static int GetRank(int index) {
        return index / 8;
    }
    
    public static int GetFile(int index) {
        return index % 8;
    }

    public static bool IsBoardOut(int index) {
        return index < 0 || index > 63;
    }

    public static bool IsLightSquare(int index) {
        return (GetRank(index) + GetFile(index)) % 2 != 0;
    }

    public static int EnpassantRank(bool isWhite) {
        return isWhite ? 4 : 3;
    }

    public static int NotEnpassantRank(bool isWhite) {
        return isWhite ? 6 : 1;
    }

    public static int EndRank(bool isWhite) {
        return isWhite ? 7 : 0;
    }

    public static int NotMatchingPawn(bool isWhite) {
        return isWhite ? Piece.Pawn + Piece.Black : Piece.Pawn;
    }

    public static int PawnDirection(bool isWhite) {
        return isWhite ? 1 : -1;
    }

    public static int GetOtherSide(int index) {
        return index + 7 * 8;
    }
}