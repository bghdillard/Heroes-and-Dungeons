using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitOrder : IOrder
{

    private readonly string orderType = "transit";
    private GameObject initialTarget;
    private GameObject finalTarget;
    private bool isStarted;
    private float time;

    public TransitOrder(GameObject toGrab, GameObject toTarget)
    {
        initialTarget = toGrab;
        finalTarget = toTarget;
        isStarted = false;
        time = Time.time;
    }

    public Vector3 GetLocation()
    {
        return initialTarget.transform.position;
    }

    public string GetOrderType()
    {
        return orderType;
    }

    public void StartOrder()
    {
        isStarted = true;
    }

    public void CancelOrder()
    {
        Debug.Log("TransitOrder cancel was somehow called, I don't think that should be happening probably");
    }

    public bool GetStarted()
    {
        return isStarted;
    }

    public float GetTime()
    {
        return time;
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
}
