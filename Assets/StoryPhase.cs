using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryPhase : ScriptableObject
{
    public string phaseName = "New Phase";
    public Sprite phaseImage;
    [TextArea(15, 20)]
    public string phaseText;
    public bool showResultNextBeat = false;
    public List<Prerequisites> storyPhasePrerequisites;
    [HideInInspector] public List<StoryPhase> choices;
    public List<WorkingStat> statsToChange;
    [HideInInspector] public bool IsRoot = false;

    [HideInInspector] public string guid;

    [HideInInspector] public Vector2 postion;
}
