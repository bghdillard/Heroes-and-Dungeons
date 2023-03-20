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
        if (group != null) return NodeState.SUCCESS;
        Patrol patrol = (Patrol)parent.GetData("patrol");
        if (patrol == null) return NodeState.FAILURE;
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
