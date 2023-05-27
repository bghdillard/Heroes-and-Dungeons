using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssignButton : MonoBehaviour
{
    private Monster monster;
    private Room room;
    private Patrol patrol;
    private Button button;
    private AssignmentPanel panel;

    public void Setup(Monster toSetup, AssignmentPanel panel)
    {
        monster = toSetup;
        this.panel = panel;
        button = GetComponent<Button>();
        button.onClick.AddListener(Assign);
    }

    public void Connect(Room toConnect)
    {
        Debug.Log("Assign connect room called with room of size " + toConnect.GetSize());
        room = toConnect;
        patrol = null;
        if (monster.GetGuardRoom(out Room toCheck)) if (toCheck == room) button.interactable = false;
            else button.interactable = true;
        else button.interactable = true;
    }

    public void Connect(Patrol toConnect)
    {
        Debug.Log("Assign connect patrol called");
        patrol = toConnect;
        room = null;
        if (monster.GetPatrol(out Patrol toCheck)) if (toCheck == patrol) button.interactable = false;
            else button.interactable = true;
        else button.interactable = true;
    }

    public void Reset()
    {
        button.interactable = true;
    }

    private void Assign()
    {
        if (room != null) monster.AssignDefense(room);
        else if (patrol != null) monster.AssignDefense(patrol);
        else Debug.LogError("Assign called with no valid assignment"); //Room is of size " + room.GetSize());
        button.interactable = false;
        transform.parent.GetComponent<Image>().color = Color.green;
        panel.UnassignReset();
    }
}
