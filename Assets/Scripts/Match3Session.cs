using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3Session : MonoBehaviour {

    private HexGrid grid;
    private HexTile checkedTile;

    private bool isBlockedClickHandler = false;

    private void Start() {
        grid = GetComponent<HexGrid>();
        grid.Init();
        grid.animationEnd += UnblockClickHandler;
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.autorotateToPortrait = false;
        grid.FillGrid();
        SetListeners();
    }

    public void TurnLeft() {
        Debug.Log("Turn left");
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        grid.TurnField(RotationDirection.Left);
    }

    public void TurnRight() {
        Debug.Log("Turn Right");
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
        if(checkedTile == null) {
            checkedTile = tile;
            checkedTile.SetHighlight(true);
        } else if (checkedTile == tile) {
            checkedTile.SetHighlight(false);
            checkedTile = null;
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
}