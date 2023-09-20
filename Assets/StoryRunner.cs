using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StoryRunner : MonoBehaviour
{
    [Header("Story Area")]
    public List<WorkingStory> MainStoryLines;
    public List<WorkingStory> storyLines;
    public List<WorkingStat> currentStats;

    [Header("Gameobjects")]
    public TMP_Text mainText;
    public Image imageArea;
    public GameObject nextDay;
    public GameObject choiceArea;
    public GameObject choicePrefab;
    public WorkingStory fallBackStory;
    public TMP_Text reportText;

    WorkingStory currentViewedStoryPhase;

    [Header("Stat Area")]
    public Stat kingPower;
    public Stat militaryPower;
    public Stat clergyPower;
    public Stat nobilityPower;
    public Stat militaryLoyalty;
    public Stat clergyLoyalty;
    public Stat nobilityLoyalty;
    public Sprite militaryImage;
    public Sprite clergyImage;
    public Sprite nobilityImage;
    public Stat materials;
    public Stat gold;
    public Stat manpower;
    public Stat days;
    public List<Image> peopleImages;

    private const int CHECKLIMIT = 10;
    bool giveResults = false;

    int BASESTATINCREASE = 10;

    private void Start()
    {
        UpdateUI();
        NewTurn();
    }

    void UpdateUI()
    {
        int index = 0;

        for (int i = 0; i < Mathf.RoundToInt(currentStats.Find((x) => x.stat == militaryPower).value/10); i++)
        {
            peopleImages[index].gameObject.SetActive(true);
            peopleImages[index].sprite = militaryImage;
            index++;
        }

        for (int i = 0; i < Mathf.RoundToInt(currentStats.Find((x) => x.stat == clergyPower).value/10); i++)
        {
            peopleImages[index].gameObject.SetActive(true);
            peopleImages[index].sprite = clergyImage;
            index++;
        }

        for (int i = 0; i < Mathf.RoundToInt(currentStats.Find((x) => x.stat == nobilityPower).value/10); i++)
        {
            peopleImages[index].gameObject.SetActive(true);
            peopleImages[index].sprite = nobilityImage;
            index++;
        }

        for (int i = index; i < peopleImages.Count; i++)
        {
            peopleImages[index].sprite = null;
            peopleImages[index].gameObject.SetActive(false);
            index++;
        }
        reportText.text = 
            "Materials\n" + currentStats.Find((x) => x.stat == materials).value +
            "\nGold\n" + currentStats.Find((x) => x.stat == gold).value + 
            "\nManpower\n" + currentStats.Find((x) => x.stat == manpower).value + 
            "\nClergy\nThe clergy are currently " + LoyaltyString(currentStats.Find((x) => x.stat == clergyLoyalty).value) +
            "\nMilitary\nThe Military are currently " + LoyaltyString(currentStats.Find((x) => x.stat == militaryLoyalty).value) +
            "\nNobility\nThe Nobility are currently " + LoyaltyString(currentStats.Find((x) => x.stat == nobilityLoyalty).value);
    }

    string LoyaltyString(int value)
    {
        if (value <= 10)
            return "Extremely disloyal";
        if (value <= 25)
            return "Very disloyal";
        if (value <= 40)
            return "Disloyal";
        if (value <= 60)
            return "Loyal enough";
        if (value <= 80)
            return "Very loyal";
        return "Extremely loyal";
    }

    public void NewTurn()
    {
        if (!giveResults)
        {
            CheckStoryPhases();
            if (currentViewedStoryPhase == null || currentViewedStoryPhase.story == null)
            {
                NewRandomStory();
            }  
        }
        LoadStoryPhase();
    }

    void UpdateStats()
    {
        currentStats.Find((x) => x.stat == manpower).value += Mathf.RoundToInt(BASESTATINCREASE * (currentStats.Find((x) => x.stat == clergyPower).value / 10 * (currentStats.Find((x) => x.stat == clergyLoyalty).value / 50)));
        currentStats.Find((x) => x.stat == materials).value += Mathf.RoundToInt(BASESTATINCREASE * (currentStats.Find((x) => x.stat == militaryPower).value / 10 * (currentStats.Find((x) => x.stat == militaryLoyalty).value / 50)));
        currentStats.Find((x) => x.stat == gold).value += Mathf.RoundToInt(BASESTATINCREASE * (currentStats.Find((x) => x.stat == nobilityPower).value / 10 * (currentStats.Find((x) => x.stat == nobilityLoyalty).value / 50)));
        currentStats.Find((x) => x.stat == days).value++;
    }

    void CheckStoryPhases()
    {
        int limit = 0;
        while ((currentViewedStoryPhase == null || currentViewedStoryPhase.story == null) && limit < MainStoryLines.Count)
        {
            currentViewedStoryPhase = MainStoryLines[limit];
            if (currentViewedStoryPhase.currentPhase == null)
            {
                currentViewedStoryPhase.currentPhase = currentViewedStoryPhase.story.StartingPhase;
            }
            bool proced = true;
            foreach (Prerequisites stat in currentViewedStoryPhase.currentPhase.storyPhasePrerequisites)
            {
                foreach (WorkingStat item in currentStats)
                {
                    if (!stat.isChoicePrereq && stat.statToCheck.statName == item.stat.statName && (item.value > stat.upperBound || item.value < stat.lowerBound))
                    {
                        proced = false;
                    }
                }
            }
            if (currentViewedStoryPhase.isComplete)
            {
                proced = false;
            }
            if (!proced)
            {
                currentViewedStoryPhase = null;
            }
            limit++;
        }
    }

    void NewRandomStory()
    {
        if (currentViewedStoryPhase == null || currentViewedStoryPhase.story == null)
        {
            int limit = 0;
            while ((currentViewedStoryPhase == null || currentViewedStoryPhase.story == null) && limit < CHECKLIMIT)
            {
                currentViewedStoryPhase = storyLines[Random.Range(0, storyLines.Count)];

                if (currentViewedStoryPhase.currentPhase == null)
                {
                    currentViewedStoryPhase.currentPhase = currentViewedStoryPhase.story.StartingPhase;
                }
                bool proced = true;
                foreach (Prerequisites stat in currentViewedStoryPhase.currentPhase.storyPhasePrerequisites)
                {
                    foreach (WorkingStat item in currentStats)
                    {
                        if (!stat.isChoicePrereq && stat.statToCheck.statName == item.stat.statName && (item.value > stat.upperBound || item.value < stat.lowerBound))
                        {
                            proced = false;
                        }
                    }
                }
                if (currentViewedStoryPhase.isComplete)
                {
                    proced = false;
                }
                if (!proced)
                {
                    currentViewedStoryPhase = null;
                }
                limit++;
            }
            if (currentViewedStoryPhase == null || currentViewedStoryPhase.story == null)
            {
                currentViewedStoryPhase = fallBackStory;
                if (currentViewedStoryPhase.currentPhase == null)
                {
                    currentViewedStoryPhase.currentPhase = currentViewedStoryPhase.story.StartingPhase;
                }
            }
        }
    }

    void LoadStoryPhase()
    {
        giveResults = false;
        imageArea.gameObject.SetActive(true);
        StoryPhase currentPhase = currentViewedStoryPhase.currentPhase;
        mainText.text = currentPhase.phaseName + "\n" + currentPhase.phaseText;
        imageArea.sprite = currentPhase.phaseImage;

        if (currentPhase.choices.Count > 0)
        {
            int index = 0;
            foreach (StoryPhase phase in currentPhase.choices)
            {
                bool proced = true;
                foreach (Prerequisites stat in phase.storyPhasePrerequisites)
                {
                    foreach (WorkingStat item in currentStats)
                    {
                        if (stat.isChoicePrereq && stat.statToCheck.statName == item.stat.statName && (item.value > stat.upperBound || item.value < stat.lowerBound))
                        {
                            proced = false;
                        }
                    }
                }
                RectTransform choice = Instantiate(choicePrefab, choiceArea.transform).GetComponent<RectTransform>();
                choice.GetComponentInChildren<TMP_Text>().text = phase.phaseName;
                choice.localPosition = getChoiceLocation(index, currentPhase.choices.Count);
                choice.gameObject.GetComponent<Button>().onClick.AddListener(delegate { makeChoice(phase); });
                if (!proced)
                {
                    choice.gameObject.GetComponent<Button>().interactable = false;
                }
                index++;
            }
        }
        else
        {
            RectTransform choice = Instantiate(choicePrefab, choiceArea.transform).GetComponent<RectTransform>();
            choice.GetComponentInChildren<TMP_Text>().text = "Ok";
            choice.localPosition = getChoiceLocation(0, 1);
            choice.gameObject.GetComponent<Button>().onClick.AddListener(delegate { makeChoice(null); });
        }
    }


    public Vector2 getChoiceLocation(int index, int count)
    {
        switch (count)
        {
            case 1:
                return new Vector2(0,100);
            case 2:
                return new Vector2(100 - (index - 1) * 200, 100);
            case 3:
                return new Vector2(100 - Mathf.Abs(index % 2 - 1) * 200, 100 - 50 * Mathf.Clamp(index - 1, 0, 2));
            case 4:
                return new Vector2(100 - Mathf.Abs(index % 2 - 1) * 200, 100 - 50 * Mathf.Clamp(index - 1, 0, 1));
            default:
                break;
        }
        return new Vector2(0, 0);
    }

    public void makeChoice(StoryPhase phase)
    {
        if (phase != null)
        {
            if (phase.phaseName == "")
            {
                currentViewedStoryPhase = null;
            }
            else
            {
                if (currentViewedStoryPhase.currentPhase.showResultNextBeat)
                {
                    currentViewedStoryPhase.currentPhase = phase;
                    giveResults = true;
                }
                else
                {
                    currentViewedStoryPhase.currentPhase = phase;
                    currentViewedStoryPhase = null;
                }
                foreach (WorkingStat stat in phase.statsToChange)
                {
                    foreach (WorkingStat item in currentStats)
                    {
                        if (stat.stat.statName == item.stat.statName)
                        {
                            item.value += stat.value;
                        }
                    }
                }
            }
        }else
        {
            currentViewedStoryPhase.isComplete = true;
            currentViewedStoryPhase = null;
        }
        foreach(Transform chil in choiceArea.transform)
        {
            Destroy(chil.gameObject);
        }
        if (giveResults)
        {
            NewTurn();
        }else
        {
            imageArea.gameObject.SetActive(false);
            UpdateStats();
            UpdateUI();
        } 
    }
}


[System.Serializable]
public class WorkingStory
{
    public StoryBase story;
    public StoryPhase currentPhase;
    public bool isComplete = false;
}

[System.Serializable]
public class WorkingStat
{
    public Stat stat;
    public int value;
}