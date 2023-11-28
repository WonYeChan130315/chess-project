using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    public Sprite squareTexture;
    public BoardTheme boardTheme;
    public PieceTheme pieceTheme;

    public static BoardCreator instance;
    
    public static SpriteRenderer[] squareRenderers = new SpriteRenderer[64];
    public static SpriteRenderer[] pieceRenderers = new SpriteRenderer[64];

    private void Awake() {
        instance = this;

        GreateBoard();
    }

    private void GreateBoard() {
        for(int rank = 0; rank < 8; rank++) {
            for(int file = 0; file < 8; file++) {
                bool IsLightSquare = (file + rank) % 2 != 0;
                var squareColor = IsLightSquare ? boardTheme.lightTheme.defaultColor : boardTheme.darkTheme.defaultColor;

                DrawSquare(squareColor, file, rank);
            }
        }
    }

    private void DrawSquare(Color color, int file, int rank) {
        // 칸 생성
        GameObject squareObject = CreateSquareObject(color, file, rank);
        squareRenderers[rank * 8 + file] = squareObject.GetComponent<SpriteRenderer>();

        // 안에 기물 오브젝트 생성
        GameObject pieceObject = CreatePieceObject(squareObject);
        pieceRenderers[rank * 8 + file] = pieceObject.GetComponent<SpriteRenderer>();
    }

    private GameObject CreateSquareObject(Color color, int file, int rank) {
        GameObject squareObject = new((rank * 8 + file).ToString());
        SpriteRenderer squareRenderer = squareObject.AddComponent<SpriteRenderer>();
        BoxCollider2D squareCollider = squareObject.AddComponent<BoxCollider2D>();

        squareCollider.size = Vector2.one;

        squareRenderer.sprite = squareTexture;
        squareRenderer.color = color;

        squareObject.layer = LayerMask.NameToLayer("Square");
        squareObject.transform.parent = transform;
        squareObject.transform.position = Board.GetVectorPosition(file, rank);

        return squareObject;
    }
    private GameObject CreatePieceObject(GameObject parentSquare) {
        GameObject pieceObject = new("Piece");
        pieceObject.transform.parent = parentSquare.transform;
        pieceObject.transform.localPosition = Vector2.zero;

        SpriteRenderer pieceRenderer = pieceObject.AddComponent<SpriteRenderer>();
        pieceRenderer.sortingOrder = 1;

        return pieceObject;
    }

    public static void DrawPiece(int targetIndex, int piece) {
        Board.squares[targetIndex] = piece;
        pieceRenderers[targetIndex].sprite = Piece.IsWhitePiece(piece) ? instance.pieceTheme.whiteTheme[piece] : instance.pieceTheme.blackTheme[Piece.GetSprite(piece)];
    }

    public static void EragePiece(int targetIndex) {
        Board.squares[targetIndex] = Piece.None;
        pieceRenderers[targetIndex].sprite = null;
    }
}
