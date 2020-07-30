using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class corners : MonoBehaviour
{
    public Vector3[] cornerCords = new Vector3[4];

    void Awake()
    {
        Bounds bounds = transform.GetComponent<Renderer>().bounds;

        cornerCords[0] = new Vector3(bounds.min.x, bounds.min.y, 0);
        cornerCords[1] = new Vector3(bounds.max.x, bounds.max.y, 0);
        cornerCords[2] = new Vector3(bounds.min.x, bounds.max.y, 0);
        cornerCords[3] = new Vector3(bounds.max.x, bounds.min.y, 0);

    }
}
