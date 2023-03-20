using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolGroup
{
    private List<Monster> monsters;
    private PatrolPoint currPoint;
    private Patrol patrol;
    private int numArrived;

    public PatrolGroup(Monster initial, PatrolPoint startPoint, Patrol patrol)
    {
        currPoint = startPoint;
        this.patrol = patrol;
        monsters = new List<Monster>() { initial };
    }

    public void AddMonster(Monster toAdd)
    {
        monsters.Add(toAdd);
    }

    public void RemoveMonster(Monster toRemove)
    {
        monsters.Remove(toRemove);
        if (monsters.Count == 0) patrol.RemoveGroup();
    }

    public PatrolPoint GetPoint()
    {
        return currPoint;
    }

    public void IteratePoint()
    {
        currPoint = currPoint.GetNext();
    }

    public void AddArrival()
    {
        if(numArrived++ == monsters.Count)
        {
            IteratePoint();
            numArrived = 0;
            foreach (Monster monster in monsters) monster.GroupIterate();
        }
    }
}
