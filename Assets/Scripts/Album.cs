using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Album{

    private int value;
    private string name;
    private Dictionary<GameProperties.Instrument, int> instrumentValues;

    private GameProperties.AlbumMarketingState marketingState;
    
    public Album(string name)
    {
        this.marketingState = GameProperties.AlbumMarketingState.NON_PUBLISHED;
        this.name = name;
    }
    
    public GameProperties.AlbumMarketingState GetMarketingState()
    {
        return this.marketingState;
    }

    public int GetAlbumValue()
    {
        return this.value;
    }
    public void SetInstrumentValue(GameProperties.Instrument instrument, int value)
    {
        int valueDiff = value - this.instrumentValues[instrument];
        this.value += valueDiff;
        this.instrumentValues[instrument]=value;
    }
    public void SetMarketingState(GameProperties.AlbumMarketingState marketingState)
    {
        this.marketingState = marketingState;
    }

}
