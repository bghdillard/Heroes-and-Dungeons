using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    [SerializeField]
    private IOrder CurrOrder;
    private NavMeshAgent agent;
    private bool orderStarted;
    private bool tryClaim;
    private Coroutine activeCoroutine;
    private BuilderController builderController;
    [SerializeField]
    private float originalIdleTime;
    private float idleTime;
    private bool recalculateOrderPath;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        idleTime = originalIdleTime;
        recalculateOrderPath = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (recalculateOrderPath)
        {
            Debug.Log("Recalculating order path");
            Vector3 temp = builderController.GetClosestPath(CurrOrder.GetLocation(), this);
            if (!temp.Equals(new Vector3()))
            {
                agent.SetDestination(temp);
                recalculateOrderPath = false;
            }
        }
        if (tryClaim) ClaimOrder();
        if (CurrOrder != null && !agent.pathPending) //begin by seeing if this builder has a current build order;
        {
            if (!orderStarted) //Look to see if we're still moving to the location
            {
                if (agent.remainingDistance <= 0.8) //if we're in range, start the order;
                {
                    agent.SetDestination(transform.position);
                    orderStarted = true;
                    if (CurrOrder.GetOrderType() == "cellBuild") activeCoroutine = StartCoroutine(BuildCell());
                    else if (CurrOrder.GetOrderType() == "itemBuild") activeCoroutine = StartCoroutine(BuildItem());
                    else if (CurrOrder.GetOrderType() == "transit")
                    {
                        TransitOrder temp = (TransitOrder)CurrOrder;
                        temp.GetToTransit().transform.SetParent(transform);
                        agent.SetDestination(temp.GetFinalLocation());
                    }
                }
                else agent.SetDestination(agent.destination); //still don't know if this is needed, but I'll keep it for now, it still might maybe get expensive to do, so keep it in mind as one thing to remove;
            }
            else if (CurrOrder.GetOrderType() == "transit")
            {
                if (agent.remainingDistance <= 0.8) //if we are close enough to the container, put the resource into the container
                {
                    TransitOrder temp = (TransitOrder)CurrOrder;
                    Resource resource = temp.GetToTransit().GetComponent<Resource>();
                    Container container = temp.GetToStore().GetComponent<Container>();
                    if (container.CheckAmount(resource.GetAmount()))
                    {
                        Debug.Log("Container not full from this resource");
                        container.AddResources(resource.GetAmount());
                        Destroy(resource.gameObject);
                    }
                    else
                    {
                        Debug.Log("Container full from this resource");
                        resource.SubtractAmount(container.GetRemaining());
                        container.Fill();
                        resource.transform.parent = null;
                        resource.RemoveTarget();
                    }
                    //Debug.Log("this much is working");
                    CurrOrder = null;
                    agent.SetDestination(transform.position);
                    Debug.Log("Item transported");
                }
                else agent.SetDestination(agent.destination); //and again here, just remember to look here if things are getting expensive;
            }
        }
        else if (CurrOrder == null && agent.remainingDistance == 0.0) //if we are not currently doing an order or already idleing, decrease time to next idle
        {
            idleTime -= Time.deltaTime;
            if(idleTime <= 0)
            {
                Idle();
                idleTime = originalIdleTime;
            }
        }
    }

    private void ClaimOrder()
    {
        tryClaim = false;
        Vector3 start = CurrOrder.GetLocation(); //get the coordinates of the newly finished order
        int x = (int)start.x;
        int y = (int)start.y;
        int z = (int)start.z;
        List<Cell> cells = new List<Cell>();
        for (int i = x - 1; i < x + 2; i++) //look at the cells adjacent and see if they have orders generated at the same time;
        {         
            if (i < 0 || i > 99) continue;
            Cell temp = GridManager.GetCellAt(i, y, z);
            IOrder order = temp.GetOrder();
            if (order != null)
            {
                if (!order.GetStarted() && CurrOrder.GetTime() == order.GetTime()) cells.Add(temp); //if they do, add them to the list of possible orders;
            }
        }
        for (int i = z - 1; i < z + 2; i++) //now do the same for the z axis;
        {
            if (i < 0 || i > 99) continue;
            Cell temp = GridManager.GetCellAt(x, y, i);
            IOrder order = temp.GetOrder();
            if (order != null)
            {
                if (!order.GetStarted() && CurrOrder.GetTime() == order.GetTime()) cells.Add(temp);
            }
        }
        if (cells.Count != 0)
        {
            if(cells.Count == 1)
            {
                Debug.Log("Only one adjacent");
                builderController.ClaimOrder(cells[0].GetOrder(), this);
                SetOrder(cells[0].GetOrder(), builderController.GetClosestPath(cells[0].GetOrder().GetLocation(), this));
            }
            else
            {
                Debug.Log(cells.Count + " Adjacent Cell(s) Found");
                Cell currSelected = null;
                int currAdjacent = int.MaxValue;
                foreach(Cell cell in cells) //find which cell amoung those we found has the least amount of adjacent orders
                {
                    int numAdjacent = 0;
                    start = cell.GetLocation(); //piggyback off of some previously used variables to make this bit simpler
                    x = (int)start.x;
                    y = (int)start.y;
                    z = (int)start.z;
                    for (int i = x - 1; i < x + 2; i++) //look at the cells adjacent and see if they have orders generated at the same time;
                    {
                        if (i < 0 || i > 99) continue;
                        Cell temp = GridManager.GetCellAt(i, y, z);
                        IOrder order = temp.GetOrder();
                        if (order != null)
                        {
                            if (!order.GetStarted() && CurrOrder.GetTime() == order.GetTime())
                            {
                                Debug.Log("x adjacent found");
                                numAdjacent++; //if they do, add 1 to the amount of adjacent cells;
                            }
                        }
                    }
                    for (int i = z - 1; i < z + 2; i++) //now do the same for the z axis;
                    {
                        if (i < 0 || i > 99) continue;
                        Cell temp = GridManager.GetCellAt(x, y, i);
                        IOrder order = temp.GetOrder();
                        if (order != null)
                        {
                            if (!order.GetStarted() && CurrOrder.GetTime() == order.GetTime())
                            {
                                Debug.Log("z adjacent found");
                                numAdjacent++;
                            }
                        }
                    }
                    if (numAdjacent < currAdjacent)
                    {
                        currSelected = cell;
                        currAdjacent = numAdjacent;
                    }
                    else if (numAdjacent == currAdjacent)
                    {
                        Debug.Log("adjacent amount same");
                        if (Random.Range(0, 2) == 0) currSelected = cell;
                    }
                }
                builderController.ClaimOrder(currSelected.GetOrder(), this);
                SetOrder(currSelected.GetOrder(), builderController.GetClosestPath(currSelected.GetOrder().GetLocation(), this));
            }
        }
        else CurrOrder = null;
    }

    public void SetOrder(IOrder toDo, Vector3 location)
    {
        if (location.Equals(new Vector3())) recalculateOrderPath = true;
        else agent.SetDestination(location);
        CurrOrder = toDo;
        CurrOrder.StartOrder();
        orderStarted = false;
    }

    public bool IsWorking()
    {
        return CurrOrder != null;
    }

    private void Idle()
    {
        Debug.Log("Builder at " + transform.position + " now Idling.");
        List<Room> rooms = GridManager.GetRooms();
        Debug.Log("Room count is currently " + rooms.Count + " Rooms are: ");
        foreach (Room room in rooms) Debug.Log(room);
        agent.SetDestination(rooms[Random.Range(0, rooms.Count)].GetRandomPoint()); //might be necessary to come back here to ensure the destination lands on the navmesh
    }

    IEnumerator BuildCell() //Remember to come back here and connect it to canceling order in some way, same with build item;
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
        CellOrder temp = (CellOrder)CurrOrder;
        GridManager.UpdateGrid(temp.GetCell(), temp.GetToBuild());//now that we're done, remove the order and change the cell
        builderController.RemoveOrder(CurrOrder);
        tryClaim = true; ; //now that we're done building, try to claim a new adjacent order
        builderController.DoClaimStutter();
        Debug.Log("Build End");
    }

    IEnumerator BuildItem()
    {
        ItemOrder toDo = (ItemOrder)CurrOrder;
        Debug.Log("Build Item Begin");
        WaitForSeconds wait = new WaitForSeconds(1);
        for (int i = 0; i < 4; i++)
        {
            //we'll use have an animator controller here to "build" the item, and send something to the location to visually update the building progress but those are future problems
            Debug.Log("Building Progress: " + i);
            yield return wait;
        }
        Debug.Log("Building progress End");
        ItemOrder temp = (ItemOrder)CurrOrder;
        GridManager.UpdateItemGrid(temp.GetCells(), temp.GetToBuild(), temp.GetLocation(),temp.GetRotation());//now that we're done, remove the order and add the item
        builderController.RemoveOrder(CurrOrder);
        tryClaim = true; //now that we're done building, try to claim a new adjacent order
        builderController.DoClaimStutter();
        Debug.Log("Build Item End");
    }

    public void CancelOrder() //Cancel the active order
    {
        agent.SetDestination(transform.position);
        if (orderStarted && CurrOrder.GetOrderType() == "cellBuild" || CurrOrder.GetOrderType() == "itemBuild") StopCoroutine(activeCoroutine);
        CurrOrder = null;
    }

    public void AddController(BuilderController toAdd)
    {
        builderController = toAdd;
    }

}
