using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<Cell> cells;

    private readonly bool isGuardRoom;
    private bool isVisible = false;
    private bool isSelected;

    private int numAssigned;
    private int numActive;

    private string roomName;
    private AssignedPointPanel panel;

    public Room()
    {
        //parameterless constructor for the ground;
        cells = new List<Cell>();
        isGuardRoom = false;
        GridManager.AddRoom(this);
        isSelected = false;
        numAssigned = 0;
        numActive = 0;
    }

    public Room(Cell origin)
    {
        cells = new List<Cell> { origin };
        isGuardRoom = origin.TraitsContains("GuardRoom");
        GridManager.AddRoom(this);
        Debug.Log("New Room Created");
        isSelected = false;
        numAssigned = 0;
        numActive = 0;
        roomName = origin.GetName();
    }

    public void AddCell(Cell toMerge)
    {
        cells.Add(toMerge);
        if (isVisible)
        {
            toMerge.SetColor(1);
            if (isSelected && isGuardRoom) panel.UpdateMaxCount(1);
        }
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

    public bool IsGuardRoom()
    {
        return isGuardRoom;
    }

    public void ToggleVisible(bool toToggle)
    {
        if (toToggle)
        {
            foreach (Cell cell in cells) cell.SetColor(1);
        }
        else foreach (Cell cell in cells) cell.ResetColor();
        isVisible = toToggle;
    }

    public virtual Vector3 GetRandomPoint() //used to get a random point in the room for creatures to idle towards;
    {
        Cell selected = cells[Random.Range(0, cells.Count)]; //start by getting a random cell in the room
        Vector3 point = selected.transform.position; //then get a random point in that cell by getting its transform
        point.x += Random.Range(-0.99f, 0.99f); //then add or subtract a random value less than its full size to its x and z
        point.z += Random.Range(-0.99f, 0.99f);
        return point;
    }

    public void SetPanel(AssignedPointPanel toSet)
    {
        panel = toSet;
    }

    public void Select()
    {
        isSelected = true;
        panel.AssignPoint(this);
    }

    public void Deselect()
    {
        isSelected = false;
        panel.Deactivate();
    }

    public string GetName()
    {
        return roomName;
    }

    public int GetAssignedCount()
    {
        return numAssigned;
    }

    public override bool Equals(object other)
    {
        //Debug.Log("Room.equals called");
        if (other == null || !GetType().Equals(other.GetType())) return false;
        else
        {
            Room room = (Room)other;
            return room.GetCells().Contains(cells[0]);
        }
    }

    public override int GetHashCode()
    {
        //Debug.Log("Room.GetHashCode called");
        return cells[0].GetHashCode();
    }

    public override string ToString()
    {
        return cells[0].ToString();
    }
    
    public static bool operator == (Room o1, Room o2) //quite honestly, I don't know why I'm needing these, but it's not catching null otherwise, so...
    {
        if (ReferenceEquals(o1, o2)) return true;
        if ((object)o1 == null || (object)o2 == null) return false;
        return o1.GetCells()[0] == o2.GetCells()[0];
    }

    public static bool operator != (Room o1, Room o2)
    {
        return !(o1 == o2);
    }
}
