using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerControls : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit = new RaycastHit();
            LayerMask mask = LayerMask.GetMask("Active Layer");
            if (Physics.Raycast(GameObject.Find("Main Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 300, mask) && hit.collider.gameObject.layer == 6)
            {
                if(hit.collider.GetComponent<Cell>().GetName() != "Empty") GridManager.AddtoQueue(new Order("build", hit.collider.GetComponent<Cell>(), "Empty"));
                Debug.Log(hit.collider.GetComponent<Cell>().GetName());
                //GameObject.Find("Builder(Clone)").GetComponent<NavMeshAgent>().SetDestination(hit.point);
            }
            else Debug.Log(hit.collider.gameObject.layer);
        }
    }
}
