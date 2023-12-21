using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{

    public int x, y;

    public bool isTraversable = true;
    public bool unTouchable;
    
    public int Cost { get; private set; }
    public int Distance { get; private set; }
    public int SumCost { get; private set; } = int.MaxValue;
    //public int weight = 1;

    // public int parentX;
    // public int parentY;
    public Tile Parent;

    [SerializeField]private TextMeshProUGUI sumCostField;
    [SerializeField]private TextMeshProUGUI CostField;
    [SerializeField]private TextMeshProUGUI DistanceField;

    public void SetTraversable(bool onOff)
    {
        isTraversable = onOff;
        if (!onOff && !unTouchable)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
            sumCostField.enabled = false;
            CostField.enabled = false;
            DistanceField.enabled = false;
        }
    }

    public void SetCost(int value)
    {
        Cost = value;
        CostField.text = Cost.ToString();
    }
    
    public void SetDistance(int value)
    {
        Distance = value;
        DistanceField.text = Distance.ToString();
    }
    public void SetSumCost(int value)
    {
        SumCost = value;
        sumCostField.text = SumCost.ToString();
    }
}
