using UnityEngine;

public class LevelSession : MonoBehaviour {

    private HexGrid grid;
    private HexTile checkedTile;
    private HexTile �heckedTile {
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
    private LevelUIMediator levelUI;

    private void Awake() {
        grid = GetComponent<HexGrid>();
        grid.gameObject.SetActive(false);
    }

    public void StartNewLevel(LevelSettings settings, LevelUIMediator levelUIMediator) {
        grid.SetSettings(settings);
        levelUI = levelUIMediator;
        levelUI.SetLeftTurn(TurnLeft);
        levelUI.SetRightTurn(TurnRight);
        gameplay = new();
        gameplay.ScoreUpdated += levelUI.OnScoreUpdate;
        gameplay.targetScore = settings.targetScore;
        gameplay.grid = grid;
        gameplay.MoveEnded += UnblockClickHandling;
        gameplay.TargetScoreAchived += OnTargetScoreAchived;
        gameplay.Init();
        grid.Init();
        SetTileListeners();
    }

    public void ClearLevel() {
        grid.ClearGrid();
        gameObject.SetActive(false);
        UnblockClickHandling();
    }

    private void TurnLeft() {
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        Match3AnimationsMediator.DoTurnFieldAnimation(transform, RotationDirection.Left, UnblockClickHandling);
    }

    private void TurnRight() {
        if (isBlockedClickHandler) return;
        isBlockedClickHandler = true;
        Match3AnimationsMediator.DoTurnFieldAnimation(transform, RotationDirection.Right, UnblockClickHandling);
    }

    private void SetTileListeners() {
        foreach (var tile in grid.Grid) {
            tile.Value.OnHexMouseClick += OnTileClick;
            tile.Value.OnHexSwipe += OnTileHold;
        }
    }

    private void OnTileHold(HexTile tile) {
        Vector2 swipeVector = Camera.main.ScreenToWorldPoint(Input.mousePosition) - tile.transform.position;
        float angle = Vector2.SignedAngle(Vector2.up, swipeVector);
        int sixths = HexMath.GetSixthsOfRotation(angle);
        Vector3 relatedNeighbor = HexMath.RotateHexAroundCenter(HexMath.CubeDirections[CubeHexDirectionsFlat.N], -sixths);
        relatedNeighbor = HexMath.GetGridTurnCompensatedPos(relatedNeighbor, grid.transform.rotation.eulerAngles.z);
        Vector3 neighbor = HexMath.CubeAddVector(HexMath.AxialToCube(tile.axialPos), relatedNeighbor);
        if (grid.Grid.ContainsKey(neighbor)) {
            BlockClickHandling();
            gameplay.TrySwapGemsToGathering(tile, grid.Grid[neighbor]);
        }
        �heckedTile = null;
    }

    private void OnTileClick(HexTile tile) {
        if (isBlockedClickHandler) return;
        if (�heckedTile == null) {
            �heckedTile = tile;
        } else if (�heckedTile == tile) {
            �heckedTile = null;
        } else if (!HexMath.IsCubeNeighbor(tile.axialPos, �heckedTile.axialPos)) {
            �heckedTile = tile;
        } else {
            BlockClickHandling();
            gameplay.TrySwapGemsToGathering(tile, �heckedTile);
            �heckedTile = null;
        }
    }

    private void BlockClickHandling() {
        isBlockedClickHandler = true;
    }

    private void UnblockClickHandling() {
        isBlockedClickHandler = false;
    }

    private void OnTargetScoreAchived() {
        gameplay.MoveEnded += FinishLevel;
    }

    private void FinishLevel() {
        Debug.Log("FinishLevel");
        gameplay.MoveEnded -= FinishLevel;
        BlockClickHandling();
        levelUI.OpenCompleteLevelMenu();
    }
}