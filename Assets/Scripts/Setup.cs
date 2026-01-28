using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
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
    public GameObject WhitePieceCap;
    public GameObject BlackPieceCap;
    public TMP_Text WhiteAdv;
    public TMP_Text BlackAdv;
    public TMP_Text WhitePlayerIndicator;
    public TMP_Text BlackPlayerIndicator;
    public TMP_Text WhitePlayerIndicator2;
    public TMP_Text BlackPlayerIndicator2;
    public GameObject PieceDisplayPrefab;
    public ClockManager WhiteClock;
    public ClockManager BlackClock;
    public ExportManager ExportWindow;
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
    string WhiteEngine;
    string BlackEngine;
    string WhiteName;
    string BlackName;
    bool timed = false;
    int increment = 0;
    bool GameOver = false;
    GameObject CheckIndicator;
    List<string> PGNmoves = new List<string>();

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
            PGNmoves.RemoveAt(i);
        }
    }
    public void AddHistory(Vector2Int from, Vector2Int to, string san){
        GameObject pc = Pieces[to.y, to.x];
        Piece PcScript = pc.GetComponent<Piece>();

        HistoryFen.Add(Board.GenerateFen());
        HistoryTo.Add(to);
        HistoryFrom.Add(from);
        HistoryType.Add(PcScript.type);
        HistoryWtime.Add(WhiteClock.time);
        HistoryBtime.Add(BlackClock.time);
        PGNmoves.Add(san);

        GameObject btn = Instantiate(MoveButtonPrefab, MoveHistory.transform);
        int hmsnap = HMclock;
        btn.GetComponent<Button>().onClick.AddListener(() => PreviewMove(hmsnap));
        btn.transform.Find("Text").gameObject.GetComponent<TMP_Text>().text = san;
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

        RefreshAdvantageIndicator();

        if (timed){
            if (Board.turn) WhiteClock.StartClock();
            else BlackClock.StartClock();
        }

        HMclock = hm+1;
    }
    public void PieceClicked(Vector2Int position, bool col){
        DestroySelectors();
        if(col != Board.turn || (col && WhiteEngine != "") || (!col && BlackEngine != "") || GameOver) return;

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
        string san = Board.MovePieceSAN(MoveBB);

        // Handle checks
        if (Board.IsCheck()){
            CheckIndicator = Instantiate(CheckPrefab);
            int kingPos = Board.turn ? Board.ctz(Board.wk) : Board.ctz(Board.bk);
            CheckIndicator.transform.position = BoardToGlobalPos(7-(kingPos%8), kingPos/8, 3);
            CheckIndicator.transform.localScale = new Vector2(scale, scale);
        }

        RefreshAdvantageIndicator();

        // Timer
        if (timed){
            if (Board.turn) BlackClock.AddTime(increment);
            else WhiteClock.AddTime(increment);
        }

        // Add to move history
        AddHistory(from, to, san);

        // Game over check
        if (Board.IsGameOver()){
            string msg = "";
            if (Board.IsCheckmate()){
                msg += "Game result : Checkmate";
                if (Board.turn)
                    msg += "\n Winner : " + BlackName;
                else
                    msg += "\n Winner : " + WhiteName;
                
                if (WhiteEngine != "" || BlackEngine != ""){
                    if ((WhiteEngine == "" && !Board.turn) || (BlackEngine == "Human" && Board.turn))
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

        if (timed){
            if (Board.turn) WhiteClock.StartClock();
            else BlackClock.StartClock();
        }

        // Engine move
        if((Board.turn && WhiteEngine != "") || (!Board.turn && BlackEngine != "")) EngineMove();
    }
    public void RefreshAdvantageIndicator(){
        int queens = math.countbits(Board.wq) - math.countbits(Board.bq);
        int rooks = math.countbits(Board.wr) - math.countbits(Board.br);
        int bishops = math.countbits(Board.wb) - math.countbits(Board.bb);
        int knights = math.countbits(Board.wn) - math.countbits(Board.bn);
        int pawns = math.countbits(Board.wp) - math.countbits(Board.bp);
        int adv = queens*9 + rooks*5 + bishops*3 + knights*3 + pawns;

        foreach (Transform child in WhitePieceCap.transform) Destroy(child.gameObject);
        foreach (Transform child in BlackPieceCap.transform) Destroy(child.gameObject);

        for(int i = 0; i < math.abs(queens); ++i){
            bool col = (queens > 0) ? true : false;
            GameObject obj = Instantiate(PieceDisplayPrefab);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(col ? "DefaultPieceSet/Bqueen" : "DefaultPieceSet/Wqueen");
            obj.transform.SetParent(col ? WhitePieceCap.transform : BlackPieceCap.transform, false);
            obj.transform.localScale = Vector3.one;
        }
        for(int i = 0; i < math.abs(rooks); ++i){
            bool col = (rooks > 0) ? true : false;
            GameObject obj = Instantiate(PieceDisplayPrefab);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(col ? "DefaultPieceSet/Brook" : "DefaultPieceSet/Wrook");
            obj.transform.SetParent(col ? WhitePieceCap.transform : BlackPieceCap.transform, false);
            obj.transform.localScale = Vector3.one;
        }
        for(int i = 0; i < math.abs(bishops); ++i){
            bool col = (bishops > 0) ? true : false;
            GameObject obj = Instantiate(PieceDisplayPrefab);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(col ? "DefaultPieceSet/Bbishop" : "DefaultPieceSet/Wbishop");
            obj.transform.SetParent(col ? WhitePieceCap.transform : BlackPieceCap.transform, false);
            obj.transform.localScale = Vector3.one;
        }
        for(int i = 0; i < math.abs(knights); ++i){
            bool col = (knights > 0) ? true : false;
            GameObject obj = Instantiate(PieceDisplayPrefab);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(col ? "DefaultPieceSet/Bknight" : "DefaultPieceSet/Wknight");
            obj.transform.SetParent(col ? WhitePieceCap.transform : BlackPieceCap.transform, false);
            obj.transform.localScale = Vector3.one;
        }
        for(int i = 0; i < math.abs(pawns); ++i){
            bool col = (pawns > 0) ? true : false;
            GameObject obj = Instantiate(PieceDisplayPrefab);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>(col ? "DefaultPieceSet/Bpawn" : "DefaultPieceSet/Wpawn");
            obj.transform.SetParent(col ? WhitePieceCap.transform : BlackPieceCap.transform, false);
            obj.transform.localScale = Vector3.one;
        }

        WhiteAdv.text = adv.ToString("+0;-0");
        BlackAdv.text = (-adv).ToString("+0;-0");
    }
    public void ShowNotification(string title, string message){
        NotificationObject.transform.Find("Title").GetComponent<TMP_Text>().text = title;
        NotificationObject.transform.Find("Message").GetComponent<TMP_Text>().text = message;
        NotificationObject.SetActive(true);
    }
    public void CloseNotification(){
        NotificationObject.SetActive(false);
    }
    public void NewGame(string opp, bool col, int time, int inc, string wp, string bp){
        HMclock = 0;
        DestroySelectors();
        if (CheckIndicator != null) Destroy(CheckIndicator);
        DestroyHistory();
        GameOver = false;

        Board.ParseFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        SetBoard();
        eng.Kill();
        if(opp == ""){
            WhiteEngine = "";
            BlackEngine = "";
        }
        else if (col){
            WhiteEngine = "";
            BlackEngine = opp;
        }
        else{
            WhiteEngine = opp;
            BlackEngine = "";
        }
        WhiteName = wp;
        BlackName = bp;
        WhitePlayerIndicator.text = WhiteName;
        BlackPlayerIndicator.text = BlackName;
        WhitePlayerIndicator2.text = WhiteName;
        BlackPlayerIndicator2.text = BlackName;

        if (opp!=""){
            try{
                eng.Spawn(PlayerPrefs.GetString(opp));
            }
            catch (Exception e){
                Debug.LogError(e.Message);
                ShowNotification("ENGINE ERROR!", e.Message);
                NewGame("", true, 0, 0, "Player 1", "Player 2"); 
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

        RefreshAdvantageIndicator();

        if(WhiteEngine != "") EngineMove();
    }
    public void TimeOut(){
        GameOver = true;
        DestroySelectors();

        string msg = "Game result : Time Out";
        if (Board.turn)
            msg += "\n Winner : " + BlackName;
        else
            msg += "\n Winner : " + WhiteName;
        
        if (WhiteEngine != "" || BlackEngine != ""){
            if ((WhiteEngine == "" && !Board.turn) || (BlackEngine == "" && Board.turn))
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
    public void OpenExportWin(){
        string res = "*";
        if (Board.IsGameOver()){
            if (Board.IsCheckmate()){
                if (Board.turn) res = "0-1";
                else res = "1-0";
            }
            else res = "½-½";
        }

        string Pgn = "";
        Pgn += "[Event \"Match\"]\n";
        Pgn += "[Site \"OpenBoard\"]\n";
        Pgn += "[Date \"" + DateTime.Now.ToString("yyyy.MM.dd") + "\"]\n";
        Pgn += "[Round \"1\"]\n";
        Pgn += "[White \"" + WhiteName + "\"]\n";
        Pgn += "[Black \"" + BlackName + "\"]\n";
        Pgn += "[Result \"" + res + "\"]\n";
        Pgn += "\n";

        for(int i=0; i<PGNmoves.Count; ++i){
            if(i%2 == 0) Pgn += (i/2+1).ToString() + ".";
            Pgn += PGNmoves[i] + " ";
        }
        if (res != "*") Pgn += res;

        ExportWindow.Spawn(Board.GenerateFen(), Pgn);
    }
    void Start(){
        GenerateBoard(LightColor, DarkColor);
        NewGame("", true, 0, 0, "Player 1", "Player 2");
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
