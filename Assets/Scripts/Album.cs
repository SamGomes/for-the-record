using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Album{

    private string name;
    private int value;

    private GameProperties.AlbumMarketingState marketingState;
    
    public Album(string name)
    {
        this.marketingState = GameProperties.AlbumMarketingState.NON_PUBLISHED;
        this.name = name;
    }

    public int GetValue()
    {
        return this.value;
    }
    public GameProperties.AlbumMarketingState GetMarketingState()
    {
        return this.marketingState;
    }

    public void SetValue(int value)
    {
        this.value = value;
    }
    public void SetMarketingState(GameProperties.AlbumMarketingState marketingState)
    {
        this.marketingState = marketingState;
    }

}
