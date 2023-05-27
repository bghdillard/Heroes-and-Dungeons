using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPanel : MonoBehaviour
{
    [SerializeField]
    MovePointButton moveButton;
    [SerializeField]
    DeletePointButton deletePoint;
    [SerializeField]
    DeletePatrolButton deletePatrol;

    public void Connect(PatrolPoint toConnect)
    {
        gameObject.SetActive(true);
        moveButton.Connect(toConnect);
        deletePoint.Connect(toConnect);
        deletePatrol.Connect(toConnect);
    }

    public void Disconnect()
    {
        gameObject.SetActive(false);
    }
}
