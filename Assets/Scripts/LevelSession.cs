using UnityEngine;
using TMPro;
using System;

public class LevelSession : MonoBehaviour {

    private HexGrid grid;
    private HexTile checkedTile;
    private HexTile ÑheckedTile {
        get { return checkedTile; }
        set {
            if (checkedTile != null)
                checkedTile.SetHighlight(false);
            checkedTile = value;
            if (checkedTile != null)
                checkedTile.SetHighlight(true);
        }
    }
    private bool isBlockedClickHandler = false;
    private GameplayLogic gameplay;

    public event Action<int> ScoreUpdated {
        add { gameplay.PointsUpdated += value; }
        remove { gameplay.PointsUpdated -= value; }
    }

    private void Awake() {
        grid = GetComponent<HexGrid>();
        grid.gameObject.SetActive(false);
    }

    public void StartNewLevel(LevelSettings settings) {
        grid.AnimationEnd += UnblockClickHandling;
        grid.SetSettings(settings);
        grid.Init();
        gameplay = new();
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
        if (ÑheckedTile == null) {
            ÑheckedTile = tile;
        } else if (ÑheckedTile == tile) {
            ÑheckedTile = null;
        } else if (!HexMath.IsCubeNeighbor(tile.axialPos, ÑheckedTile.axialPos)) {
            ÑheckedTile = tile;
        } else {
            BlockClickHandling();
            gameplay.TrySwapGemsToGathering(tile, ÑheckedTile);
            ÑheckedTile = null;
        }
    }

    private void BlockClickHandling() {
        isBlockedClickHandler = true;
    }

    private void UnblockClickHandling() {
        isBlockedClickHandler = false;
    }
}