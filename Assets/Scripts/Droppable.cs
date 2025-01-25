using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Droppable : MonoBehaviour
{
    private Vector3 startingScale;
    private Vector3 hoveringScale;
    private bool isCardIn = false;
    private void Awake()
    {
        startingScale = transform.localScale;
        hoveringScale = new Vector3(startingScale.x * 1.2f, startingScale.y * 1.2f, startingScale.z);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Draggable>() != null && collision.GetComponent<Draggable>().IsDragged())
        {
            isCardIn = true;
            transform.DOScale(hoveringScale, 0.3f);
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Draggable>() != null)
        {
            isCardIn = false;
        }
        transform.DOScale(startingScale, 0.3f);
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<Draggable>() != null && !collision.GetComponent<Draggable>().IsDragged())
        {
            transform.DOScale(startingScale, 0.3f);
            if (isCardIn)
            {
                collision.gameObject.transform.position = transform.position;
            }
        }
    }
}
