using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : Room
{
    public override Vector3 GetRandomPoint()
    {
        Bounds bounds = gameObject.GetComponent<MeshCollider>().bounds;
        return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
    }

    public override string ToString()
    {
        return transform.position.ToString();
    }
}
