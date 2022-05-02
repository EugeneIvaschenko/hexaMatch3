using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Match3Session : MonoBehaviour {

    private HexGrid grid;
    private HexTile checkedTile;

    [SerializeField] private TextMeshProUGUI scoreText;

    private int score = 0;
    private bool isBlockedClickHandler = false;

    private void Awake() {
        grid = GetComponent<HexGrid>();
        grid.Init();
        grid.AnimationEnd += UnblockClickHandler;
        grid.GemsGathered += SumScore;
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
            grid.SearchToExplodeBySwap(tile, checkedTile);
            checkedTile = null;
        }
    }

    private void BlockClickHandler() {
        isBlockedClickHandler = true;
    }

    private void UnblockClickHandler() {
        isBlockedClickHandler = false;
    }

    private void SumScore(List<Gem> gatheredGems) {
        AddScore(gatheredGems.Count);
    }

    private void AddScore(int points) {
        score += points;
        if(scoreText != null) scoreText.text = "Score: " + score.ToString();
    }
}