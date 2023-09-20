using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newStoryBase", menuName = "StoryItems/StoryBase", order = 1)]
public class StoryBase : ScriptableObject
{
    public string storysegmentName = "New Story";
    public StoryPhase StartingPhase;

    public List<StoryPhase> phases = new List<StoryPhase>();

    public StoryPhase CreatePhase(Vector2 position)
    {
        StoryPhase phase = ScriptableObject.CreateInstance("StoryPhase") as StoryPhase;
        if (StartingPhase == null)
        {
            StartingPhase = phase;
            phase.IsRoot = true;
        }
        phase.name = "NewPhase";
        phase.guid = GUID.Generate().ToString();
        phase.postion = position;
        phases.Add(phase);


        AssetDatabase.AddObjectToAsset(phase, this);
        AssetDatabase.SaveAssets();
        return phase;
    }

    public void DeletePhase(StoryPhase phase)
    {
        if (phase.IsRoot && phase.choices.Count > 0)
        {         
            phase.choices[0].IsRoot = true;
            StartingPhase = phase.choices[0];
        }
        phases.Remove(phase);
        AssetDatabase.RemoveObjectFromAsset(phase);
        AssetDatabase.SaveAssets();
    }

    public void AddChoice(StoryPhase parent, StoryPhase child)
    {
        if (parent != null && child != null)
        {
            parent.choices.Add(child);
        }
    }
    public void RemoveChoice(StoryPhase parent, StoryPhase child)
    {
        if (parent != null && parent.choices.Contains(child))
        {
            parent.choices.Remove(child);
        } 
    }

    public List<StoryPhase> GetChildren(StoryPhase parent)
    {
        return parent.choices;
    }
}


[System.Serializable]
public class Prerequisites
{
    public Stat statToCheck;
    public int upperBound;
    public int lowerBound;
    // if True is needed for it to show up as option else only for the story beat to fire
    public bool isChoicePrereq;
}


