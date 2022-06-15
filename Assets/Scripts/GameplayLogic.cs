using System;
using System.Collections.Generic;

public class GameplayLogic {
    public HexGrid grid;

    private int score = 0;
    public int targetScore = 0;

    public event Action<int> ScoreUpdated;
    public event Action MoveEnded;
    public event Action TargetScoreAchived;

    public void Init() {
        score = 0;
        grid.GridRefilled += CheckGatheringByAutofill;
    }

    public void TrySwapGemsToGathering(HexTile tile, HexTile checkedTile) {
        tile.SwapGems(checkedTile);
        List<GemsLine> lines = grid.SearchLinesToGathering();
        if (lines.Count == 0) {
            tile.SwapGems(checkedTile);
            Match3AnimationsMediator.DoFailSwapAnimation(tile, checkedTile, () => MoveEnded?.Invoke());
        } else {
            Match3AnimationsMediator.DoSwapGemsAnimation(tile, checkedTile, () => GatherGems(grid.GetTiles(lines)));
        }
    }

    private void CheckGatheringByAutofill() {
        List<GemsLine> lines = grid.SearchLinesToGathering();
        if (lines.Count == 0) {
            MoveEnded?.Invoke();
        } else {
            GatherGems(grid.GetTiles(lines));
        }
    }

    private void GatherGems(List<HexTile> gems) {
        AddScore(gems.Count);
        Match3AnimationsMediator.DoGatheringAnimation(gems, () => DestroyGems(gems));
    }

    private void DestroyGems(List<HexTile> gems) {
        grid.DestroyGems(gems);
        grid.RefillGrid();
    }

    private void AddScore(int points) {
        score += points;
        ScoreUpdated?.Invoke(score);
        if (score >= targetScore && targetScore != 0)
            TargetScoreAchived?.Invoke();
    }
}