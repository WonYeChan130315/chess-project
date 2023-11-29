using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int currentOrder = Piece.White;
    public static List<string> moveList = new();

    private int pieceIndex;
    private int currentIndex;
    private int targetIndex;
    private bool isDragging = false;
    private Transform dragObj;

    private void Update() {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = -3;

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Square"));

        if(hit.collider != null && Input.GetMouseButtonDown(0) && !isDragging && hit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite != null) {
            isDragging = true;
            ClearBoard();

            currentIndex = int.Parse(hit.collider.name);
            pieceIndex = Board.squares[currentIndex];

            dragObj = hit.transform.GetChild(0);
            
            ColoringMouseDown();
        } else if(Input.GetMouseButtonUp(0) && isDragging) {
            isDragging = false;
            ClearBoard();

            if(PieceMovement.IsPossibleMove(currentIndex, targetIndex) && Piece.IsEqualColor(Board.squares[currentIndex], currentOrder))
                MovePiece();

            if(dragObj != null) {
                dragObj.localPosition = Vector2.zero;
                dragObj = null;
            }
        }

        if(isDragging && hit.collider != null) {
            targetIndex = int.Parse(hit.collider.name);
            dragObj.position = mousePosition;
        }
    }

    private void MovePiece() {
        // 기물 위치 바꾸기
        BoardCreator.EragePiece(currentIndex);
        BoardCreator.DrawPiece(targetIndex, pieceIndex);
        
        currentOrder = currentOrder == Piece.White ? Piece.Black : Piece.White;

        // 기물이 움직인거 거장
        string moveStr = string.Format("{0:00}{1:00}{2:00}", Board.squares[targetIndex], currentIndex, targetIndex);
        moveList.Add(moveStr);

        bool isWhite = Piece.IsWhitePiece(GetSplitMove(moveStr)[0]);

        // 킹 캐슬링
        CastlingCheckManager.CheckCastling();

        // 폰
        if(DidEnpasaant(moveStr))
            BoardCreator.EragePiece(GetSplitMove(moveStr)[2] - 8 * Board.PawnDirection(isWhite));

        if(IsPromotion(moveStr, isWhite))
            BoardCreator.DrawPiece(GetSplitMove(moveStr)[2], isWhite ? Piece.Queen : Piece.Queen + Piece.Black);

        int castlingIdx = DidCastling(moveStr, isWhite);
        if(castlingIdx > 0) {
            bool isQueenSide = castlingIdx == 1;
            int cornerIndex = isQueenSide ? CastlingCheckManager.whiteQueenSideCorner : CastlingCheckManager.whiteKingSideCorner;

            if(isWhite)
                BoardCreator.EragePiece(cornerIndex);
            else
                BoardCreator.EragePiece(Board.GetOtherSide(cornerIndex));

            BoardCreator.DrawPiece(isQueenSide ? GetSplitMove(moveStr)[2] + 1 : GetSplitMove(moveStr)[2] - 1, isWhite ? Piece.Rook : Piece.Rook + Piece.Black);
        }

        // 4를 킹 인덱스로 바꿔야한다
        if(AttackedBySquares.IsAttackedTargetSquare(4))
            print("check");

        ColoringMouseUp();
    }

    private bool DidEnpasaant(string moveStr) {
        bool isWhite = Piece.IsWhitePiece(GetSplitMove(moveStr)[0]);

        int enpassantRank = Board.EnpassantRank(isWhite);
        int notMatchingColor = Board.NotMatchingPawn(isWhite);
        int direction = Board.PawnDirection(isWhite);

        bool isEnpassantRank = Board.GetRank(GetSplitMove(moveStr)[1]) == enpassantRank && Board.GetRank(GetSplitMove(moveStr)[2]) == enpassantRank + 1 * direction;
        return Piece.GetSprite(GetSplitMove(moveStr)[0]) == Piece.Pawn && Board.squares[GetSplitMove(moveStr)[2] - 8 * direction] == notMatchingColor && isEnpassantRank;
    }

    private bool IsPromotion(string moveStr, bool isWhite) {
        return Piece.GetSprite(GetSplitMove(moveStr)[0]) == Piece.Pawn && Board.GetRank(GetSplitMove(moveStr)[2]) == Board.EndRank(isWhite);
    }

    private int DidCastling(string moveStr, bool isWhite) {
        int originIndex = isWhite ? CastlingCheckManager.OriginKingIndex : Board.GetOtherSide(CastlingCheckManager.OriginKingIndex);
        int kingIndex = isWhite ? Piece.King : Piece.King + Piece.Black;
        int color = isWhite ? 0 : Board.GetOtherSide(0);

        int queenSide = CastlingCheckManager.whiteQueenSideTo + color;
        int kingSide = CastlingCheckManager.whiteKingSideTo + color;

        bool isOriginKingPosition = GetSplitMove(moveStr)[0] == kingIndex && GetSplitMove(moveStr)[1] == originIndex;
        bool isTargetKingPosition = GetSplitMove(moveStr)[2] == queenSide || GetSplitMove(moveStr)[2] == kingSide;

        bool isCastling = isOriginKingPosition && isTargetKingPosition;

        if(isCastling)
            return GetSplitMove(moveStr)[2] == queenSide ? 1 : 2;
        
        return 0;
    }

    public void ClearBoard() {
        for(int i = 0; i < BoardCreator.squareRenderers.Length; i++) {
            if(i != currentIndex) {
                Color lightColor = BoardCreator.instance.boardTheme.lightTheme.defaultColor;
                Color darkColor = BoardCreator.instance.boardTheme.darkTheme.defaultColor;

                BoardCreator.squareRenderers[i].color = Board.IsLightSquare(i) ? lightColor : darkColor;
            }
        }
    }

    public void ColoringMouseDown() {
        Color fromLightColor = BoardCreator.instance.boardTheme.lightTheme.moveColor;
        Color fromDarkColor = BoardCreator.instance.boardTheme.darkTheme.moveColor;

        BoardCreator.squareRenderers[currentIndex].color = Board.IsLightSquare(currentIndex) ? fromLightColor : fromDarkColor;

        if(!Piece.IsEqualColor(Board.squares[currentIndex], currentOrder))
            return;

        for(int i = 0; i < BoardCreator.squareRenderers.Length; i++) {
            if(PieceMovement.IsPossibleMove(currentIndex, i)) {
                bool isLightSquare = Board.IsLightSquare(i);

                Color lightColor = BoardCreator.instance.boardTheme.lightTheme.highlightColor;
                Color darkColor = BoardCreator.instance.boardTheme.darkTheme.highlightColor;

                BoardCreator.squareRenderers[i].color = isLightSquare ? lightColor : darkColor;
            }
        }
    }

    public void ColoringMouseUp() {
        Color toLightColor = BoardCreator.instance.boardTheme.lightTheme.moveColor;
        Color toDarkColor = BoardCreator.instance.boardTheme.darkTheme.moveColor;

        BoardCreator.squareRenderers[targetIndex].color = Board.IsLightSquare(targetIndex) ? toLightColor : toDarkColor;
    }

    public static int[] GetSplitMove(string move) {
        int[] splits = new int[3];

        splits[0] = int.Parse(move.Substring(0, 2));
        splits[1] = int.Parse(move.Substring(2, 2));
        splits[2] = int.Parse(move.Substring(4, 2));

        return splits;
    }
}