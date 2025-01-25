using UnityEngine;

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
