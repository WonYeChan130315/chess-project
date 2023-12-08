using System.Collections.Generic;
using System.Linq;

public static class PieceMovement
{
    public static int u = 8, d = -8, l = -1, r = 1;

    public static bool IsPossibleMove(int currentIndex, int targetIndex, bool checkIgnore = false) {
        List<int> possibleMoves = MakeMove(currentIndex, checkIgnore);
        foreach(int move in possibleMoves) {
            int possibleMove = move;
            if(targetIndex == possibleMove)
                return true;
        }

        return false;
    }

    public static List<int> MakeMove(int currentIndex, bool checkIgnore = false) {
        List<int> moves = new();
        CheckKingIndex();
        
        int piece = Piece.GetSprite(Board.squares[currentIndex]);
        switch(piece) {
            // 일반 기물
            case Piece.King : // 캐슬링, 위, 아래, 오른쪽, 왼쪽, 대각선
                moves = KingMove(currentIndex);
                break;
            case Piece.Pawn : // 앙파상, 프로모션, 첫수 잎으로 두칸 또는 한칸, 앞으로 한칸
                moves = PawnMove(currentIndex);
                break;
            case Piece.Knight : // 걍 이상함
                moves = KnightMove(currentIndex);
                break;

            // 슬라이딩 기물
            case Piece.Bishop : // 대각선
                moves = BishopMove(currentIndex);
                break;
            case Piece.Rook : // 직선
                moves = RookMove(currentIndex);
                break;
            case Piece.Queen : // 대각선, 직선
                moves = BishopMove(currentIndex, Piece.Bishop);
                moves.AddRange(RookMove(currentIndex, Piece.Rook)); // 룩 움직임 추가
                break;
        }

        if(!checkIgnore)
            moves = CheckMoveFix(currentIndex, moves);

        return moves;
    }

    private static List<int> CheckMoveFix(int currentIndex, List<int> moves) {
        int piece = Board.squares[currentIndex];
        List<int> fixResult = new();

        foreach(int move in moves) {
            int targetPiece = Board.squares[move];

            Board.squares[currentIndex] = Piece.None;
            Board.squares[move] = piece;

            if(!AttackedBySquares.IsCheck()) 
                fixResult.Add(move);

            Board.squares[currentIndex] = piece;
            Board.squares[move] = targetPiece;
        }

        return fixResult;
    }

    private static void CheckKingIndex() {
        int kingNum = Piece.King + GameManager.currentOrder;

        for(int square = 0; square < Board.squares.Length; square++) {
            if(Board.squares[square] == kingNum) {
                if(GameManager.currentOrder == Piece.White)
                    GameManager.whiteKingIndex = square;
                else
                    GameManager.blackKingIndex = square;
            }
        }
    }

    private static List<int> GenerateSlidingMove(int currentIndex, int[] directions, int piece = -10) {
        List<int> moves = new();
        foreach(int direction in directions) {
            int target = currentIndex + direction;
            
            while(!Board.IsBoardOut(target)) {
                if(Piece.IsEqualColor(Board.squares[currentIndex], Board.squares[target])) break;

                if(!SlidingPieceCondition(currentIndex, target, piece)) break;

                moves.Add(target);

                if(Board.squares[target] != Piece.None) break;

                target += direction;
            }
        }

        return moves;
    }

    private static bool SlidingPieceCondition(int currentIndex, int targetIndex, int piece = -10) {
        int pieceIndex = piece == -10 ? Piece.GetSprite(Board.squares[currentIndex]) : piece;

        if(pieceIndex == Piece.Bishop) {
            return Board.IsLightSquare(currentIndex) == Board.IsLightSquare(targetIndex);
        } else if(pieceIndex == Piece.Rook) {
            int currentRank = Board.GetRank(currentIndex), currentFile = Board.GetFile(currentIndex);
            int targetRank = Board.GetRank(targetIndex), targetFile = Board.GetFile(targetIndex);

            bool isEqualRank = currentRank == targetRank;
            bool isEqualFile = currentFile == targetFile;

            // 둘중 한개만 false 여야됨
            return (!isEqualRank || !isEqualFile) && !(!isEqualRank && !isEqualFile);
        }
        
        return false;
    }

    private static bool IsEnpassant(bool isWhite, int currentIndex, int direction) {
        int[] splitArray = GameManager.GetSplitMove(GameManager.moveList[GameManager.moveList.Count - 1]);
        return splitArray[0] == Board.NotMatchingPawn(isWhite) && Board.GetRank(splitArray[1]) == Board.NotEnpassantRank(isWhite) && splitArray[2] == currentIndex + direction;
    }

