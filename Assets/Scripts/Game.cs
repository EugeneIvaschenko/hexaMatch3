using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private LevelSession session;
    [SerializeField] private LevelUIMediator levelUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private LevelSettings settings;

    private void Start() {
        mainMenuUI.SetActive(true);
        levelUI.gameObject.SetActive(false);
    }

    public void StartNewSession() {
        session.StartNewLevel(settings);
        session.ScoreUpdated += levelUI.OnScoreUpdate;
        session.gameObject.SetActive(true);
        levelUI.gameObject.SetActive(true);
        mainMenuUI.SetActive(false);

        levelUI.SetLeftTurn(session.TurnLeft);
        levelUI.SetRightTurn(session.TurnRight);
        levelUI.SetQuit(CloseSession);
    }

    public void CloseSession() {
        session.CloseLevel();
        mainMenuUI.SetActive(true);
        levelUI.gameObject.SetActive(false);
        levelUI.ClearHandlers();
    }
}