using UnityEngine;

public static class CastlingCheckManager
{
    // 캐슬링 할 수 있나
    public static bool canWhiteQueenSide;
    public static bool canWhiteKingSide;
    public static bool canBlackQueenSide;
    public static bool canBlackKingSide;

    // 캐슬링 했을 때 이동할 위치
    public static int whiteKingSideTo = 6;
    public static int whiteQueenSideTo = 2;

    // 캐슬링 사이 인덱스
    private static readonly int[] WhiteQueenSideSquares = { 1, 2, 3 };
    private static readonly int[] WhiteKingSideSquares = { 5, 6 };

    private static readonly int[] BlackQueenSideSquares = { 57, 58, 59 };
    private static readonly int[] BlackKingSideSquares = { 61, 62 };

    // 캐슬링 구석
    public static readonly int whiteQueenSideCorner = 0;
    public static readonly int whiteKingSideCorner = 7;
    
    // 움직였나
    private static bool isMoveWhiteQueenSide = false;
    private static bool isMoveWhiteKingSide = false;
    private static bool isMoveBlackQueenSide = false;
    private static bool isMoveBlackKingSide = false;

    // 킹 원래 포지션
    public static readonly int OriginKingIndex = 4;

    public static void CheckCastling() {
        // 백 사이드
        WhiteSide();

        // 흑 사이드
        BlackSide();
    }

    private static void WhiteSide() {
        isMoveWhiteQueenSide = CheckMove(isMoveWhiteQueenSide, whiteQueenSideCorner, Piece.White);
        isMoveWhiteKingSide = CheckMove(isMoveWhiteKingSide, whiteKingSideCorner, Piece.White);

        bool isQueenSideEmpty = CheckSideEmpty(WhiteQueenSideSquares);
        bool isKingSideEmpty = CheckSideEmpty(WhiteKingSideSquares);

        canWhiteQueenSide = !isMoveWhiteQueenSide && isQueenSideEmpty;
        canWhiteKingSide = !isMoveWhiteKingSide && isKingSideEmpty;
    }

    private static void BlackSide() {
        isMoveBlackQueenSide = CheckMove(isMoveBlackQueenSide, Board.GetOtherSide(whiteQueenSideCorner), Piece.Black);
        isMoveBlackKingSide = CheckMove(isMoveBlackKingSide, Board.GetOtherSide(whiteKingSideCorner), Piece.Black);

        bool isBlackQueenSideEmpty = CheckSideEmpty(BlackQueenSideSquares);
        bool isBlackKingSideEmpty = CheckSideEmpty(BlackKingSideSquares);

        canBlackQueenSide = !isMoveBlackQueenSide && isBlackQueenSideEmpty;
        canBlackKingSide = !isMoveBlackKingSide && isBlackKingSideEmpty;
    }

    private static bool CheckMove(bool condition, int cornerIndex, int color)  {
        if(condition) 
            return true;

        if(Board.squares[color == Piece.White ? OriginKingIndex : Board.GetOtherSide(OriginKingIndex)] != Piece.King + color) {
            return false;
        }

        if(Board.squares[cornerIndex] != Piece.Rook + color)
            return true;

        return false;
    }

    private static bool CheckSideEmpty(int[] side) {
        foreach (int checkTarget in side) {
            if (Board.squares[checkTarget] != Piece.None)
                return false;
        }
        return true;
    }
}
