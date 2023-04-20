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
        foreach (AssignmentPanel panel in panels) panel.Connect(toAssign);
    }

    public void AssignPoint(Patrol toAssign)
    {
        
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
