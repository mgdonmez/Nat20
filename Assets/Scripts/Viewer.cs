using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    [SerializeField]
    float duration = 0.0f;
    [SerializeField]
    Vector3 destination;
    [SerializeField]
    float distance;
    bool isWalking = false;
    Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isWalking = true;
            Walk();
        }
        if (Input.GetKeyDown(KeyCode.D))
            isWalking = false;

    }
    private void Walk()
    {
        if (!isWalking)
        {
            return;
        }
        if (this.transform.localScale != Vector3.one)
        {
            transform.DOScale(1f, 2f);
        }
        distance = Random.Range(-8f,8f);

        destination = new Vector3(distance, startPosition.y, transform.position.z);
        duration = Mathf.Abs(distance - transform.position.x);
        if (distance - transform.position.x < 0)
        {
            Vector3 rot = new Vector3(0f, 180f, 0f);
            transform.DOLocalRotate(rot, 1f);
        }
        else
        {
            Vector3 rot = new Vector3(0f, 0f, 0f);
            transform.DOLocalRotate(rot, 1f);
        }
        GetComponent<Animator>().SetBool("Walk", enabled);

        transform.DOMove(destination, duration).OnComplete(() => { if (isWalking) Walk(); else Stop(); });
    }
    private void Stop()
    {
        //transform.DOMove(new Vector3(transform.position.x, -2.5f, transform.position.z), 2f);
        //transform.DOScale(0.9f, 3f).OnComplete(() => { if (!isWalking) Walk(); else Stop(); });
        GetComponent<Animator>().SetBool("Walk", !enabled);
    }
    private void Watch()
    {

    }
}
