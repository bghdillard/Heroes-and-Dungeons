using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{

    private Order CurrOrder;
    private NavMeshAgent agent;
    private Coroutine toStop;
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
                    toStop = StartCoroutine(Build());
                }
            }
        }
        else
        {
            if (GridManager.GetQueue().TryDequeue(out Order toDo)) //If no current order, try and get a new order from the queue in the grid manager;
            {
                if (GridManager.CheckAdjacent(toDo.GetLocation())) SetOrder(toDo); //if in order is recieved, make sure it can be accessed, and, if it can, set it to be active;
                else GridManager.GetQueue().Enqueue(toDo); //if it can't be accessed, put it back in the queue;
            }
        }
    }

    private void SetOrder(Order toDo) //sets the destination of the nav mesh agent to reach the current order, as well as sets the order to be active;
    {
        CurrOrder = toDo;
        Vector3 location = toDo.GetLocation().transform.position;
        int x = (int)location.x;
        int z = (int)location.z;
        Vector3 closest = new Vector3();
        float closestDistance = float.MaxValue;
        NavMeshPath path = new NavMeshPath();
        for (int i = x - 1; i < x + 2; i++)
        {
            if (NavMesh.CalculatePath(transform.position, new Vector3(i, location.y, z), agent.areaMask, path)){
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for(int y = 1; y < path.corners.Length; y++)
                {
                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                }
                if(distance < closestDistance)
                {
                    closest = new Vector3(i, location.y, z);
                }
            }
        }
        for (int i = z - 1; i < x + 2; i++)
        {
            if(NavMesh.CalculatePath(transform.position, new Vector3(x, location.y, i), agent.areaMask, path)){
                float distance = Vector3.Distance(transform.position, path.corners[0]);
                for (int y = 1; y < path.corners.Length; y++)
                {
                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                }
                if (distance < closestDistance)
                {
                    closest = new Vector3(i, location.y, z);
                }
            }
        }
        agent.SetDestination(closest);
        orderStarted = false;
    }

    IEnumerator Build()
    {
        Debug.Log("Build Begin");
        WaitForSeconds wait = new WaitForSeconds(1);
        for(int i = 0; i < 4; i++)
        {
            //we'll use have an animator controller here to "mine" the cell, and send something to the cell to visual update the mining progress
            Debug.Log("Mining Progress: " + i);
            yield return wait;
        }
        Debug.Log("Mining progress End");
        GridManager.UpdateGrid(CurrOrder.GetLocation(), CurrOrder.GetBuild());//now that we're done, remove the order and change the cell
        CurrOrder = null;
        Debug.Log("Build End");
        StopCoroutine(toStop);
    }

}
