using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionPoint : MonoBehaviour
{
    private bool inUse;
    private Monster user;
    private float startDelay;
    private float currDelay;
    private string stat;
    private int amount;

    public void Begin(float delay, string stat, int amount)
    {
        currDelay = startDelay = delay;
        this.stat = stat;
        this.amount = amount;
    }

    public void CheckUse()
    {
        if ((currDelay -= Time.deltaTime) <= 0)
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

    public Vector3 GetLocation()
    {
        return transform.position;
    }

    public Monster GetUser()
    {
        return user;
    }

    public void SetUser(Monster toSet)
    {
        user = toSet;
        inUse = true;
    }

    public void RemoveUser()
    {
        user = null;
        inUse = false;
    }

    public bool GetInUse()
    {
        return inUse;
    }
}
