using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Helpers // A class containing whatever Helper functions I want accessible everywhere
{

    public static float GetPathLength(NavMeshPath path)
    {
        Vector3[] corners = path.corners;
        if (corners.Length < 2) return 0;
        float length = 0;
        for(int i = 1; i < corners.Length; i++) length += Vector3.Distance(corners[i - 1], corners[i]);
        return length;
    }
}
