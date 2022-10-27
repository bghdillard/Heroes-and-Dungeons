using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitOrder : IOrder
{

    private readonly string orderType = "transit";
    private GameObject initialTarget;
    private GameObject finalTarget;

    public TransitOrder(GameObject toGrab, GameObject toTarget)
    {
        initialTarget = toGrab;
        finalTarget = toTarget;
    }

    public Vector3 GetLocation()
    {
        return initialTarget.transform.position;
    }

    public string GetOrderType()
    {
        return orderType;
    }

    public bool IsAccessible() //for now, I believe that transit orders should always be accessible;
    {
        return true;
    }

    public GameObject GetToTransit()
    {
        return initialTarget;
    }

    public Vector3 GetFinalLocation()
    {
        return finalTarget.transform.position;
    }

    public GameObject GetToStore()
    {
        return finalTarget;
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
