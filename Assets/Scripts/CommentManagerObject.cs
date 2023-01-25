using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Video;
using UnityEngine.Networking;

public class CommentManagerObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private Transform player;
    public GameObject UICommentIcon;
    public GameObject UIListOfComments;
    public ScrollRect scrollRect;
    private bool isCommentListFocused;
    public static string newCommentinputText = "";
    public Text newCommentTextToShown;
    public LineRenderer lineRenderer;
    public GameObject DemoComment;
    public GameObject DemoComment2;
    public Text testText;
    public DomePointerEvents domeOfPointerInteractions;
    public VideoManager videoManager = null;
    public InputManager inputManager;

    public Transform content;
    public GameObject commentInstancePrefab;

    private bool pointerDown = false;
    private bool checkNewMessage = false;

    private List<GameObject> commentList;
    public GameObject container;
    public string path;
    public VideoPlayer videoPlayer;
    private GameObject[] controlLabels;
    private List<IEnumerator> commentsCoroutines;
    private AudioClip temporalAudioClip;
    private bool commentPreviewActive;
    private int commentsLoaded = 0;
    private int commentsToLoad = 0;
    private GameObject previewComment;

    // Start is called before the first frame update
    void Awake()
    { 
        domeOfPointerInteractions = GameObject.FindGameObjectWithTag("Dome").GetComponent<DomePointerEvents>();
        videoManager = GameObject.FindGameObjectWithTag("VideoManager").GetComponent<VideoManager>();
        controlLabels = GameObject.FindGameObjectsWithTag("GameObjectLabel");
        inputManager = GameObject.FindGameObjectWithTag("InputManager").GetComponent<InputManager>();
        videoPlayer = videoManager.videoPlayer;
        newCommentTextToShown = domeOfPointerInteractions.newCommentTextToShown;
        lineRenderer = domeOfPointerInteractions.lineRenderer;
        DemoComment = domeOfPointerInteractions.DemoComment;
        DemoComment2 = domeOfPointerInteractions.DemoComment2;
        testText = domeOfPointerInteractions.testText;
        commentList = new List<GameObject>();
        commentsCoroutines = new List<IEnumerator>();
        commentPreviewActive = false;
        player = GameObject.FindGameObjectWithTag("eye").transform;
    }

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player);
        path = domeOfPointerInteractions.path;
        if (isCommentListFocused == true)
        {
            if ((OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickUp, OVRInput.Controller.All) || Input.GetKey(KeyCode.W)) && scrollRect.verticalNormalizedPosition <= 1)
            {
                Debug.Log("SCROLL RECT:" + scrollRect.verticalNormalizedPosition);
                scrollRect.verticalNormalizedPosition += 0.2f;
            }
            else if ((OVRInput.GetDown(OVRInput.Button.SecondaryThumbstickDown, OVRInput.Controller.All) || Input.GetKey(KeyCode.S)) && scrollRect.verticalNormalizedPosition >= 0)
            {
                Debug.Log("SCROLL RECT:" + scrollRect.verticalNormalizedPosition);
                scrollRect.verticalNormalizedPosition -= 0.2f;
            }
        }

        if(domeOfPointerInteractions.inputManager.currentCommentState == InputManager.CommentState.Microphone) 
        { 
            if (checkNewMessage == true)
            {
                DemoComment.SetActive(true);
                DemoComment.SendMessage("UpdateTimerText", domeOfPointerInteractions.capturedTime.ToString("F0"));
                DemoComment.SendMessage("UpdateCommentManagerReference", this);       
            }
        }

        else if(domeOfPointerInteractions.inputManager.currentCommentState == InputManager.CommentState.Text)
        {
            if (checkNewMessage == true)
            {
                if (domeOfPointerInteractions.overlayKeyboard != null)
                {
                    newCommentinputText = domeOfPointerInteractions.overlayKeyboard.text;
                    newCommentTextToShown.text = newCommentinputText;
                    lineRenderer.gameObject.SetActive(false);
                    DemoComment2.SetActive(true);
                    testText.text = "";
                    DemoComment2.SendMessage("UpdateTimerText", domeOfPointerInteractions.capturedTime.ToString("F0"));
                    for (int i = 0; i < controlLabels.Length; i++)
                    {
                        controlLabels[i].SetActive(false);
                    }
                    if (domeOfPointerInteractions.overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
                    {
                        lineRenderer.gameObject.SetActive(true);
                        DemoComment2.SetActive(false);
                        for (int i = 0; i < controlLabels.Length; i++)
                        {
                            controlLabels[i].SetActive(true);
                        }
                        testText.text = "COMMENT-MODE";
                        domeOfPointerInteractions.overlayKeyboard = null;
                        Destroy(domeOfPointerInteractions.temporalMarker);
                        domeOfPointerInteractions.videoManager.PauseToggle();
                        checkNewMessage = false;
                        //FALTA DESACTIVAR LABELS

                        if (newCommentinputText != "")
                        {
                            newCommentinputText = newCommentinputText.Replace(",", " ");
                            var newComment = Instantiate(commentInstancePrefab);
                            newComment.GetComponentInChildren<Text>().text = newCommentinputText;
                            newComment.GetComponent<CommentInstance>().tiempoInicial = domeOfPointerInteractions.tiempoInicialComment;
                            newComment.GetComponent<CommentInstance>().duración = domeOfPointerInteractions.capturedTime;
                            AddNewComment(newComment, true);
                        }
                    }
                }
            }
        }

        bool AnyCommentActive = false;
        for (int i = 0; i < commentList.Count && AnyCommentActive == false; i++)
        {
            GameObject currentComment = commentList[i];
            if (currentComment.activeSelf == true)
            {
                AnyCommentActive = true;
            }
        }
        if (AnyCommentActive == true)
        {
            container.SetActive(true);
        }
        else
        {
            container.SetActive(false);
        }
        
    }


    public void DiscardRecording()
    {
        checkNewMessage = false;
        Destroy(domeOfPointerInteractions.temporalMarker);
        DemoComment.SetActive(false);
        domeOfPointerInteractions.videoManager.PauseToggle();
 
    }

    public void SaveRecording()
    {
        checkNewMessage = false;
        Destroy(domeOfPointerInteractions.temporalMarker);
        DemoComment.SetActive(false);
        domeOfPointerInteractions.videoManager.PauseToggle();
      


        //MODIFICAR LO QUE ES UN NUEVO COMMENT AHORA
        var newComment = Instantiate(commentInstancePrefab);
        newComment.GetComponentInChildren<Text>().text = "";
        newComment.GetComponentInChildren<AudioSource>().clip = inputManager.audioSource.clip;
        newComment.GetComponent<CommentInstance>().tiempoInicial = domeOfPointerInteractions.tiempoInicialComment;
        newComment.GetComponent<CommentInstance>().duración = 30f;
        AddNewComment(newComment, true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UICommentIcon.SetActive(false);
        UIListOfComments.SetActive(true);
        isCommentListFocused = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UICommentIcon.SetActive(true);
        UIListOfComments.SetActive(false);
        isCommentListFocused = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (domeOfPointerInteractions.inputManager.currentGameState == InputManager.GameState.CommentMode)
        {
            if (pointerDown == false && !DemoComment.activeSelf && commentPreviewActive == false)
            {
                pointerDown = true;
                domeOfPointerInteractions.SendMessage("SetStartTimeCaptureTime", (float)videoManager.videoPlayer.time);
                domeOfPointerInteractions.SendMessage("TakeTimeAndMarkPosition", eventData);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (domeOfPointerInteractions.inputManager.currentGameState == InputManager.GameState.CommentMode)
        {
            if (pointerDown == true && !DemoComment.activeSelf && commentPreviewActive == false)
            {
                pointerDown = false;
                StartCoroutine("WaitSecond");        
                checkNewMessage = true;
                
            }
        }
    }

    public void AddNewComment(GameObject newComment, bool seGuarda)
    {
        Transform contentTransform = content;
        newComment.transform.SetParent(contentTransform);
        newComment.transform.localScale = Vector3.one;
        newComment.transform.localRotation = Quaternion.identity;
        newComment.transform.localPosition = Vector3.zero;
        newComment.GetComponent<SphereCollider>().center = new Vector3(0, 0, 1100);
        commentList.Add(newComment);
        StartCoroutine("StopCommentPreview", newComment);
        SaveCommentToLog(newComment, seGuarda);
    }


    public void ResetTimerForComments()
    {
        StopAllCoroutines();
        commentsCoroutines.Clear();
        if (previewComment != null)
        {
            previewComment.SetActive(false);
            commentPreviewActive = false;
        }
        foreach (GameObject comment in commentList)
        {
            comment.SetActive(false);
        }
        Debug.Log("SI LLEGA ACA AAAAAAAH");
        foreach (GameObject comment in commentList)
        {
            //comment.GetComponent<CommentInstance>().SetActivationTimerForComment();
            IEnumerator e;
            e = StartTimer2(comment);
            StartCoroutine(e);
            commentsCoroutines.Add(e);

        }
        Debug.Log("SI LLEGA ACA AAAAAAAH2");
    }
    private IEnumerator WaitSecond()
    {
        yield return new WaitForSeconds(0.5f);
        domeOfPointerInteractions.SendMessage("StopTimerandOpenKeyboard", false);
        checkNewMessage = true;
    }
    private IEnumerator StartTimer(GameObject comment)
    {
        yield return new WaitForSeconds(comment.GetComponent<CommentInstance>().tiempoInicial);
        comment.SetActive(true);
        yield return new WaitForSeconds(comment.GetComponent<CommentInstance>().duración);
        comment.SetActive(false);
    }

    private IEnumerator StartTimer2(GameObject comment)
    {
        bool commentDone = false;
        float tiempoIncial = comment.GetComponent<CommentInstance>().tiempoInicial;
        float duracion = comment.GetComponent<CommentInstance>().duración;
        while (true && !commentDone)
        {
            while (inputManager.VideoManagerisPaused)
            {
                yield return null;
            }
            // Do Something
            yield return new WaitForSeconds(1f);
            tiempoIncial--;
            if(tiempoIncial <= 0)
            {
                comment.SetActive(true);
                duracion--;
                
            }
            if(duracion <= 0)
            {
                comment.SetActive(false);
                commentDone = true;

            }
            yield return null;
        }
    }

    public void HideAllComments2()
    {
        StopAllCoroutines();
        commentPreviewActive = false;
        foreach (GameObject comment in commentList)
        {
            comment.SetActive(false);
        }
    }
    public void HideAllComments()
    {
        if(previewComment != null)
        {
            previewComment.SetActive(false);
            commentPreviewActive = false;
        }
        foreach (GameObject comment in commentList)
        {
            comment.SetActive(false);
        }
    }

    public void HideAllComments3()
    {
        StopAllCoroutines();
        commentPreviewActive = false;
        foreach (GameObject comment in commentList)
        {
            comment.SetActive(true);
        }
    }

    private IEnumerator StopCommentPreview(GameObject comment)
    {
        if(domeOfPointerInteractions.inputManager.currentCommentState == InputManager.CommentState.Text)
        {
            comment.SetActive(false);
        }
        previewComment = comment;
        commentPreviewActive = true;
        yield return new WaitForSeconds(15f);
        comment.SetActive(false);
        commentPreviewActive = false;
    }

    public void LoadCommentsforCurrentVideo(string currentpath)
    {
        commentsToLoad = 0;
        commentList.Clear();
        Transform contentTransform = content;
        foreach (Transform child in contentTransform)
        {
            GameObject.Destroy(child.gameObject);
        }
        if (!File.Exists(currentpath))
        {
            File.WriteAllText(currentpath, "");
        }
        string[] lines = File.ReadAllLines(currentpath);
         for(int i = 0; i<lines.Length;i++)
         {
            string currentLine = lines[i];
            string[] splitLine = currentLine.Split(',');
            float x = float.Parse(splitLine[0]);
            float y = float.Parse(splitLine[1]);
            float z = float.Parse(splitLine[2]);
            /*
            Debug.Log("x:" + splitLine[0]);
            Debug.Log("y:" + splitLine[1]);
            Debug.Log("z:" + splitLine[2]);
            Debug.Log("--------------");*/
            //x == transform.localPosition.x && y == transform.localPosition.y && z == transform.localPosition.z
            if (Mathf.Approximately(x, transform.localPosition.x) && Mathf.Approximately(y, transform.localPosition.y) && Mathf.Approximately(z, transform.localPosition.z))
            {               
                var newComment = Instantiate(commentInstancePrefab);
                newComment.GetComponentsInChildren<Text>()[1].text = splitLine[5];
                if(splitLine[5] == "")
                {
                    string pathAudio = path.Replace("DB.txt", "mySavedAudio");
                    StartCoroutine(LoadFile(pathAudio + splitLine[6] + ".wav", newComment));
                    commentsToLoad++;
                }
                else if(splitLine[5] != "")
                {
                    newComment.GetComponentsInChildren<Image>()[1].enabled = false;
                    Transform text = newComment.gameObject.transform.Find("TextToChange");
                    text.transform.localPosition = new Vector3(0,30,0);
                    newComment.gameObject.transform.Find("Image").gameObject.SetActive(false);
                }
               
             
                //Debug.Log("commentCargado:"+splitLine[5]);
                newComment.GetComponent<CommentInstance>().tiempoInicial = float.Parse(splitLine[3]);
                newComment.GetComponent<CommentInstance>().duración = float.Parse(splitLine[4]);
                newComment.transform.SetParent(contentTransform);
                newComment.transform.localScale = Vector3.one;
                newComment.transform.localRotation = Quaternion.identity;
                newComment.transform.localPosition = Vector3.zero;

                float time = Mathf.Floor(float.Parse(splitLine[3]));
                string minutes = (Mathf.Floor(time/60)).ToString();
                string seconds = (time - (Mathf.Floor(time / 60)) * 60).ToString();

                newComment.GetComponentsInChildren<Text>()[0].text = minutes + ":" + seconds;
                newComment.GetComponent<SphereCollider>().center = new Vector3(0, 0, 1100);
                commentList.Add(newComment);
            }
        }
        StartCoroutine("WaitForCommentsToLoad");
       
    }
    private IEnumerator WaitForCommentsToLoad()
    {
        yield return new WaitForSeconds(0.1f);
        bool allCommentsLoaded = false;
        while (!allCommentsLoaded)
        {
            if (commentsToLoad == 0)
            {
                allCommentsLoaded = true;
            }
            if ((commentsLoaded == commentsToLoad) && (commentsToLoad > 0))
            {
                allCommentsLoaded = true;
            }
        }

        StopAllCoroutines();

        commentsCoroutines.Clear();

        if(domeOfPointerInteractions.inputManager.currentGameState2 == InputManager.GameState.ViewMode) 
        { 
            foreach (GameObject comment in commentList)
            {
                //comment.GetComponent<CommentInstance>().SetActivationTimerForComment();
                IEnumerator e;
                e = StartTimer2(comment);
                StartCoroutine(e);
                commentsCoroutines.Add(e);

            }
        }
        else if(domeOfPointerInteractions.inputManager.currentGameState2 == InputManager.GameState.AllCommentsMode)
        {
            foreach (GameObject comment in commentList)
            {
                comment.SetActive(true);

            }
        }
        commentsLoaded = 0;
        commentsToLoad = 0;
    }

        private IEnumerator LoadFile(string fullpath, GameObject newComment)
    {

        print("LOADING CLIP " + fullpath);

        if (!System.IO.File.Exists(fullpath))
        {
            print("DIDN'T EXIST: " + fullpath);
            yield break;
        }

        temporalAudioClip = null;
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + fullpath, AudioType.WAV))
        {
            print("EL CLIP ACABO SIENDO NULL... 1");
            yield return www.SendWebRequest();
            print("EL CLIP ACABO SIENDO NULL... 2");
            if (www.isNetworkError)
            {
                Debug.Log(www.error);
                print("EL CLIP ACABO SIENDO NULL... 3");
            }
            else
            {
                print("EL CLIP ACABO SIENDO NULL... 4");
                temporalAudioClip = DownloadHandlerAudioClip.GetContent(www);
                if(temporalAudioClip == null)
                {
                    print("EL CLIP ACABO SIENDO NULL...5");
                }
                newComment.GetComponentInChildren<AudioSource>().clip = temporalAudioClip;
            }
        }

        commentsLoaded++;


    }
    public void PauseorUnpauseCommentsCorroutines()
    {
        /*
        if (inputManager.VideoManagerisPaused == true)
        {
            foreach (IEnumerator corrutina in commentsCoroutines)
            {

                StopCoroutine(corrutina);
            }
        }
        else
        {
            foreach (IEnumerator corrutina in commentsCoroutines)
            {
                StartCoroutine(corrutina);
            }
        }*/
    }
    private void SaveCommentToLog(GameObject comment, bool seguarda)
    {
        if (seguarda)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "");
            }
            else if (!(new FileInfo(path).Length == 0))
            {
                string space = "\n";
                File.AppendAllText(path, space);
            }
            string content = "";
            if (domeOfPointerInteractions.inputManager.currentCommentState == InputManager.CommentState.Microphone) 
            { 
                int newIndex = PlayerPrefs.GetInt("MaxVideoIndex", 0);
                PlayerPrefs.SetInt("MaxVideoIndex", newIndex + 1);
                string pathAudio = path.Replace("DB.txt", "");
                inputManager.SaveAudio(pathAudio, newIndex);
                content = transform.localPosition.x + "," + transform.localPosition.y + "," + transform.localPosition.z + "," + comment.GetComponent<CommentInstance>().tiempoInicial + "," + comment.GetComponent<CommentInstance>().duración + "," + comment.transform.Find("Text").GetComponent<Text>().text + "," + newIndex;
            }
            else if (domeOfPointerInteractions.inputManager.currentCommentState == InputManager.CommentState.Text)
            {
                content = transform.localPosition.x + "," + transform.localPosition.y + "," + transform.localPosition.z + "," + comment.GetComponent<CommentInstance>().tiempoInicial + "," + comment.GetComponent<CommentInstance>().duración + "," + comment.transform.Find("Text").GetComponent<Text>().text + "," + 0;
            }
            File.AppendAllText(path, content);
        }
    }
}
