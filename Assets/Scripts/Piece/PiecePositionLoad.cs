using System.Collections.Generic;
using UnityEngine;

public class PiecePositionLoad : MonoBehaviour
{
    public const string startFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";

    private void Start() {
        LoadPositionFromFen(startFEN);
    }

    public static void LoadPositionFromFen(string fen) {
        var pieceTypeFromSymbol = new Dictionary<char, int>() {
            ['k'] = Piece.King, ['p'] = Piece.Pawn, ['n'] = Piece.Knight,
            ['b'] = Piece.Bishop, ['r'] = Piece.Rook, ['q'] = Piece.Queen
        };

        string fenBoard = fen.Split(' ')[0];
        int file = 0, rank = 7;

        foreach(char symbol in fenBoard) {
            if(symbol == '/') {
                file = 0;
                rank--;
            } else {
                if(char.IsDigit(symbol)) {
                    file += (int) char.GetNumericValue(symbol);
                } else {
                    int color = char.IsUpper(symbol) ? Piece.White : Piece.Black;
                    char piece = char.ToLower(symbol);

                    BoardCreator.DrawPiece(rank * 8 + file, pieceTypeFromSymbol[piece] + color);
                    file++;
                }
            }
        }
    }
}
