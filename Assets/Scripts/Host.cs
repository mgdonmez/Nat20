using Unity.VisualScripting;
using UnityEngine;

public class Host : MonoBehaviour
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
        SetTalkingAnim(true);
        SetFrontAnim(true);
        audioSource.clip = audioClips[0];
        audioSource.Play();
    }
    public void Ending()
    {
        SetTalkingAnim(true);
        SetFrontAnim(true);
        audioSource.clip = audioClips[2];
        audioSource.Play();
    }
    public void News()
    {
        SetTalkingAnim(true);
        SetFrontAnim(Random.Range(0.0f, 1.0f) > 0.5f);
        audioSource.clip = audioClips[1];
        audioSource.Play();
    }

    public void Next()
    {
        SetTalkingAnim(false);
        SetFrontAnim(true);
        audioSource.clip = audioClips[3];
        audioSource.Play();
    }
    public void Outro()
    {
        SetTalkingAnim(false);
        SetFrontAnim(true);
        audioSource.clip = audioClips[4];
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
}
