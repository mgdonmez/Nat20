using Unity.VisualScripting;
using UnityEngine;

public class Host :MonoBehaviour
{
    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;

    [SerializeField]
    private Animator headAnimator;
    [SerializeField]
    private Animator bodyAnimator;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void Greeting()
    {
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }
    public void Ending()
    {
        audioSource.clip = audioClips[2];
        audioSource.Play();
    }
    public void News()
    {
        audioSource.clip = audioClips[1];
        audioSource.Play();
    }

    public void SetTalkingAnim(bool enabled)
    {
        headAnimator.SetBool("Talking", enabled);
        bodyAnimator.SetBool("Talking", enabled);
    }

    public void SetFrontAnim(bool front)
    {
        headAnimator.SetBool("Front", front);
    }

    public void Next()
    {
        audioSource.clip = audioClips[3];
        audioSource.Play();
    }
    public void Outro()
    {
        audioSource.clip = audioClips[4];
        audioSource.Play();
    }
}
