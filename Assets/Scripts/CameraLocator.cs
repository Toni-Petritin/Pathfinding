using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLocator : MonoBehaviour
{
    [SerializeField] private PathFinding center;

    private void Start()
    {
        transform.position = new Vector3(center.Width / 2, Mathf.Max(center.Width, center.Height), center.Height / 2);
    }
}
