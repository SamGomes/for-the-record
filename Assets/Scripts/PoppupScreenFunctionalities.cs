using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoppupScreenFunctionalities
{
    private Button UIcloseButton;
    private GameObject poppupInstance;
    private string audioPath;

    private Func<int> OnShow;
    private Func<int> OnHide;

    private PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities;

    // Use this for initialization
    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor)
    {
        poppupInstance = UnityEngine.Object.Instantiate(poppupPrefab, canvas.transform);
        if (isGlobal)
        {
            UnityEngine.Object.DontDestroyOnLoad(poppupInstance); //do not despawn poppups
        }

        Image backround = poppupInstance.transform.Find("messageBackground").GetComponent<Image>();
        backround.color = backgroundColor;
        poppupInstance.transform.Find("icon").GetComponent<Image>().sprite = icon;

        this.UIcloseButton = poppupInstance.transform.Find("closeButton").GetComponent<Button>();

        this.playerMonoBehaviourFunctionalities = playerMonoBehaviourFunctionalities;

        this.OnHide = OnHide;
        this.OnShow = OnShow;

        HidePoppupPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            if (OnHide != null)
            {
                OnHide();
            }
            //GameGlobals.gameManager.ContinueGame();
            HidePoppupPanel();
        });

        this.audioPath = null;
    }

    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor, string audioPath)
        : this(isGlobal, OnShow, OnHide, poppupPrefab, canvas, playerMonoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.audioPath = audioPath;
    }
    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor, System.Func<int> additionalCloseButtonFunctionalities)
        : this(isGlobal, OnShow, OnHide, poppupPrefab, canvas, playerMonoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.UIcloseButton.onClick.AddListener(delegate () { additionalCloseButtonFunctionalities(); });
    }
    public PoppupScreenFunctionalities(bool isGlobal, Func<int> OnShow, Func<int> OnHide, GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor, string audioPath, System.Func<int> additionalCloseButtonFunctionalities)
        : this(isGlobal, OnShow, OnHide, poppupPrefab, canvas, playerMonoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.audioPath = audioPath;
        this.UIcloseButton.onClick.AddListener(delegate () { additionalCloseButtonFunctionalities(); });
    }

    public void SetOnShow(Func<int> OnShow)
    {
        this.OnShow = OnShow;
    }
    public void SetOnHide(Func<int> OnHide)
    {
        this.OnHide = OnHide;
    }


    public void DestroyPoppupPanel()
    {
        UnityEngine.Object.Destroy(poppupInstance);
    }
    public void HidePoppupPanel()
    {
        poppupInstance.gameObject.SetActive(false);
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
    }

    private IEnumerator DisplayPoppupWithDelayCoroutine(string text, float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayPoppup(text);
    }


    public void DisplayPoppupWithDelay(string text, float delay)
    {
        playerMonoBehaviourFunctionalities.StartCoroutine(DisplayPoppupWithDelayCoroutine(text,delay));
    }
}
