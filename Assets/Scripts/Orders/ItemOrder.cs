using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOrder : IOrder
{
    private readonly string orderType = "itemBuild";
    Cell location;
    string toBuild;
    int rotation;
    bool isStarted;
    GameObject placeHolder;
    float time;

    public ItemOrder(Cell location, string toBuild, int rotation)
    {
        this.location = location;
        this.toBuild = toBuild;
        this.rotation = rotation;
        isStarted = false;
        placeHolder = GameObject.Instantiate(Resources.Load<GameObject>("Preview/" + toBuild));
        time = Time.time;
        location.SetOrder(this);
    }

    public Vector3 GetLocation()
    {
        return location.transform.position;
    }

    public int GetRotation()
    {
        return rotation;
    }

    public string GetOrderType()
    {
        return orderType;
    }

    public void StartOrder()
    {
        //update the target item's color;
        isStarted = true;
    }

    public void CancelOrder()
    {
        //Destroy the placeholder object and remove the order from the cell;
        GameObject.Destroy(placeHolder);
        location.CancelOrder();
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
