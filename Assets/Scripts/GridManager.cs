using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class GridManager : MonoBehaviour
{

    private static Cell[,  ,] grid; 
    private static int activeLayer;
    private static GameObject worldGeography;
    private static Queue<Order> priorityQueue;
    private Coroutine toStop;

    // Start is called before the first frame update
    void Start()
    {
        worldGeography = GameObject.Find("WorldGeography");
        priorityQueue = new Queue<Order>();
        toStop = StartCoroutine(BuildGrid());
    }

    IEnumerator BuildGrid()
    {
        grid = new Cell[100, 4, 100];
        activeLayer = 3;
        int centerX = Random.Range(20, 71);
        int centerZ = Random.Range(20, 71); //Select a random x and z coordinate to be the center of the initial dungeon
        GameObject temp;
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    //If at the square forming the initial dungeon...
                    if ((x == centerX || x == centerX - 1 || x == centerX - 2 || x == centerX - 3 || x == centerX + 1 || x == centerX + 2 || x == centerX + 3)
                        && (z == centerZ || z == centerZ - 1 || z == centerZ - 2 || z == centerZ - 3 || z == centerZ + 1 ||  z == centerZ + 2 || z == centerZ + 3))
                    {
                        Debug.Log(centerX);
                        Debug.Log(centerZ);
                        if (y == activeLayer)//...and at the right layer, set those cells to be the initial room...
                        {
                            temp = Instantiate(Resources.Load<GameObject>("Cells/Empty"), worldGeography.transform);
                            grid[x, y, z] = temp.GetComponent<Cell>();
                            temp.transform.position = new Vector3(x, y, z);
                            temp.layer = 6;
                        }
                        else
                        {
                            temp = Instantiate(Resources.Load<GameObject>("Cells/Stone"), worldGeography.transform); //On game start, set all possible slots to plain stone.
                            grid[x, y, z] = temp.GetComponent<Cell>();
                            temp.transform.position = new Vector3(x, y, z);
                            if (y == activeLayer - 1) temp.layer = 7; //if directly below the initial active layer, set to be visible but non interactive
                            else temp.layer = 8; //otherwise, set to be invisible and non interactive
                        }
                    }
                    else if ( y == activeLayer) //If at the top of the dungeon, use a stone that prevents a navmesh from generating on top
                    {
                        temp = Instantiate(Resources.Load<GameObject>("Cells/Impassable Stone"), worldGeography.transform);
                        grid[x, y, z] = temp.GetComponent<Cell>();
                        temp.transform.position = new Vector3(x, y, z);
                        temp.layer = 6;
                    }
                    else
                    {
                        temp = Instantiate(Resources.Load<GameObject>("Cells/Stone"), worldGeography.transform); //On game start, set all possible slots to plain stone.
                        grid[x, y, z] = temp.GetComponent<Cell>();
                        temp.transform.position = new Vector3(x, y, z);
                        temp.layer = 8; //set to be invisible and non interactive
                    }
                }
            }
            yield return null;
        }
        GameObject camera = GameObject.Find("Main Camera");
        camera.transform.position = new Vector3(centerX -4, activeLayer * 4, centerZ - 4);
        camera.transform.LookAt(new Vector3(centerX, activeLayer * 2, centerZ));
        worldGeography.transform.GetComponent<NavMeshSurface>().BuildNavMesh();
        for(int i = 0; i < 4; i++)
        {
            temp = Instantiate(Resources.Load<GameObject>("Undead/Builder"));
            if(i % 2 == 0) temp.GetComponent<NavMeshAgent>().Warp (new Vector3(centerX + i * 2 - 2, activeLayer, centerZ));
            else temp.GetComponent<NavMeshAgent>().Warp (new Vector3(centerX, activeLayer, centerZ + (i - 1) * 2 - 2));
        }
        StopCoroutine(toStop);
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
            if (grid[i, y, z].TraitsContains("Traversable"))
            {
                Debug.Log("Can be reached");
                return true;
            }
        }
        for (int i = z - 1; i < z + 2; i++)
        {
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
