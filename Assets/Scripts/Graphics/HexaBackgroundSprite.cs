using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HexaBackgroundSprite : MonoBehaviour {
    public Color mainColor { get; set; } = Color.black;

    private Texture2D tex;
    private SpriteRenderer ren;
    private float radius = 7;
    private float thickness = 2;
    private float distanceInRadius = 2.1f;
    private float slisingSize = 10;

    public void Init(){
        tex = CreateTexture();
        ren = GetComponent<SpriteRenderer>();
        ren.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        ren.material.mainTexture = tex;
        ren.size *= slisingSize;
    }

    private Texture2D CreateTexture() {
        List<Vector2> circlesPoses = new List<Vector2>();
        int colCount = 15;//must be odd
        int rowCount = 15;
        float distance = radius * distanceInRadius + thickness * 2;

        for (int x = 0; x < colCount; x++) {
            for (int y = 0; y < rowCount; y++) {
                Vector2 vector2 = new Vector2(x * distance, (y * distance + distance * (x % 2 == 0 ? 0 : 0.5f)) / HexMath.smallSizeRatio);
                circlesPoses.Add(vector2);
            }
        }

        int texWidth = Mathf.RoundToInt((colCount - 1) * distance);
        int texHeight = Mathf.RoundToInt((rowCount - 1) * distance / HexMath.smallSizeRatio);
        Texture2D texture = new(texWidth, texHeight);

        for (int x = 0; x < texWidth; x++) {
            for (int y = 0; y < texHeight; y++) {
                texture.SetPixel(x, y, mainColor);
            }
        }

        foreach (var circle in circlesPoses) {
            int squareSize = Mathf.RoundToInt(radius * 2 + thickness * 2);

            int squareMinX = Mathf.RoundToInt(circle.x - squareSize * 0.5f);
            int squareMinY = Mathf.RoundToInt(circle.y - squareSize * 0.5f);
            int squareMaxX = squareMinX + squareSize;
            int squareMaxY = squareMinY + squareSize;

            int drawMinX = Mathf.Clamp(squareMinX, 0, texWidth);
            int drawMinY = Mathf.Clamp(squareMinY, 0, texHeight);
            int drawMaxX = Mathf.Clamp(squareMaxX, 0, texWidth);
            int drawMaxY = Mathf.Clamp(squareMaxY, 0, texHeight);

            for (int x = drawMinX; x < drawMaxX; x++) {
                for (int y = drawMinY; y < drawMaxY; y++) {
                    texture.SetPixel(x, y, GetColorForPoint(circle, new Vector2(x, y)));
                }
            }
        }
        texture.Apply();

        return texture;
    }

    private Color GetColorForPoint(Vector2 point1, Vector2 point2) {
        Color color;
        float distance = GetDistance(point1, point2);
        if (distance > radius + thickness) {
            color = mainColor;
        } else if (distance < radius) {
            color = new Color(mainColor.r, mainColor.g, mainColor.b, 0);
        } else {
            color = new Color(mainColor.r, mainColor.g, mainColor.b, (distance - radius) / thickness);
        }
        return color;
    }

    private float GetDistance(Vector2 point1, Vector2 point2) {
        return Mathf.Abs((point1 - point2).magnitude);
    }
}