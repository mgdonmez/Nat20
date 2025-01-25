using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Droppable : MonoBehaviour
{
    private Vector3 startingScale;
    private Vector3 hoveringScale;
    [SerializeField] private bool isCardIn;
    [SerializeField] private bool hasCard;
    [SerializeField] private GameObject heldCard;

    public GameObject HeldCard { get => heldCard; set => heldCard = value; }

    private void Awake()
    {
        startingScale = transform.localScale;
        hoveringScale = new Vector3(startingScale.x * 1.2f, startingScale.y * 1.2f, startingScale.z);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCard && collision.GetComponent<Draggable>() != null && !collision.GetComponent<Draggable>().isInSlot && collision.GetComponent<Draggable>().IsDragged())
        {
            isCardIn = true;
            collision.GetComponent<Draggable>().isInSlot = true;
            collision.GetComponent<Draggable>().usedSlot = gameObject;
            transform.DOScale(hoveringScale, 0.3f);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (!hasCard && collision.GetComponent<Draggable>() != null)
        {
            isCardIn = false;
            collision.GetComponent<Draggable>().isInSlot = false;
            collision.GetComponent<Draggable>().usedSlot = null;
        }
        transform.DOScale(startingScale, 0.3f);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Draggable>() != null)
        {
            if (!hasCard && (!collision.GetComponent<Draggable>().isInSlot || collision.GetComponent<Draggable>().usedSlot == gameObject) && !collision.GetComponent<Draggable>().IsDragged())
            {
                transform.DOScale(startingScale, 0.3f);
                if (isCardIn)
                {
                    collision.gameObject.transform.position = transform.position;
                    hasCard = true;
                    heldCard = collision.gameObject;
                    ChangeGameManagerStatus(false);
                }
            }
            else if (hasCard && collision.gameObject == heldCard && collision.GetComponent<Draggable>().IsDragged())
            {
                hasCard = false;
                ChangeGameManagerStatus(true);
                heldCard = null;
            }
        }
    }

    public void ChangeGameManagerStatus(bool remove = false)
    {
        if (Manager.Instance.CardTraySlots.Contains(gameObject.transform))
        {
            int index = Manager.Instance.CardTraySlots.IndexOf(gameObject.transform);
            Manager.Instance.CardTray[index] = remove ? null : heldCard;
        }
        else
        {
            int index = Manager.Instance.NewsSectionsSlots.IndexOf(gameObject.transform);
            Manager.Instance.NewsSections[index] = remove ? null : heldCard;
        }
    }
}
