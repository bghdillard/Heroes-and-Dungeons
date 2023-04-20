using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AssignmentPanel : MonoBehaviour
{
    [SerializeField]
    private AssignButton assign;
    [SerializeField]
    private UnassignButton unassign;
    [SerializeField]
    private TextMeshProUGUI monsterName;

    private Monster monster;

    public void Setup(Monster toSetup)
    {
        monster = toSetup;
        monsterName.text = toSetup.GetName();
        assign.Setup(toSetup);
        unassign.Setup(toSetup);
    }

    public void Connect(Room toConnect)
    {
        Debug.Log("Panel connect room called with room of size" + toConnect.GetSize());
        assign.Connect(toConnect);
        unassign.Connect(toConnect);
        if (monster.GetGuardRoom(out Room room)) if (room == toConnect) GetComponent<UnityEngine.UI.Image>().color = Color.green;
            else GetComponent<UnityEngine.UI.Image>().color = Color.red;
        else GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }

    public void Connect(Patrol toConnect)
    {
        Debug.Log("Panel connect panel called");
        assign.Connect(toConnect);
        unassign.Connect(toConnect);
        if (monster.GetPatrol(out Patrol patrol)) if (patrol == toConnect) GetComponent<UnityEngine.UI.Image>().color = Color.green;
            else GetComponent<UnityEngine.UI.Image>().color = Color.red;
        else GetComponent<UnityEngine.UI.Image>().color = Color.white;
    }
}
