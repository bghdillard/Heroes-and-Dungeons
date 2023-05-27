using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class PatrolNode : Node
{
    NavMeshAgent agent;
    float initialTimer;
    float currTimer;
    Monster monster;
    PatrolPoint lastPoint;
    bool groupNotified;

    public PatrolNode(NavMeshAgent agent, float timer, Monster monster): base()
    {
        this.agent = agent;
        initialTimer = timer;
        currTimer = timer;
        this.monster = monster;
        groupNotified = false;
    }

    public override NodeState Evaluate()
    {
        PatrolGroup group = monster.GetGroup();
        if(agent.remainingDistance <= 0.05f)
        {
            if(group.GetPoint() == lastPoint)
            {
                if(!groupNotified && (currTimer -= Time.deltaTime) <= 0)
                {
                    Debug.Log("Notifying group");
                    currTimer = initialTimer;
                    group.AddArrival();
                    groupNotified = true;
                }
                return NodeState.RUNNING;
            }
            groupNotified = false;
            lastPoint = group.GetPoint();
            Vector3 target = lastPoint.GetPoint();
            target.x += Random.Range(-0.5f, 0.5f);
            target.z += Random.Range(-0.5f, 0.5f);
            agent.SetDestination(target);
        }
        return NodeState.RUNNING;
    }
}
