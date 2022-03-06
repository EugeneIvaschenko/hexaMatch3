using System.Collections;
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

    private static Dictionary<CubeHexDirectionsFlat, Vector3> cubeDirections = new Dictionary<CubeHexDirectionsFlat, Vector3>() {
        { CubeHexDirectionsFlat.NW, new Vector3(-1, 0, +1)},
        { CubeHexDirectionsFlat.N, new Vector3(0, -1, +1)},
        { CubeHexDirectionsFlat.NE, new Vector3(+1, -1, 0)},
        { CubeHexDirectionsFlat.SE, new Vector3(+1, 0, -1)},
        { CubeHexDirectionsFlat.S, new Vector3(0, +1, -1)},
        { CubeHexDirectionsFlat.SW, new Vector3(-1, +1, 0)}
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

    public static float CubeDistance(Vector3 a, Vector3 b) {
        return Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y), Mathf.Abs(a.z - b.z));
    }

    public static float CubeDistance(Vector2 a, Vector2 b) {
        return CubeDistance(AxialToCube(a), AxialToCube(b));
    }

    /*public static Vector3 LerpCube(Vector3 a, Vector3 b, float t) {
        return new Vector3(Mathf.Lerp(a.x, b.x, t), Mathf.Lerp(a.y, b.y, t), Mathf.Lerp(a.z, b.z, t));
    }

    public static Vector3 RoundCube(Vector3 a) {
        return new Vector3((int)Mathf.Round(a.x), (int)Mathf.Round(a.y), (int)Mathf.Round(a.z));
    }

    public static List<Vector3> CubeLine(Vector3 a, Vector3 b) {
        List<Vector3> line = new List<Vector3>();
        int d = CubeDistance(a, b);
        Debug.Log(d);
        for (int i = 0; i < d + 1; i++) {
            line.Add(RoundCube(LerpCube(a, b, 1.0f / d * i)));
            Debug.Log(line[line.Count - 1].ToString());
        }
        Debug.Log(line.ToString());
        return line;
    }

    public static List<Vector3> CubeLine(Vector2 a, Vector2 b) {
        return CubeLine(AxialToCube(a), AxialToCube(b));
    }*/
}