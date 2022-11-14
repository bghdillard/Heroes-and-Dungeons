using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellOrder : IOrder
{
    private readonly string orderType = "cellBuild";
    Cell location;
    string toBuild;
    bool isStarted;

    public CellOrder(Cell location, string toBuild)
    {
        this.location = location;
        this.toBuild = toBuild;
        isStarted = false;
    }

    public Vector3 GetLocation()
    {
        return location.transform.position;
    }

    public string GetOrderType()
    {
        return orderType;
    }

    public void StartOrder()
    {
        //update the target Cell's color;
        isStarted = true;
    }

    public void CancelOrder()
    {
        //return the cell's color to it's original;
    }

    public string GetToBuild()
    {
        return toBuild;
    }

    public Cell GetCell()
    {
        return location;
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
