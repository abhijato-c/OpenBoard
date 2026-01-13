using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.EventSystems;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
public class Piece : MonoBehaviour, IPointerDownHandler{
    public bool color; // true = white, false = black
    public Vector2Int Position; // file, rank
    public PieceType type; // Pawn, Rook etc

    public void MoveTo(Vector2Int pos) {
        Position = pos;
        transform.position = new Vector3(pos.x - 3.5f, pos.y - 3.5f, 2f);
    }
    public void ChangePieceSet(string PieceSet){
        // Determine file path
        string File = (color ? "W" : "B") + type.ToString().ToLower();
        string Path = PieceSet + "PieceSet/" + File;

        // Load the sprite
        Sprite newSprite = Resources.Load<Sprite>(Path);
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }
    public void Spawn(bool color, Vector2Int pos, PieceType type, string PieceSet = "Default"){
        this.color = color;
        this.type = type;
        name = type.ToString();
        MoveTo(pos);
        ChangePieceSet(PieceSet);
    }
    public void OnPointerDown(PointerEventData eventData){
        Setup.Instance.PieceClicked(Position, color);
    }
}
