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
    public bool isWalking = false;
    Vector3 startPosition;
    [SerializeField]
    DemographicType type;
    [SerializeField]
    private AudioClip[] audioClipSources;

    private void Start()
    {
        startPosition = transform.position;
        if(GetComponentsInChildren<SpriteRenderer>()[1].sprite.name.Contains("circle_head_0"))
        {
            type = DemographicType.Circle;
        }
        else if(GetComponentsInChildren<SpriteRenderer>()[1].sprite.name.Contains("triangle_head_0"))
        {
            type = DemographicType.Triangle;
        }
        else if (GetComponentsInChildren<SpriteRenderer>()[1].sprite.name.Contains("square_head_0"))
        {
            type = DemographicType.Square;
        }

        isWalking = true;
        Walk();
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
        GetComponent<UnityEngine.Animator>().SetBool("Walk", enabled);

        transform.DOMove(destination, duration).OnComplete(() => { if (isWalking) Walk(); else Stop(); });
    }
    private void Stop()
    {

        GetComponent<UnityEngine.Animator>().SetBool("Walk", !enabled);
    }
    public void React()
    {
        bool approved = Manager.Instance.Population[(int)type].ViewerRatio > Random.Range(0f, 1f);

        if (approved)
        {
            int choice = Random.Range(0, 2);
            GetComponent<AudioSource>().clip = audioClipSources[0+choice];
        }
        else
        {
            int choice = Random.Range(0, 2);
            GetComponent<AudioSource>().clip = audioClipSources[3 + choice];
        }
        GetComponent<AudioSource>().Play();
    }
    public void Interest()
    {
        int choice = Random.Range(0, 3);
        GetComponent<AudioSource>().clip = audioClipSources[6 + choice];
        GetComponent<AudioSource>().Play();
    }
}
