using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order //starting to think I should've made this an abstract class
{

    private string name;
    private Cell location;
    private Resource toGrab;
    private Container toBring;
    private string toDo;
    private Builder builder;
    private int rotation;

    public Order (string name, Cell location, string toDo)
    {
        this.name = name;
        this.location = location;
        this.toDo = toDo;
    }

    public Order (string name, Container toBring, Resource toGrab)
    {
        this.name = name;
        this.toGrab = toGrab;
        this.toBring = toBring;
    }

    public Order(string name, Cell location, string toDo, int rotation)
    {
        this.name = name;
        this.location = location;
        this.toDo = toDo;
        this.rotation = rotation;
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

    public int GetRotation()
    {
        return rotation;
    }

    public void Cancel()
    {
        builder.CancelOrder();
    }

    public void SetBuilder(Builder toSet)
    {
        builder = toSet;
    }

    public Resource GetToGrab()
    {
        return toGrab;
    }

    public Container GetToBring()
    {
        return toBring;
    }

    public override bool Equals(object obj)
    {
        //Check for null and compare types.
        if ((obj == null) || !GetType().Equals(obj.GetType())) return false;
        else
        {
            Order toCheck = (Order) obj;
            return location == toCheck.GetLocation() && name == toCheck.GetName();
        }
    }

    public override int GetHashCode()
    {
        Debug.Log("Order.GetHashCode called"); //put a debug here just in case... if this is an issue, hopefully this'll key me in pretty quickly
        return location.GetHashCode() + name.GetHashCode(); //this may be problematic, but I don't really know enough about hashcodes to immediately know
    }
}
