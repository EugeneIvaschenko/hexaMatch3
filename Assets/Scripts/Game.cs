using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private LevelSession session;
    [SerializeField] private LevelUIMediator levelUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private LevelSettings settings;

    private void Start() {
        mainMenuUI.SetActive(true);
        levelUI.gameObject.SetActive(false);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
    }

    public void StartNewSession() {
        session.StartNewLevel(settings, levelUI);
        session.gameObject.SetActive(true);
        levelUI.gameObject.SetActive(true);
        mainMenuUI.SetActive(false);

        levelUI.SetQuit(CloseSession);
    }

    private void CloseSession() {
        session.ClearLevel();
        mainMenuUI.SetActive(true);
        levelUI.gameObject.SetActive(false);
        levelUI.ResetUI();
    }
}