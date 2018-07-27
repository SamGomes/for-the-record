using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Album {

    private int id;

    private int value;
    private string name;
    private Dictionary<GameProperties.Instrument, int> instrumentValues;

    private GameProperties.AlbumMarketingState marketingState;

    private GameObject albumUI;
    private Text UInameText;
    private GameObject UIinstrumentValuesTable;
    private Text UIvalueText;
    private Text UImarketingStateText;

    private Button UIbackgroundImageEventSystem;

    public Album(string name, GameObject albumUIPrefab)
    {

        this.id = GameGlobals.albumIdCount++;

        this.albumUI = Object.Instantiate(albumUIPrefab);

        this.UIbackgroundImageEventSystem = albumUI.transform.Find("button").GetComponent<Button>();

        //make the album ui selectable
        UIbackgroundImageEventSystem.onClick.AddListener(delegate () {
            Debug.Log("selected?");
            this.albumUI.transform.SetAsLastSibling();
        });

        this.UInameText = albumUI.transform.Find("DelayedElements/elements/albumName").GetComponent<Text>();
        this.UIvalueText = albumUI.transform.Find("DelayedElements/elements/valueElements/valueDisplay/albumValueText").GetComponent<Text>();
        this.UImarketingStateText = albumUI.transform.Find("DelayedElements/elements/marketingStateElements/albumMarketingStateText").GetComponent<Text>();

        this.UIinstrumentValuesTable = albumUI.transform.Find("DelayedElements/elements/valueElements/skillLevels").gameObject;


        this.albumUI.transform.Find("DelayedElements/elements/backgroundOverride").GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures\\AlbumNON_PUBOverlay");

        this.marketingState = GameProperties.AlbumMarketingState.NON_PUBLISHED;
        UImarketingStateText.text = marketingState.ToString();

        this.name = name;
        this.UInameText.text = name;

        this.value = 0;
        this.UIvalueText.text = value.ToString();

        instrumentValues = new Dictionary<GameProperties.Instrument, int>();

        //add values to the dictionary
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            if (instrument == GameProperties.Instrument.NONE || instrument == GameProperties.Instrument.MARKETING)
            {
                continue;
            }
            instrumentValues[instrument] = 0;
            UIinstrumentValuesTable.GetComponentsInChildren<Text>()[(int)instrument].text = "0";
        }
    }


    public int GetValue()
    {
        return this.value;
    }
    public string GetName()
    {
        return this.name;
    }
    public int GetId()
    {
        return this.id;
    }



    public GameProperties.AlbumMarketingState GetMarketingState()
    {
        return this.marketingState;
    }

    public int CalcAlbumValue()
    {
        this.value = 0;
        foreach (GameProperties.Instrument instrument in instrumentValues.Keys)
        {
            this.value += instrumentValues[instrument];
        }
        UIvalueText.text = value.ToString();
        foreach (Animator animator in this.albumUI.GetComponentsInChildren<Animator>()) {
            animator.Rebind();
            animator.Play(0); //play animation when calculating new value
        }
        return this.value;
    }
    public GameObject GetAlbumUI()
    {
        return this.albumUI;
    }

    public void SetInstrumentValue(GameProperties.Instrument instrument, int value)
    {
        instrumentValues[instrument] = value;
        UIinstrumentValuesTable.GetComponentsInChildren<Text>()[(int)instrument].text = value.ToString();
        this.value = CalcAlbumValue();
    }
    public void SetMarketingState(GameProperties.AlbumMarketingState marketingState)
    {
        if (marketingState == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            this.albumUI.transform.Find("DelayedElements/elements/backgroundOverride").GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures\\AlbumMEGA_HITOverlay");
        }
        else if(marketingState == GameProperties.AlbumMarketingState.FAIL)
        {
            this.albumUI.transform.Find("DelayedElements/elements/backgroundOverride").GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures\\AlbumFAILOverlay");
        }
        this.marketingState = marketingState;
        UImarketingStateText.text = marketingState.ToString();
    }
    

}
