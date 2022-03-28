using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    [SerializeField] [Min(3)] private int _size;
    [SerializeField] private float _padding;
    [SerializeField] private HexTile _tilePrefab;

    public Dictionary<Vector2, HexTile> hexGrid { get; private set; }
    private HexCoordinateSystem HexCoordinateSystem = HexCoordinateSystem.Axial;

    public void Init() {
        GenerateRoundGrid(_size);
    }

    private void GenerateRoundGrid(int n) {
        hexGrid = new Dictionary<Vector2, HexTile>();
        for (int dx = -n; dx <= n; dx++) {
            for (int dy = Mathf.Max(-n, -dx - n); dy <= Mathf.Min(n, -dx + n); dy++) {
                int dz = -dx - dy;
                Vector2 hexPos = HexMath.CubeToAxial(new Vector3(dx, dy, dz));
                hexGrid[hexPos] = Instantiate(_tilePrefab, HexMath.GetPixelPos(HexMath.AxialToEvenQ(hexPos), HexCoordinateSystem.EvenQOffset, _padding), Quaternion.identity, transform);
                //hexGrid[hexPos].SetText(hexPos);
                hexGrid[hexPos].axialPos = hexPos;
            }
        }
    }

    public Vector2 GetPosOfTile(HexTile a) {
        foreach(var hex in hexGrid) {
            if (hex.Value == a) return hex.Key;
        }
        throw new System.Exception("Has no found searched hex");
    }

    public int Size() {
        return _size;
    }
}