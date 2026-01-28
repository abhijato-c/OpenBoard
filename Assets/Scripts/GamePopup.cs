using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class GamePopup : MonoBehaviour{
    public GameObject WhiteSelector;
    public GameObject BlackSelector;
    public GameObject RandomSelector;
    public TMP_Dropdown OppSel;
    public TMP_Text TimeText;
    public TMP_Text IncText;
    public Slider TimeSlider;
    public Slider IncSlider;
    public TMP_Text P2NameText;
    public TMP_InputField Player1Name;
    public TMP_InputField Player2Name;

    List<string> EngineNames = new List<string>();
    int[] Times = {1, 2, 3, 4, 5, 10, 15, 20, 25, 30, 40, 50, 60};
    int[] Increments = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10};
    string opp = "";
    bool col = true;
    int Time = 0;
    int Inc = 0;
    public void Spawn(){ 
        gameObject.SetActive(true); 

        // Get engines
        EngineNames.Clear();
        EngineNames = PlayerPrefs.GetString("EngineNames").Split(';').ToList();
        EngineNames = EngineNames.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

        // Set GUI
        OppSel.ClearOptions();
        OppSel.options.Add(new TMP_Dropdown.OptionData("Pass & Play"));
        foreach (string opp in EngineNames){
            OppSel.options.Add(new TMP_Dropdown.OptionData(opp));
        }
        TimeSlider.maxValue = Times.Length - 1;
        TimeSlider.value = 5;
        IncSlider.maxValue = Increments.Length - 1;
        IncSlider.value = 0;

        Player1Name.text = "Player 1";
        Player2Name.text = "Player 2";

        // Init values
        ChangeOpp(0);
        ChangeCol(0);
    }
    public void ChangeOpp(int Opponent){ 
        if (Opponent == 0) {
            opp = ""; 
            P2NameText.text = "Player 2 Name";
            Player2Name.text = "Player 2";
        }
        else {
            opp = EngineNames[Opponent - 1];
            P2NameText.text = "Engine Name";
            Player2Name.text = opp;
        }
        OppSel.value = Opponent; 
        OppSel.RefreshShownValue();
    }
    public void ChangeCol(int color){
        // 0 for random, 1 for white, 2 for black
        RandomSelector.GetComponent<Image>().color = new Color32(70, 70, 70, 200);
        BlackSelector.GetComponent<Image>().color = new Color32(70, 70, 70, 200);
        WhiteSelector.GetComponent<Image>().color = new Color32(70, 70, 70, 200);
        if (color == 0){
            col = Random.Range(0,2) == 0;
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
    public void RefreshTime(){
        Time = Times[(int)TimeSlider.value];
        TimeText.text = Time.ToString() + " min";
    }
    public void RefreshInc(){
        Inc = Increments[(int)IncSlider.value];
        IncText.text = Inc.ToString() + " sec";
    }
    public void Submit(){
        string WhitePlayer = col ? Player1Name.text : Player2Name.text;
        string BlackPlayer = col ? Player2Name.text : Player1Name.text;
        gameObject.SetActive(false);
        Setup.Instance.NewGame(opp, col, Time, Inc, WhitePlayer, BlackPlayer);
    }
    public void Close(){
        gameObject.SetActive(false);
    }
}
