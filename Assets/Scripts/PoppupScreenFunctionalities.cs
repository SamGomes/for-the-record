using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PoppupScreenFunctionalities
{
    private Button UIcloseButton;
    private GameObject poppupInstance;
    private string audioPath;

    private Func<int> OnShow;
    private Func<int> OnHide;

    private MonoBehaviourFunctionalities monoBehaviourFunctionalities;

    private int StopAllAnimations()
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<Animator> mainSceneAnimators = new List<Animator>();
        for (int i=0; i< rootGameObjects.Length; i++)
        {
            GameObject root = rootGameObjects[i];
            mainSceneAnimators.AddRange(root.GetComponents<Animator>());
            mainSceneAnimators.AddRange(root.GetComponentsInChildren<Animator>());
        }

        for (int i = 0; i < mainSceneAnimators.Count; i++)
        {
            mainSceneAnimators[i].enabled = false;
        }
        return 0;
    }
    private int PlayAllAnimations()
    {
        GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        List<Animator> mainSceneAnimators = new List<Animator>();
        for (int i = 0; i < rootGameObjects.Length; i++)
        {
            GameObject root = rootGameObjects[i];
            mainSceneAnimators.AddRange(root.GetComponents<Animator>());
            mainSceneAnimators.AddRange(root.GetComponentsInChildren<Animator>());
        }

        for (int i = 0; i < mainSceneAnimators.Count; i++)
        {
            mainSceneAnimators[i].enabled = true;
        }
        return 0;
    }

    // Use this for initialization
    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, MonoBehaviourFunctionalities monoBehaviourFunctionalities, Sprite icon, Color backgroundColor)
    {
        poppupInstance = UnityEngine.Object.Instantiate(poppupPrefab, canvas.transform).gameObject;
        if (isGlobal)
        {
            UnityEngine.Object.DontDestroyOnLoad(canvas); //canvas is the one to call as it is the root of poppupInstance. Also do not despawn poppups
        }

        Image backround = poppupInstance.transform.Find("messageBackground").GetComponent<Image>();
        backround.color = backgroundColor;
        poppupInstance.transform.Find("icon").GetComponent<Image>().sprite = icon;

        this.UIcloseButton = poppupInstance.transform.Find("closeButton").GetComponent<Button>();

        this.monoBehaviourFunctionalities = monoBehaviourFunctionalities;

        this.OnHide = OnHide;
        this.OnShow = OnShow;

        if (OnShow != null)
        {
            this.AddOnShow(OnShow);
        }
        if(OnHide != null)
        { 
            this.AddOnHide(OnHide);
        }

        //avoid warnings
        OnHide = null;
        OnShow = null;

        HidePoppupPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            HidePoppupPanel();
        });

        this.audioPath = null;
    }

    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, MonoBehaviourFunctionalities monoBehaviourFunctionalities, Sprite icon, Color backgroundColor, string audioPath)
        : this(isGlobal, OnShow, OnHide, poppupPrefab, canvas, monoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.audioPath = audioPath;
    }
    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, MonoBehaviourFunctionalities monoBehaviourFunctionalities, Sprite icon, Color backgroundColor, System.Func<int> additionalCloseButtonFunctionalities)
        : this(isGlobal, OnShow, OnHide, poppupPrefab, canvas, monoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.UIcloseButton.onClick.AddListener(delegate () { additionalCloseButtonFunctionalities(); });
    }
    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, MonoBehaviourFunctionalities monoBehaviourFunctionalities, Sprite icon, Color backgroundColor, string audioPath, System.Func<int> additionalCloseButtonFunctionalities)
        : this(isGlobal, OnShow, OnHide, poppupPrefab, canvas, monoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.audioPath = audioPath;
        this.UIcloseButton.onClick.AddListener(delegate () { additionalCloseButtonFunctionalities(); });
    }

    public void AddOnShow(Func<int> OnShow)
    {
        this.OnShow = OnShow;
    }
    public void AddOnHide(Func<int> OnHide)
    {
        this.OnHide = OnHide;
        UIcloseButton.onClick.AddListener(delegate ()
        {
            OnHide();
        });
    }


    public void DestroyPoppupPanel()
    {
        UnityEngine.Object.Destroy(poppupInstance);
    }
    public void HidePoppupPanel()
    {
        poppupInstance.gameObject.SetActive(false);
        PlayAllAnimations();
    }
    public void DisplayPoppup(string text)
    {
        if (OnShow!=null)
        {
            OnShow();
        }
        poppupInstance.transform.Find("text").GetComponent<Text>().text = text;
        poppupInstance.SetActive(true);
        if (audioPath != null)
        {
            GameGlobals.audioManager.PlayClip(audioPath);
        }
        StopAllAnimations();
    }

    private IEnumerator DisplayPoppupWithDelayCoroutine(string text, float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayPoppup(text);
    }


    public void DisplayPoppupWithDelay(string text, float delay)
    {
        monoBehaviourFunctionalities.StartCoroutine(DisplayPoppupWithDelayCoroutine(text,delay));
    }
}
