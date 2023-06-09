using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI oreText;
    public TextMeshProUGUI prestigeText;

    public Slider goldSlider;
    public Slider oreSlider;

    public TextMeshProUGUI nameText;
    public GameObject monsterPanel;
    public Slider currHealthBar;
    public Slider maxHealthBar;
    public Slider currStaminaBar;
    public Slider maxStaminaBar;
    public Slider currMagicBar;
    public Slider maxMagicBar;
    public Slider currLoyaltyBar;
    public Slider maxLoyaltyBar;

    public void UpdateGoldText(int toIncrease)
    {
        goldSlider.value += toIncrease;
        goldText.SetText(goldSlider.value.ToString());
    }

    public void UpdateOreText(int toIncrease)
    {
        oreSlider.value += toIncrease;
        oreText.SetText(oreSlider.value.ToString());
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
