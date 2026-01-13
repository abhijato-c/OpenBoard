using UnityEngine;
using UnityEngine.EventSystems;

public class Selector : MonoBehaviour, IPointerDownHandler{
    public Vector2Int position; // file, rank
    public Vector2Int PiecePos; // Position of the piece it moves
    public bool isCapture;
    public void Spawn(Vector2Int from, Vector2Int to){
        name = $"Selector {to.x} {to.y}";
        position = to;
        PiecePos = from;
        transform.position = new Vector3(to.x - 3.5f, to.y - 3.5f, 1f);
    }
    public void OnPointerDown(PointerEventData eventData){
        Setup.Instance.MovePiece(PiecePos, position);
    }
}
