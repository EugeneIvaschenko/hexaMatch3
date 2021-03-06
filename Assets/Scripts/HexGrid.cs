using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour {
    private int _size;
    private float _padding = 0;
    [SerializeField] private HexTile _tilePrefab;
    [SerializeField] private Gem gemPrefab;
    private ColorsSO colors;
    private ShapesSO shapes;

    public Dictionary<Vector2, HexTile> Grid { get; private set; }
    public event Action GridRefilled;

    public void SetSettings(LevelSettings settings) {
        _size = settings.gridSize;
        colors = settings.colors;
        shapes = settings.shapes;
    }

    public void RefillGrid() {
        List<HexTile[]> columns = GetColumns();
        List<Transform[]> colsOfNew = new();
        List<Transform> oldTransforms = new();

        foreach (var column in columns) {
            foreach (var tile in column) {
                if (tile.content != null)
                    oldTransforms.Add(tile.content.transform);
            }

            List<Gem> gems = RefillColumnAndGetNewGems(column);
            List<Transform> newGems = new();
            foreach (var gem in gems) {
                newGems.Add(gem.transform);
            }
            colsOfNew.Add(newGems.ToArray());
        }
        Match3AnimationsMediator.DoFallAndRiseGemsAnimation(oldTransforms, colsOfNew, () => GridRefilled?.Invoke());
    }

    private List<HexTile[]> GetColumns() {
        List<HexTile[]> columns = new();
        for (int x = -_size; x <= _size; x++) { //all columns left to right
            columns.Add(GetColumn(x, _size).ToArray());
        }
        return columns;
    }

    private List<HexTile> GetColumn(int x, int size) {
        List<HexTile> column = new();
        for (int y = HexMath.LowerYInX(x, size); y >= HexMath.HigherYinX(x, size); y--) { //all cells in column down to top
            Vector2 tilePos = new(x, y);
            tilePos = HexMath.GetGridTurnCompensatedPos(HexMath.AxialToCube(tilePos), transform.rotation.eulerAngles.z);
            HexTile tile = Grid[tilePos];
            column.Add(tile);
        }
        return column;
    }

    private List<Gem> RefillColumnAndGetNewGems(HexTile[] column) {
        List<Gem> newGems = new List<Gem>();
        for (int i = 0; i < column.Length; i++) {
            if (column[i].content != null)
                continue;
            if (i == column.Length - 1) {
                CreateNewGemFor(column[i]);
                newGems.Add(column[i].content);
                continue;
            }
            Gem gem = TakeGemFromHigherTileOrNull(column, i + 1);
            if (gem != null) {
                SetGemFor(column[i], gem);
            } else {
                CreateNewGemFor(column[i]);
                newGems.Add(column[i].content);
            }
        }
        return newGems;
    }

    private Gem TakeGemFromHigherTileOrNull(HexTile[] column, int higherIndex) {
        if (column[higherIndex].content != null) {
            Gem gem = column[higherIndex].content;
            column[higherIndex].content = null;
            return gem;
        }
        if (higherIndex < column.Length - 1)
            return TakeGemFromHigherTileOrNull(column, higherIndex + 1);
        return null;
    }

    private void SetGemFor(HexTile tile, Gem gem) {
        tile.content = gem;
        gem.transform.parent = tile.transform;
    }

    public void Init() {
        GenerateRoundGrid(_size);
        FillGrid();
    }

    private void GenerateRoundGrid(int n) {
        Grid = new Dictionary<Vector2, HexTile>();
        for (int dx = -n; dx <= n; dx++) {
            for (int dy = Mathf.Max(-n, -dx - n); dy <= Mathf.Min(n, -dx + n); dy++) {
                int dz = -dx - dy;
                Vector2 hexPos = HexMath.CubeToAxial(new Vector3(dx, dy, dz));
                Grid[hexPos] = Instantiate(_tilePrefab, HexMath.GetPixelPos(HexMath.AxialToEvenQ(hexPos), HexCoordinateSystem.EvenQOffset, _padding), Quaternion.identity, transform);
                //hexGrid[hexPos].SetText(hexPos);
                Grid[hexPos].axialPos = hexPos;
            }
        }
    }

    private void FillGrid() {
        CubeHexDirectionsFlat[] dirs = { CubeHexDirectionsFlat.N, CubeHexDirectionsFlat.NW, CubeHexDirectionsFlat.SW };
        foreach (var tile in Grid) {
            List<GemColorType> newColors = colors.ColorList();
            List<GemShapeType> newShapes = shapes.ShapeList();
            foreach (var dir in dirs) {
                RemoveDublicateColorFromColorList(ref newColors, CheckExistingLineFrom(tile.Key, dir));
                RemoveDuplicateShapeFromShapeList(ref newShapes, CheckExistingLineFrom(tile.Key, dir));
            }

            SetRandomGem(tile.Value, colors.GetNewColorsSO(newColors), shapes.GetNewShapesSO(newShapes));
        }
    }

    private void CreateNewGemFor(HexTile tile) {
        SetRandomGem(tile, colors, shapes);
    }

    private void SetRandomGem(HexTile tile, ColorsSO colors, ShapesSO shapes) {
        Gem gem = Instantiate(gemPrefab, tile.transform);
        gem.SetParams(colors.list[UnityEngine.Random.Range(0, colors.list.Count)], shapes.list[UnityEngine.Random.Range(0, shapes.list.Count)]);
        tile.content = gem;
        gem.transform.localRotation = Quaternion.Inverse(transform.rotation);
    }

    public List<GemsLine> SearchLinesToGathering() {
        List<GemsLine> lines = new();
        foreach (var tilePair in Grid) {
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
            if (line.Count >= 3)
                lines.Add(line); //TODO Refactor. Add correct check on !lines.Contains(line)
        }
    }

    private GemsLine GetMatchedLine(HexTile tile, KeyValuePair<CubeHexDirectionsFlat, CubeHexDirectionsFlat> dirs) {
        GemsLine colorLine = new() { tile };
        colorLine.AddRange(GetMatchedLine(tile, dirs.Key, GemSign.Color));
        colorLine.AddRange(GetMatchedLine(tile, dirs.Value, GemSign.Color));

        GemsLine shapeLine = new() { tile };
        shapeLine.AddRange(GetMatchedLine(tile, dirs.Key, GemSign.Shape));
        shapeLine.AddRange(GetMatchedLine(tile, dirs.Value, GemSign.Shape));

        if (colorLine.Count > shapeLine.Count)
            return colorLine;
        else
            return shapeLine;
    }

    //?????????? ????? ??????? ?????, ????????? ?? ????? ????????? ??????????? ?? ?????????? ????????.
    private GemsLine GetMatchedLine(HexTile tile, CubeHexDirectionsFlat dir, GemSign sign) {
        GemsLine colorLine = new GemsLine();
        GemsLine shapeLine = new GemsLine();
        int length = 1;
        if (sign == GemSign.Color) {
            while (true) {
                Vector3 nextTilePos = HexMath.CubeAddVector(HexMath.AxialToCube(tile.axialPos), HexMath.CubeVector(dir) * length);
                if (Grid.ContainsKey(nextTilePos) && Grid[nextTilePos].content.ColorType == tile.content.ColorType) {
                    colorLine.Add(Grid[nextTilePos]);
                    length++;
                } else
                    break;
            }
            return colorLine;
        } else {
            while (true) {
                Vector3 nextTilePos = HexMath.CubeAddVector(HexMath.AxialToCube(tile.axialPos), HexMath.CubeVector(dir) * length);
                if (Grid.ContainsKey(nextTilePos) && Grid[nextTilePos].content.ShapeType == tile.content.ShapeType) {
                    shapeLine.Add(Grid[nextTilePos]);
                    length++;
                } else
                    break;
            }
            return shapeLine;
        }
    }

    //?????????? ????? ?????? ?? ????? lineLength
    private List<Vector2> CheckExistingLineFrom(Vector2 startHex, CubeHexDirectionsFlat dir, int lineLength = 3) {
        List<Vector2> hexes = new() {
            startHex
        };
        for (int i = 1; i < lineLength; i++) {
            Vector3 nextHex = HexMath.CubeNeighbor(hexes[hexes.Count - 1], dir);
            if (Grid.ContainsKey(nextHex)) {
                hexes.Add(nextHex);
            } else
                break;
        }
        return hexes;
    }

    private bool RemoveDublicateColorFromColorList(ref List<GemColorType> colors, List<Vector2> line) {
        if (line.Count < 3)
            return false;
        GemColorType lastColor = Grid[line[1]].content.ColorType;

        for (int i = 2; i < line.Count; i++) {
            if (lastColor != Grid[line[i]].content.ColorType)
                return false;
        }

        if (colors.Contains(lastColor))
            colors.Remove(lastColor);
        return true;
    }

    private bool RemoveDuplicateShapeFromShapeList(ref List<GemShapeType> shapes, List<Vector2> line) {
        if (line.Count < 3)
            return false;
        GemShapeType lastShape = Grid[line[1]].content.ShapeType;

        for (int i = 2; i < line.Count; i++) {
            if (lastShape != Grid[line[i]].content.ShapeType)
                return false;
        }

        if (shapes.Contains(lastShape))
            shapes.Remove(lastShape);
        return true;
    }

    public List<HexTile> GetTiles(List<GemsLine> lines) {
        List<HexTile> list = new();
        foreach (var line in lines) {
            foreach (var tile in line) {
                if (!list.Contains(tile))
                    list.Add(tile);
            }
        }
        return list;
    }

    public void DestroyGems(List<HexTile> tiles) {
        foreach (var tile in tiles) {
            if (tile.content == null || tile.content.gameObject == null)
                continue;
            tile.content.gameObject.SetActive(false);
            Destroy(tile.content.gameObject);
            tile.content = null;
        }
    }

    public void ClearGrid() {
        foreach(var tilePair in Grid) {
            Destroy(tilePair.Value.content.gameObject);
            tilePair.Value.content = null;
            Destroy(tilePair.Value.gameObject);
        }
        Grid.Clear();
        GridRefilled = null;
        transform.eulerAngles = Vector3.zero;
    }
}