using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LevelUIMediator : MonoBehaviour {
    [SerializeField] private Button leftTurn;
    [SerializeField] private Button rightTurn;
    [SerializeField] private Button quitLevel;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject completeLevelMenu;
    [SerializeField] private Button quitLevel2;

    public void SetLeftTurn(UnityAction action) {
        leftTurn.onClick.AddListener(action);
    }
    public void SetRightTurn(UnityAction action) {
        rightTurn.onClick.AddListener(action);
    }

    public void SetQuit(UnityAction action) {
        quitLevel.onClick.AddListener(action);
        quitLevel2.onClick.AddListener(action);
    }

    public void OnScoreUpdate(int score) {
        scoreText.text = "Score: " + score.ToString();
    }

    public void ResetUI() {
        completeLevelMenu.SetActive(false);
        OnScoreUpdate(0);
        ClearListeners();
    }

    private void ClearListeners() {
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (var button in buttons) {
            button.onClick.RemoveAllListeners();
        }
    }

    public void OpenCompleteLevelMenu() {
        quitLevel.gameObject.SetActive(false);
        completeLevelMenu.SetActive(true);
    }
}