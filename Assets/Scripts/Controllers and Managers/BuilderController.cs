using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuilderController : MonoBehaviour 
{
    private List<Builder> builders;

    private Queue<IOrder> highPriorityQueue;
    private Queue<IOrder> lowPriorityQueue;
    private List<IOrder> activeOrders;
    private Dictionary<IOrder, Builder> builderOrders;
    private bool claimStutter;

    private void Start()
    {
        builders = new List<Builder>();
        highPriorityQueue = new Queue<IOrder>();
        lowPriorityQueue = new Queue<IOrder>();
        activeOrders = new List<IOrder>();
        builderOrders = new Dictionary<IOrder, Builder>();
        claimStutter = false;
    }

    //In the future, I may make it so that there's one of this class for each floor, limiting each builder to work only on their own, though that might cause issues with the initial building of each new floor, regardless, future problem;

    private void Update()
    {
        if (!claimStutter)
        {
            if (highPriorityQueue.TryDequeue(out IOrder toDo))
            {
                Debug.Log("Order removed from queue");
                Builder closestBuilder = null;
                NavMeshPath path = new NavMeshPath();
                float closestDistance = float.MaxValue;
                Vector3 target = new Vector3();
                foreach (Builder builder in builders) //find the closest builder; for building, this requires checking the four adjacent cells;
                {
                    if (!builder.IsWorking())
                    {
                        Debug.Log("Now checking builder at: " + builder.transform.position);
                        Vector3 center = toDo.GetLocation();
                        for (int i = (int)center.x - 1; i < center.x + 2; i++) //A note to future me, this area is very likely to get very expensive;
                        {
                            if (NavMesh.CalculatePath(builder.transform.position, new Vector3(i, center.y, center.z), -1, path))
                            {
                                float distance = Vector3.Distance(builder.transform.position, path.corners[0]);
                                for (int y = 1; y < path.corners.Length; y++)
                                {
                                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                                }
                                Debug.Log("Final distance on this path is " + distance);
                                if (distance < closestDistance)
                                {
                                    Debug.Log("Distance " + distance + " is less than current closest " + closestDistance + ". Builder at " + builder.transform.position + " is new closest");
                                    closestBuilder = builder;
                                    closestDistance = distance;
                                    target = new Vector3(i, center.y, center.z);
                                }
                            }
                        }
                        Debug.Log("Closest distance moving to z is: " + closestDistance);
                        for (int i = (int)center.z - 1; i < center.z + 2; i++)
                        {
                            if (NavMesh.CalculatePath(builder.transform.position, new Vector3(center.x, center.y, i), -1, path))
                            {
                                float distance = Vector3.Distance(builder.transform.position, path.corners[0]);
                                for (int y = 1; y < path.corners.Length; y++)
                                {
                                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                                }
                                Debug.Log("Final distance on this path is " + distance);
                                if (distance < closestDistance)
                                {
                                    Debug.Log("Distance " + distance + " is less than current closest " + closestDistance + ". Builder at " + builder.transform.position + " is new closest");
                                    closestBuilder = builder;
                                    closestDistance = distance;
                                    target = new Vector3(center.x, center.y, i);
                                }
                            }
                        }
                    }
                }
                if (closestBuilder != null)
                {
                    Debug.Log("Selecting builder at: " + closestBuilder.transform.position);
                    closestBuilder.SetOrder(toDo, target);
                    activeOrders.Add(toDo);
                    builderOrders.Add(toDo, closestBuilder);
                }
                else
                {
                    Debug.Log("Builder not Selecetd; returning Order");
                    highPriorityQueue.Enqueue(toDo);
                }
            }
            else if (lowPriorityQueue.TryDequeue(out toDo))
            {
                Builder closestBuilder = null;
                float closestDistance = float.MaxValue;
                foreach (Builder builder in builders)
                {
                    if (!builder.IsWorking())
                    {
                        float distance = Vector3.Distance(builder.transform.position, toDo.GetLocation());
                        if (distance < closestDistance)
                        {
                            closestBuilder = builder;
                            closestDistance = distance;
                        }
                    }
                }
                if (closestBuilder != null)
                {
                    closestBuilder.SetOrder(toDo, toDo.GetLocation());
                    activeOrders.Add(toDo);
                    builderOrders.Add(toDo, closestBuilder);
                }
                else lowPriorityQueue.Enqueue(toDo);
            }
        }
        else claimStutter = false;
    }

    public void AddBuilder(Builder toAdd)
    {
        builders.Add(toAdd);
        toAdd.AddController(this);
    }

    public bool ContainsOrder(IOrder toCheck)
    {
        return highPriorityQueue.Contains(toCheck) || lowPriorityQueue.Contains(toCheck) || activeOrders.Contains(toCheck);
    }

    public void CancelOrder(HashSet<IOrder> toCancel)
    {
        Queue<IOrder> temp = new Queue<IOrder>();
        IOrder order;
        while (highPriorityQueue.TryDequeue(out order)) if (!toCancel.Contains(order)) temp.Enqueue(order); //check the high priority queue for the orders and remove them if found
        highPriorityQueue = temp;
        while (lowPriorityQueue.TryDequeue(out order)) if (!toCancel.Contains(order)) temp.Enqueue(order); // check the low priority queue for the orders and remove them if found
        lowPriorityQueue = temp;
        List<IOrder> toRemove = new List<IOrder>();
        foreach (IOrder toCheck in activeOrders) //check the active orders for the orders and remove them if found
        {
            if (toCancel.Contains(toCheck))
            {
                toRemove.Add(toCheck);
                builderOrders[toCheck].CancelOrder();
                builderOrders.Remove(toCheck);
            }
        }
        if (toRemove.Count != 0) foreach (IOrder toCheck in toRemove) activeOrders.Remove(toCheck);
        foreach (IOrder toCheck in toCancel) toCheck.CancelOrder(); //finalize canceling the orders
        /*
        if (highPriorityQueue.Contains(toCancel)) //If the queue contains the order, remove the order from the queue
        {
            Debug.Log("Order in high Queue");
            Queue<IOrder> temp = new Queue<IOrder>();
            Debug.Log("high queue size before removing: " + highPriorityQueue.Count);
            foreach (IOrder order in highPriorityQueue)
            {
                if (order == toCancel) continue;
                Debug.Log("Order not found here");
                temp.Enqueue(order);
            }
            Debug.Log("temp queue size after removing: " + temp.Count);
            highPriorityQueue = temp;
            Debug.Log("high queue size after removing: " + highPriorityQueue.Count);
        }
        else if (lowPriorityQueue.Contains(toCancel))
        {
            Queue<IOrder> temp = new Queue<IOrder>();
            foreach (IOrder order in lowPriorityQueue)
            {
                if (order == toCancel) continue;
                temp.Enqueue(order);
            }
            lowPriorityQueue = temp;
        }
        else if (activeOrders.Contains(toCancel)) //If the list contains the order, remove the order from the list and stop the builder
        {
            activeOrders.Remove(toCancel);
            builderOrders[toCancel].CancelOrder();
            builderOrders.Remove(toCancel);
        }
        else Debug.LogWarning("Canceled Order not found"); //If none of those contain the order, we have a problem
        toCancel.CancelOrder();
        */
    }

    public void AddToHighQueue(IOrder toAdd)
    {
        highPriorityQueue.Enqueue(toAdd);
    }

    public void AddToLowQueue(IOrder toAdd)
    {
        lowPriorityQueue.Enqueue(toAdd);
    }

    public void RemoveOrder(IOrder toRemove)
    {
        activeOrders.Remove(toRemove);
        builderOrders.Remove(toRemove);
    }

    public void ClaimOrder(IOrder toClaim, Builder claimer)
    {
        Queue<IOrder> temp = new Queue<IOrder>();
        foreach(IOrder order in highPriorityQueue)
        {
            if (order == toClaim) continue;
            temp.Enqueue(order);
        }
        highPriorityQueue = temp;
        builderOrders.Add(toClaim, claimer);
        activeOrders.Add(toClaim);
    }

    public Vector3 GetClosestPath(Vector3 target, Builder builder)
    {
        NavMeshPath path = new NavMeshPath();
        float closestDistance = float.MaxValue;
        Vector3 destination = new Vector3();
        for (int i = (int)target.x - 1; i < target.x + 2; i++) //A note to future me, this area is very likely to get very expensive;
        {
            Debug.Log("Test x");
            if (NavMesh.CalculatePath(builder.transform.position, new Vector3(i, target.y, target.z), -1, path))
            {
                Debug.Log("PathCalculated");
                float distance = Vector3.Distance(builder.transform.position, path.corners[0]);
                for (int y = 1; y < path.corners.Length; y++)
                {
                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                }
                Debug.Log("Final distance on this path is " + distance);
                if (distance < closestDistance)
                {
                    //Debug.Log("Distance " + distance + " is less than current closest " + closestDistance + ". Builder at " + builder.transform.position + " is new closest");
                    closestDistance = distance;
                    destination = new Vector3(i, target.y, target.z);
                }
            }
        }
        Debug.Log("Closest distance moving to z is: " + closestDistance);
        for (int i = (int)target.z - 1; i < target.z + 2; i++)
        {
            Debug.Log("Test z");
            if (NavMesh.CalculatePath(builder.transform.position, new Vector3(target.x, target.y, i), -1, path))
            {
                Debug.Log("PathCalculated");
                float distance = Vector3.Distance(builder.transform.position, path.corners[0]);
                for (int y = 1; y < path.corners.Length; y++)
                {
                    distance += Vector3.Distance(path.corners[y - 1], path.corners[y]);
                }
                Debug.Log("Final distance on this path is " + distance);
                if (distance < closestDistance)
                {
                    //Debug.Log("Distance " + distance + " is less than current closest " + closestDistance + ". Builder at " + builder.transform.position + " is new closest");
                    closestDistance = distance;
                    destination = new Vector3(target.x, target.y, i);
                }
            }
        }
        Debug.Log(destination);
        return destination;
    }

    public void DoClaimStutter() //prevent an order being claimed twice by preventing the order update on a frame when a builder might be making a claim itself
    {
        claimStutter = true;
    }
}