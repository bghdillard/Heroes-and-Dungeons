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
        InteractionPoint target = (InteractionPoint)parent.GetData("Target");
        Vector3 location = target.GetLocation();
        agent.SetDestination(location);
        if (agent.remainingDistance <= 0.01)
        {
            Debug.Log("Arrived At Location");
            return NodeState.SUCCESS;
        }
        Debug.Log("Still moving, " + Vector3.Distance(agent.transform.position, location) + " left to go");
        return NodeState.RUNNING;
    }
}
