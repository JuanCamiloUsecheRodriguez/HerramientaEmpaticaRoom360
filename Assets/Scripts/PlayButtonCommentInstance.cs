using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayButtonCommentInstance : MonoBehaviour, IPointerClickHandler
{
    private AudioSource audioSource;
    public Sprite pause;
    public Sprite play;
    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (audioSource.isPlaying)
        {
            GetComponentsInChildren<Image>()[1].sprite = pause;
        }
        else
        {
            GetComponentsInChildren<Image>()[1].sprite = play;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(audioSource.clip == null)
        {
            print("NO CLIP");
        }
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        else
        {
            audioSource.Play();
        }
    }
}