using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PatrolEditButton : MonoBehaviour
{
    private TextMeshProUGUI text;
    [SerializeField]
    private PlayerControls pc;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        active = false;
        GetComponent<Button>().onClick.AddListener(PatrolToggle);
    }

    private void OnDisable()
    {
        text.text = "Create New Patrol";
        active = false;
    }

    private void PatrolToggle()
    {
        pc.TogglePatrolMode();
        active = !active;
        if (active)
        {
            text.text = "End Editing";
        }
        else text.text = "Create New Patrol";
    }
}
