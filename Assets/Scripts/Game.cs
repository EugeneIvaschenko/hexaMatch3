using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private LevelSession sessionPrefab;
    [SerializeField] private LevelUIMediator levelUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private LevelSettings settings;

    private LevelSession session;

    private void Start() {
        mainMenuUI.SetActive(true);
        levelUI.gameObject.SetActive(false);
    }

    public void StartNewSession() {
        session = Instantiate(sessionPrefab);
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
        Destroy(session.gameObject);
        mainMenuUI.SetActive(true);
        session.gameObject.SetActive(false);
        levelUI.gameObject.SetActive(false);
        levelUI.ClearHandlers();
    }
}