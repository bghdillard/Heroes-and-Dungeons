using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restorative : MonoBehaviour
{
    [SerializeField]
    private string stat;
    [SerializeField]
    private float startDelay;
    private float currDelay;
    [SerializeField]
    private int amount;
    [SerializeField]
    private bool inUse; //might need a vector3 as well to keep track of where the interaction point is, some items may have multiple restoratives attached for this reason
    [SerializeField]
    private Vector3 location;
    private Creature user;


    private void Awake()
    {
        currDelay = startDelay;
        GridManager.AddRestorative(this, stat);
    }
    private void Update()
    {
        if (inUse)
        {
            if(Vector3.Distance(user.transform.position, location) <= 0.5f)
            {
                if((currDelay -= Time.deltaTime) <= 0)
                {
                    currDelay = startDelay;
                    switch (stat)
                    {
                        case "Health":
                            user.HealMaxHealth(amount);
                            break;
                        case "Stamina":
                            user.HealMaxStamina(amount);
                            break;
                        case "Magic":
                            user.HealMaxMagic(amount);
                            break;
                        default:
                            Debug.LogError("Restorative attempting to restore stat, but no stat found. Stat listed as: " + stat);
                            break;
                    }
                }
            }
        }
    }

    public void SetUser(Creature toSet)
    {
        if (user != null) Debug.LogError("Restorative being selected despite having a user"); 
        GridManager.RemoveRestorative(this, stat); //I think there's a potential race condition here, so come back here if there are issues
        user = toSet;
        currDelay = startDelay;
        inUse = true;
    }

    public void RemoveUser()
    {
        user = null;
        inUse = false;
        GridManager.AddRestorative(this, stat);
    }
    
    public bool GetInUse()
    {
        return inUse;
    }

    public Vector3 GetLocation()
    {
        return location;
    }

    public string GetStat()
    {
        return stat;
    }
}
