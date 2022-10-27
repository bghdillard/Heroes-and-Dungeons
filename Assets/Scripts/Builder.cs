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
    private Coroutine activeCoroutine;
    private BuilderController builderController;

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
    }

    public void SetOrder(IOrder toDo, Vector3 location)
    {
        agent.SetDestination(location);
        CurrOrder = toDo;
        orderStarted = false;
    }

    public bool IsWorking()
    {
        return CurrOrder != null;
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
        CurrOrder = null;
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
        GridManager.UpdateItemGrid(temp.GetCell(), temp.GetToBuild(), temp.GetRotation());//now that we're done, remove the order and add the item
        builderController.RemoveOrder(CurrOrder);
        CurrOrder = null;
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
