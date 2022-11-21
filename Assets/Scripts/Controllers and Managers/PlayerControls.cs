using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    [SerializeField]
    private GameObject buildUI;
    [SerializeField]
    private BuilderController builderController;
    [SerializeField]
    new private Camera camera;

    private List<string> toBuild;

    private bool buildMode;
    private static int activeLayer;
    private static Cell[,,] grid;
    private HashSet<Cell> selectedCells;
    private Dictionary<Cell, GameObject> previewItems;

    [SerializeField]
    private RectTransform selectionBox;
    private float mouseDelay = 0.1f;
    private float mouseTime;

    private Vector3 mouseStartPosition;
    private bool dragSelectStarted;
    private bool orderCancelMode;
    private int itemRotation;

    // Start is called before the first frame update
    void Start()
    {
        toBuild = new List<string> { "Empty", "Cell" };
        buildMode = false;
        selectedCells = new HashSet<Cell>();
        previewItems = new Dictionary<Cell, GameObject>();
        dragSelectStarted = false;
        orderCancelMode = false;
        itemRotation = 0;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(1)) buildUI.SetActive(!buildUI.activeSelf);
        if (Input.GetMouseButtonDown(0) && !buildUI.activeSelf)
        {
            RaycastHit hit = new RaycastHit();
            LayerMask mask = LayerMask.GetMask("Active Layer");
            if (toBuild[1] == "Cell") //Check what kind of item is attempting to be built, and use the right name to keep track of it
            {
                if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 300, mask)
                    && hit.collider.GetComponent<Cell>() != null)
                {
                    if (hit.collider.GetComponent<Cell>().GetName() != toBuild[0])
                    {
                        CellOrder order = new CellOrder(hit.collider.GetComponent<Cell>(), toBuild[0]);
                        if (builderController.ContainsOrder(order)) builderController.CancelOrder(order);
                        else builderController.AddToHighQueue(order);
                    }
                    Debug.Log(hit.collider.GetComponent<Cell>().GetName());
                }
            }
            else if (toBuild[1] == "Item")
            {
                if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 300, mask)
                    && hit.collider.GetComponent<Cell>() != null)
                {
                    if (hit.collider.GetComponent<Cell>().TraitsContains(Resources.Load<Container>("Items/" + toBuild[0]).type))
                    {
                        ItemOrder order = new ItemOrder(hit.collider.GetComponent<Cell>(), toBuild[0], 0);
                        if (builderController.ContainsOrder(order)) builderController.CancelOrder(order);
                        else builderController.AddToHighQueue(order);
                    }
                }
            }
            else Debug.Log(hit.collider.gameObject.layer);
            
        }
        */
        if (buildMode) HandleBuildInput();

    }

    private void HandleBuildInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            buildUI.SetActive(!buildUI.activeSelf);
            if (dragSelectStarted)
            {
                dragSelectStarted = false;
                foreach (Cell cell in selectedCells) cell.ResetColor();
                selectedCells.Clear();
                orderCancelMode = false;
            }
        }
        else if (Input.GetMouseButtonDown(0) && !buildUI.activeSelf)
        {
            //selectionBox.sizeDelta = Vector2.zero;
            //selectionBox.gameObject.SetActive(true);
            //mouseStartPosition = Input.mousePosition;
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Active Layer");
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 300, mask) && hit.collider.GetComponent<Cell>() != null)
            {
                mouseStartPosition = hit.point;
                Debug.Log(hit.point);
                Debug.Log(mouseStartPosition);
                dragSelectStarted = true;
                if (hit.collider.GetComponent<Cell>().GetOrder() != null) orderCancelMode = true;
            }
        }
        else if (Input.GetMouseButton(0) && dragSelectStarted && !buildUI.activeSelf) ResizeSelectionBox();
        else if (Input.GetMouseButtonUp(0) && dragSelectStarted && !buildUI.activeSelf)
        {
            //selectionBox.sizeDelta = Vector2.zero;
            //selectionBox.gameObject.SetActive(false);
            //Generate all the orders;
            foreach (Cell cell in selectedCells)
            {
                if (toBuild[1] == "Cell")
                {
                    if (!orderCancelMode)
                    {
                        builderController.AddToHighQueue(new CellOrder(cell, toBuild[0]));
                        //cell.ResetColor();
                    }
                    else
                    {
                        builderController.CancelOrder(cell.GetOrder());
                        cell.ResetColor();
                    }
                }
                else if (toBuild[1] == "Item")
                {
                    if (!orderCancelMode)
                    {
                        builderController.AddToHighQueue(new ItemOrder(cell, toBuild[0], itemRotation));
                    }
                    else
                    {
                        builderController.CancelOrder(new ItemOrder(cell, toBuild[0], itemRotation));
                    }
                }
            }
            selectedCells.Clear();
            orderCancelMode = false;
        }

    }

    public void UpdateToBuildName(string toUpdate)
    {
        toBuild[0] = toUpdate;
        Debug.Log("NameChanged");
    }
    public void UpdateToBuildType(string toUpdate)
    {
        toBuild[1] = toUpdate;
        Debug.Log("TypeChanged");
    }

    public static void SetInfo(Cell[ , , ] toSet, int layer)
    {
        grid = toSet;
        activeLayer = layer;
    }

    public void ToggleBuildMode()
    {
        buildMode = !buildMode;
        if (buildMode) buildUI.SetActive(true);
    }

    private void ResizeSelectionBox()
    {
        /*
        float width = Input.mousePosition.x - mouseStartPosition.x;
        float height = Input.mousePosition.y - mouseStartPosition.y;

        selectionBox.anchoredPosition = mouseStartPosition + new Vector2(width / 2, height / 2); //remember that this exists when you want to do something similar to select units
        selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        Bounds bounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
        
        for(int i = 0; i < grid.GetLength(0); i++)
        {
            for(int x = 0; x < grid.GetLength(2); x++)
            {
                Cell temp = grid[i, activeLayer, x];
                //Debug.Log("Iterating through cells: currently at cell " + temp);
                if (CellIsSelected(camera.WorldToScreenPoint(temp.transform.position), bounds))
                {
                    //Debug.Log("Cell in box");
                    selectedCells.Add(temp);
                    if (toBuild[1] == "Cell") temp.SetColor(Color.yellow);
                    else
                    {
                        //might want to object pool here to keep it cheaper
                        GameObject item = Instantiate(Resources.Load<GameObject>("Placeholder/" + toBuild[0]));
                        previewItems.Add(temp, item);
                    }
                }
                else
                {
                    //Debug.Log("Cell not in box");
                    selectedCells.Remove(temp);
                    if (toBuild[1] == "Cell" && !temp.TraitsContains("Transparent")) temp.ResetColor();
                    else
                    {
                        //again, probably a spot to object pool
                        if (previewItems.ContainsKey(temp))
                        {
                            Destroy(previewItems[temp]);
                            previewItems.Remove(temp);
                        }
                    }
                }
            }
        }
        */

        RaycastHit hit;
        LayerMask mask = LayerMask.GetMask("Active Layer");
        if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 300, mask))
        {
            Vector3 location = hit.point; //get the current position of the mouse, and correct it to be within the bounds of the array if needed
            int x = Mathf.RoundToInt(location.x);
            if (x < 0) x = 0;
            else if (x > 99) x = 99;
            int z = Mathf.RoundToInt(location.z);
            if (z < 0) z = 0;
            else if (z > 99) z = 99;
            HashSet<Cell> temp = new HashSet<Cell>();
            for(int i = Mathf.Min(x, Mathf.RoundToInt(mouseStartPosition.x)); i < Mathf.Max(x, Mathf.RoundToInt(mouseStartPosition.x)) + 1; i++)
            {
                for (int y = Mathf.Min(z, Mathf.RoundToInt(mouseStartPosition.z)); y < Mathf.Max(z, Mathf.RoundToInt(mouseStartPosition.z)) + 1; y++) temp.Add(grid[i, activeLayer, y]); //add every cell within the between the mouse start and the current mouse position to a temporary set
            }
            HashSet<Cell> toRemove = new HashSet<Cell>();
            foreach(Cell cell in selectedCells)
            {
                if (!temp.Contains(cell)) toRemove.Add(cell); //create a set for cells that were previously selected but no longer are
            }
            if (toBuild[1] == "Cell") //if building a cell, highlight the cell to be built, and unhighlight the ones that have been deselected
            {
                if (toRemove.Count != 0)
                {
                    foreach (Cell cell in toRemove)
                    {
                        selectedCells.Remove(cell); //remove every no longer selected cell
                        cell.ResetColor();
                    }
                }
                foreach (Cell cell in temp)
                {
                    if (!orderCancelMode)
                    {
                        if (cell.GetOrder() == null) //make sure the cell doesn't already have an order
                        {
                            if (selectedCells.Add(cell)) cell.SetColor(1); //if a cell is newly added, change it's color
                        }
                    }
                    else
                    {
                        if (cell.GetOrder() != null) if (selectedCells.Add(cell)) cell.SetColor(0); //if we are instead canceling orders, check if this cell has a pending order before adding it to the set and changing its color
                    }
                }
            }
            else if(toBuild[1] == "Item") //if building an item, check if the room type can hold the item type, then place a preview of the item in the room, while removing the previews from cells that have been deselcted
            {
                if (toRemove.Count != 0)
                {
                    foreach (Cell cell in toRemove) //if the cell is being removed, find the item preview for that cell and destroy it;
                    {
                        selectedCells.Remove(cell);
                        Destroy(previewItems[cell]);
                        previewItems.Remove(cell);
                    }
                }
                foreach (Cell cell in temp)
                {
                    if(cell.TraitsContains(Resources.Load<Container>("Items/" + toBuild[0]).type)){ //confirm the cell can hold the type of item
                        if (!orderCancelMode)
                        {
                            if (selectedCells.Add(cell)) //if a cell is newly added, create a preview of the item and add it to the dictionary
                            {
                                GameObject preview = Instantiate(Resources.Load<GameObject>("Preview/" + toBuild[0]));
                                previewItems.Add(cell, preview);
                                preview.transform.parent = cell.transform;
                                if (itemRotation == 0) //set the location in the cell depending on the rotation
                                {
                                    preview.transform.Rotate(0, 0, 0, Space.Self);
                                    preview.transform.localPosition = new Vector3(0, -0.5f + (preview.transform.localScale.y / 2), -0.5f + (preview.transform.localScale.z / 2));
                                }
                                else if (itemRotation == 90)
                                {
                                    preview.transform.Rotate(0, 0, 0, Space.Self);
                                    preview.transform.localPosition = new Vector3(-0.5f + (preview.transform.localScale.x / 2), -0.5f + (preview.transform.localScale.y / 2), 0);
                                }
                                else if (itemRotation == 180)
                                {
                                    preview.transform.Rotate(0, 0, 0, Space.Self);
                                    preview.transform.localPosition = new Vector3(0, -0.5f + (preview.transform.localScale.y / 2), 0.5f - (preview.transform.localScale.z / 2));
                                }
                                else if (itemRotation == 270)
                                {
                                    preview.transform.Rotate(0, 0, 0, Space.Self);
                                    preview.transform.localPosition = new Vector3(0.5f - (preview.transform.localScale.x / 2), -0.5f + (preview.transform.localScale.y / 2), 0);
                                }
                            }
                        }
                        else
                        {
                            if (cell.GetOrder() != null) if (selectedCells.Add(cell)) Debug.Log("Item cancel logged, come back later for visual");
                        }
                    }
                }
            }
        }

    }

    /*
    private bool CellIsSelected(Vector2 position, Bounds bounds)
    {
        //Debug.Log("Cell position: " + position  +"\n Bounds position: " + bounds);
        return position.x > bounds.min.x && position.x < bounds.max.x && position.y > bounds.min.y && position.y < bounds.max.y;
    }
    */
}
