using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class RestorePathfindNode : Node
{
    NavMeshAgent agent;

    public RestorePathfindNode(NavMeshAgent agent) : base()
    {
        this.agent = agent;
    }

    public override NodeState Evaluate()
    {
        Restorative target = (Restorative)parent.GetData("Target");

        Vector3 location = target.GetLocation();
        if (Vector3.Distance(agent.transform.position, location) <= 0.01) return NodeState.SUCCESS;
        agent.SetDestination(location);
        return NodeState.RUNNING;
    }
}
