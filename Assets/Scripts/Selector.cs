using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour, IPointerDownHandler{
    public Vector2Int position; // file, rank
    public Vector2Int PiecePos; // Position of the piece it moves
    public void Spawn(Vector2Int from, Vector2Int to){
        name = $"Selector {to.x} {to.y}";
        position = to;
        PiecePos = from;
        transform.position = Setup.Instance.BoardToGlobalPos(to.x, to.y, 1);
        transform.localScale = new Vector2(Setup.Instance.scale, Setup.Instance.scale);
    }
    public void OnPointerDown(PointerEventData eventData){
        Setup.Instance.MovePiece(PiecePos, position);
    }
}
