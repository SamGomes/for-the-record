using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoppupScreenFunctionalities
{
    private Button UIcloseButton;
    private GameObject poppupInstance;
    private string audioPath;

    private PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities;

    // Use this for initialization
    public PoppupScreenFunctionalities(GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor)
    {
        poppupInstance = Object.Instantiate(poppupPrefab, canvas.transform);
        Image backround = poppupInstance.transform.Find("messageBackground").GetComponent<Image>();
        backround.color = backgroundColor;
        poppupInstance.transform.Find("icon").GetComponent<Image>().sprite = icon;

        this.UIcloseButton = poppupInstance.transform.Find("closeButton").GetComponent<Button>();

        this.playerMonoBehaviourFunctionalities = playerMonoBehaviourFunctionalities;

        HidePoppupPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            GameGlobals.gameManager.ContinueGame();
            HidePoppupPanel();
        });

        this.audioPath = null;
    }

    public PoppupScreenFunctionalities(GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor, string audioPath)
        : this(poppupPrefab, canvas, playerMonoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.audioPath = audioPath;
    }
    public PoppupScreenFunctionalities(GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor, System.Func<int> additionalCloseButtonFunctionalities)
        : this(poppupPrefab, canvas, playerMonoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.UIcloseButton.onClick.AddListener(delegate () { additionalCloseButtonFunctionalities(); });
    }
    public PoppupScreenFunctionalities(GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor, string audioPath, System.Func<int> additionalCloseButtonFunctionalities)
        : this(poppupPrefab, canvas, playerMonoBehaviourFunctionalities, icon, backgroundColor)
    {
        this.audioPath = audioPath;
        this.UIcloseButton.onClick.AddListener(delegate () { additionalCloseButtonFunctionalities(); });
    }


    public void DestroyPoppupPanel()
    {
        Object.Destroy(poppupInstance);
    }
    public void HidePoppupPanel()
    {
        poppupInstance.gameObject.SetActive(false);
    }
    public void DisplayPoppup(string text)
    {
        GameGlobals.gameManager.InterruptGame();
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
