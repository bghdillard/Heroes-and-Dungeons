using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementItem : Item
{
    [SerializeField]
    private BuilderController builderController;
    [SerializeField]
    protected List<Renderer> renderers;
    protected bool placeable = false;
    private int numTriggers = 0;
    private List<Cell> cells = new List<Cell>();

    protected void OnTriggerEnter(Collider other)
    {
        if(other.tag != "Roof" && other.tag != "PlacementItem"){
            numTriggers++;
            Debug.Log("Entering trigger, trigger count is now: " + numTriggers);
            Cell cell = other.GetComponentInParent<Cell>();
            if (other.tag != "Wall" && cell != null && (cell.TraitsContains(itemType) || itemType =="all"))
            {
                cells.Add(cell);
                Debug.Log("Entering cell, cell count is now: " + cells.Count);
                Debug.Log("Cell entered is cell at: " + cell.GetLocation().ToString());
            }
            placeable = numTriggers == cells.Count;
            if (placeable)
            {
                Debug.Log("Changing color to green on enter");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.green);
                    }
                }
            }
            else
            {
                Debug.Log("Changing color to red on enter");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.red);
                    }
                }
            }
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.tag != "Roof" && other.tag != "PlacementItem")
        {
            numTriggers--;
            Debug.Log("Leaving Trigger, trigger count is now: " + numTriggers);
            Cell cell = other.GetComponentInParent<Cell>();
            if (other.tag != "Wall" && cell != null && (cell.TraitsContains(itemType) || itemType == "all"))
            {
                cells.Remove(cell);
                Debug.Log("Leaving Cell, cell count is now: " + cells.Count);
            }
            placeable = numTriggers == cells.Count;
            if (placeable)
            {
                Debug.Log("Changing color to green on exit");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.green);
                    }
                }
            }
            else
            {
                Debug.Log("Changing color to red on exit");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.red);
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        numTriggers = 0;
        cells.Clear();
    }

    public virtual bool AttemptPlace()
    {
        if (placeable)
        {
            builderController.AddToHighQueue(new ItemOrder(cells.ToArray(), name, gameObject));
            return true;
        }
        return false;
    }
}
