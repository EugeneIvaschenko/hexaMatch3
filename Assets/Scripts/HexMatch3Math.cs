using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexMatch3Math {

    public static int gridSize;

    public static List<List<HexTile>> SearchLinesToGathering(HexTile tile, HexTile checkedTile, Dictionary<Vector2, HexTile> hexGrid) {
        List<List<HexTile>> lines = new List<List<HexTile>>();
        foreach (var tilePair in hexGrid) {
            FindMatchLines(ref lines, tilePair.Value, hexGrid);
        }
        return lines;
    }

    public static void FindMatchLines(ref List<List<HexTile>> lines, HexTile tile, Dictionary<Vector2, HexTile> hexGrid) {
        Dictionary<CubeHexDirectionsFlat, CubeHexDirectionsFlat> oppositeDirPairs = new Dictionary<CubeHexDirectionsFlat, CubeHexDirectionsFlat> {
            { CubeHexDirectionsFlat.NW, CubeHexDirectionsFlat.SE},
            { CubeHexDirectionsFlat.N, CubeHexDirectionsFlat.S},
            { CubeHexDirectionsFlat.NE, CubeHexDirectionsFlat.SW}
        };

        foreach (KeyValuePair<CubeHexDirectionsFlat, CubeHexDirectionsFlat> dirs in oppositeDirPairs) {
            List<HexTile> line = GetMatchedLine(tile, dirs, hexGrid);
            if (line.Count >= 3) lines.Add(line); //TODO Refactor. Add correct check on !lines.Contains(line)
        }
    }

    private static List<HexTile> GetMatchedLine(HexTile tile, KeyValuePair<CubeHexDirectionsFlat, CubeHexDirectionsFlat> dirs, Dictionary<Vector2, HexTile> hexGrid) {
        List<HexTile> colorLine = new List<HexTile>() { tile };
        colorLine.AddRange(GetMatchedLine(tile, dirs.Key, GemSign.Color, hexGrid));
        colorLine.AddRange(GetMatchedLine(tile, dirs.Value, GemSign.Color, hexGrid));

        List<HexTile> shapeLine = new List<HexTile>() { tile };
        shapeLine.AddRange(GetMatchedLine(tile, dirs.Key, GemSign.Shape, hexGrid));
        shapeLine.AddRange(GetMatchedLine(tile, dirs.Value, GemSign.Shape, hexGrid));

        if (colorLine.Count > shapeLine.Count) return colorLine;
        else return shapeLine;
    }

    //Возвращает самую длинную линию, состоящию из гемов полностью совпадающих по указанному признаку.
    private static List<HexTile> GetMatchedLine(HexTile tile, CubeHexDirectionsFlat dir, GemSign sign, Dictionary<Vector2, HexTile> hexGrid) {
        List<HexTile> colorLine = new List<HexTile>();
        List<HexTile> shapeLine = new List<HexTile>();
        int length = 1;
        if (sign == GemSign.Color) {
            while (true) {
                Vector3 nextTilePos = HexMath.CubeAddVector(HexMath.AxialToCube(tile.axialPos), HexMath.CubeVector(dir) * length);
                if (IsTileExist(nextTilePos, gridSize) && hexGrid[nextTilePos].content.ColorType == tile.content.ColorType) {
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
                if (IsTileExist(nextTilePos, gridSize) && hexGrid[nextTilePos].content.ShapeType == tile.content.ShapeType) {
                    shapeLine.Add(hexGrid[nextTilePos]);
                    length++;
                }
                else break;
            }
            return shapeLine;
        }
    }

    public static bool IsTileExist(Vector3 startHex, int gridSize) {
        return HexMath.MaxAbs(startHex) <= gridSize;
    }
}