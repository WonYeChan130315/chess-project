public static class Piece {
    public const int None = -1;
    public const int King = 0;
    public const int Pawn = 1;
    public const int Knight = 2;
    public const int Bishop = 3;
    public const int Rook = 4;
    public const int Queen = 5;

    public const int White = 0;
    public const int Black = 6;

    public static bool IsWhitePiece(int piece) {
        return piece < Black;
    }

    public static bool IsEqualColor(int a, int b) {
        if(a == None || b == None) return false;
        return IsWhitePiece(a) == IsWhitePiece(b);
    }

    public static int GetSprite(int piece) {
        return IsWhitePiece(piece) ? piece : piece - Black;
    }
}