    private static List<int> KingMove(int currentIndex) {
        List<int> moves = new();

        // 위, 아래, 왼, 오, 대각선
        int[] originMoves = { u, d, l, r, u+l, u+r, d+l, d+r };
        foreach(int originMove in originMoves) {
            int targetIndex = originMove + currentIndex;
            int targetAndCurrentSpace = Board.GetFile(currentIndex) - Board.GetFile(targetIndex);

            if(!Board.IsBoardOut(targetIndex) && !Piece.IsEqualColor(Board.squares[currentIndex], Board.squares[targetIndex]) && targetAndCurrentSpace < 2 && targetAndCurrentSpace > -2)
                moves.Add(targetIndex);
        }

        // 캐슬링
        bool isWhite = Piece.IsWhitePiece(Board.squares[currentIndex]);
        if(isWhite) {
            if(CastlingCheckManager.canWhiteQueenSide)
                moves.Add(CastlingCheckManager.whiteQueenSideTo);
            if(CastlingCheckManager.canWhiteKingSide)
                moves.Add(CastlingCheckManager.whiteKingSideTo);
        } else {
            if(CastlingCheckManager.canBlackQueenSide)
                moves.Add(Board.GetOtherSide(CastlingCheckManager.whiteQueenSideTo));
            if(CastlingCheckManager.canBlackKingSide)
                moves.Add(Board.GetOtherSide(CastlingCheckManager.whiteKingSideTo));
        }

        return moves;
    }

    private static List<int> PawnMove(int currentIndex) {
        List<int> moves = new();
        bool isWhite = Piece.IsWhitePiece(Board.squares[currentIndex]);

        // 한칸 앞
        int oneFront = currentIndex + u * Board.PawnDirection(isWhite);
        if(!Board.IsBoardOut(oneFront) && Board.squares[oneFront] == Piece.None) {
            moves.Add(oneFront);

            // 두칸 앞
            int twoFront = currentIndex + u * 2 * Board.PawnDirection(isWhite);
            if(Board.GetRank(currentIndex) == Board.NotEnpassantRank(!isWhite) && Board.squares[twoFront] == Piece.None)
                moves.Add(twoFront);
        }

        // 대각선으로 잡기 근데 이제 앙파상을 곁들인
        int rightUp = currentIndex + u * Board.PawnDirection(isWhite) + r;
        int leftUp = currentIndex + u * Board.PawnDirection(isWhite) + l;

        int rightAndCurrentSpace = Board.GetFile(currentIndex) - Board.GetFile(rightUp);
        int leftAndCurrentSpace = Board.GetFile(currentIndex) - Board.GetFile(leftUp);

        if(rightAndCurrentSpace < 2 && rightAndCurrentSpace > -2) {
            if(!Piece.IsEqualColor(Board.squares[currentIndex], Board.squares[rightUp]) && Board.squares[rightUp] != Piece.None) {
                moves.Add(rightUp);
            } else if(GameManager.moveList.Count > 0 && Board.squares[rightUp] == Piece.None && IsEnpassant(isWhite, currentIndex, r)) {
                moves.Add(rightUp);
            }
        }
        if(leftAndCurrentSpace < 2 && leftAndCurrentSpace > -2) {
            if(!Piece.IsEqualColor(Board.squares[currentIndex], Board.squares[leftUp]) && Board.squares[leftUp] != Piece.None) {
                moves.Add(leftUp);
            } else if(GameManager.moveList.Count > 0 && Board.squares[leftUp] == Piece.None && IsEnpassant(isWhite, currentIndex, l)) {
                moves.Add(leftUp);
            }
        }

        return moves;
    }

    private static List<int> KnightMove(int currentIndex) {
        List<int> moves = new int[] {
            u*2 + l, u*2 + r, d*2 + l, d*2 + r,
            l*2 + u, l*2 + d, r*2 + u, r*2 + d
        }.ToList();

        // 나갈때 처리
        List<int> moveArray = new();
        foreach(int move in moves) {
            int targetIndex = currentIndex + move;
            int targetAndCurrentSpace = Board.GetFile(currentIndex) - Board.GetFile(targetIndex);

            if(!Board.IsBoardOut(targetIndex) && !Piece.IsEqualColor(Board.squares[currentIndex], Board.squares[targetIndex]) && targetAndCurrentSpace < 3 && targetAndCurrentSpace > -3)
                moveArray.Add(targetIndex);
        }
        
        return moveArray;
    }

    private static List<int> BishopMove(int currentIndex, int piece = -10) {
        return GenerateSlidingMove(currentIndex, new int[] { u+l, u+r, d+l, d+r }, piece);
    }

    private static List<int> RookMove(int currentIndex, int piece = -10) {
        return GenerateSlidingMove(currentIndex, new int[] { u, d, l, r }, piece);
    }
}