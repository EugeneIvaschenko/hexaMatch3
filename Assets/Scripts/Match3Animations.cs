using DG.Tweening;
using UnityEngine;

public class Match3Animations {

    private Sequence fillingSequence;
    private float fallingDuration = 0.3f;
    private float fillingDuration = 0.35f;
    private float swappingDuration = 0.4f;
    private float failSwappingDuration = 0.25f;
    private float explosionDuration = 0.6f;
    private float fieldRotationDuration = 0.6f;

    public void RorateField(Transform transform, Vector3 angle, TweenCallback callback = null) {
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
            sequence.Join(transform.DOScale(0, explosionDuration));
        }
        sequence.AppendCallback(callback);
    }

    public void MoveToParent(Transform transform) {
        if (IsAnactiveAndComplete()) fillingSequence = DOTween.Sequence();
        fillingSequence.Join(transform.DOLocalMove(Vector3.zero, fallingDuration).SetEase(Ease.Linear));
    }

    public void RiseGem(Transform transform) {
        if (IsAnactiveAndComplete()) fillingSequence = DOTween.Sequence();
        Vector3 scale = transform.localScale;
        transform.localScale = Vector3.zero;
        fillingSequence.Join(transform.DOScale(scale, fillingDuration).SetEase(Ease.InSine));
    }

    private bool IsAnactiveAndComplete() {
        return !fillingSequence.IsActive() || fillingSequence.IsComplete();
    }

    public void AddCallback(TweenCallback callback) {
        if (fillingSequence.IsActive())
            fillingSequence.AppendCallback(callback);
    }
}