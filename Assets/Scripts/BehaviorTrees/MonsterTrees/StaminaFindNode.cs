using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviorTree;

public class StaminaFindNode : Node
{
    Monster monster;

    public StaminaFindNode(Monster monster) : base()
    {
        this.monster = monster;
    }

    public override NodeState Evaluate()
    {
        if (parent.parent.GetData("MagicRestore") != null) return NodeState.FAILURE;
        if (parent.GetData("Target") != null) return NodeState.SUCCESS;
        if (!monster.GetStaminaStatus()) return NodeState.FAILURE;
        List<Restorative> restoratives = GridManager.GetRestoratives("Stamina");
        if (restoratives.Count == 0) return NodeState.FAILURE;
        float closestDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        Restorative toUse = null;
        InteractionPoint toSpecify = null;
        foreach (Restorative restorative in restoratives)
        {
            List<InteractionPoint> points = restorative.GetLocations();
            foreach (InteractionPoint point in points)
            {
                if (!point.GetInUse())
                {
                    if (NavMesh.CalculatePath(monster.transform.position, point.GetLocation(), -1, path))
                    {
                        float distance = Vector3.Distance(monster.transform.position, path.corners[0]);
                        for (int y = 1; y < path.corners.Length; y++)
                        {
                            distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                        }
                        if (distance < closestDistance)
                        {
                            toUse = restorative;
                            toSpecify = point;
                            closestDistance = distance;
                        }
                    }
                    else Debug.LogError("Attempted to calculate path from monster at " + monster.transform.position +
                        " to restorative at " + point.GetLocation() + " but path calculation failed");
                }
            }
        }
        parent.SetData("Target", toSpecify);
        parent.SetData("Restorative", toUse);
        parent.parent.SetData("StaminaRestore", true);
        toUse.SetUser(toSpecify, monster);
        return NodeState.SUCCESS;
    }
}
