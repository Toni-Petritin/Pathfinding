using UnityEngine;

public class CameraLocator : MonoBehaviour
{
    [SerializeField] private AStar center;

    private void Start()
    {
        transform.position =
            new Vector3(((float)center.Width + 1) / 2,
                Mathf.Max(center.Width, center.Height),
                ((float)center.Height + 1) / 2);
    }
}
