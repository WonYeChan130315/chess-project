using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCreator : MonoBehaviour
{
    public Sprite squareTexture;
    public BoardTheme boardTheme;
    
    [HideInInspector] public SpriteRenderer[,] squareRenderers = new SpriteRenderer[8, 8];
    [HideInInspector] public SpriteRenderer[,] pieceRenderers = new SpriteRenderer[8, 8];

    private void Awake() {
        GreateBoard();
    }

    private void GreateBoard() {
        for(int file = 0; file < 8; file++) {
            for(int rank = 0; rank < 8; rank++) {
                bool IsLightSquare = (file + rank) % 2 != 0;
                var squareColor = IsLightSquare ? boardTheme.lightTheme.defaultColor : boardTheme.darkTheme.defaultColor;

                DrawSquare(squareColor, file, rank);
            }
        }
    }

    private void DrawSquare(Color color, int file, int rank) {
        // 칸 생성
        GameObject squareObject = new("Square");
        SpriteRenderer squareRenderer = squareObject.AddComponent<SpriteRenderer>();

        squareRenderer.sprite = squareTexture;
        squareRenderer.color = color;
        squareObject.transform.parent = transform;

        squareObject.transform.position = GameManager.GetVectorPosition(file, rank);
        squareRenderers[file, rank] = squareRenderer;

        // 안에 기물 오브젝트 생성
        GameObject pieceObject = new("Piece");
        pieceObject.transform.parent = squareObject.transform;

        SpriteRenderer pieceRenderer = pieceObject.AddComponent<SpriteRenderer>();
        pieceRenderers[file, rank] = pieceRenderer;
    }
}
