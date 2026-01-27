using UnityEngine;
using UnityEngine.EventSystems;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
public class Piece : MonoBehaviour, IPointerDownHandler{
    public bool color; // true = white, false = black
    public Vector2Int Position; // file, rank
    public PieceType type; // Pawn, Rook etc
    public float speed = 0.1f;
    Vector3 TargetPos;

    public void MoveTo(Vector2Int pos) {
        Position = pos;
        TargetPos = Setup.Instance.BoardToGlobalPos(pos.x, pos.y, 2);
    }
    public void ChangePieceSet(string PieceSet){
        // Determine file path
        string File = (color ? "W" : "B") + type.ToString().ToLower();
        string Path = PieceSet + "PieceSet/" + File;

        // Load the sprite
        Sprite newSprite = Resources.Load<Sprite>(Path);
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }
    public GameObject Spawn(bool color, Vector2Int pos, PieceType type, string PieceSet = "Default"){
        this.color = color;
        this.type = type;
        name = type.ToString();
        Position = pos;

        transform.position = Setup.Instance.BoardToGlobalPos(pos.x, pos.y, 2);
        transform.localScale = new Vector2(Setup.Instance.scale,Setup.Instance.scale);
        TargetPos = transform.position;
        ChangePieceSet(PieceSet);
        
        return gameObject;
    }
    public void OnPointerDown(PointerEventData eventData){
        Setup.Instance.PieceClicked(Position, color);
    }

    void Update(){
        if (Vector2.Distance(transform.position, TargetPos) > 0.001f) 
            transform.position = Vector3.Lerp(transform.position, TargetPos, speed);
    }
}
