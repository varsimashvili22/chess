using Chess.Scripts.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightController : MonoBehaviour
{
    private GameObject clickedPiece;
    private ChessPlayerPlacementHandler[] allPieces;

    private void Start()
    {
        allPieces = FindObjectsOfType<ChessPlayerPlacementHandler>();//find all objects that have ChessPlayerPlacementHandler component
        foreach(ChessPlayerPlacementHandler piece in allPieces)
        {
            MakeChildOfOther(piece.gameObject, CBPH.GetTile(piece.row, piece.column));
            //all of those pieces become child of tile they are standing on
        }
    }
    void Update()
    {
        GameObject newClickedPiece = GetClickedPiece();

        if (Input.GetMouseButtonDown(0))
        {
            CBPH.ClearHighlights();

            if (newClickedPiece == null) { return; }//if we did not click on a piece rest of Update does not execute so only highlights that were visible dissapear

            if (clickedPiece == newClickedPiece)//if we click on already highlighted piece it unselects piece and deletes highlights
            {
                clickedPiece = null;
                return;
            }
            clickedPiece = newClickedPiece;
            
            HighlightBasedOnPiece(clickedPiece.tag);//calls method based on piece tag 
        }
    }

    private void HighlightBasedOnPiece(string piece)
    {
        switch (piece)
        {
            case "Pawn":
                PawnHighlights();
                break;
            case "Knight":
                KnightHighlights();
                break;
            case "Bishop":
                BishopHighlights();
                break;
            case "Rook":
                RookHighlights();
                break;
            case "Queen":
                QueenHighlights();
                break;
            case "King":
                KingHighlights();
                break;

            default:
                ChessBoardPlacementHandler.Instance.ClearHighlights();
                break;
        }
    }

    private void PawnHighlights()
    {
        int x = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().column;
        int y = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().row;

        HighlightIfEmpty(y + 1, x);

        if (y == 1)
        {
            HighlightIfEmpty(y + 2, x);//if it's pawn's first move it highlights 2 tiles in front
        }
    }

    private void KnightHighlights()
    {
        int x, y;
        x = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().column;
        y = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().row;

        HighlightIfValid(y + 2, x + 1);
        HighlightIfValid(y + 2, x - 1);
        HighlightIfValid(y - 2, x + 1);
        HighlightIfValid(y - 2, x - 1);
        HighlightIfValid(y + 1, x + 2);
        HighlightIfValid(y + 1, x - 2);
        HighlightIfValid(y - 1, x + 2);
        HighlightIfValid(y - 1, x - 2);
        //this is knights movement based on games rules 2 up 1 right, 2 up one left and so on
    }

    private void BishopHighlights()
    {
        int x, y;
        x = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().column;
        y = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().row;

        HighlightDiagonalMoves(y, x, 1, 1);
        HighlightDiagonalMoves(y, x, 1, -1);
        HighlightDiagonalMoves(y, x, -1, 1);
        HighlightDiagonalMoves(y, x, -1, -1);
        //bishop moves diagonally and this method checks if bishop can move in certain directions and stops when something is in way
    }

    private void RookHighlights()
    {
        int x = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().column;
        int y = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().row;

        HighlightInDirection(y, x, 1, 0);
        HighlightInDirection(y, x, -1, 0);
        HighlightInDirection(y, x, 0, 1);
        HighlightInDirection(y, x, 0, -1);
        //rook is like bishop and this method is similar to bishop's it checks if it can move in certain direction and stops if something is in way
    }

    private void QueenHighlights()
    {
        int x = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().column;
        int y = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().row;

        HighlightInDirection(y, x, 1, 0);
        HighlightInDirection(y, x, -1, 0);
        HighlightInDirection(y, x, 0, 1);
        HighlightInDirection(y, x, 0, -1);
        HighlightDiagonalMoves(y, x, 1, 1);
        HighlightDiagonalMoves(y, x, 1, -1);
        HighlightDiagonalMoves(y, x, -1, 1);
        HighlightDiagonalMoves(y, x, -1, -1);
        //queen is just bishop and rook combined nothing special
    }
    private void KingHighlights()
    {
        int x = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().column;
        int y = clickedPiece.GetComponent<ChessPlayerPlacementHandler>().row;

        HighlightIfValid(y + 1, x);
        HighlightIfValid(y - 1, x);
        HighlightIfValid(y, x + 1);
        HighlightIfValid(y, x - 1);
        HighlightIfValid(y + 1, x + 1);
        HighlightIfValid(y + 1, x - 1);
        HighlightIfValid(y - 1, x + 1);
        HighlightIfValid(y - 1, x - 1);
        //king checks if it can move on certain tiles based on his position and uses same method as knight
    }



    private void HighlightIfEmpty(int row, int col)
    {
        if (row >= 0 && row < 8 &&
            CBPH.GetTile(row, col).GetComponentInChildren<ChessPlayerPlacementHandler>() == null)
        {
            CBPH.Highlight(row, col);
        }
    }

    private void HighlightIfValid(int row, int col)
    {
        if (row >= 0 && row < 8 && col >= 0 && col < 8 &&
            CBPH.GetTile(row, col).GetComponentInChildren<ChessPlayerPlacementHandler>() == null)
        {
            CBPH.Highlight(row, col);
        }
    }

    private void HighlightDiagonalMoves(int startRow, int startCol, int rowDirection, int colDirection)
    {
        int row = startRow + rowDirection;
        int col = startCol + colDirection;

        while (row >= 0 && row < 8 && col >= 0 && col < 8)
        {
            if (CBPH.GetTile(row, col).GetComponentInChildren<ChessPlayerPlacementHandler>() == null)
            {
                CBPH.Highlight(row, col);
            }
            else
            {
                break; //stops when interrupted
            }

            row += rowDirection;
            col += colDirection;
        }
    }

    private void HighlightInDirection(int startRow, int startCol, int rowDirection, int colDirection)
    {
        int row = startRow + rowDirection;
        int col = startCol + colDirection;

        while (row >= 0 && row < 8 && col >= 0 && col < 8)
        {
            if (CBPH.GetTile(row, col).GetComponentInChildren<ChessPlayerPlacementHandler>() == null)
            {
                CBPH.Highlight(row, col);
            }
            else
            {
                break; //stops when interrupted
            }

            row += rowDirection;
            col += colDirection;
        }
    }


    private GameObject GetClickedPiece()
    {
        //gets mouse position in world
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //gets gameobject wiches collider mouse is hovering on
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.gameObject;//returns object that had collider
        }
        else
        {
            return null; // return null if no collider was hit
        }
    }
    private ChessBoardPlacementHandler CBPH //name is too long and it's not really practical so this just kind of shortens it
    {
        get { return ChessBoardPlacementHandler.Instance; }
        //typing GBPH has same result as typing ChessBoardPlacementHandler.Instance but only in this class
    }
    private void MakeChildOfOther(GameObject toBeChild, GameObject toBeParent)//makes first parameter child of second
    {
        toBeChild.transform.SetParent(toBeParent.transform, true);
    }
}
