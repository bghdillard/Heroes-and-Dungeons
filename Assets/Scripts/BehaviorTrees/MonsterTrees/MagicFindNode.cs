using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;


public class MagicFindNode : Node
{
    Monster monster;

    public MagicFindNode(Monster monster) : base()
    {
        this.monster = monster;
    }

    public override NodeState Evaluate()
    {
        if (parent.GetData("Target") != null) return NodeState.SUCCESS;
        if (!monster.GetMagicStatus()) return NodeState.FAILURE;
        List<Restorative> restoratives = GridManager.GetRestoratives("Magic");
        if (restoratives.Count == 0) return NodeState.FAILURE;
        float closestDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        Restorative toUse = null;
        foreach (Restorative restorative in restoratives)
        {
            if (NavMesh.CalculatePath(monster.transform.position, restorative.GetLocation(), -1, path))
            {
                float distance = Vector3.Distance(monster.transform.position, path.corners[0]);
                for (int y = 1; y < path.corners.Length; y++)
                {
                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                }
                if (distance < closestDistance)
                {
                    toUse = restorative;
                    closestDistance = distance;
                }
            }
            else Debug.LogError("Attempted to calculate path from monster at " + monster.transform.position +
                " to restorative at " + restorative.GetLocation() + " but path calculation failed");
        }
        parent.SetData("Target", toUse);
        toUse.SetUser(monster);
        return NodeState.SUCCESS;
    }
}
