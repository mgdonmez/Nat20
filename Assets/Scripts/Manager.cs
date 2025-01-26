using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements.Experimental;

public enum DemographicType
{
    Circle,
    Square,
    Triangle
}

[Serializable]
public class Demographic
{
    [SerializeField] private DemographicType type;
    [SerializeField] private float ratio;
    [SerializeField] private float viewerRatio;
    [SerializeField] private int wisdomSaveDCBonus;

    public DemographicType Type { get => type; set => type = value; }
    public float Ratio { get => ratio; set => ratio = value; }
    public float ViewerRatio { get => viewerRatio; set => viewerRatio = value; }
    public int WisdomSaveDCBonus { get => wisdomSaveDCBonus; set => wisdomSaveDCBonus = value; }

    public Demographic(DemographicType type = DemographicType.Circle,
                      float ratio = 0.33f,
                      float viewerRatio = 0.5f)
    {
        this.type = type;
        this.ratio = ratio;
        this.viewerRatio = viewerRatio;
        this.wisdomSaveDCBonus = (int)type - 1;
    }

    public int WisdomSaveDC()
    {
        return UnityEngine.Random.Range(1, 21) + this.wisdomSaveDCBonus;
    }
}

public class Manager : MonoBehaviour
{
    [SerializeField] private int cardTraySize;
    [SerializeField] private int rerollCost;
    [SerializeField] private float budget;
    [SerializeField] private List<bool> newsSectionsProfitability;
    [SerializeField] private List<Demographic> population;
    [SerializeField] private List<GameObject> cardTray;
    [SerializeField] private List<GameObject> newsSections;
    [SerializeField] private List<Transform> cardTraySlots;
    [SerializeField] private List<Transform> newsSectionsSlots;
    [SerializeField] private GameObject cardPrefab;

    public static Manager Instance;
    public static DemographicType[] DemographicTypeValues = (DemographicType[])Enum.GetValues(typeof(DemographicType));
    public static Genuinity[] GenuinityValues = (Genuinity[])Enum.GetValues(typeof(Genuinity));

    public List<Demographic> Population { get => population; }
    public List<GameObject> CardTray { get => cardTray; }
    public List<GameObject> NewsSections { get => newsSections; }
    public List<Transform> CardTraySlots { get => cardTraySlots; }
    public List<Transform> NewsSectionsSlots { get => newsSectionsSlots; }
    public float Budget { get => budget; set => budget = value; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else if (this != Instance)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }


    private void Start()
    {
        this.population = new List<Demographic>();
        foreach (DemographicType dt in DemographicTypeValues)
        {
            Population.Add(new Demographic(dt, 1.0f / DemographicTypeValues.Length));
        }

        RerollCardTray(true);

        this.newsSections = new List<GameObject>() { null, null, null };

        this.newsSectionsProfitability = new List<bool> { false, false, false };

        //FunctionTimer.Create(() => { RerollCardTray(); }, 10f);
        //FunctionTimer.Create(() => { BroadCastTheNews(); }, 20f);
    }

