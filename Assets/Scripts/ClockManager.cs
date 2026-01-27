using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClockManager : MonoBehaviour{
    public TMP_Text TimeText;

    public float time;
    bool running = false;

    public void Spawn(float seconds) {
        running = false;
        time = seconds;
        UpdateUI();
        if (seconds == 0) gameObject.GetComponent<Image>().color = Setup.Instance.RegualarClockColor;
    }

    public void StartClock() => running = true;
    public void PauseClock() => running = false;
    public void AddTime(int seconds) {
        time += seconds;
        UpdateUI();
    }
    public void SetTime(float seconds){
        time = seconds;
        UpdateUI();
    }

    void Update() {
        if (!running) return;
        if (time <= 0){
            time = 0;
            running = false;
            UpdateUI();
            Setup.Instance.TimeOut();
        }
        
        time -= Time.deltaTime;
        if (time < 0) time = 0;
        UpdateUI();
    }

    void UpdateUI() {
        int minutes = (int) time / 60;
        int seconds = (int)time % 60;
        TimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        gameObject.GetComponent<Image>().color = time < 60 ? Setup.Instance.AlarmClockColor : Setup.Instance.RegualarClockColor;
    }
}
