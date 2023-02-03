using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField]
    private List<string> traits;
    [SerializeField]
    private string cellName;
    [SerializeField]
    private string resourceType;
    private List<GameObject> items;
    [SerializeField]
    private List<Material> orderMaterials;
    private IOrder order;
    private Room room;

    private void Awake()
    {
        items = new List<GameObject>();
        //startColor = GetComponent<Renderer>().material.color;
    }

    public void CheckRoomStatus()
    {
        List<Cell> adjacentCells = GridManager.GetAdjacent(transform.position);
        List<Room> adjacentRooms = new List<Room>();
        foreach (Cell cell in adjacentCells)
        {
            if (cell.GetName() == cellName && !adjacentRooms.Contains(cell.GetRoom())) adjacentRooms.Add(cell.GetRoom());
        }
        Debug.Log("This cell is adjacent to " + adjacentRooms.Count + " rooms.");
        if (adjacentRooms.Count == 0) room = new Room(this);
        else if (adjacentRooms.Count == 1) adjacentRooms[0].AddCell(this);
        else
        {
            Debug.Log("Finding Largest Room");
            Room largestRoom = null;
            int largestSize = int.MinValue;
            foreach (Room room in adjacentRooms)
            {
                if (room.GetSize() > largestSize)
                {
                    largestSize = room.GetSize();
                    largestRoom = room;
                }
            }
            Debug.Log("Largest Room found: contains " + largestRoom.GetSize() + " Cells");
            foreach (Room room in adjacentRooms) if (!room.Equals(largestRoom)) largestRoom.MergeRoom(room);
            largestRoom.AddCell(this);
        }
    }

    public bool TraitsContains(string toCheck)
    {
        return traits.Contains(toCheck);
    }

    public string GetName()
    {
        return cellName;
    }

    public Vector3 GetLocation()
    {
        return gameObject.transform.position;
    }

    public List<GameObject> GetItems()
    {
        return items;
    }

    public void AddItem(GameObject toAdd)
    {
        items.Add(toAdd);
    }

    public void RemoveItem(GameObject toRemove)
    {
        items.Remove(toRemove);
    }

    public string GetResourceType()
    {
        return resourceType;
    }

    public void SetColor(int toSet)
    {
        //Debug.Log("Cell SetColor called");
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            //GetComponent<Renderer>().material.color = toSet;
            Material[] temp = new Material[2];
            temp[0] = GetComponent<Renderer>().materials[0];
            temp[1] = orderMaterials[toSet];
            GetComponent<Renderer>().materials = temp;
        }
    }

    public void ResetColor()
    {
        //Debug.Log("Cell ResetColor called");
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Material[] temp = new Material[1];
            temp[0] = GetComponent<Renderer>().materials[0];
            GetComponent<Renderer>().materials = temp;
        }
    }

    public void SetOrder(IOrder toSet)
    {
        order = toSet;
    }

    public void CancelOrder()
    {
        order = null;
    }

    public IOrder GetOrder()
    {
        return order;
    }

    public Room GetRoom()
    {
        return room;
    }

    public void SetRoom(Room toSet)
    {
        room = toSet;
    }
}
