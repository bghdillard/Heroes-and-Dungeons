using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerControls : MonoBehaviour
{

    [SerializeField]
    private GameObject buildUI;
    [SerializeField]
    private GameObject recruitUI;
    [SerializeField]
    private BuilderController builderController;
    [SerializeField]
    new private Camera camera;
    [SerializeField]
    private GameObject modeIndicator;
    [SerializeField]
    private GameObject patrolPointButton;
    [SerializeField]
    private AssignedPointPanel assignedPointPanel;

    [SerializeField]
    private string faction;
    private List<string> toBuild;
    private string toRecruit;

    private bool buildMode;
    private bool recruitMode;
    private bool defenseMode;
    private bool patrolMode;

    private static int activeLayer;
    //private static Cell[,,] grid;
    private HashSet<Cell> selectedCells;
    private Dictionary<Cell, GameObject> previewItems;
    private List<Monster> selectedMonsters;

    [SerializeField]
    private RectTransform selectionBox;
    private float mouseDelay = 0.1f;
    private float mouseTime;

    private Vector3 mouseStartPosition;
    private bool dragSelectStarted;
    private bool orderCancelMode;
    private int itemRotation;

    private Room activeRoom;
    private Patrol activePatrol;
    private PatrolPoint activePoint;

    // Start is called before the first frame update
    void Start()
    {
        toBuild = new List<string> { "Empty", "Cell" };

        buildMode = false;
        recruitMode = false;
        defenseMode = false;
        patrolMode = false;

        selectedCells = new HashSet<Cell>();
        previewItems = new Dictionary<Cell, GameObject>();
        selectedMonsters = new List<Monster>();

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
        if (recruitMode) HandleRecruitInput();
        if (patrolMode) HandlePatrolInput();
        if (!buildMode && !recruitMode && !patrolMode) HandleSelectInput();

    }

    private void HandleBuildInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            buildUI.SetActive(!buildUI.activeSelf);
            if (dragSelectStarted)
            {
                dragSelectStarted = false;
                foreach (Cell cell in selectedCells)
                {
                    if(!orderCancelMode) cell.ResetColor();
                    else
                    {
                        if (cell.GetOrder().GetStarted()) cell.SetColor(2);
                        else cell.SetColor(1);
                    }
                }
                selectedCells.Clear();
                orderCancelMode = false;
            }
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject() && !buildUI.activeSelf)
        {
            //selectionBox.sizeDelta = Vector2.zero;
            //selectionBox.gameObject.SetActive(true);
            //mouseStartPosition = Input.mousePosition;
            RaycastHit hit;
            LayerMask mask = LayerMask.GetMask("Active Layer");
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 300, mask) && hit.collider.GetComponentInParent<Cell>() != null)
            {
                mouseStartPosition = hit.point;
                Debug.Log(hit.point);
                Debug.Log(mouseStartPosition);
                dragSelectStarted = true;
                if (hit.collider.GetComponentInParent<Cell>().GetOrder() != null) orderCancelMode = true;
            }
        }
        else if (Input.GetMouseButton(0) && dragSelectStarted && !buildUI.activeSelf) ResizeCellBox();
        else if (Input.GetMouseButtonUp(0) && dragSelectStarted && !buildUI.activeSelf)
        {
            //selectionBox.sizeDelta = Vector2.zero;
            //selectionBox.gameObject.SetActive(false);
            //Generate all the orders;
            HashSet<IOrder> orders = new HashSet<IOrder>();
            foreach (Cell cell in selectedCells)
            {
                if (toBuild[1] == "Cell")
                {
                    if (!orderCancelMode)
                    {
                        List<Cell> adjacent = GridManager.GetAdjacent(cell);
                        if (adjacent[0] == null || adjacent[0].TraitsContains("Traversable"))
                        {
                            Debug.Log("Adding to high priority queue from PlayerControls");
                            builderController.AddToHighQueue(new CellOrder(cell, toBuild[0]));
                            continue;
                        }
                        if (adjacent[1] != null)
                        {
                            if (adjacent[1].TraitsContains("Traversable"))
                            {
                                Debug.Log("Adding to high priority queue from PlayerControls");
                                builderController.AddToHighQueue(new CellOrder(cell, toBuild[0]));
                                continue;
                            }
                        }
                        if (adjacent[2] != null)
                        {
                            if (adjacent[2].TraitsContains("Traversable"))
                            {
                                Debug.Log("Adding to high priority queue from PlayerControls");
                                builderController.AddToHighQueue(new CellOrder(cell, toBuild[0]));
                                continue;
                            }
                        }
                        if (adjacent[3] != null)
                        {
                            if (adjacent[3].TraitsContains("Traversable"))
                            {
                                Debug.Log("Adding to high priority queue from PlayerControls");
                                builderController.AddToHighQueue(new CellOrder(cell, toBuild[0]));
                                continue;
                            }
                        }
                        builderController.AddToInterim(new CellOrder(cell, toBuild[0]));
                        //builderController.AddToHighQueue(new CellOrder(cell, toBuild[0]));
                        //cell.ResetColor();
                    }
                    else
                    {
                        orders.Add(cell.GetOrder());
                        //builderController.CancelOrder(cell.GetOrder());
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
                        orders.Add(new ItemOrder(cell, toBuild[0], itemRotation));
                        //builderController.CancelOrder(new ItemOrder(cell, toBuild[0], itemRotation));
                    }
                }
            }
            if (orders.Count != 0) builderController.CancelOrder(orders);
            selectedCells.Clear();
            orderCancelMode = false;
            dragSelectStarted = false;
        }
    }

    private void HandleRecruitInput()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            if (toRecruit != null && Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)
                && (hit.collider.GetComponentInParent<Cell>() != null && hit.collider.GetComponentInParent<Cell>().TraitsContains("Traversable") || hit.collider.GetComponent<Ground>() != null))
            {
                GameObject recruit = Instantiate(Resources.Load<GameObject>("Monster/" + faction + "/" + toRecruit)); //, hit.point, new Quaternion());
                recruit.GetComponent<NavMeshAgent>().Warp(hit.point);
                Monster monster = recruit.GetComponent<Monster>();
                DungeonStats.AddMonster(monster);
                GameObject panel = Instantiate(Resources.Load<GameObject>("UI/MonsterPanel"));
                assignedPointPanel.AddPanel(panel);
                panel.GetComponent<AssignmentPanel>().Setup(monster);
            }
        }
    }

    private void HandlePatrolInput()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)
                && (hit.collider.GetComponentInParent<Cell>() != null && hit.collider.GetComponentInParent<Cell>().TraitsContains("Traversable") || hit.collider.GetComponent<Ground>() != null || hit.collider.GetComponent<PatrolPoint>()))
            {
                if(activePatrol != null)
                {
                    if (activePatrol.GetSize() < DungeonStats.GetStat("PatrolPointLimit"))
                    {
                        if (activePoint.ContainsPoint(hit.point, out PatrolPoint point)) //If we are too close to an existing point in the current patrol, don't actually add a new point, add a copy of that point to the list
                        {
                            PatrolPoint newPoint = Instantiate(Resources.Load<GameObject>("Special/PatrolPoint"), point.GetPoint(), new Quaternion()).GetComponent<PatrolPoint>().GetComponent<PatrolPoint>();
                            newPoint.ToggleVisible(true);
                            newPoint.Setup(activePatrol);
                            activePatrol.AddPoint(newPoint);
                        }
                        else
                        {
                            NavMeshPath path = new NavMeshPath();
                            NavMesh.CalculatePath(hit.point, activePatrol.GetEndPoint(), -1, path);
                            if (Helpers.GetPathLength(path) < DungeonStats.GetStat("PatrolPointDistance"))
                            {
                                PatrolPoint newPoint = Instantiate(Resources.Load<GameObject>("Special/PatrolPoint"), hit.point, new Quaternion()).GetComponent<PatrolPoint>().GetComponent<PatrolPoint>();
                                newPoint.ToggleVisible(true);
                                newPoint.Setup(activePatrol);
                                activePatrol.AddPoint(newPoint);
                            }
                        }
                    }
                }
                else //if we do not have a current patrol selected, create a new patrol with this point as the origin, and then select it;
                {
                    activePatrol = new Patrol(Instantiate(Resources.Load<GameObject>("Special/PatrolPoint"), hit.point, new Quaternion()).GetComponent<PatrolPoint>());
                    activePatrol.ToggleVisible(true);
                    activePatrol.SetPanel(assignedPointPanel);
                    activePatrol.Select();
                    activePoint = activePatrol.GetOrigin();
                    activePoint.Setup(activePatrol);
                    activePoint.SetPanel(assignedPointPanel);
                    activePoint.Select();
                }
            }
        }
    }

    public void UpdateToRecruitName(string toUpdate)
    {
        toRecruit = toUpdate;
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

    public static void SetInfo(int layer)
    {
        //grid = toSet;
        activeLayer = layer;
    }

    public void ToggleBuildMode()
    {
        recruitMode = false;
        recruitUI.SetActive(false);
        defenseMode = false;
        patrolMode = false;
        GridManager.ToggleDefenseVisibility(false);
        patrolPointButton.SetActive(false);
        assignedPointPanel.gameObject.SetActive(false);
        buildMode = !buildMode;
        buildUI.SetActive(buildMode);
        modeIndicator.SetActive(buildMode);
    }

    public void ToggleDefenseMode()
    {
        buildMode = false;
        buildUI.SetActive(false);
        recruitMode = false;
        recruitUI.SetActive(false);
        patrolMode = false;
        defenseMode = !defenseMode;
        modeIndicator.SetActive(defenseMode);
        patrolPointButton.SetActive(defenseMode);
        GridManager.ToggleDefenseVisibility(defenseMode);
        if (!defenseMode) assignedPointPanel.gameObject.SetActive(false);
    }

    public void ToggleRecruitMode()
    {
        toRecruit = null;
        buildMode = false;
        buildUI.SetActive(false);
        defenseMode = false;
        patrolMode = false;
        GridManager.ToggleDefenseVisibility(false);
        modeIndicator.SetActive(false);
        patrolPointButton.SetActive(false);
        assignedPointPanel.gameObject.SetActive(false);
        recruitMode = !recruitMode;
        recruitUI.SetActive(recruitMode);
    }

    public void TogglePatrolMode()
    {
        patrolMode = !patrolMode;
        if (activePoint != null)
        {
            activePoint.Deselect();
            activePoint = null;
            activePatrol.Deselect();
            activePatrol = null;
        }
    }

    private void HandleSelectInput()
    {
        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit)) {
                if (!defenseMode) //if we are selecting monsters
                {
                    if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit) && hit.collider.GetComponent<Monster>() != null)
                    {
                        Monster monster = hit.collider.GetComponent<Monster>();
                        monster.PseudoSelect();
                        foreach (Monster oldMonster in selectedMonsters) oldMonster.Deselect();
                        selectedMonsters = new List<Monster>()
                        {
                            monster
                        };
                    }
                    else
                    {
                        foreach (Monster oldMonster in selectedMonsters) oldMonster.Deselect();
                        selectedMonsters = new List<Monster>();
                    }
                    if (selectedMonsters.Count == 1) selectedMonsters[0].Select();
                }
                else  //if we are selecting guardrooms and patrolpoints
                {
                    Cell cell = GridManager.GetCellAt(Mathf.RoundToInt(hit.point.x), activeLayer, Mathf.RoundToInt(hit.point.z));
                    if (cell != null && cell.TraitsContains("Traversable")) 
                    {
                        Room room = cell.GetRoom();
                        if (room.IsGuardRoom())
                        {
                            room.SetPanel(assignedPointPanel);
                            if (activeRoom != null && room != activeRoom) activeRoom.Deselect();
                            if (activePoint != null)
                            {
                                activePoint.Deselect();
                                activePoint = null;
                                activePatrol.Deselect();
                                activePatrol = null;
                            }
                            room.Select();
                            activeRoom = room;
                            Debug.Log("You clicked on a guard room while in defense mode");
                        }
                        else if (activeRoom != null)
                        {
                            activeRoom.Deselect();
                            activeRoom = null;
                            Debug.Log("You clicked off of a guard room");
                        }
                        else Debug.Log("You did not click on a guard room");
                    }
                    else if (hit.collider.GetComponent<PatrolPoint>() != null)
                    {
                        PatrolPoint point = hit.collider.GetComponent<PatrolPoint>();
                        if (activePoint != null && point != activePoint) activePoint.Deselect();
                        if(activeRoom != null)
                        {
                            activeRoom.Deselect();
                            activeRoom = null;
                        }
                        point.SetPanel(assignedPointPanel);
                        point.Select();
                        activePoint = point;
                        if (activePatrol != null && activePatrol != point.GetPatrol()) activePatrol.Deselect();
                        activePatrol = point.GetPatrol();
                        Debug.Log("You clicked on a patrol point while in defense mode");
                    }
                    else
                    {
                        if(activeRoom != null)
                        {
                            activeRoom.Deselect();
                            activeRoom = null;
                        }
                        else if(activePoint != null)
                        {
                            activePoint.Deselect();
                            activePoint = null;
                            activePatrol.Deselect();
                            activePatrol = null;
                            Debug.Log("Hey, you clicked off a selected point");
                        }
                        assignedPointPanel.Deactivate();
                    }
                }
            }
        }
    }
    /*
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            selectionBox.sizeDelta = Vector2.zero;
            selectionBox.gameObject.SetActive(true);
            mouseStartPosition = Input.mousePosition;
            mouseTime = Time.time;
        }
        else if (Input.GetMouseButton(0) && mouseTime + mouseDelay < Time.time)
        {

            float width = Input.mousePosition.x - mouseStartPosition.x;
            float height = Input.mousePosition.y - mouseStartPosition.y;

            selectionBox.anchoredPosition = (Vector2)mouseStartPosition + new Vector2(width / 2, height / 2); //remember that this exists when you want to do something similar to select units. Hey past me, I remember!
            selectionBox.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
            Bounds bounds = new Bounds(selectionBox.anchoredPosition, selectionBox.sizeDelta);
            List<Monster> temp = DungeonStats.GetMonsters();
            List<Monster> currSelected = new List<Monster>();
            foreach(Monster monster in temp)
            {
                Debug.Log("Checking for Monster Position");
                if (MonsterIsSelected(camera.WorldToScreenPoint(monster.transform.position), bounds)) currSelected.Add(monster); //Add all monsters in the bounding box to a temporary list
            }
            foreach(Monster monster in selectedMonsters)
            {
                if (!currSelected.Contains(monster)) monster.Deselect(); //If a previously selected monster is no longer selected, remove its selection circle
            }
            selectedMonsters = currSelected;
            foreach(Monster monster in selectedMonsters)
            {
                monster.PseudoSelect(); //Now tell all selected monsters to activate their selection circles
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (mouseTime + mouseDelay < Time.time) //if we are relesing the button after dragging the box, simply make the box disapear
            {
                selectionBox.sizeDelta = Vector2.zero;
                selectionBox.gameObject.SetActive(false);
            }
            else if(!EventSystem.current.IsPointerOverGameObject())
                //if we are only clicking instead of dragging we either select only the clicked monster,
                //add it to the list of selected monsters if are holding shift, or remove all the selected monsters if we don't click on any
            {
                RaycastHit hit;
                if (Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit) && hit.collider.GetComponent<Monster>() != null) 
                {
                    Monster monster = hit.collider.GetComponent<Monster>();
                    monster.PseudoSelect();
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {
                        if(!selectedMonsters.Contains(monster)) selectedMonsters.Add(monster); //if we are holding shift when 
                        else
                        {
                            monster.Deselect();
                            selectedMonsters.Remove(monster);
                        }
                    }
                    else
                    {
                        foreach (Monster oldMonster in selectedMonsters) oldMonster.Deselect();
                        selectedMonsters = new List<Monster>()
                        {
                        monster
                        };
                    }
                }
                else
                {
                    foreach (Monster oldMonster in selectedMonsters) oldMonster.Deselect();
                    selectedMonsters = new List<Monster>();
                }
            }
            if (selectedMonsters.Count == 1) selectedMonsters[0].Select();
        }
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
    }
    */

    private void ResizeCellBox()
    {
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
            for (int i = Mathf.Min(x, Mathf.RoundToInt(mouseStartPosition.x)); i < Mathf.Max(x, Mathf.RoundToInt(mouseStartPosition.x)) + 1; i++)
            {
                for (int y = Mathf.Min(z, Mathf.RoundToInt(mouseStartPosition.z)); y < Mathf.Max(z, Mathf.RoundToInt(mouseStartPosition.z)) + 1; y++)
                    temp.Add(GridManager.GetCellAt(i, activeLayer, y)); //add every cell within the between the mouse start and the current mouse position to a temporary set
            }
            HashSet<Cell> toRemove = new HashSet<Cell>();
            foreach (Cell cell in selectedCells)
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
                        if (!orderCancelMode) cell.ResetColor();
                        else if (cell.GetOrder().GetStarted()) cell.SetColor(2);
                        else cell.SetColor(1);
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
            else if (toBuild[1] == "Item") //if building an item, check if the room type can hold the item type, then place a preview of the item in the room, while removing the previews from cells that have been deselcted
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
                    if (cell.TraitsContains(Resources.Load<Item>("Items/" + toBuild[0]).itemType))
                    { //confirm the cell can hold the type of item
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
}
    /*
    private bool MonsterIsSelected(Vector2 position, Bounds bounds)
    {
        //Debug.Log("Monster position: " + position  +"\n Bounds position: " + bounds);
        return position.x > bounds.min.x && position.x < bounds.max.x && position.y > bounds.min.y && position.y < bounds.max.y;
    }
    */
