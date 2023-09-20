using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newStoryBase", menuName = "StoryItems/Stat", order = 1)]
[System.Serializable]
public class Stat : ScriptableObject
{
    public string statName;
    public bool isDiametric;
    public Stat diametricStat;
}
