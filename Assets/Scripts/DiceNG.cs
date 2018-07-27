using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDiceNG
{
    int RollTheDice(int diceNumbers, int rollOrderNumber);
}

public class RandomDiceNG : IDiceNG
{
    private System.Random random = new System.Random();

    public int RollTheDice(int diceNumbers, int rollOrderNumber)
    {
        return random.Next(1, diceNumbers + 1);
    }
}

public abstract class FixedDiceNG : IDiceNG
{
    private int[,] seriesForDices6 = new int[,]{{ 2, 5, 6, 2, 1, 3 },{ 4, 3, 1, 2, 6, 2 },{ 3, 2, 1, 5, 6, 4 }};
    private int throwSeriesIndex;
    protected System.Random random = new System.Random();
    protected int BadMarketRNG(int diceNumbers)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        if (currAlbum.GetValue() >= 40)
        {
            return 20;
        }
        return random.Next(Mathf.CeilToInt(currAlbum.GetValue() / 2) + 1, diceNumbers + 1);
    }
    protected int GoodMarketRNG(int diceNumbers)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        return random.Next(1, Mathf.CeilToInt(currAlbum.GetValue() / 2) - 1);
    }

    protected int RollDiceFor6(int diceNumbers, int rollOrderNumber)
    {
        if (rollOrderNumber == 0) //first dart being launched
        { 
            throwSeriesIndex = random.Next(0, seriesForDices6.GetLength(1));
        }
        return seriesForDices6[rollOrderNumber, throwSeriesIndex];
    }
    protected int RollDiceFor20(int diceNumbers, int rollOrderNumber)
    {
        return (GameGlobals.currGameRoundId % 2 == 0) ? BadMarketRNG(diceNumbers) : GoodMarketRNG(diceNumbers);
    }

    public abstract int RollTheDice(int diceNumbers, int rollOrderNumber);
}

public class VictoryDiceNG : FixedDiceNG
{
    public override int RollTheDice(int diceNumbers, int rollOrderNumber)
    {
        //trigger for market
        if (diceNumbers == 20)
        {
            if (GameGlobals.currGameRoundId < GameProperties.numberOfAlbumsPerGame - 1)
            {
                return RollDiceFor20(diceNumbers, rollOrderNumber);
            }
            else
            {
                return GoodMarketRNG(diceNumbers);
            }
        }
        else
        {
            return RollDiceFor6(diceNumbers,rollOrderNumber);
        }

    }
}
public class LossDiceNG : FixedDiceNG
{
    public override int RollTheDice(int diceNumbers, int rollOrderNumber)
    {

        //trigger for market
        if (diceNumbers == 20)
        {
            if (GameGlobals.currGameRoundId < GameProperties.numberOfAlbumsPerGame - 1)
            {
                return RollDiceFor20(diceNumbers, rollOrderNumber);
            }
            else
            {
                return BadMarketRNG(diceNumbers);
            }
        }
        else
        {
            return RollDiceFor6(diceNumbers, rollOrderNumber);
        }
    }
}