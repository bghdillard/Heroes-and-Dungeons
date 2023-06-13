using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGroup
{
    private List<Monster> monsters;
    private PatrolPoint currPoint;
    private Patrol patrol;
    private bool reverse = false;
    private int numArrived = 0;

    public PatrolGroup(Monster initial, PatrolPoint startPoint, Patrol patrol)
    {
        currPoint = startPoint;
        this.patrol = patrol;
        monsters = new List<Monster>() { initial };
        initial.SetGroup(this);
    }

    public void AddMonster(Monster toAdd)
    {
        monsters.Add(toAdd);
    }

    public void RemoveMonster(Monster toRemove)
    {
        Debug.Log("Removing Monster");
        monsters.Remove(toRemove);
        if (monsters.Count == 0) patrol.RemoveGroup();
    }

    public PatrolPoint GetPoint()
    {
        return currPoint;
    }

    public void IteratePoint()
    {
        if (reverse)
        {
            if(currPoint.GetPrevious() == null)
            {
                reverse = false;
                IteratePoint();
                return;
            }
            currPoint = currPoint.GetPrevious();
        }
        else
        {
            if(currPoint.GetNext() == null)
            {
                if (patrol.GetOrigin().IsPoint(currPoint.GetPoint()))
                {
                    Debug.Log("Patrol is a loop");
                    currPoint = patrol.GetOrigin().GetNext();
                    return;
                }
                reverse = true;
                IteratePoint();
                return;
            }
            currPoint = currPoint.GetNext();
        }
    }

    public void AddArrival()
    {
        Debug.Log("number of monsters in group " + monsters.Count);
        numArrived++;
        if (numArrived >= monsters.Count)
        {
            Debug.Log("All have arrived");
            IteratePoint();
            numArrived = 0;
            foreach (Monster monster in monsters) monster.GroupIterate();
        }
        else Debug.Log("Not all have arrived, numArrived is: " + numArrived);
    }
}
