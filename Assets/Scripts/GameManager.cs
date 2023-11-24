using UnityEngine;

public static class GameManager {
    public static int curOrder = PieceSymbol.White;

    public static Vector2 GetVectorPosition(int file, int rank) {
        return new Vector2(file, rank) - new Vector2(3.5f, 3.5f);
    }
}