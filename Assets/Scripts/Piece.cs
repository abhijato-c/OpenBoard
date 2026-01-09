using UnityEngine;
using UnityEngine.Rendering;

public enum PieceType { Pawn, Knight, Bishop, Rook, Queen, King }
public class Piece : MonoBehaviour{
    public bool color; // true = white, false = black
    public Vector2Int position; // file, rank
    public PieceType type; // Pawn, Rook etc

    public void MoveTo(Vector2Int position) {
        this.position = position;

        // Convert grid (0-7) to Unity World Space (-3.5 to 3.5)
        float x = position.x - 3.5f;
        float y = position.y - 3.5f;
        transform.position = new Vector3(x, y, 0f);
    }
    public void ChangePieceSet(string PieceSet){
        // Determine file path
        string File = (color ? "W" : "B") + type.ToString().ToLower();
        string Path = PieceSet + "PieceSet/" + File;

        // Load the sprite
        Sprite newSprite = Resources.Load<Sprite>(Path);
        GetComponent<SpriteRenderer>().sprite = newSprite;
    }
    public void Spawn(bool color, Vector2Int position, PieceType type, string PieceSet = "Default"){
        this.color = color;
        this.type = type;
        this.name = type.ToString();
        MoveTo(position);
        ChangePieceSet(PieceSet);
    }
}
