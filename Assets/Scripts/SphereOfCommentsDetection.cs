using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereOfCommentsDetection : MonoBehaviour
{

    public GameObject LeftController;
    private MeshRenderer meshRenderer;

    public string currentDetectionMode;
    private string currentGameMode;
    public DomePointerEvents domeOfPointerInteractions;

    // Start is called before the first frame update
    void Awake()
    {
        meshRenderer = this.GetComponent<MeshRenderer>();
        currentDetectionMode = "CommentDetectionOFF";
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Dot(LeftController.transform.up - new Vector3(0.5f,0.5f,0.5f), Vector3.down) > 0f) && domeOfPointerInteractions.inputManager.currentGameState == InputManager.GameState.ViewMode)
        {
            meshRenderer.enabled = true;
            currentDetectionMode = "CommentDetectionON";
        }
        else
        {
            meshRenderer.enabled = false;
            currentDetectionMode = "CommentDetectionOFF";
        }
    }
}
