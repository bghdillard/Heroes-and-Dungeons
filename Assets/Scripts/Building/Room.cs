using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<Cell> cells;
    private bool isGuardRoom;

    public Room()
    {
        //parameterless constructor for the ground;
        cells = new List<Cell>();
        isGuardRoom = false;
        GridManager.AddRoom(this);
    }

    public Room(Cell origin)
    {
        cells = new List<Cell> { origin };
        isGuardRoom = origin.TraitsContains("GuardRoom");
        GridManager.AddRoom(this);
        Debug.Log("New Room Created");
    }

    public void AddCell(Cell toMerge)
    {
        cells.Add(toMerge);
        toMerge.SetRoom(this);
        Debug.Log("Cell added to Room. There are now " + cells.Count + " cells in this room.");
    }

    public List<Cell> GetCells()
    {
        return cells;
    }

    public void MergeRoom(Room other)
    {
        foreach (Cell cell in other.GetCells()) cells.Add(cell);
        GridManager.RemoveRoom(other);
        Debug.Log("Rooms Merged");
    }

    public int GetSize()
    {
        return cells.Count;
    }

    public virtual Vector3 GetRandomPoint() //used to get a random point in the room for creatures to idle towards;
    {
        Cell selected = cells[Random.Range(0, cells.Count)]; //start by getting a random cell in the room
        Vector3 point = selected.GetLocation(); //then get a random point in that cell by getting its center
        point.x += Random.Range(-0.49f, 0.49f); //then add or subtract a random value less than its full size to its x and z
        point.z += Random.Range(-0.49f, 0.49f);
        return point;
    }

    public override bool Equals(object other)
    {
        Debug.Log("Room.equals called");
        if (other == null || !GetType().Equals(other.GetType())) return false;
        else
        {
            Room room = (Room)other;
            return room.GetCells().Contains(cells[0]);
        }
    }

    public override int GetHashCode()
    {
        Debug.Log("Room.GetHashCode called");
        return cells[0].GetHashCode();
    }
}
