using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookedPlacementItem : PlacementItem
{
    [SerializeField]
    private List<Hooks> hooks;
    private bool hookPlaceable = false;

    new private void OnTriggerEnter(Collider other)
    {
        placeable = true;
        base.OnTriggerEnter(other);
        if (placeable)
        {
            hookPlaceable = HookCheck();
            if (hookPlaceable)
            {
                Debug.Log("Final Change to Green");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.green);
                    }
                }
            }
            else
            {
                Debug.Log("Final Change to Red");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.red);
                    }
                }
            }
        }
    }

    new private void OnTriggerExit(Collider other)
    {
        placeable = true;
        base.OnTriggerExit(other);
        if (placeable)
        {
            hookPlaceable = HookCheck();
            if (hookPlaceable)
            {
                Debug.Log("Final Change to Green");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.green);
                    }
                }
            }
            else
            {
                Debug.Log("Final Change to Red");
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        material.EnableKeyword("_EMISSION");
                        material.SetColor("_EmissionColor", Color.red);
                    }
                }
            }
        }
    }

    private bool HookCheck()
    {
        Debug.Log("HookCheck");
        foreach (Hooks hook in hooks)
        {
            if (!hook.Evaluate()) return false;
        }
        Debug.Log("None returned false");
        return true;
    }

    public void PlacementCheck()
    {
        hookPlaceable = HookCheck();
        if (hookPlaceable)
        {
            Debug.Log("Final Change to Green");
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.green);
                }
            }
        }
        else
        {
            Debug.Log("Final Change to Red");
            foreach (Renderer renderer in renderers)
            {
                foreach (Material material in renderer.materials)
                {
                    material.EnableKeyword("_EMISSION");
                    material.SetColor("_EmissionColor", Color.red);
                }
            }
        }
    }

    public override bool AttemptPlace()
    {
        if (hookPlaceable) return base.AttemptPlace();
        else return false;
    }
}
