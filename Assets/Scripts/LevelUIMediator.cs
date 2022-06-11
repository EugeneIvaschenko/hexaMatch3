using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUIMediator : MonoBehaviour {
    [SerializeField] private Button leftTurn;
    [SerializeField] private Button rightTurn;
    [SerializeField] private Button quitLevel;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetLeftTurn(UnityAction action) {
        leftTurn.onClick.AddListener(action);
    }
    public void SetRightTurn(UnityAction action) {
        rightTurn.onClick.AddListener(action);
    }

    public void SetQuit(UnityAction action) {
        quitLevel.onClick.AddListener(action);
    }

    public void OnScoreUpdate(int score) {
        scoreText.text = "Score: " + score.ToString();
    }

    public void ClearHandlers() {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach(var button in buttons) {
            button.onClick.RemoveAllListeners();
        }
    }
}