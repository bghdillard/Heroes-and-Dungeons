using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{

    private Order CurrOrder;
    private NavMeshAgent agent;
    private bool orderStarted;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (CurrOrder != null) //begin by seeing if this builder has a current build order;
        {
            if (!orderStarted)
            {
                Debug.Log("Builder Moving");
                if (Vector3.Distance(transform.position, CurrOrder.GetLocation().GetLocation()) <= 1.5) //If this builder has an order, check if in range of the cell;
                {
                    Debug.Log("Reached Destination");
                    agent.SetDestination(transform.position); //If so, stop moving and begin work;
                    orderStarted = true;
                    if (CurrOrder.GetName() == "buildCell") StartCoroutine(BuildCell());
                    else if (CurrOrder.GetName() == "buildItem") StartCoroutine(BuildItem());
                }
                else //If not in range of the cell, recalculate the path (I don't know at this point if this'll be necessary, or, on top of that, if it'll get expensive, so keep that in mind me)
                {
                    agent.SetDestination(agent.destination);
                }
            }
        }
        else
        {
            if (GridManager.GetHighQueue().TryDequeue(out Order toDo)) //If no current order, try and get a new order from the queue in the grid manager;
            {
                if (GridManager.CheckAdjacent(toDo.GetLocation())) SetOrder(toDo); //if in order is recieved, make sure it can be accessed, and, if it can, set it to be active;
                else GridManager.GetHighQueue().Enqueue(toDo); //if it can't be accessed, put it back in the queue;
            }
            if (CurrOrder == null) //If we didn't get an order from the high priority queue, check the low priority;
            {
                if (GridManager.GetLowQueue().TryDequeue(out toDo)) SetOrder(toDo); //In theory, all these orders should be accessable;
            }
        }
    }

    private void SetOrder(Order toDo) //sets the destination of the nav mesh agent to reach the current order, as well as sets the order to be active;
    {
        CurrOrder = toDo;
        if (toDo.GetName() == "buildItem" || toDo.GetName() == "buildCell") //If the order is for building...
        {
            Vector3 location = toDo.GetLocation().transform.position;
            int x = (int)location.x;
            int z = (int)location.z;
            Vector3 closest = new Vector3();
            float closestDistance = float.MaxValue;
            NavMeshPath path = new NavMeshPath();
            for (int i = x - 1; i < x + 2; i++) //Find the closest adjacent point to the target cell from the current position
            {
                if (NavMesh.CalculatePath(transform.position, new Vector3(i, location.y, z), agent.areaMask, path))
                {
                    Debug.Log("X path calculated");
                    float distance = Vector3.Distance(transform.position, path.corners[0]);
                    for (int y = 1; y < path.corners.Length; y++)
                    {
                        distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                    }
                    if (distance < closestDistance)
                    {
                        closest = new Vector3(i, location.y, z);
                        closestDistance = distance;
                    }
                }
                else Debug.Log("X path failed");
            }
            for (int i = z - 1; i < z + 2; i++) //mention this to mama for fun
            {
                if (NavMesh.CalculatePath(transform.position, new Vector3(x, location.y, i), agent.areaMask, path))
                {
                    Debug.Log("Z path calculated");
                    float distance = Vector3.Distance(transform.position, path.corners[0]);
                    for (int y = 1; y < path.corners.Length; y++)
                    {
                        distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                    }
                    if (distance < closestDistance)
                    {
                        closest = new Vector3(x, location.y, i);
                        closestDistance = distance;
                    }
                }
                else Debug.Log("Z path failed");
            }
            Debug.Log("Closest location: " + closest);
            agent.SetDestination(closest);
        }
        else if(toDo.GetName() == "Transport") //if the order is for transporting
        {

        }
        orderStarted = false;
        GridManager.AddtoList(CurrOrder);
        CurrOrder.SetBuilder(this);
    }

    IEnumerator BuildCell()
    {
        Debug.Log("Build Begin");
        WaitForSeconds wait = new WaitForSeconds(1);
        for(int i = 0; i < 4; i++)
        {
            //we'll use have an animator controller here to "mine" the cell, and send something to the cell to visually update the mining progress but those are future problems
            Debug.Log("Mining Progress: " + i);
            yield return wait;
        }
        Debug.Log("Mining progress End");
        GridManager.UpdateGrid(CurrOrder.GetLocation(), CurrOrder.GetBuild());//now that we're done, remove the order and change the cell
        CurrOrder = null;
        Debug.Log("Build End");
    }
    IEnumerator BuildItem()
    {
        Debug.Log("Build Item Begin");
        WaitForSeconds wait = new WaitForSeconds(1);
        for (int i = 0; i < 4; i++)
        {
            //we'll use have an animator controller here to "build" the item, and send something to the location to visually update the building progress but those are future problems
            Debug.Log("Building Progress: " + i);
            yield return wait;
        }
        Debug.Log("Building progress End");
        GridManager.UpdateItemGrid(CurrOrder.GetLocation(), CurrOrder.GetBuild(), CurrOrder.GetRotation());//now that we're done, remove the order and add the item
        CurrOrder = null;
        Debug.Log("Build Item End");
    }

    public void CancelOrder() //Cancel the active order
    {
        agent.SetDestination(transform.position);
        CurrOrder = null;
    }

}
