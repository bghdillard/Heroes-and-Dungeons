using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class IdleNode : Node
{
    private NavMeshAgent agent;
    private float initialTimer;
    private float currTimer;

    public IdleNode(NavMeshAgent agent, float timer) : base()
    {
        this.agent = agent;
        initialTimer = timer;
        currTimer = initialTimer;
    }
    public override NodeState Evaluate()
    {
        /*
        Debug.Log("Idle Eval Started");
        Debug.Log("monster distance equality is: " + agent.remainingDistance);
        Debug.Log("IdleTimer is: " + currTimer);
        */
        if(agent.remainingDistance <= 0.05f && (currTimer -= Time.deltaTime) <= 0)
        {
            //Debug.Log("Idle set target Started");
            currTimer = initialTimer;
            List<Room> rooms = GridManager.GetRooms();
            if (rooms.Count == 1) agent.SetDestination(rooms[0].GetRandomPoint());
            else agent.SetDestination(rooms[Random.Range(1, rooms.Count)].GetRandomPoint());
        }
        //Debug.Log("Monster is currently idling");
        return NodeState.RUNNING;
    }
}
