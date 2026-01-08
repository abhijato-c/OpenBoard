using System;
using UnityEditor.Search;
using UnityEngine;

public class Setup : MonoBehaviour{
    public GameObject SquarePrefab;
    public GameObject SquaresFolder;
    public GameObject PiecePrefab;
    char[] files = {'a','b','c','d','e','f','g','h'};
    private Chess Board = new Chess();
    GameObject[,] Pieces = new GameObject[8, 8];
    void GenerateBoard(Color LightColor, Color DarkColor){
        for (int file = 0; file < 8; file++){
            for (int rank = 0; rank < 8; rank++){
                // Create the square
                GameObject newSquare = Instantiate(SquarePrefab, new Vector3(file - 3.5f, rank - 3.5f, 1), Quaternion.identity);
                newSquare.name = $"Square {files[file]}{rank+1}";
                newSquare.transform.parent = SquaresFolder.transform;

                // Set the color
                bool isLightSquare = (file + rank) % 2 == 0;
                SpriteRenderer sr = newSquare.GetComponent<SpriteRenderer>();
                sr.color = isLightSquare ? LightColor : DarkColor;
            }
        }
    }
    // Sets pieces according to Board
    void SetBoard(){
        for (int rank = 0; rank < 8; rank++){
            for (int file = 0; file < 8; file++){
                // Calculate the bit index
                int bitIndex = (rank * 8) + (7 - file);
                ulong mask = 1UL << bitIndex;
                if ((Board.pieces & mask) == 0) continue;
                
                // Instantiate the piece
                GameObject pc = Instantiate(PiecePrefab);
                Piece PieceScript = pc.GetComponent<Piece>();
                Pieces[rank,file] = pc;

                // Check each bitboard to see if a piece exists at this mask
                if ((Board.wp & mask) != 0) PieceScript.Spawn(true, new Vector2Int(file, rank), PieceType.Pawn);
                else if ((Board.wr & mask) != 0) PieceScript.Spawn(true, new Vector2Int(file, rank), PieceType.Rook);
                else if ((Board.wn & mask) != 0) PieceScript.Spawn(true, new Vector2Int(file, rank), PieceType.Knight);
                else if ((Board.wb & mask) != 0) PieceScript.Spawn(true, new Vector2Int(file, rank), PieceType.Bishop);
                else if ((Board.wq & mask) != 0) PieceScript.Spawn(true, new Vector2Int(file, rank), PieceType.Queen);
                else if ((Board.wk & mask) != 0) PieceScript.Spawn(true, new Vector2Int(file, rank), PieceType.King);
                
                else if ((Board.bp & mask) != 0) PieceScript.Spawn(false, new Vector2Int(file, rank), PieceType.Pawn);
                else if ((Board.br & mask) != 0) PieceScript.Spawn(false, new Vector2Int(file, rank), PieceType.Rook);
                else if ((Board.bn & mask) != 0) PieceScript.Spawn(false, new Vector2Int(file, rank), PieceType.Knight);
                else if ((Board.bb & mask) != 0) PieceScript.Spawn(false, new Vector2Int(file, rank), PieceType.Bishop);
                else if ((Board.bq & mask) != 0) PieceScript.Spawn(false, new Vector2Int(file, rank), PieceType.Queen);
                else if ((Board.bk & mask) != 0) PieceScript.Spawn(false, new Vector2Int(file, rank), PieceType.King);
            }
        }
    }
    void Start(){
        GenerateBoard(new Color(1f,1f,1f), new Color(0f,0f,0f));
        Board.ParseFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        SetBoard();
    }
}
