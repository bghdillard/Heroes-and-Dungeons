using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GridManager : MonoBehaviour
{

    private static Cell[,  ,] grid; 
    [HideInInspector]
    public static int activeLayer;
    private static GameObject worldGeography; // world geography will manage the navmesh
    private static GameObject cellHolder; //cell holder will keep track of the cells for the purposes of interacting and visuals
    private static Queue<Order> priorityQueue;
    private static List<Order> activeOrders;
    private static Dictionary<string, int> dungeonStats;
    private static Dictionary<string, List<Container>> containers;
    private static bool updateMesh;
    private static List<NavMeshBuildSource> sources;

    public float xOffset;
    public float yOffset;
    public float seed;

    // Start is called before the first frame update
    void Start()
    {
        worldGeography = GameObject.Find("WorldGeography");
        cellHolder = GameObject.Find("CellHolder");
        priorityQueue = new Queue<Order>();
        activeOrders = new List<Order>();
        dungeonStats = new Dictionary<string, int>() //Cells will add and subtract to here to keep track of important stats
        {
            {"Prestige", 0}, // Prestige will affect progression, higher prestige comes from higher quality cell types and minions, and, in turn, will attract higher quality heroes
            {"Tier 1 Cap", 0}, // Recruitment based rooms will increase the amount of minions of certain tiers that can be hired. higher tiered rooms will be more expensive to make
            {"Gold", 0}, // Treasury Items will increase the amount of gold that can be stored. Gold will be used to build rooms of higher quality, and to hire and pay minions of higher qualities. Perhaps some item crafting as well
            {"Ore", 0}, // Some Items will increase the amount of special ores that can be held for the use of crafting
            {"Weapons", 0 } // Special weapons will need to be stored in specific armory items
        };
        containers = new Dictionary<string, List<Container>>() //Containers will be added to here to keep track of their locations for the builders
        {
            {"Gold", new List<Container>()},
            {"Ore", new List<Container>()},
            {"Weapons", new List<Container>()}
        };
        Instantiate(Resources.Load<GameObject>("Special/Ground"), worldGeography.transform).layer = 6;
        updateMesh = false;
        activeLayer = 3;
        grid = new Cell[100, 4, 100];
        StartCoroutine(DungeonBuilder.BuildGrid(grid, activeLayer, worldGeography,cellHolder, xOffset, yOffset, seed));
    }

    void LateUpdate()
    {
        if (updateMesh)
        {
            updateMesh = false;
            StartCoroutine(UpdateMesh());
        }
    }

    public static void SetSources(List<NavMeshBuildSource> toSet)
    {
        sources = toSet;
    }

    public static void AddtoQueue(Order toAdd)
    {
        priorityQueue.Enqueue(toAdd);
    }

    public static Queue<Order> GetQueue()
    {
        return priorityQueue;
    }

    public static void AddtoList(Order toAdd)
    {
        activeOrders.Add(toAdd);
    }

    public static void RemoveFromList(Order toRemove)
    {
        activeOrders.Remove(toRemove);
    }

    public static bool ListContains(Order toCheck)
    {
        return activeOrders.Contains(toCheck);
    }

    public static bool QueueContains(Order toCheck)
    {
        return priorityQueue.Contains(toCheck);
    }

    public static void CancelOrder(Order toCancel)
    {
        if (QueueContains(toCancel)) //If the queue contains the order, remove the order from the queue
        {
            Queue<Order> temp = new Queue<Order>();
            foreach (Order order in priorityQueue)
            {
                if (order == toCancel) continue;
                temp.Enqueue(order);
            }
            priorityQueue = temp;
        }
        else if (ListContains(toCancel)) //If the list contains the order, remove the order from the list and stop the builder
        {
            foreach (Order order in activeOrders)
            {
                if (order.Equals(toCancel))
                {
                    order.Cancel();
                    break;
                }
            }
            activeOrders.Remove(toCancel);
        }
        else Debug.Log("Canceled Order not found"); //If neither contain the order, we have a problem
    }

    public static bool CheckAdjacent(Cell toCheck) //Checks to see if the cell attempting to be accessed can be accessed
    {
        Vector3 location = toCheck.GetLocation();
        int x = (int)location.x;
        int y = (int)location.y;
        int z = (int)location.z;
        for (int i = x - 1; i < x + 2; i++){
            if (i < 0) return true;
            if (i > 99) continue;
            if (grid[i, y, z].TraitsContains("Traversable"))
            {
                Debug.Log("Can be reached");
                return true;
            }
        }
        for (int i = z - 1; i < z + 2; i++)
        {
            if (i < 0 || i > 99) continue;
            if (grid[x, y, i].TraitsContains("Traversable"))
            {
                Debug.Log("Can be reached");
                return true;
            }
        }
        Debug.Log("Can't be reached");
        return false;
    }

    public static void UpdateGrid(Cell toUpdate, string toFetch) //Changes the cell in toUpdate with the one stored in toFetch
    {
        Vector3 location = toUpdate.GetLocation();
        int x =  (int)location.x;
        int y = (int) location.y;
        int z = (int) location.z;
        GameObject temp = Instantiate(Resources.Load<GameObject>("Cells/" + toFetch), cellHolder.transform);
        grid[x, y, z] = temp.GetComponent<Cell>();
        temp.transform.position = location;
        if (y == activeLayer)
        {
            Debug.Log("Built on ActiveLayer");
            temp.layer = 6;
            if (temp.GetComponent<Cell>().TraitsContains("Transparent") && !temp.GetComponent<Cell>().TraitsContains("Traversable")) grid[x, y-1, z].gameObject.layer = 7;
        }
        else if (y == activeLayer - 1 && grid[x, y + 1, z].TraitsContains("Transparent")) temp.layer = 7;
        else temp.layer = 8;
        Destroy(toUpdate.gameObject);
        Debug.Log(worldGeography.GetComponent<NavMeshSurface>().collectObjects);
        if (temp.GetComponent<Cell>().TraitsContains("Traversable"))
        {
            GameObject floor = Instantiate(Resources.Load<GameObject>("Special/Floor"), worldGeography.transform);
            floor.transform.position = new Vector3(x, y - 0.5f, z);
        }
        //updateMesh = true;
        worldGeography.GetComponent<NavMeshSurface>().UpdateNavMesh(worldGeography.GetComponent<NavMeshSurface>().navMeshData); // I want to come back here to see if I can find a more cost-effective way of doing this. I would love to see if there was a way to just add a single 1x1 cube to the existing mesh
    }

    private static IEnumerator UpdateMesh()
    {
        var operation = NavMeshBuilder.UpdateNavMeshDataAsync(
        worldGeography.GetComponent<NavMeshSurface>().navMeshData,
        worldGeography.GetComponent<NavMeshSurface>().GetBuildSettings(),
        sources,
        new Bounds(Vector3.zero, new Vector3(1000, 1000, 1000)) // set these accordingly
    );
        do { yield return null; } while (!operation.isDone);
        /*
        AsyncOperation operation = worldGeography.GetComponent<NavMeshSurface>().UpdateNavMesh(worldGeography.GetComponent<NavMeshSurface>().navMeshData);
        Debug.Log(operation.isDone);
        while (!operation.isDone)
        {
            Debug.Log("still working");
            Debug.Log("Progress: " + operation.progress);
            yield return null;
        }
        */
    }

    private static void UpdateResources(Container updateFrom)
    {
        dungeonStats[updateFrom.type] += updateFrom.maxAmount;
        containers[updateFrom.type].Add(updateFrom);
    }

    public static void UpdateItemGrid(Cell toUpdate, string toFetch, int rotation) //Changes the item in the location to the one stored in toFetch
    {
        //Debug.Log("Yeah... I still need to cry over this one a bit");
        GameObject temp = Instantiate(Resources.Load<GameObject>("Items/" + toFetch), toUpdate.transform);
        if (rotation == 0) //set the location in the cell depending on the rotation
        {
            temp.transform.Rotate(0, 0, 0, Space.Self);
            temp.transform.localPosition = new Vector3(0, -0.5f + (temp.transform.localScale.y / 2), -0.5f + (temp.transform.localScale.z / 2));
        }
        else if (rotation == 90)
        {
            temp.transform.Rotate(0, 0, 0, Space.Self);
            temp.transform.localPosition = new Vector3(-0.5f + (temp.transform.localScale.x / 2), -0.5f + (temp.transform.localScale.y / 2), 0);
        }
        else if (rotation == 180)
        {
            temp.transform.Rotate(0, 0, 0, Space.Self);
            temp.transform.localPosition = new Vector3(0, -0.5f + (temp.transform.localScale.y / 2), 0.5f - (temp.transform.localScale.z / 2));
        }
        else if (rotation == 270)
        {
            temp.transform.Rotate(0, 0, 0, Space.Self);
            temp.transform.localPosition = new Vector3(0.5f - (temp.transform.localScale.x / 2), -0.5f + (temp.transform.localScale.y / 2), 0);
        }

        Container toAdd = temp.GetComponent<Container>();
        if (toAdd != null) UpdateResources(toAdd); //If the item being added is a container, add it's stat changes to the dungeon
        temp.layer = toUpdate.gameObject.layer; //because the item is inside a cell, it should have the same layer
        
        /* notes for rotaion:
         * 0 = 0x, 0z
         * 90 = 1x, 0z
         * 180 = 0x, 0z
         * 270 = 1x, 1z
         */


    }
}
