using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Album{

    private int value;
    private string name;
    private Dictionary<GameProperties.Instrument, int> instrumentValues;

    private GameProperties.AlbumMarketingState marketingState;

    private GameObject albumUI;
    private Text UInameText;
    private Text UIvalueText;
    private Text UImarketingStateText;

    public Album(string name, GameObject albumUIPrefab, GameObject canvas)
    {
        this.albumUI = Object.Instantiate(albumUIPrefab, canvas.transform);
        this.UInameText = albumUI.transform.Find("albumName").GetComponent<Text>();
        this.UIvalueText = albumUI.transform.Find("albumValueText").GetComponent<Text>();
        this.UImarketingStateText = albumUI.transform.Find("albumMarketingStateText").GetComponent<Text>();

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
            instrumentValues[instrument] = 0;
        }

    }

    public GameProperties.AlbumMarketingState GetMarketingState()
    {
        return this.marketingState;
    }

    public int CalcAlbumValue()
    {
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            this.value += instrumentValues[instrument];
        }
        UIvalueText.text = value.ToString();
        return this.value;
    }
    public GameObject GetAlbumUI()
    {
        return this.albumUI;
    }

    public void SetInstrumentValue(GameProperties.Instrument instrument, int value)
    {
        instrumentValues[instrument] = value;
    }
    public void SetMarketingState(GameProperties.AlbumMarketingState marketingState)
    {
        if (marketingState == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            this.albumUI.transform.GetChild(0).GetComponent<Image>().color = Color.green;
        }
        else if(marketingState == GameProperties.AlbumMarketingState.FAIL)
        {
            this.albumUI.transform.GetChild(0).GetComponent<Image>().color = Color.red;
        }
        this.marketingState = marketingState;
        UImarketingStateText.text = marketingState.ToString();
    }

}
