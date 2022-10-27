using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellOrder : IOrder
{
    private readonly string orderType = "cellBuild";
    Cell location;
    string toBuild;

    public CellOrder(Cell location, string toBuild)
    {
        this.location = location;
        this.toBuild = toBuild;
    }

    public Vector3 GetLocation()
    {
        return location.transform.position;
    }

    public string GetOrderType()
    {
        return orderType;
    }

    public string GetToBuild()
    {
        return toBuild;
    }

    public Cell GetCell()
    {
        return location;
    }

    public bool IsAccessible()
    {
        Debug.Log("Checking Accessible");
        int x = (int)location.transform.position.x;
        int y = (int)location.transform.position.y;
        int z = (int)location.transform.position.z;
        for (int i = x - 1; i < x + 2; i++)
        {
            if (i < 0) return true;
            if (i > 99) continue;
            if (GridManager.GetGrid()[i, y, z].TraitsContains("Traversable")) return true;
        }
        for (int i = z - 1; i < z + 2; z++)
        {
            if (i < 0 || i > 99) continue;
            if (GridManager.GetGrid()[x, y, i].TraitsContains("Traversable")) return true;
        }
        return false;
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            Debug.Log("Type Inequality in Order equals");
            return false;
        }
        else
        {
            IOrder toCheck = (IOrder)obj;
            return location.transform.position == toCheck.GetLocation() && orderType == toCheck.GetOrderType();
        }
    }

    public override int GetHashCode()
    {
        //Debug.Log("Order.GetHashCode called");
        return location.GetHashCode() + orderType.GetHashCode(); //this may be problematic, but I don't really know enough about hashcodes to immediately know
    }
}
