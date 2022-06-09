using UnityEngine;
using TMPro;

public class LevelSession : MonoBehaviour {

    private HexGrid grid;
    private HexTile checkedTile;

    [SerializeField] private TextMeshProUGUI scoreText;
    private bool isBlockedClickHandler = false;
    private GameplayLogic gameplay;

    private void Awake() {
        grid = GetComponent<HexGrid>();
        grid.gameObject.SetActive(false);
    }

    public void StartNewLevel(LevelSettings settings) {
        grid.AnimationEnd += UnblockClickHandling;
        grid.SetSettings(settings);
        grid.Init();
        gameplay = new();
        gameplay.PointsUpdated += UpdateScore;
        gameplay.grid = grid;
        gameplay.MoveEnded += UnblockClickHandling;
        gameplay.Init();
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.autorotateToPortrait = false;
        grid.FillGrid();
        SetListeners();
    }

    public void TurnLeft() {
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        Match3AnimationsMediator.DoTurnFieldAnimation(transform, RotationDirection.Left, UnblockClickHandling);
    }

    public void TurnRight() {
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        Match3AnimationsMediator.DoTurnFieldAnimation(transform, RotationDirection.Right, UnblockClickHandling);
    }

    private void SetListeners() {
        foreach (var tile in grid.Grid) {
            tile.Value.OnHexMouseClick += OnTileClick;
        }
    }

    private void OnTileClick(HexTile tile) {
        if (isBlockedClickHandler) return;
        if (checkedTile == null) {
            checkedTile = tile;
            checkedTile.SetHighlight(true);
        } else if (checkedTile == tile) {
            checkedTile.SetHighlight(false);
            checkedTile = null;
        } else if (!HexMath.IsCubeNeighbor(tile.axialPos, checkedTile.axialPos)) {
            checkedTile.SetHighlight(false);
            checkedTile = tile;
            checkedTile.SetHighlight(true);
        } else {
            checkedTile.SetHighlight(false);
            BlockClickHandling();
            gameplay.TrySwapGemsToGathering(tile, checkedTile);
            checkedTile = null;
        }
    }

    private void BlockClickHandling() {
        isBlockedClickHandler = true;
    }

    private void UnblockClickHandling() {
        isBlockedClickHandler = false;
    }

    private void UpdateScore(int score) {
        if (scoreText != null) scoreText.text = "Score: " + score.ToString();
    }
}