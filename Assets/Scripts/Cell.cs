using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField]
    private List<string> traits;
    [SerializeField]
    private string name;


    public bool TraitsContains(string toCheck)
    {
        return traits.Contains(toCheck);
    }

    public string GetName()
    {
        return name;
    }

    public Vector3 GetLocation()
    {
        return gameObject.transform.position;
    }

}
