using UnityEngine;

public class PaintItBlack : MonoBehaviour
{
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.enabled = false;
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                hit.collider.gameObject.GetComponent<Tile>().SetTraversable(false);
            }
        }
    }
}
