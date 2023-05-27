using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssignedPointPanel : MonoBehaviour //This script relates to the two types of defenses that monsters can be assigned to, patrols and guardRooms, and how they connect to the UI;
{
    #region UIElements
    [SerializeField]
    private TextMeshProUGUI pointName;
    [SerializeField]
    private TextMeshProUGUI currCountText;
    [SerializeField]
    private TextMeshProUGUI maxCountText;
    [SerializeField]
    private GameObject scroll;
    [SerializeField]
    private PatrolPanel patrolPanel;
    #endregion

    private List<AssignmentPanel> panels = new List<AssignmentPanel>();

    private static int currCount;
    private static int maxCount;

    public void AssignPoint(Room toAssign)
    {
        Debug.Log("Room is of size " + toAssign.GetSize());
        gameObject.SetActive(true);
        pointName.text = toAssign.GetName();
        SetCurrCount(toAssign.GetAssignedCount());
        SetMaxCount(toAssign.GetSize());
        patrolPanel.gameObject.SetActive(false);
        foreach (AssignmentPanel panel in panels) panel.Connect(toAssign);
    }

    public void AssignPoint(PatrolPoint toAssign)
    {
        Debug.Log("We are assigning a patrol");
        Patrol patrol = toAssign.GetPatrol();
        gameObject.SetActive(true);
        pointName.text = patrol.GetName();
        SetCurrCount(patrol.GetAssignedCount());
        SetMaxCount(patrol.GetSize()); //in the future, the max amount of assigned monsters per patrol will be handled elsewhere
        patrolPanel.gameObject.SetActive(true);
        foreach (AssignmentPanel panel in panels) panel.Connect(patrol);
        patrolPanel.Connect(toAssign);
    }

    public void AddPanel(GameObject toAdd)
    {
        toAdd.transform.SetParent(scroll.transform);
        panels.Add(toAdd.GetComponent<AssignmentPanel>());
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    private void SetCurrCount(int toAssign)
    {
        currCount = toAssign;
        currCountText.text = toAssign.ToString();
    }

    private void SetMaxCount(int toAssign)
    {
        maxCount = toAssign;
        maxCountText.text = toAssign.ToString();
    }

    public void UpdateCurrCount(int toAssign)
    {
        currCount += toAssign;
        currCountText.text = currCount.ToString();
    }

    public void UpdateMaxCount(int toAssign)
    {
        maxCount += toAssign;
        maxCountText.text = maxCount.ToString();
    }
}
