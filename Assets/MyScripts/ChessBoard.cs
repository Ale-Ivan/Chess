using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public Material defaultMaterial;

    public GameObject blackKing;
    public Transform blackKingPositionReference;

    private GameObject[,] pieces;

    private Player white;
    private Player black;
    public Player currentPlayer;
    public Player otherPlayer;


    public void initialSetup()
    {
        pieces = new GameObject[8, 8];
        white = new Player("white", true);
        black = new Player("black", false);

        currentPlayer = white;
        otherPlayer = black;

        AddPiece(blackKing, black, 0, 4);
    }

    public void AddPiece(GameObject prefab, Player player, int col, int row)
    {
        GameObject pieceObject = this.AddPiece(prefab, col, row);
        player.pieces.Add(pieceObject);
        pieces[col, row] = pieceObject;
    }

    private GameObject AddPiece(GameObject piece, int col, int row)
    {
        Vector2Int gridPoint = Geometry.GridPoint(col, row);
        GameObject newPiece = Instantiate(piece, Geometry.PointFromGrid(gridPoint), Quaternion.identity, gameObject.transform);
        return newPiece;
    }

}
