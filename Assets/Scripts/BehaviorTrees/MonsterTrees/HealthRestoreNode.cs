using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class HealthRestoreNode : Node
{
    private Monster monster;

    public HealthRestoreNode(Monster monster) : base()
    {
        this.monster = monster;
    }

    public override NodeState Evaluate()
    {
        NodeState toReturn = monster.GetHealthMax() ? NodeState.SUCCESS : NodeState.RUNNING;
        if(toReturn == NodeState.SUCCESS)
        {
            parent.RemoveData("Target");
        }
        return toReturn;
    }
}
