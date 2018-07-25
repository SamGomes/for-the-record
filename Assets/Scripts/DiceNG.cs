using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDiceNG
{
    int RollTheDice(int diceNumbers);
}

public class RandomDiceNG : IDiceNG
{
    private System.Random random = new System.Random();

    public int RollTheDice(int diceNumbers)
    {
        return random.Next(1, diceNumbers + 1);
    }
}

public abstract class FixedDiceNG : IDiceNG
{
    protected System.Random random = new System.Random();
    protected int BadMarketRNG(int diceNumbers)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        return random.Next(Mathf.CeilToInt(currAlbum.GetValue() / 2), diceNumbers + 1);
    }
    protected int GoodMarketRNG(int diceNumbers)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        return random.Next(1, Mathf.CeilToInt(currAlbum.GetValue() / 2));
    }

    public abstract int RollTheDice(int diceNumbers);
}

public class AlbumVictoryDiceNG : FixedDiceNG
{
    public override int RollTheDice(int diceNumbers)
    {
        //trigger for market
        if (diceNumbers == 20)
        {
            if (GameGlobals.currGameRoundId < GameProperties.numberOfAlbumsPerGame - 1)
            {
                return (GameGlobals.currGameRoundId % 2 == 0) ? BadMarketRNG(diceNumbers) : GoodMarketRNG(diceNumbers);
            }
            else
            {
                return GoodMarketRNG(diceNumbers);
            }
        }
        else
        {
            return random.Next(1, diceNumbers + 1);
        }

    }
}
public class AlbumLossDiceNG : FixedDiceNG
{
    public override int RollTheDice(int diceNumbers)
    {
        //trigger for market
        if (diceNumbers == 20)
        {
            if (GameGlobals.currGameRoundId < GameProperties.numberOfAlbumsPerGame - 1)
            {
                return (GameGlobals.currGameRoundId % 2 == 0) ? BadMarketRNG(diceNumbers) : GoodMarketRNG(diceNumbers); ;
            }
            else
            {
                return BadMarketRNG(diceNumbers);
            }
        }
        else
        {
            return random.Next(1, diceNumbers + 1);
        }
    }
}