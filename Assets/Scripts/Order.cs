using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{

    private string name;
    private Cell location;
    private string toDo;
    private Builder builder;

    public Order(string name, Cell location)
    {
        this.name = name;
        this.location = location;
    }
    public Order (string name, Cell location, string toDo)
    {
        this.name = name;
        this.location = location;
        this.toDo = toDo;
    }
    public string GetName()
    {
        return name;
    }

    public Cell GetLocation()
    {
        return location;
    }

    public string GetBuild()
    {
        return toDo;
    }

    public void Cancel()
    {
        builder.CancelOrder();
    }

    public void SetBuilder(Builder toSet)
    {
        builder = toSet;
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare types.
        if ((obj == null) || !GetType().Equals(obj.GetType())) return false;
        else
        {
            Order toCheck = (Order) obj;
            return location == toCheck.GetLocation();
        }
    }

    public override int GetHashCode()
    {
        return location.GetHashCode();
    }
}
