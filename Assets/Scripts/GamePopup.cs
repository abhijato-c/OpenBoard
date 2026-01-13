using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePopup : MonoBehaviour{
    string opp = "Stockfish";
    bool col = true;
    public GameObject WhiteSelector;
    public GameObject BlackSelector;
    public GameObject RandomSelector;
    public TMP_Dropdown OppSel;
    public void Spawn(){ gameObject.SetActive(true); }
    public void ChangeOpp(int Opponent){ 
        if (Opponent == 0) opp = "Stockfish"; 
        else if (Opponent == 1) opp = "PassPlay"; 
        else if (Opponent == 2) opp = "Jimbo"; 
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
    public void Submit(){
        gameObject.SetActive(false);
        Setup.Instance.NewGame(opp, col);
    }
}
