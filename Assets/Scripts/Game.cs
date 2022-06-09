using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private LevelSession sessionPrefab;
    private LevelSession session;
    [SerializeField] private GameObject levelUI;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private LevelSettings settings;

    private void Start() {
        mainMenuUI.SetActive(true);
        levelUI.SetActive(false);
    }

    public void StartNewSession() {
        session = Instantiate(sessionPrefab);
        session.StartNewLevel(settings);
        mainMenuUI.SetActive(false);
        session.gameObject.SetActive(true);
        levelUI.SetActive(true);
    }

    public void CloseSession() {
        Destroy(session.gameObject);
        mainMenuUI.SetActive(true);
        session.gameObject.SetActive(false);
        levelUI.SetActive(false);
    }
}