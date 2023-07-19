using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hooks : MonoBehaviour
{
    protected bool placeable = false;
    private int numTriggers = 0;
    private List<Cell> cells = new List<Cell>();
    [SerializeField]
    private HookedPlacementItem parent;

    protected void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Roof" && other.tag != "Floor" && other.tag != "PlacementItem")
        {
            numTriggers++;
            Debug.Log("Entering trigger, trigger count is now: " + numTriggers);
            Cell cell = other.GetComponentInParent<Cell>();
            if (other.tag == "Wall" && cell != null)
            {
                cells.Add(cell);
                Debug.Log("Entering cell, cell count is now: " + cells.Count);
                Debug.Log("Cell entered is cell at: " + cell.ToString());
            }
            placeable = numTriggers == cells.Count;
            if (placeable) parent.PlacementCheck();
        }
    }

    protected void OnTriggerExit(Collider other)
    {
        if (other.tag != "Roof" && other.tag != "Floor" && other.tag != "PlacementItem")
        {
            numTriggers--;
            Debug.Log("Leaving Trigger, trigger count is now: " + numTriggers);
            Cell cell = other.GetComponentInParent<Cell>();
            if (other.tag == "Wall" && cell != null)
            {
                cells.Remove(cell);
                Debug.Log("Leaving Cell, cell count is now: " + cells.Count);
                Debug.Log("Cell left is cell at: " + cell.ToString());
            }
            placeable = numTriggers == cells.Count;
        }
        if (placeable) parent.PlacementCheck();
    }

    private void OnDisable()
    {
        numTriggers = 0;
        cells.Clear();
    }

    public bool Evaluate()
    {
        if (numTriggers == 0) return false;
        //Debug.Log("MultipleTriggers");
        return placeable;
    }
}
