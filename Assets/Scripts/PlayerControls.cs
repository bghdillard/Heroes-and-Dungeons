using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    [SerializeField]
    private GameObject buildUI;

    private List<string> toBuild;

    private GameObject preview;

    // Start is called before the first frame update
    void Start()
    {
        toBuild = new List<string> { "Empty", "Cell" };
    }

    // Update is called once per frame
    void Update()
    {
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
                        Order order = new Order("buildCell", hit.collider.GetComponent<Cell>(), toBuild[0]);
                        if (GridManager.HighQueueContains(order) || GridManager.ListContains(order)) GridManager.CancelOrder(order);
                        else GridManager.AddtoHighQueue(order);
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
                        Order order = new Order("buildItem", hit.collider.GetComponent<Cell>(), toBuild[0], 0);
                        if (GridManager.HighQueueContains(order) || GridManager.ListContains(order)) GridManager.CancelOrder(order);
                        else GridManager.AddtoHighQueue(order);
                    }
                }
            }
            else Debug.Log(hit.collider.gameObject.layer);
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
}
