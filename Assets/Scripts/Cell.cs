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

    /*private void OnDisable() //fun note for future Brian, calling OnDestroy for instantiating causes amusing issues when you stop the scene, even more fun note, same goes for OnDisable. Time to start object pooling!
    {
        if (resourceType != "None")
        {
            Debug.Log("Load Resource: " + resourceType);
            Instantiate(Resources.Load<GameObject>("Minerals/" + resourceType)).transform.position = transform.position; //load the resource and spawn in where the center of this cell used to be;
        }
        else Debug.Log("No Resource here");
    }
    */

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
