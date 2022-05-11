using System.Collections.Generic;
using UnityEngine;

public static class HexMath {
    //column == x, row == y
    public static float smallSizeRatio = 0.866025404f;

    public static Vector2 GetPosByIndex(float x, float y, HexCoordinateSystem coordSys, float padding = 0) {
        if (coordSys == HexCoordinateSystem.EvenQOffset) {
            float xPos = x * 0.75f + padding * x * 0.75f;
            float evenOffset = x % 2 * 0.5f;
            float yPos = (-y + evenOffset) * smallSizeRatio + padding * -y * smallSizeRatio + padding * evenOffset;
            return new Vector2(xPos, yPos);
        }
        else return Vector2.zero;
    }

    public static Vector2 GetPixelPos(Vector2 vector2, HexCoordinateSystem coordSys, float padding = 0) {
        return GetPosByIndex(vector2.x, vector2.y, coordSys, padding);
    }

    public static Vector3 GetPosByCube(Vector3 hexPos, float offset = 0) {
        return new Vector3();
    }

    public static Vector2 CubeToAxial(Vector3 pos) {
        return new Vector2(pos.x, pos.y);
    }

    public static Vector3 AxialToCube(Vector2 pos) {
        return new Vector3(pos.x, pos.y, -pos.x - pos.y);
    }

    public static Vector2 AxialToEvenQ(Vector2 pos) {
        return new Vector2(pos.x, pos.y + (pos.x + pos.x % 2) / 2);
    }

    public static Vector2 EvenQToAxial(Vector2 pos) {
        return new Vector2(pos.x, pos.y - (pos.x + pos.x % 2) / 2);
    }

    public static Vector2 CubeToEvenQ(Vector3 pos) {
        return new Vector2(pos.x, pos.y + (pos.x + pos.x % 2) / 2);
    }

    public static Vector3 EvenQToCube(Vector2 pos) {
        float y = pos.y - (pos.x + pos.x % 2) / 2;
        return new Vector3(pos.x, y, -pos.x - y);
    }

    public static int LowerYInX(int x, int gridSize) {
        return x < 0 ? gridSize : gridSize - x;
    }

    public static int HigherYinX(int x, int gridSize) {
        return x < 0 ? -gridSize - x : -gridSize;
    }

    public static int HexesInX(int x, int gridSize) {
        return gridSize * 2 + 1 - Mathf.Abs(x);
    }

    public static readonly Dictionary<CubeHexDirectionsFlat, Vector3> cubeDirections = new Dictionary<CubeHexDirectionsFlat, Vector3>() {
        { CubeHexDirectionsFlat.NW, new Vector3(-1, 0, +1)},
        { CubeHexDirectionsFlat.N, new Vector3(0, -1, +1)},
        { CubeHexDirectionsFlat.NE, new Vector3(+1, -1, 0)},
        { CubeHexDirectionsFlat.SE, new Vector3(+1, 0, -1)},
        { CubeHexDirectionsFlat.S, new Vector3(0, +1, -1)},
        { CubeHexDirectionsFlat.SW, new Vector3(-1, +1, 0)}
    };

    public static readonly Dictionary<CubeHexDirectionsFlat, CubeHexDirectionsFlat> oppositeDirs = new Dictionary<CubeHexDirectionsFlat, CubeHexDirectionsFlat>() {
        { CubeHexDirectionsFlat.NW, CubeHexDirectionsFlat.SE},
        { CubeHexDirectionsFlat.N, CubeHexDirectionsFlat.S},
        { CubeHexDirectionsFlat.NE, CubeHexDirectionsFlat.SW},
        { CubeHexDirectionsFlat.SE, CubeHexDirectionsFlat.NW},
        { CubeHexDirectionsFlat.S, CubeHexDirectionsFlat.N},
        { CubeHexDirectionsFlat.SW, CubeHexDirectionsFlat.NE}
    };

    public static Vector3 CubeVector(CubeHexDirectionsFlat direction) {
        return cubeDirections[direction];
    }

    public static Vector3 CubeAddVector(Vector3 hex, Vector3 vector) {
        return hex + vector;
    }

    public static Vector3 CubeNeighbor(Vector3 hex, CubeHexDirectionsFlat direction) {
        return hex + cubeDirections[direction];
    }

    public static Vector3 CubeNeighbor(Vector2 hex, CubeHexDirectionsFlat direction) {
        return CubeNeighbor(AxialToCube(hex), direction);
    }

    public static float CubeDistance(Vector3 a, Vector3 b) {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
    }

    public static float CubeDistance(Vector2 a, Vector2 b) {
        return CubeDistance(AxialToCube(a), AxialToCube(b));
    }

    public static bool IsCubeNeighbor(Vector2 a, Vector2 b) {
        return IsCubeNeighbor(AxialToCube(a), AxialToCube(b));
    }

    public static bool IsCubeNeighbor(Vector3 a, Vector3 b) {
        if (CubeDistance(a, b) == 1) return true;
        else return false;
    }

    public static Vector3 RotateHexAround(Vector3 hex, Vector3 aroundHex, int sixthsCircle) {
        hex -= aroundHex;
        for (int i = 0; i < Mathf.Abs(sixthsCircle); i++) {
            if (sixthsCircle < 0) hex = new Vector3(-hex.z, -hex.x, -hex.y);
            else hex = new Vector3(-hex.y, -hex.z, -hex.x);
        }
        hex += aroundHex;
        return hex;
    }

    public static Vector3 RotateHexAroundCenter(Vector3 hex, int sixthsCircle) {
        return RotateHexAround(hex, Vector3.zero, sixthsCircle);
    }

    public static Vector3 GetGridTurnCompensatedPos(Vector3 hex, float eulerAngle) {
        return RotateHexAroundCenter(hex, GetSixthsOfRotation(eulerAngle));
    }

    public static int GetSixthsOfRotation(float eulerAngle) {
        return Mathf.RoundToInt(eulerAngle / 60);
    }

    public static float MaxAbs(Vector3 pos) {
        return Mathf.Max(Mathf.Abs(pos.x), Mathf.Abs(pos.y), Mathf.Abs(HexMath.AxialToCube(pos).z));
    }
}