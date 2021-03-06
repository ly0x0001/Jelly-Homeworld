﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIRemainTowerCount : MonoBehaviour 
{
    static public int remainTowerCount { get; private set; }
    private static Text text;

    private void Awake()
    {
        remainTowerCount = TowerManager.MaxTowerCount;
        text = GetComponent<Text>();
    }

    public static void AddTowerCount()
    {
        remainTowerCount++;
        text.text = remainTowerCount.ToString();
    }

    public static void SubTowerCount()
    {
        remainTowerCount--;
        text.text = remainTowerCount.ToString();
    }

    public static void ResetTowerCount()
    {
        remainTowerCount = TowerManager.MaxTowerCount;
        text.text = remainTowerCount.ToString();
    }

    //给UI使用
    public void Reset()
    {
        remainTowerCount = TowerManager.MaxTowerCount;
        text.text = remainTowerCount.ToString();
    }
}
