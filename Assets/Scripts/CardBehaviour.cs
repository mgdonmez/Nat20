using System.Collections.Generic;
using System;
using UnityEngine;

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

        this.revenue = 10f / Mathf.Pow(2, (int)genuinity);

        this.deceptionPenalty = (2 - (int)genuinity) * -3;

        this.wisdomSaveDCEffect = genuinity == Genuinity.Truth ? -1 : 2 - (int)genuinity;

        this.sprite = SpriteManager.Instance.GetNewsCardSprite(genuinity, this.affectedPopulation);
    }

    public int DeceptionCheck()
    {
        return UnityEngine.Random.Range(1, 21) + this.deceptionPenalty;
    }
}

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] private NewsCard cardInfo;

    public NewsCard CardInfo { get => cardInfo; set => cardInfo = value; }

    public CardBehaviour(NewsCard cardInfo)
    {
        this.cardInfo = cardInfo;
    }

    private void Update()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = cardInfo.Sprite;
    }
}
