using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class GroupNode : Node
{
    private Monster monster;

    public GroupNode(Monster monster) : base()
    {
        this.monster = monster;
    }

    public override NodeState Evaluate()
    {
        PatrolGroup group = monster.GetGroup();
        Debug.Log("Getting Group");
        if (group != null) return NodeState.SUCCESS;
        Debug.Log("Not in a group, getting patrol");
        Patrol patrol = (Patrol)parent.GetData("patrol");
        if (patrol == null) return NodeState.FAILURE;
        Debug.Log("Creating or getting group");
        group = patrol.GetGroup();
        if(group != null)
        {
            group.AddMonster(monster);
            monster.SetGroup(group);
            return NodeState.SUCCESS;
        }
        patrol.CreateGroup(monster);
        return NodeState.SUCCESS;
    }
}
