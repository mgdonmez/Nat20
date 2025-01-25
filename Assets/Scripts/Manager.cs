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

    public Genuinity Genuinity { get => genuinity; set => genuinity = value; }
    public List<DemographicType> AffectedPopulation { get => affectedPopulation; }
    public float Revenue { get => revenue; }

    public NewsCard(Genuinity genuinity = Genuinity.Lie, List<DemographicType> affectedPopulation = null)
    {
        this.genuinity = genuinity;

        this.affectedPopulation = new List<DemographicType>();
        if (affectedPopulation != null)
        {
            this.affectedPopulation.AddRange(affectedPopulation);
        }

        this.revenue = 1f / Mathf.Pow(2, (int)genuinity);
    }

    public int DeceptionCheck()
    {
        return UnityEngine.Random.Range(1, 21) + (2 - (int)genuinity) * -3;
    }
}


public class Manager : MonoBehaviour
{
    private DemographicType[] dtypeValues = (DemographicType[])Enum.GetValues(typeof(DemographicType));

    [SerializeField] private int cardTraySize;
    [SerializeField] private int rerollCost;
    [SerializeField] private float budget;
    [SerializeField] private List<Demographic> population;
    [SerializeField] private List<NewsCard> cardTray;

    public static Manager Instance;

    public List<Demographic> Population { get => population; }
    public List<NewsCard> CardTray { get => cardTray; }
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


    void Start()
    {
        this.population = new List<Demographic>();
        foreach (DemographicType dt in dtypeValues)
        {
            Population.Add(new Demographic(dt, 1.0f / dtypeValues.Length));
        }

        RerollCardTray(true);
        // FunctionTimer.Create(() => { RerollCardTray(); FunctionTimer.Create(() => { RerollCardTray(); }, 10.0f); }, 10.0f);
        // FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); FunctionTimer.Create(() => { UpdatePopulationRatios(); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f); }, 2.0f);
    }

    void Update()
    {

    }

    public void RerollCardTray(bool firstTime = false)
    {
        if (firstTime || budget > rerollCost)
        {

            if (firstTime)
            {
                this.cardTray = new List<NewsCard>();
            }
            else
            {
                budget -= rerollCost;
                this.cardTray.Clear();
            }

            for (int i = 0; i < cardTraySize; i++)
            {
                Genuinity genuinity = (Genuinity)UnityEngine.Random.Range(0, Enum.GetValues(typeof(Genuinity)).Length);

                int[] affectedPopulationInt = Enumerable.Range(0, dtypeValues.Length)
                .OrderBy(_ => UnityEngine.Random.value)
                    .Take(UnityEngine.Random.Range(1, dtypeValues.Length + 1))
                    .ToArray();

                List<DemographicType> affectedPopulation = affectedPopulationInt
                    .Select(x => (DemographicType)x)
                    .ToList();

                this.cardTray.Add(new NewsCard(genuinity, affectedPopulation));
            }
        }
    }

    public void UpdatePopulationRatios()
    {
        float remainingMaxRatio = 0.8f;
        foreach (DemographicType dt in dtypeValues)
        {
            Demographic demographic = this.population[(int)dt];
            if (dt != dtypeValues[dtypeValues.Length - 1])
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
