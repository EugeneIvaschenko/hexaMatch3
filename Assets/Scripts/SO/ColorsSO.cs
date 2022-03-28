using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorData", menuName = "ScriptableObjects/ColorsSO", order = 1)]
public class ColorsSO : ScriptableObject {
    public List<ColorPair> list;

    public List<GemColorType> ColorList() {
        List<GemColorType> list = new List<GemColorType>();
        foreach(var pair in this.list) {
             list.Add(pair.colorType);
        }
        return list;
    }

    public ColorsSO GetNewColorsSO(List<GemColorType> colors) {
        ColorsSO so = CreateInstance(typeof(ColorsSO)) as ColorsSO;
        so.list = new List<ColorPair>();
        foreach(ColorPair pair in list) {
            if (colors.Contains(pair.colorType)) so.list.Add(pair);
        }
        return so;
    }
}

[System.Serializable]
public class ColorPair {
    public GemColorType colorType;
    public Color color;
}