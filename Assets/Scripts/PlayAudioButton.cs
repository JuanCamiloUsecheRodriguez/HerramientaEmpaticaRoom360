using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayAudioButton : MonoBehaviour, IPointerClickHandler
{
    private InputManager inputManager;
    public Sprite pause;
    public Sprite play;
    // Start is called before the first frame update
    void Awake()
    {
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (inputManager.audioSource.isPlaying)
        {
            GetComponent<Image>().sprite = pause;
        }
        else
        {
            GetComponent<Image>().sprite = play;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        print("IT CLICKED!! >:p");
        if (inputManager.audioSource.isPlaying)
        {
            inputManager.StopAudio();
        }
        else
        {
            inputManager.PlayAudio();
        }
    }

}
