using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePatrolButton : MonoBehaviour
{
    private Patrol patrol;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(DeletePatrol);
    }

    public void Connect(PatrolPoint toConnect)
    {
        patrol = toConnect.GetPatrol();
    }

    private void DeletePatrol()
    {

    }
}