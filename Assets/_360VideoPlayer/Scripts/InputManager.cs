using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public VideoManager videoManager = null;

    public GameObject DomeOfEvents;

    public enum GameState {CommentMode,ViewMode, AllCommentsMode};
    public enum CommentState { Microphone, Text};

    public GameState currentGameState;
    public GameState currentGameState2;

    public CommentState currentCommentState;

    public bool VideoManagerisPaused = false;

    public AudioSource audioSource;

    public int micId;

    private int MicPosition;

    public List<Sprite> iconsForCycling;

    private GameObject[] icons;
    private GameObject[] labels;
    private GameObject[] commentManagers;
    private bool labelsActive = true;

    private int iconIndex = 0;

    public GameObject forwardIcon;
    public GameObject backwardIcon;
    public GameObject nextIcon;
    public GameObject prevIcon;
    public GameObject changeCommentModeText;

    public GameObject sphereOfCommentDetection;

    private void Start()
    {
        currentGameState = GameState.CommentMode;
        currentGameState2 = GameState.CommentMode;

        currentCommentState = CommentState.Microphone;

        //---------MIC TEST
        bool micSelected = false;
        int _micNdx = 0;
        if (Microphone.devices.Length > 1)
        {
            for (int i = 0; !micSelected && (i < Microphone.devices.Length); i++)
            {
                if (Microphone.devices[i].ToString().Contains("Rift"))
                {
                    _micNdx = i;
                    micSelected = true;
                }
            }

            for (int i = 0; !micSelected && (i < Microphone.devices.Length); i++)
            {
                if (Microphone.devices[i].ToString().Contains("Oculus"))
                {
                    _micNdx = i;
                    micSelected = true;
                }
            }
        }
        micId = _micNdx;
        audioSource = GetComponent<AudioSource>();

        //StartCoroutine("StartTimer");

        icons = GameObject.FindGameObjectsWithTag("IconImage");
        labels = GameObject.FindGameObjectsWithTag("GameObjectLabel");
        commentManagers = GameObject.FindGameObjectsWithTag("CommentManager");
    }
    /*
    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(6);
        SavWav.Save("mySavedAudio", audioSource.clip);
    }
    */
    public void StartRecording()
    {
        audioSource.clip = Microphone.Start(Microphone.devices[micId].ToString(), false, 30, 44100); //48000
        //audioSource.Play();
    }

    public void StopRecording()
    {
        StartCoroutine("EndAndTrim");

    }
    private IEnumerator EndAndTrim()
    {
        yield return new WaitForSeconds(1f);
        MicPosition = Microphone.GetPosition(Microphone.devices[micId].ToString());
        if (Microphone.IsRecording(Microphone.devices[micId].ToString()))
        {
            Microphone.End(Microphone.devices[micId].ToString());
        }
        //audioSource.clip = SavWav.TrimSilence(audioSource.clip, 10f);
        yield return new WaitForSeconds(0.5f);
        EndRecording(audioSource, Microphone.devices[micId].ToString());
    }
    public void EndRecording(AudioSource audS, string deviceName)
    {
        //Capture the current clip data
        AudioClip recordedClip = audS.clip;
        var position = MicPosition;
        var soundData = new float[recordedClip.samples * recordedClip.channels];
        recordedClip.GetData(soundData, 0);

        //Create shortened array for the data that was used for recording
        var newData = new float[position * recordedClip.channels];

        //$$anonymous$$icrophone.End (null);
        //Copy the used samples to a new array
        for (int i = 0; i < newData.Length; i++)
        {
            newData[i] = soundData[i];
        }

        //One does not simply shorten an AudioClip,
        //    so we make a new one with the appropriate length
        var newClip = AudioClip.Create(recordedClip.name, position, recordedClip.channels, recordedClip.frequency, false);
        newClip.SetData(newData, 0);        //Give it the data from the old clip

        //Replace the old clip
        AudioClip.Destroy(recordedClip);
        audS.clip = newClip;
    }

    public void SaveAudio(string path, int videoIndex)
    {
        SavWav.Save(path,"mySavedAudio" + videoIndex, audioSource.clip);
    }

    public void PlayAudio()
    {
        audioSource.Play();
    }

    public void StopAudio()
    {
        audioSource.Stop();
    }

    private void Update()
    {
        
        if (!videoManager.IsVideoReady)
            return;
        
        OculusInput();
        KeyboardInput();

    }

    private void OculusInput()
    {
        if(OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch)|| Input.GetKeyDown(KeyCode.K))
        {
            VideoManagerisPaused = !VideoManagerisPaused;
            videoManager.PauseToggle();
            if(currentGameState == GameState.ViewMode)
            {
                DomeOfEvents.SendMessage("PauseCommentsVideoManagers");
            }
        }

        if(OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickLeft, OVRInput.Controller.RTouch) && currentGameState == GameState.CommentMode)
        {
            videoManager.PreviousVideo();

            //RELOAD COMMENTS
            ReloadComments();
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickRight, OVRInput.Controller.RTouch) && currentGameState == GameState.CommentMode)
        {
            videoManager.NextVideo();
            //RELOAD COMMENTS
            ReloadComments();
        }
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, OVRInput.Controller.LTouch) && (currentGameState == GameState.CommentMode || currentGameState2 == GameState.AllCommentsMode))
        {
            videoManager.SeekBack();
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, OVRInput.Controller.LTouch) && (currentGameState == GameState.CommentMode || currentGameState2 == GameState.AllCommentsMode))
        {
            videoManager.SeekForward();
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
        {
            ChangeMode();
            if(currentGameState2 == GameState.ViewMode && labelsActive == true)
            {
                forwardIcon.SetActive(false);
                backwardIcon.SetActive(false);
                nextIcon.SetActive(false);
                prevIcon.SetActive(false);
                changeCommentModeText.SetActive(false);
            }
            else if (currentGameState2 == GameState.CommentMode && labelsActive == true)
            {
                forwardIcon.SetActive(true);
                backwardIcon.SetActive(true);
                nextIcon.SetActive(true);
                prevIcon.SetActive(true);
                changeCommentModeText.SetActive(true);
            }
            else if(currentGameState2 == GameState.AllCommentsMode && labelsActive == true)
            {
                forwardIcon.SetActive(true);
                backwardIcon.SetActive(true);
                nextIcon.SetActive(false);
                prevIcon.SetActive(false);
                changeCommentModeText.SetActive(false);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) && (currentGameState == GameState.CommentMode))
        {
            if(currentCommentState == CommentState.Microphone)
            {
                currentCommentState = CommentState.Text;
                DomeOfEvents.SendMessage("ToggleTextMode");
            }
            else if (currentCommentState == CommentState.Text)
            {
                currentCommentState = CommentState.Microphone;
                DomeOfEvents.SendMessage("ToggleMicMode");
            }
        }

            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)|| Input.GetKeyDown(KeyCode.S))
        {
           iconIndex++;
           if (iconIndex > 3)
           {
                iconIndex = 0;
           }
           foreach (GameObject icon in icons)
           {

                icon.GetComponent<Image>().sprite = iconsForCycling[iconIndex];
           }
            foreach (GameObject commentManager in commentManagers)
            {
                if (iconIndex == 0)
                {
                    Color color = new Color(0.03f, 1f, 0.04f, 0.4f);
                    commentManager.GetComponent<Renderer>().material.SetColor("_Color", color);
                }
                else if (iconIndex == 1)
                {
                    Color color = new Color(0.03f, 1f, 0.6f, 0.4f);
                    commentManager.GetComponent<Renderer>().material.SetColor("_Color", color);
                }
                else if (iconIndex == 2)
                {
                    Color color = new Color(1f, 0.03f, 0.6f, 0.4f);
                    commentManager.GetComponent<Renderer>().material.SetColor("_Color", color);
                }
                else if (iconIndex == 3)
                {
                    Color color = new Color(0.9f, 1f, 0.03f, 0.4f);
                    commentManager.GetComponent<Renderer>().material.SetColor("_Color", color);
                }

            }
            if (iconIndex == 0)
            {
                Color color = new Color(0.03f, 1f, 0.04f, 0.4f);
                sphereOfCommentDetection.GetComponent<Renderer>().material.SetColor("_Color", color);
            }
            else if (iconIndex == 1)
            {
                Color color = new Color(0.03f, 1f, 0.6f, 0.4f);
                sphereOfCommentDetection.GetComponent<Renderer>().material.SetColor("_Color", color);
            }
            else if (iconIndex == 2)
            {
                Color color = new Color(1f, 0.03f, 0.6f, 0.4f);
                sphereOfCommentDetection.GetComponent<Renderer>().material.SetColor("_Color", color);
            }
            else if (iconIndex == 3)
            {
                Color color = new Color(0.9f, 1f, 0.03f, 0.4f);               
                sphereOfCommentDetection.GetComponent<Renderer>().material.SetColor("_Color", color);
            }
        }

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch) || Input.GetKeyDown(KeyCode.B))
        {
            //buscargameobjects con tag label y desactivarlos
            if(labelsActive == true)
            {
                foreach(GameObject label in labels)
                {
                    label.SetActive(false);
                }
                labelsActive = false;
            }
            else if(labelsActive == false)
            {
                foreach (GameObject label in labels)
                {
                    label.SetActive(true);
                }
                if(currentGameState2 == GameState.CommentMode)
                {
                    forwardIcon.SetActive(true);
                    backwardIcon.SetActive(true);
                    nextIcon.SetActive(true);
                    prevIcon.SetActive(true);
                    changeCommentModeText.SetActive(true);
                }
                if (currentGameState2 == GameState.ViewMode)
                {
                    forwardIcon.SetActive(false);
                    backwardIcon.SetActive(false);
                    nextIcon.SetActive(false);
                    prevIcon.SetActive(false);
                    changeCommentModeText.SetActive(true);
                }
                if (currentGameState2 == GameState.AllCommentsMode)
                {
                    forwardIcon.SetActive(true);
                    backwardIcon.SetActive(true);
                    nextIcon.SetActive(false);
                    prevIcon.SetActive(false);
                    changeCommentModeText.SetActive(true);
                }
                labelsActive = true;
            }
        }
    }

    public void ReloadComments()
    {
        DomeOfEvents.SendMessage("UpdatePath");
        if (currentGameState2 == GameState.ViewMode)
        {
            DomeOfEvents.SendMessage("PrepViewMode");
        }

    }
    public void ReloadComments2()
    {
        DomeOfEvents.SendMessage("UpdatePath");
        if (currentGameState2 == GameState.ViewMode)
        {
            DomeOfEvents.SendMessage("ResetViewMode");
        }

    }


    public void ChangeMode()
    {
        if(currentGameState2 == GameState.CommentMode)
        {
            currentGameState2 = GameState.ViewMode;
            currentGameState = GameState.ViewMode;
            DomeOfEvents.SendMessage("PrepViewMode");
        }
        else if(currentGameState2 == GameState.ViewMode)
        {
            currentGameState2  = GameState.AllCommentsMode;
            DomeOfEvents.SendMessage("PrepAllCommentsMode");
        }
        else if(currentGameState2 == GameState.AllCommentsMode)
        {
            currentGameState2 = GameState.CommentMode;
            currentGameState = GameState.CommentMode;
            DomeOfEvents.SendMessage("PrepCommentMode");
        }
    }



    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            videoManager.PauseToggle();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            videoManager.PreviousVideo();
            DomeOfEvents.SendMessage("UpdatePath");
            if (currentGameState == GameState.ViewMode)
            {
                DomeOfEvents.SendMessage("PrepViewMode");
            }
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            videoManager.NextVideo();
            DomeOfEvents.SendMessage("UpdatePath");
            if (currentGameState == GameState.ViewMode)
            {
                DomeOfEvents.SendMessage("PrepViewMode");
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            videoManager.SeekBack();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            videoManager.SeekForward();
        }
    }
}
