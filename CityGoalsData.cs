using UnityEngine;
using System;

[CreateAssetMenu(fileName = "CityGoals", menuName = "City Builder/City Goals")]
public class CityGoalsData : ScriptableObject
{
    public CityGoal[] goals;
}

[Serializable]
public class CityGoal
{
    [Header("Requirements")]
    public int requiredPopulation;
    public int requiredSpecialBuildings;
    
    [Header("Reward")]
    public int rewardMoney;
    
    [Header("UI")]
    [TextArea(3,5)]
    public string description;
    
    [HideInInspector]
    public bool isCompleted;
}