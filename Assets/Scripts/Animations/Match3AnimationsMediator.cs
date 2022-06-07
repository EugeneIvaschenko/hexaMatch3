using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Match3AnimationsMediator {
    private static readonly Match3Animations animator = new();

    public static void DoTurnFieldAnimation(Transform transform, RotationDirection dir, TweenCallback callback) {
        Vector3 angle = new(0, 0, dir == RotationDirection.Left ? 60f : -60f);
        animator.RotateField(transform, transform.rotation.eulerAngles + angle, callback);
    }

    public static void DoSwapGemsAnimation(HexTile tile1, HexTile tile2, TweenCallback callback) {
        //TODO Refactor:  Избавится от привязки объекта гема к парент-объекту тайла
        tile1.content.transform.SetParent(tile1.transform);
        tile2.content.transform.SetParent(tile2.transform);
        animator.SwapGems(tile1.content.transform, tile2.content.transform, callback);
    }

    public static void DoFailSwapAnimation(HexTile tile1, HexTile tile2, TweenCallback callback) {
        animator.FailSwapping(tile1.content.transform, tile2.content.transform, callback);
    }

    public static void DoGatheringAnimation(List<HexTile> tiles, TweenCallback callback) {
        animator.Gathering(tiles.Select(t => t.content.transform).ToArray(), callback);
    }

    public static void DoFallAndRiseGemsAnimation(List<Transform> oldGems, List<Transform[]> newGems, TweenCallback callback) {
        animator.FallAndRiseGems(oldGems, newGems, callback);
    }
}