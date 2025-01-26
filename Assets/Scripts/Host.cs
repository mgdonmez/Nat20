using Unity.VisualScripting;
using UnityEngine;

public class Host :MonoBehaviour
{
    [SerializeField]
    private AudioClip[] audioClips;

    private AudioSource audioSource;

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

}
