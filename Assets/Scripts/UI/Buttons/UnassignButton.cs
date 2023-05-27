using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnassignButton : MonoBehaviour
{
    private Monster monster;
    private Room room;
    private Patrol patrol;
    private Button button;
    private AssignmentPanel panel;

    public void Setup(Monster toSetup, AssignmentPanel panel)
    {
        monster = toSetup;
        button = GetComponent<Button>();
        button.onClick.AddListener(Unassign);
        this.panel = panel;
    }

    public void Connect(Room toConnect)
    {
        room = toConnect;
        patrol = null;
        if (monster.GetGuardRoom(out Room toCheck)) if (toCheck != room) button.interactable = false;
            else button.interactable = true;
        else button.interactable = true;
    }

    public void Connect(Patrol toConnect)
    {
        patrol = toConnect;
        room = null;
        if (monster.GetPatrol(out Patrol toCheck)) if (toCheck != patrol) button.interactable = false;
            else button.interactable = true;
        else button.interactable = true;
    }

    public void Reset()
    {
        button.interactable = true;
    }

    private void Unassign()
    {
        monster.UnassignDefense();
        button.interactable = false;
        transform.parent.GetComponent<Image>().color = Color.white;
        panel.AssignReset();
    }
}
