using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovePointButton : MonoBehaviour
{
    private PatrolPoint point;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(MovePoint);
    }

    public void Connect(PatrolPoint toConnect)
    {
        point = toConnect;
    }

    private void MovePoint()
    {

    }
}