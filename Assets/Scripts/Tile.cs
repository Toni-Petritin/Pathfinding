using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public int x, y;

    public bool isTraversable = true;

    // Set to maximum value.
    public float cost = float.MaxValue;
    public float weight = 1;

    public int prevX;
    public int prevY;

    public TextMeshProUGUI costField;
    public TextMeshProUGUI leftField;
    public TextMeshProUGUI rightField;

}
