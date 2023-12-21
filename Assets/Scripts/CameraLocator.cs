using UnityEngine;

public class CameraLocator : MonoBehaviour
{
    [SerializeField] private AStar center;

    private void Start()
    {
        // Automatically adjusts camera position and distance based on grid size.
        transform.position =
            new Vector3(((float)center.Width + 1) / 2,
                Mathf.Max(center.Width, center.Height),
                ((float)center.Height + 1) / 2);
    }
}
