using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "ScriptableObjects/ShapesSO", order = 2)]
public class ShapesSO : ScriptableObject {
    public List<ShapePair> list;

    public List<GemShapeType> ShapeList() {
        List<GemShapeType> list = new List<GemShapeType>();
        foreach (var pair in this.list) {
            list.Add(pair.shapeType);
        }
        return list;
    }

    public ShapesSO GetNewShapesSO(List<GemShapeType> shapes) {
        ShapesSO so = CreateInstance(typeof(ShapesSO)) as ShapesSO;
        so.list = new List<ShapePair>();
        foreach (ShapePair pair in list) {
            if (shapes.Contains(pair.shapeType)) so.list.Add(pair);
        }
        return so;
    }
}

[System.Serializable]
public class ShapePair {
    public GemShapeType shapeType;
    public Sprite shapeSprite;
}