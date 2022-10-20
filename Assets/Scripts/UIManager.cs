using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshPro goldText;
    public TextMeshPro oreText;
    public TextMeshPro prestigeText;

    public Slider goldSlider;
    public Slider oreSlider;

    public void updateGoldText(int toIncrease, string toUpdate)
    {
        goldSlider.minValue += toIncrease;
        goldText.SetText(toUpdate);
    }

    public void updateOreText(int toIncrease, string toUpdate)
    {
        oreSlider.minValue += toIncrease;
        oreText.SetText(toUpdate);
    }

    public void updatePrestigeText(string toUpdate)
    {
        prestigeText.SetText(toUpdate);
    }

    public void UpdateGoldSlider(int toUpdate)
    {
        goldSlider.maxValue += toUpdate;
    }

    public void UpdateOreSlider(int toUpdate)
    {
        oreSlider.maxValue += toUpdate;
    }

}
