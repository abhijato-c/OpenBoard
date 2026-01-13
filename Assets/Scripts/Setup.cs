using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using Move = System.Int32;

public class Setup : MonoBehaviour{
    public static Setup Instance { get; private set; }
    public GameObject SquaresFolder;
    public GameObject SquarePrefab;
    public GameObject PiecePrefab;
    public GameObject SelectorPrefab;
    public GameObject SelectorCapPrefab;
    public Color LightColor;
    public Color DarkColor;
    char[] files = {'a','b','c','d','e','f','g','h'};
    Chess Board = new Chess();
    GameObject[,] Pieces = new GameObject[8, 8];
    List<GameObject> Selectors = new List<GameObject>();
    public Engine eng = new Engine();
    string WhitePlayer = "Human";
    string BlackPlayer = "Human";

    void GenerateBoard(Color LightColor, Color DarkColor){
        for (int file = 0; file < 8; file++){
            for (int rank = 0; rank < 8; rank++){
                // Create the square
                GameObject newSquare = Instantiate(SquarePrefab, new Vector3(file - 3.5f, rank - 3.5f, 3), Quaternion.identity);
                newSquare.name = $"Square {files[file]}{rank+1}";
                newSquare.transform.parent = SquaresFolder.transform;

                // Set the color
                bool isLightSquare = (file + rank) % 2 == 0;
                SpriteRenderer sr = newSquare.GetComponent<SpriteRenderer>();
                sr.color = isLightSquare ? LightColor : DarkColor;
            }
        }
    }
    void DestroySelectors(){
        foreach (GameObject sel in Selectors){
            Destroy(sel);
        }
        Selectors.Clear();
    }
    // Sets pieces according to Board
    void SetBoard(){
        // Reset board
        DestroySelectors();
        for (int rank = 0; rank < 8; rank++){
            for (int file = 0; file < 8; file++){
                Destroy(Pieces[rank,file]);
                Pieces[rank, file] = null;
            }
        }

        // Add pieces
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
    private List<Vector2Int> GetPieceMoves(Vector2Int pos){
        Piece PcScript = Pieces[pos.y, pos.x].GetComponent<Piece>();
        PieceType type = PcScript.type;
        bool color = PcScript.color;
        List<Vector2Int> ToPositions = new List<Vector2Int>();
        List<Move> Moves = new List<Move>();
        int BBpos = (pos.y*8) + (7-pos.x);
        if (type == PieceType.Pawn && color == true)         Board.WPmoves(BBpos, ref Moves);
        else if (type == PieceType.Knight && color == true)  Board.WNmoves(BBpos, ref Moves);
        else if (type == PieceType.Bishop && color == true)  Board.WBmoves(BBpos, ref Moves);
        else if (type == PieceType.Rook && color == true)    Board.WRmoves(BBpos, ref Moves);
        else if (type == PieceType.Queen && color == true)   Board.WQmoves(BBpos, ref Moves);
        else if (type == PieceType.King && color == true)    Board.WKmoves(BBpos, ref Moves);
        else if (type == PieceType.Pawn && color == false)   Board.BPmoves(BBpos, ref Moves);
        else if (type == PieceType.Knight && color == false) Board.BNmoves(BBpos, ref Moves);
        else if (type == PieceType.Bishop && color == false) Board.BBmoves(BBpos, ref Moves);
        else if (type == PieceType.Rook && color == false)   Board.BRmoves(BBpos, ref Moves);
        else if (type == PieceType.Queen && color == false)  Board.BQmoves(BBpos, ref Moves);
        else if (type == PieceType.King && color == false)   Board.BKmoves(BBpos, ref Moves);

        foreach (Move mv in Moves){
            int to = (mv >> 6) & 63;
            ToPositions.Add(new Vector2Int(7 - to%8, to/8));
        }
        return ToPositions;
    }
    public void PieceClicked(Vector2Int position, bool col){
        DestroySelectors();

        if(col != Board.turn || (col && WhitePlayer!="Human") || (!col && BlackPlayer!="Human")) return;

        // Spawn in the new selectors
        List<Vector2Int> ToPositions = GetPieceMoves(position);
        foreach (Vector2Int dest in ToPositions){
            GameObject sel;
            if (Pieces[dest.y, dest.x] == null) sel = Instantiate(SelectorPrefab);
            else sel = Instantiate(SelectorCapPrefab);
            Selectors.Add(sel);
            sel.GetComponent<Selector>().Spawn(position, dest);
        }
    }
    public void MovePiece(Vector2Int from, Vector2Int to, int promote = 0){
        DestroySelectors();
        if (Pieces[to.y, to.x] != null){
            Destroy(Pieces[to.y, to.x]);
            Pieces[to.y, to.x] = null;
        }
        Pieces[from.y, from.x].GetComponent<Piece>().MoveTo(to);
        Pieces[to.y, to.x] = Pieces[from.y, from.x];
        Pieces[from.y, from.x] = null;
        int FromBB = from.y*8 + 7-from.x;
        int ToBB = to.y*8 + 7-to.x;
        Move MoveBB = (promote << 12) |(ToBB << 6) | FromBB;
        Board.move_piece(MoveBB);
        if((Board.turn && WhitePlayer!="Human") || (!Board.turn && BlackPlayer!="Human")) EngineMove();
    }
    public void NewGame(string opp, bool col){
        Board.ParseFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        SetBoard();
        eng.Kill();
        if(opp == "PassPlay"){
            WhitePlayer = "Human";
            BlackPlayer = "Human";
        }
        else if (col){
            WhitePlayer = "Human";
            BlackPlayer = opp;
        }
        else{
            WhitePlayer = opp;
            BlackPlayer = "Human";
        }

        if (opp == "Stockfish") eng.Spawn("Stockfish");
        if (opp == "Jimbo") eng.Spawn("Jimbo.out");
        if(WhitePlayer!="Human") EngineMove();
    }
    public async void EngineMove(){
        // Send command in seperate thread
        print(Board.GenerateFen());
        string result = await Task.Run(() => {
            eng.SendCommand($"position fen {Board.GenerateFen()}");
            return eng.SendCommand("go movetime 1000"); 
        });
        print(result);
        string output = result.Split(' ')[1];

        int FromFile = output[0] - 'a';
        int FromRank = output[1] - '1';
        int ToFile   = output[2] - 'a';
        int ToRank   = output[3] - '1';
        int prom = 0;

        if (output.Length == 5){
            if (output[4] == 'n') prom = 1;
            else if (output[4] == 'b') prom = 2;
            else if (output[4] == 'r') prom = 3;
            else if (output[4] == 'q') prom = 4;
        }

        Vector2Int from = new Vector2Int(FromFile, FromRank);
        Vector2Int to = new Vector2Int(ToFile, ToRank);
        MovePiece(from, to, prom);
    }
    void Start(){
        GenerateBoard(LightColor, DarkColor);
        NewGame("PassPlay", true);
    }
    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void OnApplicationQuit(){
        eng.Kill();
    }
}
