using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoppupScreenFunctionalities
{
    private GameObject poppupInstance;

    private PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities;

    // Use this for initialization
    public PoppupScreenFunctionalities(GameObject poppupPrefab, GameObject canvas, PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, Sprite icon, Color backgroundColor)
    {
        poppupInstance = Object.Instantiate(poppupPrefab, canvas.transform);


        Image backround = poppupInstance.transform.Find("messageBackground").GetComponent<Image>();
        backround.color = backgroundColor;
        poppupInstance.transform.Find("icon").GetComponent<Image>().sprite = icon;

        Button UIcloseButton = poppupInstance.transform.Find("closeButton").GetComponent<Button>();

        this.playerMonoBehaviourFunctionalities = playerMonoBehaviourFunctionalities;

        HidePoppupPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            HidePoppupPanel();
        });
    }
    public void HidePoppupPanel()
    {
        poppupInstance.SetActive(false);
    }
    public void DisplayPoppup(string text)
    {
        poppupInstance.transform.Find("text").GetComponent<Text>().text = text;
        poppupInstance.SetActive(true);
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
