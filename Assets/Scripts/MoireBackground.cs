using UnityEngine;

public class MoireBackground : MonoBehaviour {
    [SerializeField] private HexaBackgroundSprite back1;
    [SerializeField] private HexaBackgroundSprite back2;
    [SerializeField] private Color mainColor = Color.black;
    [SerializeField] private float rotationSpeed = 1;

    private void Start() {
        back1.mainColor = mainColor;
        back2.mainColor = mainColor;
        back1.Init();
        back2.Init();
    }
    void Update() {
        if (back1 && back2) {
            back1.transform.Rotate(new Vector3(0, 0, rotationSpeed * Time.deltaTime));
            back2.transform.Rotate(new Vector3(0, 0, -rotationSpeed * Time.deltaTime));
        }
    }
}