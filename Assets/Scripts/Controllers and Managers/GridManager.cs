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
    /*
    private static Queue<Order> highPriorityQueue;
    private static Queue<Order> lowPriorityQueue;
    private static List<Order> activeOrders;
    */
    //private static Dictionary<string, int> dungeonStats;
    private static Dictionary<string, List<Container>> containers;
    private static Dictionary<string, List<Restorative>> restoratives;
    private static List<Room> rooms;
    private static List<Room> guardRooms;
    private static List<Patrol> patrols;
    //private static List<NavMeshBuildSource> sources;

    private static bool defenseVisibility;

    public float xOffset;
    public float yOffset;
    public float seed;

    // Start is called before the first frame update
    void Start()
    {
        worldGeography = GameObject.Find("WorldGeography");
        cellHolder = GameObject.Find("CellHolder");
        /*
        highPriorityQueue = new Queue<Order>();
        lowPriorityQueue = new Queue<Order>();
        activeOrders = new List<Order>();
        dungeonStats = new Dictionary<string, int>() //Cells will add and subtract to here to keep track of important stats
        {
            {"Prestige", 0}, // Prestige will affect progression, higher prestige comes from higher quality cell types and minions, and, in turn, will attract higher quality heroes
            {"Tier 1 Cap", 0}, // Recruitment based rooms will increase the amount of minions of certain tiers that can be hired. higher tiered rooms will be more expensive to make
            {"Gold", 0}, // Treasury Items will increase the amount of gold that can be stored. Gold will be used to build rooms of higher quality, and to hire and pay minions of higher qualities. Perhaps some item crafting as well
            {"Ore", 0}, // Some Items will increase the amount of special ores that can be held for the use of crafting
            {"Weapons", 0 } // Special weapons will need to be stored in specific armory items
        };
        */
        containers = new Dictionary<string, List<Container>>() //Containers will be added to here to keep track of their locations for the builders
        {
            {"Gold", new List<Container>()},
            {"Ore", new List<Container>()},
            {"Weapons", new List<Container>()}
        };
        restoratives = new Dictionary<string, List<Restorative>>() //Restorative items will be added here to keep track of their locations for the monsters that need them
        {
            {"Health", new List<Restorative>()},
            {"Stamina", new List<Restorative>()},
            {"Magic", new List<Restorative>()}
        };

        rooms = new List<Room>();
        guardRooms = new List<Room>();
        patrols = new List<Patrol>();

        activeLayer = 3;
        grid = new Cell[100, 4, 100];

        GameObject ground = Instantiate(Resources.Load<GameObject>("Special/Ground"), worldGeography.transform);
        ground.layer = 6;
        ground.AddComponent<Ground>();

        defenseVisibility = false;
        StartCoroutine(DungeonBuilder.BuildGrid(grid, activeLayer, worldGeography, cellHolder, xOffset, yOffset, seed));
        PlayerControls.SetInfo(activeLayer);
    }

    public static bool CheckAdjacent(Cell toCheck) //Checks to see if the cell attempting to be accessed can be accessed
    {
        Debug.Log("GridManager.CheckAdjacent is not yet dead");
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

    public static List<Cell> GetAdjacent(Vector3 center) //return a list containing the four adjacent cells
    {
        int x = (int)center.x;
        int y = (int)center.y;
        int z = (int)center.z;
        return new List<Cell>()
        {
            GetCellAt(x-1, y, z),
            GetCellAt(x+1, y, z),
            GetCellAt(x, y, z-1),
            GetCellAt(x, y, z+1),
        };
    }
    public static List<Cell> GetAdjacent(Cell center)
    {
        return GetAdjacent(center.GetLocation());
    }

    public static void UpdateGrid(Cell toUpdate, string toFetch) //Changes the cell in toUpdate with the one stored in toFetch
    {
        Vector3 location = toUpdate.GetLocation();
        int x =  (int)location.x;
        int y = (int) location.y;
        int z = (int) location.z;
        GameObject temp = Instantiate(Resources.Load<GameObject>("Cells/" + toFetch));//, cellHolder.transform);
        Cell cell = temp.GetComponent<Cell>();
        grid[x, y, z] = cell;
        temp.transform.position = location;
        if (y == activeLayer)
        {
            Debug.Log("Built on ActiveLayer");
            foreach(Transform transform in temp.transform)
            {
                transform.gameObject.layer = 6;
            }
            temp.layer = 6;
            if (cell.TraitsContains("Transparent") && !cell.TraitsContains("Traversable")) grid[x, y-1, z].gameObject.layer = 7;
        }
        else if (y == activeLayer - 1 && grid[x, y + 1, z].TraitsContains("Transparent")) temp.layer = 7;
        else temp.layer = 8;
        if (cell.TraitsContains("Transparent")) //set the visibilty of this and adjacent cells walls depending on if they'll be visible to the camera;
        {
            Cell adjacent;
            if (x != 99)
            {
                adjacent = grid[x + 1, y, z];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(1, false);
                    adjacent.ShowSide(2, false);
                }
                else
                {
                    cell.ShowSide(1, true);
                    adjacent.ShowSide(2, true); //I still need to more concretely decide what happens where transparent and opaque cells meet;
                }
            }

            if (x != 0)
            {
                adjacent = grid[x - 1, y, z];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(2, false);
                    adjacent.ShowSide(1, false);
                }
                else
                {
                    cell.ShowSide(2, true);
                    adjacent.ShowSide(1, true);
                }
            }

            if (z != 99)
            {
                adjacent = grid[x, y, z + 1];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(4, false);
                    adjacent.ShowSide(3, false);
                }
                else
                {
                    cell.ShowSide(4, true);
                    adjacent.ShowSide(3, true);
                }
            }

            if (z != 0)
            {
                adjacent = grid[x, y, z - 1];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(3, false);
                    adjacent.ShowSide(4, false);
                }
                else
                {
                    cell.ShowSide(3, true);
                    adjacent.ShowSide(4, true);
                }
            }
        }
        else
        {
            Cell adjacent;
            if (x != 99)
            {
                adjacent = grid[x + 1, y, z];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(1, true);
                    adjacent.ShowSide(2, true);
                }
                else
                {
                    cell.ShowSide(1, false);
                    adjacent.ShowSide(2, false); //I still need to more concretely decide what happens where transparent and opaque cells meet;
                }
            }

            if (x != 0)
            {
                adjacent = grid[x - 1, y, z];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(2, true);
                    adjacent.ShowSide(1, true);
                }
                else
                {
                    cell.ShowSide(2, false);
                    adjacent.ShowSide(1, false);
                }
            }

            if (z != 99)
            {
                adjacent = grid[x, y, z + 1];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(2, true);
                    adjacent.ShowSide(1, true);
                }
                else
                {
                    cell.ShowSide(2, false);
                    adjacent.ShowSide(1, false);
                }
            }

            if (z != 0)
            {
                adjacent = grid[x, y, z - 1];
                if (adjacent.TraitsContains("Transparent"))
                {
                    cell.ShowSide(2, true);
                    adjacent.ShowSide(1, true);
                }
                else
                {
                    cell.ShowSide(2, false);
                    adjacent.ShowSide(1, false);
                }
            }
        }

        if (toUpdate.GetResourceType() != "None")
            Instantiate(Resources.Load<GameObject>("Minerals/" + toUpdate.GetResourceType())).transform.position = toUpdate.transform.position;
        Destroy(toUpdate.gameObject);
        //Debug.Log(worldGeography.GetComponent<NavMeshSurface>().collectObjects);
        if (cell.TraitsContains("Traversable")) temp.transform.parent = worldGeography.transform;
        else temp.transform.parent = cellHolder.transform;
        //updateMesh = true;
        
        worldGeography.GetComponent<NavMeshSurface>().UpdateNavMesh(worldGeography.GetComponent<NavMeshSurface>().navMeshData); // I want to come back here to see if I can find a more cost-effective way of doing this. I would love to see if there was a way to just add a single 1x1 cube to the existing mesh
        cell.CheckRoomStatus();
    }
    
    public static void UpdateResources(Container updateFrom)
    {
        DungeonStats.UpdateStat(updateFrom.itemType, updateFrom.maxAmount);
        containers[updateFrom.itemType].Add(updateFrom);
    }

    public static Container GetClosestValidContainer(Resource toTransit, string type)
    {
        NavMeshPath path = new NavMeshPath();
        float closestDistance = float.MaxValue;
        Container closestContainer = null;
        foreach (Container container in containers[type])
        {
            //Debug.Log("New Container");
            if (!container.IsFull()) // prevent pathfinding to full containers
            {
                Debug.Log("Container Position: " + container.transform.position);
                Debug.Log("Resource Position: " + toTransit.transform.position);
                if (NavMesh.CalculatePath(toTransit.transform.position, container.transform.position, NavMesh.AllAreas, path))
                {
                    float distance = Vector3.Distance(toTransit.transform.position, path.corners[0]);
                    for (int y = 1; y < path.corners.Length; y++)
                    {
                        distance += Vector3.Distance(path.corners[y - 1], path.corners[y]); //maybe I'll do some additional math here in the future to create a preference for containers with more open room or that are more secure
                    }
                    //Debug.Log("Current closest distance is: " + closestDistance);
                    if (distance < closestDistance)
                    {
                        closestContainer = container;
                        closestDistance = distance;
                    }
                }
                else
                {
                    NavMeshHit hit = new NavMeshHit();
                    if (NavMesh.SamplePosition(toTransit.transform.position, out hit, 2, NavMesh.AllAreas))
                    {
                        NavMesh.CalculatePath(hit.position, container.transform.position, NavMesh.AllAreas, path);
                            float distance = Vector3.Distance(toTransit.transform.position, path.corners[0]);
                        for (int y = 1; y < path.corners.Length; y++)
                        {
                            distance += Vector3.Distance(path.corners[y - 1], path.corners[y]); //maybe I'll do some additional math here in the future to create a preference for containers with more open room or that are more secure
                        }
                        //Debug.Log("Current closest distance is: " + closestDistance);
                        if (distance < closestDistance)
                        {
                            closestContainer = container;
                            closestDistance = distance;
                        }
                    }
                    else
                    {
                        Debug.LogError("Invalid Path for transit");
                        return null;
                    }
                }
            }
        }
        return closestContainer;
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

        //Container toAdd = temp.GetComponent<Container>();
        //if (toAdd != null) UpdateResources(toAdd); //If the item being added is a container, add it's stat changes to the dungeon. This'll now be done in Containers awake.
        temp.layer = toUpdate.gameObject.layer; //because the item is inside a cell, it should have the same layer
        /*Vector3 toModify = temp.transform.position;
        toModify.y += temp.transform.localScale.y / 2; 
        temp.transform.position = toModify;*/
        
        /* notes for rotaion:
         * 0 = 0x, 0z
         * 90 = 1x, 0z
         * 180 = 0x, 0z
         * 270 = 1x, 1z
         */
    }

    public static void AddRoom(Room toAdd)
    {
        rooms.Add(toAdd);
        if (toAdd.IsGuardRoom())
        {
            guardRooms.Add(toAdd);
            if (defenseVisibility) toAdd.ToggleVisible(true);
        }
    }

    public static void RemoveRoom(Room toRemove)
    {
        rooms.Remove(toRemove);
        if (toRemove.IsGuardRoom()) guardRooms.Remove(toRemove);
    }

    public static List<Room> GetRooms()
    {
        return rooms;
    }

    public static void AddRestorative(Restorative toAdd, string type)
    {
        Debug.Log("Restorative of type: " + type + " added");
        restoratives[type].Add(toAdd);
        Debug.Log("There are now " + restoratives[type].Count + " restoratives of this type");
    }

    public static void RemoveRestorative(Restorative toRemove, string type)
    {
        Debug.Log("Removing a restorative of type " + type);
        restoratives[type].Remove(toRemove);
    }

    public static List<Restorative> GetRestoratives(string type)
    {
        Debug.Log("There are currently " + restoratives[type].Count + " restoratives of type " + type);
        return restoratives[type];
    }

    public static Cell GetCellAt(int x, int y, int z)
    {
        Debug.Log("Getting cell at " + new Vector3(x, y, z));
        if(x > 99 || x < 00 || z > 99 || z < 0) return null;
        return grid[x,y,z];
    }

    public static void ToggleDefenseVisibility (bool toToggle)
    {
        foreach (Room room in guardRooms) room.ToggleVisible(toToggle);
        foreach (Patrol patrol in patrols) patrol.ToggleVisible(toToggle);
        defenseVisibility = toToggle;
    }

    public static void AddPatrol(Patrol toAdd)
    {
        patrols.Add(toAdd);
    }

    public static void RemovePatrol(Patrol toRemove)
    {
        patrols.Remove(toRemove);
    }
}
