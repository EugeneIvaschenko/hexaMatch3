using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Match3Animations {
    private readonly float fallingDurationForUnit = 0.15f;
    private readonly float swappingDuration = 0.4f;
    private readonly float failSwappingDuration = 0.25f;
    private readonly float gatheringDuration = 0.6f;
    private readonly float fieldRotationDuration = 0.6f;

    public void RotateField(Transform transform, Vector3 angle, TweenCallback callback = null) {
        transform.DORotate(angle, fieldRotationDuration).OnComplete(callback).OnUpdate(() => {
            Gem[] children = transform.GetComponentsInChildren<Gem>();
            foreach(var child in children) {
                child.transform.localRotation = Quaternion.Inverse(transform.rotation);
            }
        });
    }

    public void SwapGems(Transform transform1, Transform transform2, TweenCallback callback = null) {
        transform1.DOMove(transform2.position, swappingDuration);
        transform2.DOMove(transform1.position, swappingDuration).OnComplete(callback);
    }

    public void FailSwapping(Transform transform1, Transform transform2, TweenCallback callback = null) {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(transform1.DOMove(transform2.position, failSwappingDuration));
        sequence.Join(transform2.DOMove(transform1.position, failSwappingDuration));
        sequence.Append( transform1.DOMove(transform1.position, failSwappingDuration));
        sequence.Join(transform2.DOMove(transform2.position, failSwappingDuration));
        sequence.AppendCallback(callback);
    }

    public void Gathering(Transform[] transforms, TweenCallback callback = null) {
        Sequence sequence = DOTween.Sequence();
        foreach (var transform in transforms) {
            sequence.Join(transform.DOScale(0, gatheringDuration));
        }
        sequence.AppendCallback(callback);
    }

    public void FallAndRiseGems(List<Transform> oldTransforms, List<Transform[]> newTransforms, TweenCallback callback = null) {
        Sequence oldSeq = DOTween.Sequence();
        foreach (var transform in oldTransforms) {
            oldSeq.Join(transform.DOMove(transform.parent.position, Mathf.Abs((transform.parent.position - transform.position).magnitude) * fallingDurationForUnit).SetEase(Ease.Linear));
        }
        //oldSeq.AppendCallback(callback);

        Sequence newSeqMove = DOTween.Sequence();
        Sequence newSeqScale = DOTween.Sequence();
        foreach (var colOfNew in newTransforms) {
            if (colOfNew.Length == 0) continue;
            float pointOfRising = colOfNew[colOfNew.Length - 1].position.y + HexMath.smallSizeRatio;

            for (int i = 0; i < colOfNew.Length; i++) {
                Transform transform = colOfNew[i].transform;

                float startPosY = pointOfRising + i * HexMath.smallSizeRatio;
                float targetY = transform.position.y;
                transform.position = new Vector3(transform.position.x, startPosY, transform.position.z);
                newSeqMove.Join(transform.DOMoveY(targetY, Mathf.Abs(startPosY - targetY) * fallingDurationForUnit).SetEase(Ease.Linear));

                Vector3 targetScale = transform.localScale;
                transform.localScale = Vector3.zero;
                newSeqScale.Insert(fallingDurationForUnit * (i + 0.25f), transform.DOScale(targetScale, 0.5f * fallingDurationForUnit));
            }
            //RiseGems(colOfNew);
        }

        newSeqScale.AppendCallback(callback);
    }
}