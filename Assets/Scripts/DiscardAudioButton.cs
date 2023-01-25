using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DiscardAudioButton : MonoBehaviour, IPointerClickHandler
{
    public CommentDemoPostionUpdater commentDemoPositionUpdater;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        commentDemoPositionUpdater.DiscardRecording();
    }
}
