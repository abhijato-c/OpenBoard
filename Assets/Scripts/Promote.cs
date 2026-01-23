using UnityEngine;
using Image = UnityEngine.UI.Image;

public class Promote : MonoBehaviour{
    public Vector2Int From;
    public Vector2Int To;
    public Image Queen; public Image Rook; public Image Bishop; public Image Knight;
    public Sprite WQueen; public Sprite WRook; public Sprite WBishop; public Sprite WKnight;
    public Sprite BQueen; public Sprite BRook; public Sprite BBishop; public Sprite BKnight;
    public void Spawn(Vector2Int from, Vector2Int to, bool color){
        From = from;
        To = to;
        Queen.sprite = color ? WQueen:BQueen;
        Rook.sprite = color ? WRook:BRook;
        Bishop.sprite = color ? WBishop:BBishop;
        Knight.sprite = color ? WKnight:BKnight;
        gameObject.SetActive(true);
    }
    public void PromoteTo(int PromoteTo){
        Setup.Instance.MovePiece(From, To, PromoteTo);;
        gameObject.SetActive(false);
    }
}
