using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

using Move = System.Int32;

public class Setup : MonoBehaviour{
    public static Setup Instance { get; private set; }
    public GameObject SquaresFolder;
    public GameObject SquarePrefab;
    public GameObject PiecePrefab;
    public GameObject SelectorPrefab;
    public GameObject SelectorCapPrefab;
    public GameObject CheckPrefab;
    public GameObject MoveButtonPrefab;
    public GameObject NotificationObject;
    public GameObject PromoteDialog;
    public GameObject MoveHistory;
    public TMP_Text WhitePlayerIndicator;
    public TMP_Text BlackPlayerIndicator;
    public ClockManager WhiteClock;
    public ClockManager BlackClock;
    public Color LightColor;
    public Color DarkColor;
    public Color RegualarClockColor;
    public Color AlarmClockColor;
    public float scale = 0.8f;


    char[] files = {'a','b','c','d','e','f','g','h'};
    Chess Board = new Chess();
    GameObject[,] Pieces = new GameObject[8, 8];
    List<GameObject> Selectors = new List<GameObject>();
    public Engine eng = new Engine();
    string WhitePlayer = "Human";
    string BlackPlayer = "Human";
    bool timed = false;
    int increment = 0;
    bool GameOver = false;
    GameObject CheckIndicator;

    // Move History
    List<string> HistoryFen = new List<string>();
    List<PieceType> HistoryType = new List<PieceType>();
    List<Vector2Int> HistoryFrom = new List<Vector2Int>();
    List<Vector2Int> HistoryTo = new List<Vector2Int>();
    List<float> HistoryWtime = new List<float>();
    List<float> HistoryBtime = new List<float>();
    List<GameObject> MoveButtons = new List<GameObject>();
    int HMclock = 0;

    public Vector3 BoardToGlobalPos(int file, int rank, int Zindex){
        return new Vector3((file- 3.5f) * scale - 1f, (rank - 3.5f) * scale, Zindex);
    }

