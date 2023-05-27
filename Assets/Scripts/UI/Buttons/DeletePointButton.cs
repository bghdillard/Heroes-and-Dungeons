using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeletePointButton : MonoBehaviour
{
    private PatrolPoint point;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(DeletePoint);
    }

    public void Connect(PatrolPoint toConnect)
    {
        point = toConnect;
    }

    private void DeletePoint()
    {

    }
}