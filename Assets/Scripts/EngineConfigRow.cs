using SFB;
using TMPro;
using UnityEngine;

public class EngineConfigRow : MonoBehaviour{
    public string EngineName;
    public string EnginePath;
    public TMP_InputField EngineNameInput;
    public TMP_Text EnginePathText;
    public void Delete(){
        Destroy(gameObject);
    }
    public void NameChanged(string NewName){
        EngineName = NewName;
    }
    public void Browse(){
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", "", false);
        if (paths.Length != 1) return;
        EnginePath = paths[0];
        EnginePathText.text = ".../" + EnginePath[EnginePath.LastIndexOf('/')..];
    }
    public void Spawn(string name, string path){
        EngineName = name;
        EngineNameInput.text = name;
        EngineNameInput.textComponent.SetText(name);

        if (path == "") return;
        EnginePath = path;
        EnginePathText.text = ".../" + EnginePath[EnginePath.LastIndexOf('/')..];
    }
}