using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements.Experimental;
using System.Reflection;
using UnityEngine.UI;
using DG.Tweening;
using NUnit.Framework;

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
    [SerializeField] private Animator roll;
    [SerializeField] private StopDice stopDice;
    [SerializeField] private GameObject die;
    [SerializeField] private GameObject liveButton;
    [SerializeField] private Sprite liveButtonLive;
    [SerializeField] private Sprite liveButtonFree;
    [SerializeField] private bool isLive;
    [SerializeField] private Button rerollButton;
    [SerializeField] private GameObject coins;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Host host;
    [SerializeField] private Transform viewers;
    private float newsGingleLength = 5.0f;
    private float greetingLength = 2.55f;
    private float newsSpeechLength = 5.05f;
    private float nextGingleLength = 2.5f;
    private float newsEndingLength = 3.45f;
    private float newsEndingJingleLength = 5.00f;

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

        this.stopDice = this.roll.GetBehaviour<StopDice>();
    }

    private void Update()
    {
        if (!this.isLive)
        {
            this.rerollButton.interactable = this.budget >= this.rerollCost;
        }

        for (int i = 0; i < this.coins.transform.childCount; i++)
        {
            SpriteRenderer coin = this.coins.transform.GetChild(i).GetComponent<SpriteRenderer>();
            coin.enabled = (budget - 1) >= i;
        }
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
        if (firstTime || budget >= rerollCost)
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
                    if (card != null)
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
        this.isLive = true;
        this.liveButton.GetComponent<Button>().interactable = false;
        this.liveButton.GetComponent<Image>().sprite = this.liveButtonLive;
        this.rerollButton.interactable = false;
        float timer = newsGingleLength;
        FunctionTimer.Create(() => this.host.Greeting(), timer);
        timer += greetingLength;
        FunctionTimer.Create(() => this.host.Next(), timer);
        timer += nextGingleLength;
        DetermineNewsEffect(timer);
        int nullCount = this.newsSections.Count(item => item == null);
        timer += (newsSpeechLength + nextGingleLength) * (3 - nullCount);
        FunctionTimer.Create(() => this.host.Ending(), timer);
        timer += newsEndingLength;
        FunctionTimer.Create(() => this.host.Outro(), timer);
        timer += newsEndingJingleLength + 0.1f;
        FunctionTimer.Create(() => UpdateBudget(), timer);
        timer += 0.1f;
        FunctionTimer.Create(() => UpdatePopulationRatios(), timer);
        timer += 0.1f;
        FunctionTimer.Create(() => RenewDay(), timer);
    }

    public void DetermineNewsEffect(float timer)
    {
        for (int i = 0; i < 3; i++)
        {
            int index = i;
            if (this.newsSections[index] != null)
            {
                FunctionTimer.Create(() =>
                {
                    this.host.News();
                    FunctionTimer.Create(() => this.host.Next(), newsSpeechLength);
                    Transform newsCardTransform = this.newsSections[index].transform;
                    NewsCard newsSectionCard = this.newsSections[index].GetComponent<CardBehaviour>().CardInfo;
                    Vector3 originalScale = newsCardTransform.localScale;
                    Vector3 hoverScale = newsCardTransform.localScale * 1.2f;
                    newsCardTransform.DOScale(hoverScale, 0.3f);
                    FunctionTimer.Create(() => newsCardTransform.DOScale(originalScale, 0.3f), newsSpeechLength);


                    foreach (DemographicType dt in newsSectionCard.AffectedPopulation)
                    {
                        Demographic affectedDemographic = this.population[(int)dt];
                        if (newsSectionCard.Genuinity == Genuinity.Truth)
                        {
                            this.newsSectionsProfitability[index] = true;
                            affectedDemographic.ViewerRatio = Mathf.Min(1.0f, affectedDemographic.ViewerRatio * 1.2f);
                        }
                        else
                        {
                            int deceptionCheck = newsSectionCard.DeceptionCheck(this.die, this.stopDice);
                            bool profitability = true;
                            bool deceptionSuccess = deceptionCheck >= affectedDemographic.WisdomSaveDC();
                            profitability &= deceptionSuccess;
                            affectedDemographic.ViewerRatio *= deceptionSuccess ? 1.3f : 0.5f;
                            affectedDemographic.ViewerRatio = Mathf.Max(Mathf.Min(affectedDemographic.ViewerRatio * (deceptionSuccess ? 1.3f : 0.5f), 1.0f), 0.01f / affectedDemographic.Ratio);
                        }
                        affectedDemographic.WisdomSaveDCBonus += newsSectionCard.WisdomSaveDCEffect;
                    }
                }, timer);
                timer += (newsSpeechLength + nextGingleLength);
            }
            else
            {
                this.newsSectionsProfitability[index] = false;
            }
        }
    }

    public void UpdateBudget()
    {
        for (int i = 0; i < 3; i++)
        {
            if (newsSectionsProfitability[i])
            {
                NewsCard newsSectionCard = this.newsSections[i].GetComponent<CardBehaviour>().CardInfo;
                float sectionRevenue = newsSectionCard.Revenue;
                foreach (DemographicType dt in newsSectionCard.AffectedPopulation)
                {
                    Demographic affectedDemograpic = this.population[(int)dt];
                    this.budget = Mathf.Min(this.budget + affectedDemograpic.ViewerRatio * affectedDemograpic.Ratio * sectionRevenue, 6.0f);
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

        this.liveButton.GetComponent<Button>().interactable = true;
        this.liveButton.GetComponent<Image>().sprite = this.liveButtonFree;
        this.rerollButton.interactable = true;
        this.isLive = false;
    }
}
