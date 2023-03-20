using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol
{
    private PatrolPoint origin;
    private PatrolPoint end;
    private PatrolGroup group;
    private int size;

    public Patrol(PatrolPoint origin)
    {
        this.origin = origin;
        end = origin;
        origin.SetNext(origin);
        origin.SetPrevious(origin);
        size = 1;
    }

    public void AddPoint(PatrolPoint toAdd)
    {
        toAdd.SetPrevious(end);
        toAdd.SetNext(origin);
        end.SetNext(toAdd);
        end = toAdd;
        origin.SetPrevious(end);
        size++;
    }

    private PatrolPoint GetRandomPoint()
    {
        int toGet = Random.Range(0, size + 1);
        if (toGet == 0) return origin;
        else return GetSetPoint(toGet - 1, origin.GetNext());
    }

    private PatrolPoint GetSetPoint(int toGet, PatrolPoint curr)
    {
        if (toGet == 0) return curr;
        else return GetSetPoint(toGet - 1, curr.GetNext());
    }

    public PatrolGroup CreateGroup(Monster initial)
    {
        return group = new PatrolGroup(initial, GetRandomPoint(), this);
    }

    public PatrolGroup GetGroup()
    {
        return group;
    }
    
    public void RemoveGroup()
    {
        group = null;
    }
}
