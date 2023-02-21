using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellOrder : IOrder
{
    private readonly string orderType = "cellBuild";
    Cell target;
    Vector3 location;
    string toBuild;
    bool isStarted;
    float time;

    public CellOrder(Cell target, string toBuild)
    {
        this.target = target;
        this.toBuild = toBuild;
        location = target.transform.position;
        isStarted = false;
        time = Time.time;
        target.SetOrder(this);
        Debug.Log("Order created at " + Time.time);
    }

    public Vector3 GetLocation()
    {
        return location;
    }

    public string GetOrderType()
    {
        return orderType;
    }

    public void StartOrder()
    {
        Debug.Log("Before starting this order, isStarted was" + isStarted);
        //update the target Cell's color;
        isStarted = true;
        target.SetColor(2);
    }

    public void CancelOrder()
    {
        //return the cell's color to it's original and remove the order from the cell;
        target.ResetColor();
        target.CancelOrder();
    }

    public bool GetStarted()
    {
        return isStarted;
    }

    public float GetTime()
    {
        return time;
    }

    public string GetToBuild()
    {
        return toBuild;
    }

    public Cell GetCell()
    {
        return target;
    }

    public override bool Equals(object obj)
    {
        //Debug.Log("CellOrder Equals Called");
        //Check for null and compare types.
        if ((obj == null) || !GetType().Equals(obj.GetType()))
        {
            //Debug.Log("Type Inequality in Order equals");
            return false;
        }
        else
        {
            IOrder toCheck = (IOrder)obj;
            return location == toCheck.GetLocation() && orderType == toCheck.GetOrderType();
        }
    }

    public override int GetHashCode()
    {
        //Debug.Log("Order.GetHashCode called");
        return location.GetHashCode() + orderType.GetHashCode(); //this may be problematic, but I don't really know enough about hashcodes to immediately know
    }
}
