using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemOrder : IOrder
{
    private readonly string orderType = "itemBuild";
    Cell[] cells;
    Vector3 location;
    Quaternion rotation;
    string toBuild;
    bool isStarted;
    GameObject placeHolder;
    float time;

    public ItemOrder(Cell[] cells, string toBuild, GameObject preview)
    {
        this.cells = cells;
        this.toBuild = toBuild;
        location = preview.transform.position;
        rotation = preview.transform.rotation;
        isStarted = false;
        placeHolder = GameObject.Instantiate(Resources.Load<GameObject>("Preview/" + toBuild), location, rotation);
        placeHolder.GetComponent<PreviewItem>().SetColor(1);
        time = Time.time;
        foreach (Cell cell in cells) cell.SetOrder(this);
    }

    public Vector3 GetLocation()
    {
        return location;
    }

    public Quaternion GetRotation()
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
        placeHolder.GetComponent<PreviewItem>().SetColor(2);
    }

    public void CancelOrder()
    {
        //Destroy the placeholder object and remove the order from the cell;
        GameObject.Destroy(placeHolder);
        foreach(Cell cell in cells) cell.CancelOrder();
    }

    public void ClearPlaceholder()
    {
        GameObject.Destroy(placeHolder);
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

    public Cell[] GetCells()
    {
        return cells;
    }

    public override bool Equals(object obj)
    {
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
