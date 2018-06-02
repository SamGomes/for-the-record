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
        this.marketingState = GameProperties.AlbumMarketingState.NON_PUBLISHED;
        this.name = name;
        this.value = 0;
        instrumentValues = new Dictionary<GameProperties.Instrument, int>();

        //add values to the dictionary
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            instrumentValues[instrument] = 0;
        }

        this.albumUI = Object.Instantiate(albumUIPrefab, canvas.transform);
        this.UInameText = albumUI.transform.Find("albumName").GetComponent<Text>();
        this.UIvalueText = albumUI.transform.Find("albumValue").GetComponent<Text>();
        this.UImarketingStateText = albumUI.transform.Find("albumMarketingState").GetComponent<Text>();
    }

    public GameProperties.AlbumMarketingState GetMarketingState()
    {
        return this.marketingState;
    }

    public int GetAlbumValue()
    {
        return this.value;
    }
    public GameObject GetAlbumUI()
    {
        return this.albumUI;
    }

    public void SetInstrumentValue(GameProperties.Instrument instrument, int value)
    {
        int valueDiff = value - instrumentValues[instrument];
        this.value += valueDiff;
        instrumentValues[instrument] = value;
    }
    public void SetMarketingState(GameProperties.AlbumMarketingState marketingState)
    {
        this.marketingState = marketingState;
    }

}
