using DG.Tweening;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    private bool isDragged = false;
    private Vector3 mouseDragStartPosition;
    private Vector3 spriteDragStartPosition;
    private Vector3 startingScale;
    private Vector3 hoverScale;

    public bool IsDragged()
    {
        return isDragged;
    }
    private void Awake()
    {
        startingScale = transform.localScale;
        hoverScale = transform.localScale * 1.2f;
    }
    private void OnMouseEnter()
    {
        transform.DOScale(hoverScale, 0.3f);
    }
    private void OnMouseExit()
    {
        transform.DOScale(startingScale, 0.3f);
    }
    private void OnMouseDown()
    {
        //Debug.Log("ONMOUSEDOWN");
        isDragged = true;
        transform.DOScale(hoverScale, 0.3f);
        mouseDragStartPosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        spriteDragStartPosition = transform.localPosition;
    }

    private void OnMouseDrag()
    {
        //Debug.Log("OnMouseDrag");

        if (isDragged)
        {
            transform.localPosition = spriteDragStartPosition + (Camera.main.ScreenToWorldPoint( Input.mousePosition ) - mouseDragStartPosition );
        }
    }

    private void OnMouseUp()
    {
        //Debug.Log("OnMouseUp");
        transform.DOScale(startingScale, 0.3f);
        isDragged = false;
    }
}
