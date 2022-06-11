using UnityEngine;

[CreateAssetMenu(fileName = "LevelSettings", menuName = "ScriptableObjects/LevelSettings", order = 3)]
public class LevelSettings : ScriptableObject {
    public int gridSize;
    public ColorsSO colors;
    public ShapesSO shapes;
    public int targetScore;
}