using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class StaminaRestoreNode : Node
{
    private Monster monster;

    public StaminaRestoreNode(Monster monster) : base()
    {
        this.monster = monster;
    }

    public override NodeState Evaluate()
    {
        InteractionPoint point = (InteractionPoint)parent.GetData("Target");
        point.CheckUse();
        NodeState toReturn = monster.GetStaminaMax() ? NodeState.SUCCESS : NodeState.RUNNING;
        if (toReturn == NodeState.SUCCESS)
        {
            Restorative temp = (Restorative)parent.GetData("Restorative");
            temp.RemoveUser((InteractionPoint)parent.GetData("Target"));
            parent.RemoveData("Target");
            parent.RemoveData("Restorative");
            parent.parent.RemoveData("StaminaRestore");
        }
        return toReturn;
    }
}
