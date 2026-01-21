using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineConfig : MonoBehaviour{
    public GameObject ListContainer;
    public GameObject RowPrefab;
    List<string> NewEngineNames = new List<string>();
    List<string> NewEnginePaths = new List<string>();
    List<string> OldEngineNames = new List<string>();
    List<string> OldEnginePaths = new List<string>();
    public void Spawn(){
        gameObject.SetActive(true);

        // Destroy old rows
        foreach (Transform row in ListContainer.transform) {
            Destroy(row.gameObject);
        }
        NewEngineNames = new List<string>();
        NewEnginePaths = new List<string>();
        OldEngineNames = new List<string>();
        OldEnginePaths = new List<string>();

        // Make new rows
        OldEngineNames = PlayerPrefs.GetString("EngineNames").Split(';').ToList();
        OldEngineNames = OldEngineNames.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
        foreach (string name in OldEngineNames){
            string path = PlayerPrefs.GetString(name);
            OldEnginePaths.Add(path);

            GameObject row = Instantiate(RowPrefab, ListContainer.transform);
            row.GetComponent<EngineConfigRow>().Spawn(name, path);
        }
    }
    public void AddNew(){
        GameObject row = Instantiate(RowPrefab, ListContainer.transform);
        row.GetComponent<EngineConfigRow>().Spawn("", "");
    }
    public void Save(){
        // Get the data
        foreach (Transform row in ListContainer.transform) {
            EngineConfigRow scr = row.GetComponent<EngineConfigRow>();
            string EngName = scr.EngineName;
            string EngPath = scr.EnginePath;
            if (EngName == "" || EngPath == "" || EngName.Contains(';')) continue;
            NewEngineNames.Add(EngName);
            NewEnginePaths.Add(EngPath);
        }

        // Set the names
        string names = "";
        foreach (string name in NewEngineNames){
            names += name + ";";
        }
        PlayerPrefs.SetString("EngineNames", names);

        // Delete the old paths
        foreach (string name in OldEngineNames){
            PlayerPrefs.DeleteKey(name);
        }

        // Set the new paths
        for (int i = 0; i < NewEngineNames.Count; ++i){
            PlayerPrefs.SetString(NewEngineNames[i], NewEnginePaths[i]);
        }
    }
    public void Close(){
        gameObject.SetActive(false);
    }
}
