using System.IO;
using SFB;
using TMPro;
using UnityEngine;

public class ExportManager : MonoBehaviour{
    public TMP_Text Fen;
    public TMP_Text Pgn;
    public void Spawn(string fen, string pgn){
        Fen.text = fen;
        Pgn.text = pgn;
        gameObject.SetActive(true);
    }
    public void CopyFen(){
        GUIUtility.systemCopyBuffer = Fen.text;
    }
    public void SavePgn(){
        var extensions = new[] {
            new ExtensionFilter("Portable Game Notation", "pgn"),
            new ExtensionFilter("All Files", "*"),
        };
        StandaloneFileBrowser.SaveFilePanelAsync("Save Text File", "", "GameExport.pgn", extensions, (string path) => {
            if (!string.IsNullOrEmpty(path)) Write(path);
        });
    }

    void Write(string path){
        try{
            File.WriteAllText(path, Pgn.text);
        }
        catch (System.Exception e){
            Debug.LogError("Error saving file: " + e.Message);
        }
    }
}
