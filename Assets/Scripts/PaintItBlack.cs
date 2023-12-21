using UnityEngine;

public class PaintItBlack : MonoBehaviour
{
    void FixedUpdate()
    {
        // When game is unpaused, you can no longer make tiles non-traversable.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            this.enabled = false;
        }

        // Mouse primary turns tiles non-traversable.
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
