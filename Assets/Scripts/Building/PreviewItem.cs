using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewItem : MonoBehaviour
{
    [SerializeField]
    private List<Renderer> renderers;
    [SerializeField]
    private List<Color> colors;

    public void SetColor(int toSet)
    {
        foreach (var renderer in renderers)
        {
            foreach(var material in renderer.materials)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", colors[toSet]);
            }
        }
    }

    public void ResetColor()
    {
        foreach (var renderer in renderers)
        {
            foreach(var material in renderer.materials)
            {
                material.DisableKeyword("_EMission");
            }
        }
    }
    
}
