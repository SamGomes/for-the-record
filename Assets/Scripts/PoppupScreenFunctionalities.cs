using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PoppupScreenFunctionalities : MonoBehaviour
{
    public GameObject poppupContainer;

    public Button UIcloseButton;
    public Text UIpoppupText;

    // Use this for initialization
    void Start()
    {
        HidePoppupPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            HidePoppupPanel();
        });
    }
    public void HidePoppupPanel()
    {
        poppupContainer.SetActive(false);
    }
    public void DisplayPoppup(string text)
    {
        UIpoppupText.text = text;
        poppupContainer.SetActive(true);
    }

    private IEnumerator DisplayPoppupWithDelayCoroutine(string text, float delay)
    {
        yield return new WaitForSeconds(delay);
        DisplayPoppup(text);
    }

    public void DisplayPoppupWithDelay(string text, float delay)
    {
        StartCoroutine(DisplayPoppupWithDelayCoroutine(text,delay));
    }
}
