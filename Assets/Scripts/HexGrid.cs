using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    [SerializeField] [Min(3)] private int _size;
    [SerializeField] private float _padding;
    [SerializeField] private HexTile _tilePrefab;

    public Dictionary<Vector2, HexTile> hexGrid { get; private set; }
    private HexCoordinateSystem HexCoordinateSystem = HexCoordinateSystem.Axial;

    private void Start() {
        GenerateRoundGrid(_size);
    }

    private void GenerateRoundGrid(int n) {
        hexGrid = new Dictionary<Vector2, HexTile>();
        for (int dx = -n; dx <= n; dx++) {
            for (int dy = Mathf.Max(-n, -dx - n); dy <= Mathf.Min(n, -dx + n); dy++) {
                int dz = -dx - dy;
                Vector2 hexPos = HexMath.CubeToAxial(new Vector3(dx, dy, dz));
                hexGrid[hexPos] = Instantiate(_tilePrefab, HexMath.GetPixelPos(HexMath.AxialToEvenQ(hexPos), HexCoordinateSystem.EvenQOffset, _padding), Quaternion.identity, transform);
                hexGrid[hexPos].SetText(hexPos);
            }
        }
    }

    public void ConvertateGrid() {
        Dictionary<Vector2, HexTile> newHexGrid = new Dictionary<Vector2, HexTile>();
        
        foreach(var hexPair in hexGrid) {
            Vector2 newPos;
            if (HexCoordinateSystem == HexCoordinateSystem.EvenQOffset) {
                newPos = HexMath.EvenQToAxial(hexPair.Key);
            } else {
                newPos = HexMath.AxialToEvenQ(hexPair.Key);
            }
            newHexGrid[newPos] = hexPair.Value;
            hexPair.Value.SetText(newPos);
        }
        hexGrid = newHexGrid;

        if (HexCoordinateSystem == HexCoordinateSystem.EvenQOffset) {
            HexCoordinateSystem = HexCoordinateSystem.Axial;
        }
        else {
            HexCoordinateSystem = HexCoordinateSystem.EvenQOffset;
        }
    }

    public Vector2 GetPosOfTile(HexTile a) {
        foreach(var hex in hexGrid) {
            if (hex.Value == a) return hex.Key;
        }
        throw new System.Exception("Has no found searched hex");
    }
}