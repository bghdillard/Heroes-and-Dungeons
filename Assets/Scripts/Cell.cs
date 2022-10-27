using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{

    [SerializeField]
    private List<string> traits;
    [SerializeField]
    private string cellName;
    [SerializeField]
    private string resourceType;
    private List<GameObject> items;

    private void Awake()
    {
        items = new List<GameObject>();
    }

    public bool TraitsContains(string toCheck)
    {
        return traits.Contains(toCheck);
    }

    public string GetName()
    {
        return cellName;
    }

    public Vector3 GetLocation()
    {
        return gameObject.transform.position;
    }

    public List<GameObject> GetItems()
    {
        return items;
    }

    public void AddItem(GameObject toAdd)
    {
        items.Add(toAdd);
    }

    public void RemoveItem(GameObject toRemove)
    {
        items.Remove(toRemove);
    }

    public string GetResourceType()
    {
        return resourceType;
    }
}
