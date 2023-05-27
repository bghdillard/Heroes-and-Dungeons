using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol
{
    private PatrolPoint origin;
    private PatrolPoint end;
    private PatrolGroup group;
    private int size;
    private bool visible;
    private string patrolName;
    private int numAssigned;
    private bool selected;
    private AssignedPointPanel panel;

    public Patrol(PatrolPoint origin)
    {
        this.origin = origin;
        end = origin;
        origin.SetNext(origin);
        visible = false;
        size = 1;
        patrolName = "Patrol";
        selected = false;
    }

    public PatrolPoint GetOrigin()
    {
        return origin;
    }

    public int GetSize()
    {
        return size;
    }

    public Vector3 GetEndPoint()
    {
        return end.GetPoint();
    }

    public void AddPoint(PatrolPoint toAdd)
    {
        toAdd.SetPrevious(end);
        end.SetNext(toAdd);
        end = toAdd;
        //origin.SetPrevious(end);
        size++;
        if (selected) panel.UpdateMaxCount(1);
        Debug.Log("Patrol size is: " + size);
    }

    public void LoopPatrol()
    {
        end.SetNext(origin);
    }

    public void RemovePoint(PatrolPoint toRemove)
    {
        if (toRemove != origin)
        {
            toRemove.GetPrevious().SetNext(toRemove.GetNext());
        }
        else if (toRemove != end) origin = toRemove.GetNext();
        if (toRemove != end)
        {
            toRemove.GetNext().SetPrevious(toRemove.GetPrevious());
        }
        else if (toRemove != origin) end = toRemove.GetPrevious();
        size--;
    }

    public void ToggleVisible(bool toToggle)
    {
        int count = 1;
        origin.ToggleVisible(toToggle);
        PatrolPoint pointer = origin;
        while (count < size)
        {
            pointer = pointer.GetNext();
            pointer.ToggleVisible(toToggle);
            count++;
        }
    }

    public bool ContainsPoint(Vector3 toCheck, out PatrolPoint point)
    {
        if (Vector3.Distance(origin.GetPoint(), toCheck) < 1)
        {
            point = origin;
            return true;
        }
        if (origin.GetNext() == null)
        {
            point = null;
            return false;
        }
        return origin.GetNext().ContainsPoint(toCheck, out point);
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

    public string GetName()
    {
        return patrolName;
    }

    public int GetAssignedCount()
    {
        return numAssigned;
    }

    public void SetPanel(AssignedPointPanel toAssign)
    {
        panel = toAssign;
    }

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }
}
