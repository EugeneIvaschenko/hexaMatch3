using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    [SerializeField] [Min(2)] private int _size;
    [SerializeField] private float _padding;
    [SerializeField] private HexTile _tilePrefab;
    [SerializeField] private Gem gemPrefab;
    [SerializeField] private ColorsSO colors;
    [SerializeField] private ShapesSO shapes;

    public Dictionary<Vector2, HexTile> hexGrid { get; private set; }

    public event Action AnimationEnd;
    
    private Match3Animations animator = new Match3Animations();

    public void TurnField(RotationDirection dir) {
        Vector3 angle = new Vector3(0, 0, dir == RotationDirection.Left ? 60f : -60f);
        animator.RorateField(transform, transform.rotation.eulerAngles + angle, EndAnimation);
    }

    public void RefillEmptyTiles() {
        Dictionary<Vector2, HexTile> emptyTiles = new Dictionary<Vector2, HexTile>();

        for (int x = -Size(); x <= Size(); x++) { //all columns left to right
            for (int y = HexMath.LowerYInX(x, Size()); y >= HexMath.HigherYinX(x, Size()); y--) { //all cells in column down to top
                Vector2 tilePos = new Vector2(x, y);
                if (transform.rotation.eulerAngles.z >= 180) tilePos *= -1; //filling in reverse order if z-rotation is negative
                HexTile tile = hexGrid[tilePos];
                if (tile.content == null) emptyTiles.Add(tilePos, tile);
            }
        }

        foreach (var tilePair in emptyTiles) {
            TryFillDownIn(tilePair.Value);
        }

        if (emptyTiles.Count > 0) animator.AddCallback(RefillEmptyTiles);
    }

    private void TryFillDownIn(HexTile tile) {
        if (tile.content == null) {
            Vector3 northNeighbor = HexMath.RotateTileAround(HexMath.CubeNeighbor(tile.axialPos, CubeHexDirectionsFlat.N), HexMath.AxialToCube(tile.axialPos), Mathf.RoundToInt(transform.rotation.eulerAngles.z / 60));
            if (IsTileExist(northNeighbor, Size())) {
                if (hexGrid[northNeighbor].content != null) {
                    DoMoveGemAnimation(hexGrid[northNeighbor], tile);
                    TryFillDownIn(hexGrid[northNeighbor]);
                }
            }
            else {
                CreateNewGem(tile);
            }
        }
    }
    



    //OLD MODEL-VIEW

    private void CreateNewGem(HexTile tile) {
        SetRandomGem(tile, colors, shapes);
        animator.RiseGem(tile.content.transform);
    }


    //ANIMATIONS

    public void DoSwapGemsAnimation(HexTile tile1, HexTile tile2, Action callback) {
        //TODO Refactor:  ��������� �� �������� ������� ���� � ������-������� �����
        tile1.content.transform.SetParent(tile1.transform);
        tile2.content.transform.SetParent(tile2.transform);
        animator.SwapGems(tile1.content.transform, tile2.content.transform, () => callback());
    }

    public void DoFailSwapAnimation(HexTile tile1, HexTile tile2) {
        animator.FailSwapping(tile1.content.transform, tile2.content.transform, EndAnimation);
    }

    public void DoMoveGemAnimation(HexTile from, HexTile to) {
        to.content = from.content;
        to.content.transform.parent = to.transform;
        from.content = null;
        animator.MoveToParent(to.content.transform);
    }

    public void DoGatheringAnimation(List<HexTile> tiles, Action callback) {
        List<Transform> transforms = new List<Transform>();
        foreach(var tile in tiles) {
            transforms.Add(tile.content.transform);
        }
        animator.Gathering(transforms.ToArray(), () => callback());
    }

    private void EndAnimation() {
        AnimationEnd?.Invoke();
    }



    //NEW MODEL/////////

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

    public void FillGrid() {
        CubeHexDirectionsFlat[] dirs = { CubeHexDirectionsFlat.N, CubeHexDirectionsFlat.NW, CubeHexDirectionsFlat.SW };
        foreach (var tile in hexGrid) {
            List<GemColorType> newColors = colors.ColorList();
            List<GemShapeType> newShapes = shapes.ShapeList();
            foreach (var dir in dirs) {
                RemoveDublicateColorFromColorList(ref newColors, CheckExistingLineFrom(tile.Key, Size(), dir));
                RemoveDuplicateShapeFromShapeList(ref newShapes, CheckExistingLineFrom(tile.Key, Size(), dir));
            }

            SetRandomGem(tile.Value, colors.GetNewColorsSO(newColors), shapes.GetNewShapesSO(newShapes));
        }
    }

    private void SetRandomGem(HexTile tile) {
        SetRandomGem(tile, colors, shapes);
    }

    private void SetRandomGem(HexTile tile, ColorsSO colors, ShapesSO shapes) {
        Gem gem = Instantiate(gemPrefab, tile.transform);
        gem.SetParams(colors.list[UnityEngine.Random.Range(0, colors.list.Count)], shapes.list[UnityEngine.Random.Range(0, shapes.list.Count)]);
        tile.content = gem;
        gem.transform.localRotation = Quaternion.Inverse(transform.rotation);
    }

    public List<GemsLine> SearchLinesToGathering() {
        List<GemsLine> lines = new List<GemsLine>();
        foreach (var tilePair in hexGrid) {
            FindMatchLines(ref lines, tilePair.Value);
        }
        return lines;
    }

    private void FindMatchLines(ref List<GemsLine> lines, HexTile tile) {
        Dictionary<CubeHexDirectionsFlat, CubeHexDirectionsFlat> oppositeDirPairs = new Dictionary<CubeHexDirectionsFlat, CubeHexDirectionsFlat> {
            { CubeHexDirectionsFlat.NW, CubeHexDirectionsFlat.SE},
            { CubeHexDirectionsFlat.N, CubeHexDirectionsFlat.S},
            { CubeHexDirectionsFlat.NE, CubeHexDirectionsFlat.SW}
        };

        foreach (KeyValuePair<CubeHexDirectionsFlat, CubeHexDirectionsFlat> dirs in oppositeDirPairs) {
            GemsLine line = GetMatchedLine(tile, dirs);
            if (line.Count >= 3) lines.Add(line); //TODO Refactor. Add correct check on !lines.Contains(line)
        }
    }

    private GemsLine GetMatchedLine(HexTile tile, KeyValuePair<CubeHexDirectionsFlat, CubeHexDirectionsFlat> dirs) {
        GemsLine colorLine = new GemsLine() { tile };
        colorLine.AddRange(GetMatchedLine(tile, dirs.Key, GemSign.Color));
        colorLine.AddRange(GetMatchedLine(tile, dirs.Value, GemSign.Color));

        GemsLine shapeLine = new GemsLine() { tile };
        shapeLine.AddRange(GetMatchedLine(tile, dirs.Key, GemSign.Shape));
        shapeLine.AddRange(GetMatchedLine(tile, dirs.Value, GemSign.Shape));

        if (colorLine.Count > shapeLine.Count) return colorLine;
        else return shapeLine;
    }

    //���������� ����� ������� �����, ��������� �� ����� ��������� ����������� �� ���������� ��������.
    private GemsLine GetMatchedLine(HexTile tile, CubeHexDirectionsFlat dir, GemSign sign) {
        GemsLine colorLine = new GemsLine();
        GemsLine shapeLine = new GemsLine();
        int length = 1;
        if (sign == GemSign.Color) {
            while (true) {
                Vector3 nextTilePos = HexMath.CubeAddVector(HexMath.AxialToCube(tile.axialPos), HexMath.CubeVector(dir) * length);
                if (IsTileExist(nextTilePos, Size()) && hexGrid[nextTilePos].content.ColorType == tile.content.ColorType) {
                    colorLine.Add(hexGrid[nextTilePos]);
                    length++;
                }
                else break;
            }
            return colorLine;
        }
        else {
            while (true) {
                Vector3 nextTilePos = HexMath.CubeAddVector(HexMath.AxialToCube(tile.axialPos), HexMath.CubeVector(dir) * length);
                if (IsTileExist(nextTilePos, Size()) && hexGrid[nextTilePos].content.ShapeType == tile.content.ShapeType) {
                    shapeLine.Add(hexGrid[nextTilePos]);
                    length++;
                }
                else break;
            }
            return shapeLine;
        }
    }

    //���������� ����� ������ �� ����� lineLength
    private List<Vector2> CheckExistingLineFrom(Vector2 startHex, int gridSize, CubeHexDirectionsFlat dir, int lineLength = 3) {
        List<Vector2> hexes = new List<Vector2> {
            startHex
        };
        for (int i = 1; i < lineLength; i++) {
            Vector3 nextHex = HexMath.CubeNeighbor(hexes[hexes.Count - 1], dir);
            if (IsTileExist(nextHex, gridSize)) {
                hexes.Add(nextHex);
            }
            else break;
        }
        return hexes;
    }

    private bool RemoveDublicateColorFromColorList(ref List<GemColorType> colors, List<Vector2> line) {
        if (line.Count < 3) return false;
        GemColorType lastColor = hexGrid[line[1]].content.ColorType;

        for (int i = 2; i < line.Count; i++) {
            if (lastColor != hexGrid[line[i]].content.ColorType) return false;
        }

        if (colors.Contains(lastColor)) colors.Remove(lastColor);
        return true;
    }

    private bool RemoveDuplicateShapeFromShapeList(ref List<GemShapeType> shapes, List<Vector2> line) {
        if (line.Count < 3) return false;
        GemShapeType lastShape = hexGrid[line[1]].content.ShapeType;

        for (int i = 2; i < line.Count; i++) {
            if (lastShape != hexGrid[line[i]].content.ShapeType) return false;
        }

        if (shapes.Contains(lastShape)) shapes.Remove(lastShape);
        return true;
    }


    public List<HexTile> GetTiles(List<GemsLine> lines) {
        List<HexTile> list = new List<HexTile>();
        foreach (var line in lines) {
            foreach (var tile in line) {
                if (!list.Contains(tile)) list.Add(tile);
            }
        }
        return list;
    }

    public void DestroyGems(List<HexTile> tiles) {
        foreach (var tile in tiles) {
            if (tile.content == null || tile.content.gameObject == null) continue;
            tile.content.gameObject.SetActive(false);
            tile.content = null;
        }
    }

    private bool IsTileExist(Vector3 startHex, int gridSize) {
        return HexMath.MaxAbs(startHex) <= gridSize;
    }

    public Vector2 GetPosOfTile(HexTile a) {
        foreach (var hex in hexGrid) {
            if (hex.Value == a) return hex.Key;
        }
        throw new System.Exception("Has no found searched hex");
    }

    public int Size() {
        return _size;
    }
}