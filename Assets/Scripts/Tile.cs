using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public int x, y;

    public bool is_traversable = true;

    // Set to maximum value.
    public float cost = float.MaxValue;
    public float weight = 1;

    private int prev_x, prev_y;

    public TextMeshProUGUI costField;
    public TextMeshProUGUI leftField;
    public TextMeshProUGUI rightField;

    public void SetPrev(int xi, int yi)
    {
        prev_x = xi; prev_y = yi;
    }
    public int GetPrevX()
    {
        return prev_x;
    }
    public int GetPrevY()
    {
        return prev_y;
    }


}
