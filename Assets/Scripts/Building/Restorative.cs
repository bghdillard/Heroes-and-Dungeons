using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restorative : Item
{
    [SerializeField]
    private string stat;
    [SerializeField]
    private float startDelay;
    private float currDelay;
    [SerializeField]
    private List<InteractionPoint> interactionPoints;
    [SerializeField]
    private int amount;


    private void Awake()
    {
        foreach (InteractionPoint point in interactionPoints) point.Begin(startDelay, stat, amount);
        GridManager.AddRestorative(this, stat);
    }

    public List<InteractionPoint> GetLocations()
    {
        return interactionPoints;
    }

    public void SetUser(InteractionPoint toUse, Monster toSet)
    {
        if (toUse.GetUser() != null) Debug.LogError("Restorative being selected despite having a user");
        toUse.SetUser(toSet);
        bool allInUse = true;
        foreach (InteractionPoint point in interactionPoints) if (!point.GetInUse())
            {
                allInUse = false;
                break;
            }
        if(allInUse) GridManager.RemoveRestorative(this, stat); //I think there's a potential race condition here, so come back here if there are issues
    }

    public void RemoveUser(InteractionPoint toRemove)
    {
        bool allInUse = true;
        foreach (InteractionPoint point in interactionPoints) if (!point.GetInUse())
            {
                allInUse = false;
                break;
            }
        toRemove.RemoveUser();
        if (allInUse) GridManager.AddRestorative(this, stat);
    }
}
