using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{

    [SerializeField]
    private GameObject buildUI;

    private string toBuild;

    // Start is called before the first frame update
    void Start()
    {
        toBuild = "Empty";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && !buildUI.activeSelf) buildUI.SetActive(!buildUI.activeSelf);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            LayerMask mask = LayerMask.GetMask("Active Layer");
            if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 300, mask)
                && hit.collider.gameObject.layer == 6 && hit.collider.GetComponent<Cell>() != null)
            {
                if (hit.collider.GetComponent<Cell>().GetName() != toBuild)
                {
                    Order order = new Order("build", hit.collider.GetComponent<Cell>(), toBuild);
                    if (GridManager.QueueContains(order) || GridManager.ListContains(order)) GridManager.CancelOrder(order);
                    else GridManager.AddtoQueue(order);
                }
                Debug.Log(hit.collider.GetComponent<Cell>().GetName());
            }
            else Debug.Log(hit.collider.gameObject.layer);
        }
    }

    public void UpdateToBuild(string toUpdate)
    {
        toBuild = toUpdate;
    }
}
