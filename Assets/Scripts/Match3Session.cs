using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Match3Session : MonoBehaviour {

    private HexGrid grid;
    private HexTile checkedTile;

    [SerializeField] private TextMeshProUGUI scoreText;
    private bool isBlockedClickHandler = false;
    private GameplayLogic gameplay = new GameplayLogic();

    private void Awake() {
        grid = GetComponent<HexGrid>();
        grid.AnimationEnd += UnblockClickHandler;
        grid.Init();
        gameplay.PointsUpdated += UpdateScore;
        gameplay.grid = grid;
        gameplay.MoveEnded += UnblockClickHandler;
        gameplay.Init();
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.autorotateToPortrait = false;
        grid.FillGrid();
        SetListeners();
    }

    public void TurnLeft() {
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        grid.TurnField(RotationDirection.Left);
    }

    public void TurnRight() {
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        grid.TurnField(RotationDirection.Right);
    }

    private void SetListeners() {
        foreach (var tile in grid.hexGrid) {
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
            BlockClickHandler();
            gameplay.TrySwapGemsToGathering(tile, checkedTile);
            checkedTile = null;
        }
    }

    private void BlockClickHandler() {
        isBlockedClickHandler = true;
    }

    private void UnblockClickHandler() {
        isBlockedClickHandler = false;
    }

    private void UpdateScore(int score) {
        if (scoreText != null) scoreText.text = "Score: " + score.ToString();
    }
}