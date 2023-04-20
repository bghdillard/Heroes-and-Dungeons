using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class GuardNode : Node
{
    private NavMeshAgent agent;
    private float initialTimer;
    private float currTimer;

    public GuardNode(NavMeshAgent agent, float timer) : base()
    {
        this.agent = agent;
        initialTimer = timer;
        currTimer = initialTimer;
    }

    public override NodeState Evaluate()
    {
        Room guardRoom = (Room)parent.GetData("guardRoom");
        if (guardRoom == null) return NodeState.FAILURE;
        if (agent.remainingDistance <= 0.05f && (currTimer -= Time.deltaTime) <= 0)
        {
            currTimer = initialTimer;
            agent.SetDestination(guardRoom.GetRandomPoint());
        }
        return NodeState.RUNNING;
    }
}
