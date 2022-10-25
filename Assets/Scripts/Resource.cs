using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public string resourceType;
    public int minAmount;
    public int maxAmount;
    [HideInInspector]
    public bool isGrabbed = false;

    [SerializeField]
    private int amount;
    private Container target;

    private void Awake()
    {
        amount = 90;//Random.Range(minAmount, maxAmount + 1);
        /*target = GridManager.GetClosestValidContainer(this, resourceType); Because the object spawns above the navmesh, doing this here causes issues. I think...
        if (target != null) GridManager.AddtoLowQueue(new Order("transport", target, this));
        else Debug.Log("No valid container of type: " + resourceType);
        */
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null) // might want to move this somewhere else, but that's a future problem
        {
            target = GridManager.GetClosestValidContainer(this, resourceType);
            if (target != null)
            {
                GridManager.AddtoLowQueue(new Order("transport", target, this));
                target.AddTentative(amount);
            }
            else Debug.Log("Still no valid container of type: " + resourceType);
        }
    }

    public int GetAmount()
    {
        return amount;
    }

    public void SubtractAmount(int toSubtract)
    {
        amount -= toSubtract;
    }

    public void RemoveTarget()
    {
        target = null;
    }
}
