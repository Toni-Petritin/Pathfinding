using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class Tile : MonoBehaviour
{

    public int x, y;

    public bool isTraversable = true;
    // Untouchable is a Tile which can't be made not traversable.
    public bool unTouchable;
    
    public int Cost { get; private set; }
    public int Distance { get; private set; }
    public int SumCost { get; private set; } = int.MaxValue;
    // I added weight here as well, if I ever want to add it.
    //public int weight = 1;
    
    // Instead of parent coordinates, I have the parent itself.
    public Tile Parent;

    [SerializeField]private TextMeshProUGUI sumCostField;
    [SerializeField]private TextMeshProUGUI costField;
    [SerializeField]private TextMeshProUGUI distanceField;

    // Setters to do all the visual work to reduce clutter in AStar-script.
    public void SetTraversable(bool onOff)
    {
        isTraversable = onOff;
        if (!onOff && !unTouchable)
        {
            GetComponent<MeshRenderer>().material.color = Color.black;
            sumCostField.enabled = false;
            costField.enabled = false;
            distanceField.enabled = false;
        }
    }
    
    public void SetCost(int value)
    {
        Cost = value;
        costField.text = Cost.ToString();
    }
    
    public void SetDistance(int value)
    {
        Distance = value;
        distanceField.text = Distance.ToString();
    }
    public void SetSumCost(int value)
    {
        SumCost = value;
        sumCostField.text = SumCost.ToString();
    }
}