    void GenerateBoard(Color LightColor, Color DarkColor){
        for (int file = 0; file < 8; file++){
            for (int rank = 0; rank < 8; rank++){
                // Create the square
                GameObject newSquare = Instantiate(SquarePrefab, SquaresFolder.transform);
                newSquare.transform.position = BoardToGlobalPos(file, rank, 4);
                newSquare.transform.localScale = new Vector2(scale, scale);
                newSquare.name = $"Square {files[file]}{rank+1}";

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
    GameObject SpawnPieceAt(int file, int rank){
        int bitIndex = (rank * 8) + (7 - file);
        ulong mask = 1UL << bitIndex;
        if ((Board.pieces & mask) == 0) return null;

        GameObject pc = Instantiate(PiecePrefab);
        Piece PieceScript = pc.GetComponent<Piece>();
        Pieces[rank,file] = pc;
        Vector2Int position = new(file, rank);

        if ((Board.wp & mask) != 0) return PieceScript.Spawn(true, position, PieceType.Pawn);
        else if ((Board.wr & mask) != 0) return PieceScript.Spawn(true, position, PieceType.Rook);
        else if ((Board.wn & mask) != 0) return PieceScript.Spawn(true, position, PieceType.Knight);
        else if ((Board.wb & mask) != 0) return PieceScript.Spawn(true, position, PieceType.Bishop);
        else if ((Board.wq & mask) != 0) return PieceScript.Spawn(true, position, PieceType.Queen);
        else if ((Board.wk & mask) != 0) return PieceScript.Spawn(true, position, PieceType.King);
        
        else if ((Board.bp & mask) != 0) return PieceScript.Spawn(false, position, PieceType.Pawn);
        else if ((Board.br & mask) != 0) return PieceScript.Spawn(false, position, PieceType.Rook);
        else if ((Board.bn & mask) != 0) return PieceScript.Spawn(false, position, PieceType.Knight);
        else if ((Board.bb & mask) != 0) return PieceScript.Spawn(false, position, PieceType.Bishop);
        else if ((Board.bq & mask) != 0) return PieceScript.Spawn(false, position, PieceType.Queen);
        else if ((Board.bk & mask) != 0) return PieceScript.Spawn(false, position, PieceType.King);

        return null;
    }
    void SetBoard(){
        // Reset board
        DestroySelectors();
        if (CheckIndicator != null) Destroy(CheckIndicator);

        for (int rank = 0; rank < 8; rank++){
            for (int file = 0; file < 8; file++) {
                Destroy(Pieces[rank,file]);
                SpawnPieceAt(file, rank);
            }
        }
    }
    private List<Vector2Int> GetPieceMoves(Vector2Int pos){
        List<Vector2Int> ToPositions = new List<Vector2Int>();
        foreach (int mv in Board.LegalMoves()){
            int from = mv & 63;
            int file = 7 - from%8;
            int rank = from/8;
            if (file == pos.x && rank == pos.y){
                int to = (mv >> 6) & 63;
                ToPositions.Add(new Vector2Int(7 - to%8, to/8));
            }
        }
        return ToPositions;
    }
    public void DestroyHistory(){
        for (int i = MoveButtons.Count - 1; i >= HMclock; --i){
            Destroy(MoveButtons[i]);
            MoveButtons.RemoveAt(i);
            HistoryFen.RemoveAt(i);
            HistoryType.RemoveAt(i);
            HistoryFrom.RemoveAt(i);
            HistoryTo.RemoveAt(i);
        }
    }
    public void AddHistory(Vector2Int from, Vector2Int to){
        GameObject pc = Pieces[to.y, to.x];
        Piece PcScript = pc.GetComponent<Piece>();

        HistoryFen.Add(Board.GenerateFen());
        HistoryTo.Add(to);
        HistoryFrom.Add(from);
        HistoryType.Add(PcScript.type);
        HistoryWtime.Add(WhiteClock.time);
        HistoryBtime.Add(BlackClock.time);

        string MoveText = "";
        MoveText += files[from.x];
        MoveText += from.y+1;
        MoveText += files[to.x];
        MoveText += to.y+1;

        GameObject btn = Instantiate(MoveButtonPrefab, MoveHistory.transform);
        int hmsnap = HMclock;
        btn.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => PreviewMove(hmsnap));
        btn.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = MoveText;
        MoveButtons.Add(btn);
        HMclock++;
    }
    public void PreviewMove(int hm){
        DestroySelectors();
        if (CheckIndicator != null) Destroy(CheckIndicator);

        if (timed){
            WhiteClock.PauseClock();
            BlackClock.PauseClock();
            WhiteClock.SetTime(HistoryWtime[hm]);
            BlackClock.SetTime(HistoryBtime[hm]);
        }

        if (hm >= HMclock){
            for(int i = HMclock; i <= hm; ++i){
                Board.ParseFEN(HistoryFen[i]);
                Vector2Int from = HistoryFrom[i];
                Vector2Int to = HistoryTo[i];

                GameObject pc = Pieces[from.y, from.x];
                Piece PieceScript = pc.GetComponent<Piece>();

                // Handle capture
                if (Pieces[to.y, to.x] != null) Destroy(Pieces[to.y, to.x]);

                // Move piece
                PieceScript.MoveTo(to);
                Pieces[from.y, from.x] = null;
                Pieces[to.y, to.x] = pc;

                // Hande castling
                if (PieceScript.type == PieceType.King && Math.Abs(from.x - to.x) == 2){
                    // Kingside
                    if (to.x == 6){
                        Pieces[to.y,7].GetComponent<Piece>().MoveTo(new Vector2Int(5,to.y));
                        Pieces[to.y,5] = Pieces[to.y,7];
                        Pieces[to.y,7] = null;
                    }
                    // Queenside
                    else if (to.x == 2){
                        Pieces[to.y,0].GetComponent<Piece>().MoveTo(new Vector2Int(3,to.y));
                        Pieces[to.y,3] = Pieces[to.y,0];
                        Pieces[to.y,0] = null;
                    }
                }

                PieceScript.type = HistoryType[i];
                PieceScript.ChangePieceSet("Default");
            }
        }
        else if (hm < HMclock - 1){
            for(int i = HMclock - 1; i > hm; --i){
                Board.ParseFEN(HistoryFen[i - 1]);
                Vector2Int from = HistoryFrom[i];
                Vector2Int to = HistoryTo[i];

                GameObject pc = Pieces[to.y, to.x];
                Piece PieceScript = pc.GetComponent<Piece>();

                // Move piece
                PieceScript.MoveTo(from);
                Pieces[to.y, to.x] = SpawnPieceAt(to.x, to.y); // Anti capture
                Pieces[from.y, from.x] = pc;

                // Hande anti castling
                if (PieceScript.type == PieceType.King && Math.Abs(from.x - to.x) == 2){
                    // Kingside
                    if (to.x == 6){
                        Pieces[to.y,5].GetComponent<Piece>().MoveTo(new Vector2Int(7,to.y));
                        Pieces[to.y,7] = Pieces[to.y,5];
                        Pieces[to.y,5] = null;
                    }
                    // Queenside
                    else if (to.x == 2){
                        Pieces[to.y,3].GetComponent<Piece>().MoveTo(new Vector2Int(0,to.y));
                        Pieces[to.y,0] = Pieces[to.y,3];
                        Pieces[to.y,3] = null;
                    }
                }

                PieceScript.type = HistoryType[i];
                PieceScript.ChangePieceSet("Default");
            }
        }

        // Handle checks
        if (Board.IsCheck()){
            CheckIndicator = Instantiate(CheckPrefab);
            int kingPos = Board.turn ? Board.ctz(Board.wk) : Board.ctz(Board.bk);
            CheckIndicator.transform.position = BoardToGlobalPos(7-(kingPos%8), kingPos/8, 3);
            CheckIndicator.transform.localScale = new Vector2(scale, scale);
        }

        if (timed){
            if (Board.turn) WhiteClock.StartClock();
            else BlackClock.StartClock();
        }

        HMclock = hm+1;
    }
    public void PieceClicked(Vector2Int position, bool col){
        DestroySelectors();
        if(col != Board.turn || (col && WhitePlayer!="Human") || (!col && BlackPlayer!="Human") || GameOver) return;

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
        WhiteClock.PauseClock();
        BlackClock.PauseClock();

        // Revert board if in past
        DestroyHistory();

        GameObject Piece = Pieces[from.y, from.x];
        Piece PieceScript = Piece.GetComponent<Piece>();

        DestroySelectors();
        if (CheckIndicator != null) Destroy(CheckIndicator);

        // Handle promotion
        if (PieceScript.type == PieceType.Pawn && (to.y == 0 || to.y == 7)){
            if (promote == 0){
                PromoteDialog.GetComponent<Promote>().Spawn(from, to, PieceScript.color);
                return;
            }
            else{
                if (promote == 1) PieceScript.type = PieceType.Knight;
                else if (promote == 2) PieceScript.type = PieceType.Bishop;
                else if (promote == 3) PieceScript.type = PieceType.Rook;
                else if (promote == 4) PieceScript.type = PieceType.Queen;
                PieceScript.ChangePieceSet("Default");
            }
        }
        
        // Handle capture
        if (Pieces[to.y, to.x] != null) Destroy(Pieces[to.y, to.x]);

        // Move piece
        PieceScript.MoveTo(to);
        Pieces[from.y, from.x] = null;
        Pieces[to.y, to.x] = Piece;

        // Hande castling
        if (PieceScript.type == PieceType.King && Math.Abs(from.x - to.x) == 2){
            // Kingside
            if (to.x == 6){
                Pieces[to.y,7].GetComponent<Piece>().MoveTo(new Vector2Int(5,to.y));
                Pieces[to.y,5] = Pieces[to.y,7];
                Pieces[to.y,7] = null;
            }
            // Queenside
            else if (to.x == 2){
                Pieces[to.y,0].GetComponent<Piece>().MoveTo(new Vector2Int(3,to.y));
                Pieces[to.y,3] = Pieces[to.y,0];
                Pieces[to.y,0] = null;
            }
        }

        // Update board
        int FromBB = from.y*8 + 7-from.x;
        int ToBB = to.y*8 + 7-to.x;
        Move MoveBB = (promote << 12) |(ToBB << 6) | FromBB;
        Board.move_piece(MoveBB);

        // Handle checks
        if (Board.IsCheck()){
            CheckIndicator = Instantiate(CheckPrefab);
            int kingPos = Board.turn ? Board.ctz(Board.wk) : Board.ctz(Board.bk);
            CheckIndicator.transform.position = BoardToGlobalPos(7-(kingPos%8), kingPos/8, 3);
            CheckIndicator.transform.localScale = new Vector2(scale, scale);
        }

        // Timer
        if (timed){
            if (Board.turn) BlackClock.AddTime(increment);
            else WhiteClock.AddTime(increment);
        }

        // Add to move history
        AddHistory(from, to);

        if (timed){
            if (Board.turn) WhiteClock.StartClock();
            else BlackClock.StartClock();
        }

        // Game over check
        if (Board.IsGameOver()){
            string msg = "";
            if (Board.IsCheckmate()){
                msg += "Game result : Checkmate";
                if (Board.turn)
                    msg += "\n Winner : Black";
                else
                    msg += "\n Winner : White";
                
                if (WhitePlayer != BlackPlayer){
                    if ((WhitePlayer == "Human" && !Board.turn) || (BlackPlayer == "Human" && Board.turn))
                        msg += "\n Yow Won!!";
                    else
                        msg += "\n Sorry, you lost :c";
                }
            }
            else if (Board.IsStalemate())
                msg += "Game result : Draw by stalemate";
            else if (Board.IsInsufficientMaterial())
                msg += "Game result : Draw by insufficient material";
            
            GameOver = true;
            ShowNotification("GAME OVER!", msg);
            return;
        }

        // Engine move
        if((Board.turn && WhitePlayer!="Human") || (!Board.turn && BlackPlayer!="Human")) EngineMove();
    }
    public void ShowNotification(string title, string message){
        NotificationObject.transform.Find("Title").GetComponent<TMP_Text>().text = title;
        NotificationObject.transform.Find("Message").GetComponent<TMP_Text>().text = message;
        NotificationObject.SetActive(true);
    }
    public void CloseNotification(){
        NotificationObject.SetActive(false);
    }
    public void NewGame(string opp, bool col, int time, int inc){
        HMclock = 0;
        DestroySelectors();
        if (CheckIndicator != null) Destroy(CheckIndicator);
        DestroyHistory();

        Board.ParseFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        SetBoard();
        eng.Kill();
        if(opp == ""){
            WhitePlayer = "Human";
            BlackPlayer = "Human";
            WhitePlayerIndicator.text = "You";
            BlackPlayerIndicator.text = "You";
        }
        else if (col){
            WhitePlayer = "Human";
            BlackPlayer = opp;
            WhitePlayerIndicator.text = "You";
            BlackPlayerIndicator.text = opp;
        }
        else{
            WhitePlayer = opp;
            BlackPlayer = "Human";
            WhitePlayerIndicator.text = opp;
            BlackPlayerIndicator.text = "You";
        }

        if (opp!=""){
            try{
                eng.Spawn(PlayerPrefs.GetString(opp));
            }
            catch (Exception e){
                Debug.LogError(e.Message);
                ShowNotification("ENGINE ERROR!", e.Message);
                NewGame("", true, 0, 0); 
            }
        }

        // Timer
        WhiteClock.Spawn(time*60);
        BlackClock.Spawn(time*60);
        increment = inc;
        if (time == 0){
            timed = false;
        }
        else{
            timed = true;
            WhiteClock.StartClock();
        }

        if(WhitePlayer!="Human") EngineMove();
    }
    public void TimeOut(){
        GameOver = true;
        DestroySelectors();

        string msg = "Game result : Time Out";
        if (Board.turn)
            msg += "\n Winner : Black";
        else
            msg += "\n Winner : White";
        
        if (WhitePlayer != BlackPlayer){
            if ((WhitePlayer == "Human" && !Board.turn) || (BlackPlayer == "Human" && Board.turn))
                msg += "\n Yow Won!!";
            else
                msg += "\n Sorry, you lost :c";
        }
        ShowNotification("GAME OVER!", msg);
    }
    public async void EngineMove(){
        // Send command in seperate thread
        string result = await Task.Run(() => {
            eng.SendCommand($"position fen {Board.GenerateFen()}");
            return eng.SendCommand("go movetime 1000"); 
        });
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
        NewGame("", true, 0, 0);
    }
    void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // engines
        if(!PlayerPrefs.HasKey("EngineNames"))
            PlayerPrefs.SetString("EngineNames", "");
    }
    void OnApplicationQuit(){
        eng.Kill();
    }
}
