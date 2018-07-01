using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningScreenFunctionalities : MonoBehaviour
{
    public Button UIcloseButton;
    public Text UIwarningText;

    // Use this for initialization
    void Start()
    {
        HideWarningPanel();
        UIcloseButton.onClick.AddListener(delegate ()
        {
            HideWarningPanel();
        });
    }
    public void HideWarningPanel()
    {
        this.gameObject.SetActive(false);
    }
    public void DisplayWarning(string text)
    {
        UIwarningText.text = text;
        this.gameObject.SetActive(true);
    }
}
