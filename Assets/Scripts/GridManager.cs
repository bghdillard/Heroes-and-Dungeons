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
    private static GameObject worldGeography;
    private static Queue<Order> priorityQueue;

    // Start is called before the first frame update
    void Start()
    {
        worldGeography = GameObject.Find("WorldGeography");
        priorityQueue = new Queue<Order>();
        Instantiate(Resources.Load<GameObject>("Ground"), worldGeography.transform).layer = 6;
        activeLayer = 3;
        grid = new Cell[100, 4, 100];
        StartCoroutine(DungeonBuilder.BuildGrid(grid, activeLayer, worldGeography));
    }

    public static void AddtoQueue(Order toAdd)
    {
        priorityQueue.Enqueue(toAdd);
    }

    public static Queue<Order> GetQueue()
    {
        return priorityQueue;
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
        GameObject temp = Instantiate(Resources.Load<GameObject>("Cells/" + toFetch), worldGeography.transform);
        grid[x, y, z] = temp.GetComponent<Cell>();
        temp.transform.position = location;
        if (y == activeLayer)
        {
            Debug.Log("Built on ActiveLayer");
            temp.layer = 6;
            if (temp.GetComponent<Cell>().TraitsContains("Transparent")) grid[x, y-1, z].gameObject.layer = 7;
        }
        else if (y == activeLayer - 1 && grid[x, y + 1, z].TraitsContains("Transparent")) temp.layer = 7;
        else temp.layer = 8;
        Destroy(toUpdate.gameObject);
        worldGeography.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
