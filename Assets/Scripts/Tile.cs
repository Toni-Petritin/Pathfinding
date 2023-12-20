using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public int x, y;

    public bool isTraversable = true;
    public bool unTouchable;

    
    // Set to maximum value.
    public float cost = float.MaxValue;
    public float weight = 1;

    public int prevX;
    public int prevY;

    public TextMeshProUGUI costField;
    public TextMeshProUGUI leftField;
    public TextMeshProUGUI rightField;

    public void SetTraversable(bool onOff)
    {
        isTraversable = onOff;
        if (!onOff && !unTouchable)
        {
            this.GetComponent<MeshRenderer>().material.color = Color.black;
            costField.text = "";
            leftField.text = "";
            rightField.text = "";
        }
    }
}
