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

public enum Genuinity
{
    Lie,
    Misdirection,
    Truth
}

[Serializable]
public class NewsCard
{
    [SerializeField] private Genuinity genuinity;
    [SerializeField] private List<DemographicType> affectedPopulation;
    [SerializeField] private float revenue;
    [SerializeField] private int deceptionPenalty;
    [SerializeField] private int wisdomSaveDCEffect;
    [SerializeField] private Sprite sprite;

    public Genuinity Genuinity { get => genuinity; set => genuinity = value; }
    public List<DemographicType> AffectedPopulation { get => affectedPopulation; }
    public float Revenue { get => revenue; }
    public int DeceptionPenalty { get => deceptionPenalty; }
    public int WisdomSaveDCEffect { get => wisdomSaveDCEffect; }
    public Sprite Sprite { get => sprite; }

    public NewsCard(Genuinity genuinity = Genuinity.Lie, List<DemographicType> affectedPopulation = null)
    {
        this.genuinity = genuinity;

        this.affectedPopulation = new List<DemographicType>();
        if (affectedPopulation != null)
        {
            this.affectedPopulation.AddRange(affectedPopulation);
        }

        this.revenue = 1f / Mathf.Pow(2, (int)genuinity);

        this.deceptionPenalty = (2 - (int)genuinity) * -3;

        this.wisdomSaveDCEffect = genuinity == Genuinity.Truth ? -1 : 2 - (int)genuinity;

        this.sprite = SpriteManager.Instance.GetNewsCardSprite(genuinity, this.affectedPopulation);
    }

    public int DeceptionCheck()
    {
        return UnityEngine.Random.Range(1, 21) + this.deceptionPenalty;
    }
}

public class Manager : MonoBehaviour
{
    [SerializeField] private int cardTraySize;
    [SerializeField] private int rerollCost;
    [SerializeField] private float budget;
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
    }

    private void Update()
    {

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
                this.cardTray.Clear();
            }

            for (int i = 0; i < cardTraySize; i++)
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
                GameObject newsCardObject = RandomNewsCard();
                newsCardObject.transform.position = this.cardTraySlots[i].transform.position;
                newsCardObject.GetComponent<Draggable>().isInSlot = true;
                this.cardTraySlots[i].GetComponent<Droppable>().HeldCard = newsCardObject;

                this.cardTray.Add(newsCardObject);
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
}
