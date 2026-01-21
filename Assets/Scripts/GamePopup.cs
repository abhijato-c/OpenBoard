using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GamePopup : MonoBehaviour{
    string opp = "";
    bool col = true;
    public GameObject WhiteSelector;
    public GameObject BlackSelector;
    public GameObject RandomSelector;
    public TMP_Dropdown OppSel;
    List<string> EngineNames = new List<string>();
    public void Spawn(){ 
        gameObject.SetActive(true); 

        // Get engines
        EngineNames.Clear();
        EngineNames = PlayerPrefs.GetString("EngineNames").Split(';').ToList();
        EngineNames = EngineNames.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        // Set options
        OppSel.ClearOptions();
        OppSel.options.Add(new TMP_Dropdown.OptionData("Pass & Play"));
        foreach (string opp in EngineNames){
            OppSel.options.Add(new TMP_Dropdown.OptionData(opp));
        }

        // Init values
        ChangeOpp(0);
        ChangeCol(0);
    }
    public void ChangeOpp(int Opponent){ 
        if (Opponent == 0) opp = ""; 
        else opp = EngineNames[Opponent - 1];
        OppSel.value = Opponent; 
        OppSel.RefreshShownValue();
    }
    public void ChangeCol(int color){
        // 0 for random, 1 for white, 2 for black
        RandomSelector.GetComponent<Image>().color = new Color32(70, 70, 70, 200);
        BlackSelector.GetComponent<Image>().color = new Color32(70, 70, 70, 200);
        WhiteSelector.GetComponent<Image>().color = new Color32(70, 70, 70, 200);
        if (color == 0){
            col = UnityEngine.Random.Range(0,2) == 0;
            RandomSelector.GetComponent<Image>().color = new Color32(25, 160, 100, 255);
        }
        else if (color == 1) {
            col = true;
            WhiteSelector.GetComponent<Image>().color = new Color32(25, 160, 100, 255);
        }
        else {
            col = false;
            BlackSelector.GetComponent<Image>().color = new Color32(25, 160, 100, 255);
        }
    }
    public void Submit(){
        gameObject.SetActive(false);
        Setup.Instance.NewGame(opp, col);
    }
    public void Close(){
        gameObject.SetActive(false);
    }
}
