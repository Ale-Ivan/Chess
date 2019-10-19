using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoardInitializer : MonoBehaviour
{
    public GameObject whiteKing;
    public GameObject whiteQueen;
    public GameObject whiteBishop;
    public GameObject whiteKnight;
    public GameObject whiteRook;
    public GameObject whitePawn;

    public GameObject blackKing;
    public GameObject blackQueen;
    public GameObject blackBishop;
    public GameObject blackKnight;
    public GameObject blackRook;
    public GameObject blackPawn;

    private GameObject[,] pieces;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;

    //awake is called before start
    void Awake()
    {
        pieces = new GameObject[8, 8];

        white = new Player("white", true);
        black = new Player("black", false);

        currentPlayer = white;
        otherPlayer = black;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public ChessBoard InitialSetup(ChessBoard board)
    {
        AddPiece(board, whiteRook, white, 0, 0);
        AddPiece(board, whiteKnight, white, 1, 0);
        AddPiece(board, whiteBishop, white, 2, 0);
        AddPiece(board, whiteQueen, white, 3, 0);
        AddPiece(board, whiteKing, white, 4, 0);
        AddPiece(board, whiteBishop, white, 5, 0);
        AddPiece(board, whiteKnight, white, 6, 0);
        AddPiece(board, whiteRook, white, 7, 0);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(board, whitePawn, white, i, 1);
        }

        AddPiece(board, blackRook, black, 0, 7);
        AddPiece(board, blackKnight, black, 1, 7);
        AddPiece(board, blackBishop, black, 2, 7);
        AddPiece(board, blackQueen, black, 3, 7);
        AddPiece(board, blackKing, black, 4, 7);
        AddPiece(board, blackBishop, black, 5, 7);
        AddPiece(board, blackKnight, black, 6, 7);
        AddPiece(board, blackRook, black, 7, 7);

        for (int i = 0; i < 8; i++)
        {
            AddPiece(board, blackPawn, black, i, 6);
        }
        return board;
    }

    private void AddPiece(ChessBoard board, GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = board.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

}