    public GameObject RandomNewsCard()
    {
        Genuinity genuinity = (Genuinity)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Genuinity)).Length);

        int[] affectedPopulationInt = Enumerable.Range(0, DemographicTypeValues.Length)
        .OrderBy(_ => UnityEngine.Random.value)
            .Take(UnityEngine.Random.Range(1, DemographicTypeValues.Length + 1))
            .ToArray();

        List<DemographicType> affectedPopulation = affectedPopulationInt
            .Select(x => (DemographicType)x)
            .ToList();
        affectedPopulation.Sort((a, b) => b.CompareTo(a));

        GameObject newsCardObject = Instantiate(cardPrefab);
        newsCardObject.name = "News Card";
        newsCardObject.GetComponent<CardBehaviour>().CardInfo = new NewsCard(genuinity, affectedPopulation);

        return newsCardObject;
    }

    public void AddRandomCardToTraySlot(int index)
    {
        GameObject newsCardObject = RandomNewsCard();
        newsCardObject.transform.position = this.cardTraySlots[index].transform.position;
        newsCardObject.GetComponent<Draggable>().isInSlot = true;
        Droppable cardTraySlot = this.cardTraySlots[index].GetComponent<Droppable>();
        cardTraySlot.HeldCard = newsCardObject;
        cardTraySlot.IsCardIn = true;
        cardTraySlot.HasCard = true;

        if (index >= this.cardTray.Count)
        {
            this.cardTray.Add(newsCardObject);
        }
        else
        {
            this.cardTray[index] = newsCardObject;
        }
    }

    public void RerollCardTray(bool firstTime = false)
    {
        if (firstTime || budget > rerollCost)
        {

            if (firstTime)
            {
                this.cardTray = new List<GameObject>();
            }
            else
            {
                budget -= rerollCost;
                foreach (GameObject card in this.cardTray)
                {
                    if(card != null)
                    {
                        Destroy(card);
                    }
                }
                this.cardTray.Clear();
            }

            for (int i = 0; i < cardTraySize; i++)
            {
                AddRandomCardToTraySlot(i);
            }
        }
    }

    public void BroadCastTheNews()
    {
        DetermineNewsEffect();
        UpdateBudget();
        UpdatePopulationRatios();
        RenewDay();
    }

    public void DetermineNewsEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            if (this.newsSections[i] != null)
            {
                NewsCard newsSectionCard = this.newsSections[i].GetComponent<CardBehaviour>().CardInfo;
                foreach (DemographicType dt in newsSectionCard.AffectedPopulation)
                {
                    Demographic affectedDemographic = this.population[(int)dt];
                    if (newsSectionCard.Genuinity == Genuinity.Truth)
                    {
                        this.newsSectionsProfitability[i] = true;
                        affectedDemographic.ViewerRatio *= 1.2f;
                    }
                    else
                    {
                        int deceptionCheck = newsSectionCard.DeceptionCheck();
                        bool profitability = true;
                        bool deceptionSuccess = deceptionCheck >= affectedDemographic.WisdomSaveDC();
                        profitability &= deceptionSuccess;
                        affectedDemographic.ViewerRatio *= deceptionSuccess ? 1.3f : 0.5f;
                    }
                    affectedDemographic.WisdomSaveDCBonus += newsSectionCard.WisdomSaveDCEffect;
                }
            }
            else
            {
                this.newsSectionsProfitability[i] = false;
            }
        }
    }

    public void UpdateBudget()
    {
        for (int i = 0; i < 3; i++)
        {
            if (newsSectionsProfitability[i])
            {
                NewsCard newsSectionCard = this.newsSections[i].GetComponent<NewsCard>();
                float sectionRevenue = newsSectionCard.Revenue;
                foreach (DemographicType dt in newsSectionCard.AffectedPopulation)
                {
                    Demographic affectedDemograpic = this.population[(int)dt];
                    this.budget += affectedDemograpic.ViewerRatio * affectedDemograpic.Ratio * sectionRevenue;
                }
            }
        }
    }

    public void UpdatePopulationRatios()
    {
        float remainingMaxRatio = 0.8f;
        foreach (DemographicType dt in DemographicTypeValues)
        {
            Demographic demographic = this.population[(int)dt];
            if (dt != DemographicTypeValues[DemographicTypeValues.Length - 1])
            {
                float ratio = UnityEngine.Random.Range(0.1f, remainingMaxRatio);
                demographic.Ratio = ratio;
                remainingMaxRatio -= ratio - 0.1f;
            }
            else
            {
                demographic.Ratio = remainingMaxRatio;
            }

            demographic.ViewerRatio = Mathf.Max(demographic.ViewerRatio * 0.75f, 0.01f / demographic.Ratio);
        }
    }

    public void RenewDay()
    {
        for (int i = 0; i < cardTraySize; i++)
        {
            if (this.cardTray[i] == null)
            {
                AddRandomCardToTraySlot(i);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject newsSectionCardObject = this.newsSections[i];
            if (newsSectionCardObject != null)
            {
                Destroy(newsSectionCardObject);
                this.newsSections[i] = null;
                Droppable newsSectionSlot = this.newsSectionsSlots[i].GetComponent<Droppable>();
                newsSectionSlot.HeldCard = null;
                newsSectionSlot.IsCardIn = false;
                newsSectionSlot.HasCard = false;
            }
        }
    }
}
