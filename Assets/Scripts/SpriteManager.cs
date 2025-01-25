using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;
    public Dictionary<string, Sprite> NewsCards;

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
        this.NewsCards = new Dictionary<string, Sprite>();
        foreach (Genuinity g in Manager.GenuinityValues)
        {
            int subsetCount = 1 << Manager.DemographicTypeValues.Length;
            for (int mask = 1; mask < subsetCount; mask++)
            {
                List<DemographicType> subset = new List<DemographicType>();
                for (int i = 0; i < Manager.DemographicTypeValues.Length; i++)
                {
                    if ((mask & (1 << i)) != 0)
                    {
                        subset.Add(Manager.DemographicTypeValues[i]);
                    }
                }
                subset.Sort((a, b) => b.CompareTo(a));

                string spriteFileName = GetNewsCardSpriteFileName(g, subset);

                string spritePath = Path.Combine("Visuals", "Cards", spriteFileName);
                Sprite sprite = Resources.Load<Sprite>(spritePath);

                this.NewsCards.Add(spriteFileName, sprite);
            }
        }
    }

    public string GetNewsCardSpriteFileName(Genuinity genuinity, List<DemographicType> affectedPopulation)
    {
        string spriteFileName = "";
        foreach (DemographicType dt in affectedPopulation)
        {
            spriteFileName += dt.ToString()[0];
        }
        spriteFileName += "_" + genuinity.ToString()[0];

        return spriteFileName;
    }

    public Sprite GetNewsCardSprite(Genuinity genuinity, List<DemographicType> affectedPopulation)
    {
        return this.NewsCards[GetNewsCardSpriteFileName(genuinity, affectedPopulation)];
    }
}
