using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentManagerLineRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    private LineRenderer lineRenderer = null;
    private string detectionMode;
    public GameObject container;

    private Transform SphereOfCommentsDetection;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SphereOfCommentsDetection = GameObject.FindGameObjectWithTag("CommentDetection").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        detectionMode = SphereOfCommentsDetection.GetComponent<SphereOfCommentsDetection>().currentDetectionMode;
        if (detectionMode.Equals("CommentDetectionON") && container.activeSelf)
        {
            UpdateLength();
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
        }
        
    }

    private void UpdateLength()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, SphereOfCommentsDetection.position);
    }
}
