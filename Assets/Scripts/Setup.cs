using UnityEngine;

public class Setup : MonoBehaviour{
    public GameObject SquarePrefab;
    public GameObject SquaresFolder;
    char[] files = {'a','b','c','d','e','f','g','h'};
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
    void Start(){
        GenerateBoard(new Color(1f,1f,1f), new Color(0f,0f,0f));
    }

    void Update(){
        
    }
}
