using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order
{

    private string name;
    private Cell location;
    private string toDo;

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
}
