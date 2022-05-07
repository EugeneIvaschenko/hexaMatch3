using System;
using System.Collections;
using System.Collections.Generic;

public class GameplayLogic {
    public HexGrid grid;

    private int score = 0;
    public event Action<int> PointsUpdated;
    public event Action GemsDestroyed;
    public event Action MoveEnded;

    public void Init() {
        GemsDestroyed += OnGemsDestroyed;
    }

    public void TrySwapGemsToGathering(HexTile tile, HexTile checkedTile) {
        tile.SwapGems(checkedTile);
        List<GemsLine> lines = grid.SearchLinesToGathering();
        if (lines.Count == 0) {
            tile.SwapGems(checkedTile);
            grid.DoFailSwapAnimation(tile, checkedTile);
        }
        else {
            grid.DoSwapGemsAnimation(tile, checkedTile, () => GatherGems(grid.GetTiles(lines)));
        }
    }

    private void CheckGatheringByAutofill() {
        List<GemsLine> lines = grid.SearchLinesToGathering();
        if (lines.Count == 0) {
            MoveEnded?.Invoke();
        }
        else {
            GatherGems(grid.GetTiles(lines));
        }
    }

    private void GatherGems(List<HexTile> gems) {
        AddScore(gems.Count);
        grid.DoGatheringAnimation(gems, () => DestroyGems(gems));
    }

    private void DestroyGems(List<HexTile> gems) {
        grid.DestroyGems(gems);
        GemsDestroyed?.Invoke();
    }

    private void OnGemsDestroyed() {
        grid.RefillEmptyTiles();

        CheckGatheringByAutofill();
    }

    private void AddScore(int points) {
        score += points;
        PointsUpdated?.Invoke(score);
    }
}