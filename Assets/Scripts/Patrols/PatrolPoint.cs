using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPoint
{
    private Vector3 point;
    private PatrolPoint next;
    private PatrolPoint previous;

    public PatrolPoint(Vector3 location)
    {
        point = location;
    }

    public void SetNext(PatrolPoint toSet)
    {
        next = toSet;
    }

    public PatrolPoint GetNext()
    {
        return next;
    }

    public void SetPrevious(PatrolPoint toSet)
    {
        previous = toSet;
    }

    public PatrolPoint GetPrevious()
    {
        return previous;
    }

    public Vector3 GetPoint()
    {
        return point;
    }
}
