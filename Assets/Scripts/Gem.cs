using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {
    public GemColorType ColorType { get; private set; }
    public GemShapeType ShapeType { get; private set; }

    private SpriteRenderer spriteRenderer;

    public void SetParams(ColorPair colorPair, ShapePair shapePair) {
        ColorType = colorPair.colorType;
        ShapeType = shapePair.shapeType;
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = colorPair.color;
        spriteRenderer.sprite = shapePair.shapeSprite;
    }
}

public enum GemColorType {
    Red,
    Blue,
    Green,
    Purple,
    Yellow,
    Black,
    White,
    Orange
}

public enum GemShapeType {
    Circle,
    Square,
    Triangle,
    Star,
    Cross,
    Capsule
}