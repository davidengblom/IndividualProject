﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDScript : MonoBehaviour
{
    public Text nameText;
    public Text levelText;
    public Slider hpSlider;
    
    public void SetHUD(Unit unit) //Sets the unit information
    {
        nameText.text = unit.unitName;
        levelText.text = "Level " + unit.unitLevel;
        hpSlider.maxValue = unit.maxHP;
        hpSlider.value = unit.currentHP;
    }

    public void SetHP(int hp) //Update HP function
    {
        hpSlider.value = hp;
    }
}
