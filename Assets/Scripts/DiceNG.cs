using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using Utilities;

public interface IDiceNG
{
    int RollTheDice(Player whoRollsTheDice, GameProperties.Instrument diceTarget, int diceNumbers, int rollOrderNumber, int currNumberOfRolls);
}

public class RandomDiceNG : IDiceNG
{
    private System.Random random = new System.Random();

    public int RollTheDice(Player whoRollsTheDice, GameProperties.Instrument diceTarget, int diceNumbers, int rollOrderNumber, int currNumberOfRolls)
    {
        return random.Next(1, diceNumbers + 1);
    }
}

public abstract class FixedDiceNG : IDiceNG
{

    //associate one player and one round to dice results (1 dice, 2 dices, etc. )
    private Dictionary<Player, Dictionary<GameProperties.Instrument, Dictionary<int, int[]>>> seriesForDices6;
    private int currTotalDiceValue;
    private List<int> currIndividualDiceValues;
    protected System.Random random = new System.Random();

    public FixedDiceNG(): base()
    {
        currIndividualDiceValues = new List<int>();

        //init dictionary
        seriesForDices6 = new Dictionary<Player, Dictionary<GameProperties.Instrument, Dictionary<int, int[]>>>();
        for (int i=0; i< GameGlobals.players.Count; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            seriesForDices6[currPlayer] = new Dictionary<GameProperties.Instrument, Dictionary<int, int[]>>();
            foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
            {
                seriesForDices6[currPlayer][instrument] = new Dictionary<int, int[]>();
                for (int j = 0; j < GameProperties.configurableProperties.numberOfAlbumsPerGame; j++)
                {
                    seriesForDices6[currPlayer][instrument][j] = new int[GameProperties.configurableProperties.numberOfAlbumsPerGame];
                }
            }
        }

        Player player1 = GameGlobals.players[0];
        Player player2 = GameGlobals.players[1];
        Player player3 = GameGlobals.players[2];

        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            if (instrument == GameProperties.Instrument.MARKETING)
            {
                break;
            }

            seriesForDices6[player1][instrument][0] = new int[] { 4 };
            seriesForDices6[player1][instrument][1] = new int[] { 2, 0 };
            seriesForDices6[player1][instrument][2] = new int[] { 3, 0, 0 };
            seriesForDices6[player1][instrument][3] = new int[] { 5, 0, 0, 0 };
            seriesForDices6[player1][instrument][4] = new int[] { 1, 0, 0, 0, 0 };

            seriesForDices6[player2][instrument][0] = new int[] { 2 }; // Av
            seriesForDices6[player2][instrument][1] = new int[] { 0, 10 }; // Avg: 6
            seriesForDices6[player2][instrument][2] = new int[] { 0, 0, 9 }; // Avg: 9
            seriesForDices6[player2][instrument][3] = new int[] { 0, 0, 0, 18 }; // Avg: 12
            seriesForDices6[player2][instrument][4] = new int[] { 0, 0, 0, 0, 21 }; // Avg: 15


            seriesForDices6[player3][instrument][0] = new int[] { 6 };
            seriesForDices6[player3][instrument][1] = new int[] { 4, 8 };
            seriesForDices6[player3][instrument][2] = new int[] { 3, 6, 13 };
            seriesForDices6[player3][instrument][3] = new int[] { 1, 9, 11, 15 };
            seriesForDices6[player3][instrument][4] = new int[] { 5, 10, 8, 13, 23 };
        }

        seriesForDices6[player1][GameProperties.Instrument.MARKETING][0] = new int[] { 0 };
        seriesForDices6[player1][GameProperties.Instrument.MARKETING][1] = new int[] { 5, 0 };
        seriesForDices6[player1][GameProperties.Instrument.MARKETING][2] = new int[] { 0, 0, 0 };
        seriesForDices6[player1][GameProperties.Instrument.MARKETING][3] = new int[] { 0, 0, 11, 0 };
        seriesForDices6[player1][GameProperties.Instrument.MARKETING][4] = new int[] { 0, 0, 0, 18, 0 }; // Total: 34k

        seriesForDices6[player2][GameProperties.Instrument.MARKETING][0] = new int[] { 0 };
        seriesForDices6[player2][GameProperties.Instrument.MARKETING][1] = new int[] { 0, 0 };
        seriesForDices6[player2][GameProperties.Instrument.MARKETING][2] = new int[] { 0, 0, 0 };
        seriesForDices6[player2][GameProperties.Instrument.MARKETING][3] = new int[] { 0, 0, 0, 0 };
        seriesForDices6[player2][GameProperties.Instrument.MARKETING][4] = new int[] { 0, 0, 0, 0, 0 };

        seriesForDices6[player3][GameProperties.Instrument.MARKETING][0] = new int[] { 0 };
        seriesForDices6[player3][GameProperties.Instrument.MARKETING][1] = new int[] { 4, 0 };
        seriesForDices6[player3][GameProperties.Instrument.MARKETING][2] = new int[] { 0, 0, 0 };
        seriesForDices6[player3][GameProperties.Instrument.MARKETING][3] = new int[] { 5, 7, 10, 0 };
        seriesForDices6[player3][GameProperties.Instrument.MARKETING][4] = new int[] { 2, 6, 11, 14, 0 }; //Max : 28K

}

    protected int BadMarketRNG(int diceNumbers, int currNumberOfRolls)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        return random.Next(Mathf.CeilToInt(currAlbum.GetValue() / currNumberOfRolls) + 1, diceNumbers + 1);
    }
    protected int GoodMarketRNG(int diceNumbers, int currNumberOfRolls)
    {
        Album currAlbum = GameGlobals.albums[GameGlobals.albums.Count - 1];
        return random.Next(1, Mathf.CeilToInt(currAlbum.GetValue() / currNumberOfRolls));
    }

    protected int RollDiceFor6(Player whoRollsTheDice, GameProperties.Instrument diceTarget, int diceNumbers, int rollOrderNumber, int currNumberOfRolls)
    {
        if (rollOrderNumber == 0) //first dart being launched
        {
            currTotalDiceValue = 0;
            currIndividualDiceValues.Clear();

            //achieve the right value
            int remainingPlayValue = seriesForDices6[whoRollsTheDice][diceTarget][GameGlobals.currGameRoundId][currNumberOfRolls-1];
            float decimalValueForOneDice = (float)remainingPlayValue / (float)currNumberOfRolls;
            int valueForOneDice = Mathf.FloorToInt(decimalValueForOneDice);

            for (int i=0; i< currNumberOfRolls; i++)
            {
                currIndividualDiceValues.Add(valueForOneDice);
            }

            int remainder = remainingPlayValue - (valueForOneDice * currNumberOfRolls);
            for (int i = 0; i < currNumberOfRolls; i++)
            {
                if (remainder == 0)
                {
                    break;
                }
                currIndividualDiceValues[i] += 1;
                remainder -= 1;
            }

            //randomize dices values
            //get increase
            for (int i = 0; i < currNumberOfRolls; i++)
            {
                if (currIndividualDiceValues[i] == 1)
                {
                    continue;
                }
                int randomDice = Random.Range(0, currNumberOfRolls);
                int remainderIncrease = (6 - currIndividualDiceValues[i]);
                remainderIncrease = (remainderIncrease < currIndividualDiceValues[randomDice]) ? remainderIncrease : currIndividualDiceValues[randomDice]-1;

                int randomDecrease = Random.Range(0, remainderIncrease+1);

                currIndividualDiceValues[randomDice] -= randomDecrease;
                currIndividualDiceValues[i] += randomDecrease;
            }


        }
        return currIndividualDiceValues[rollOrderNumber];
    }
    protected int RollDiceFor20(int diceNumbers, int rollOrderNumber, int currNumberOfRolls)
    {
        return (GameGlobals.currGameRoundId % 2 == 0) ? BadMarketRNG(diceNumbers, currNumberOfRolls) : GoodMarketRNG(diceNumbers, currNumberOfRolls);
    }

    public abstract int RollTheDice(Player whoRollsTheDice, GameProperties.Instrument diceTarget, int diceNumbers, int rollOrderNumber, int currNumberOfRolls);
}

public class VictoryDiceNG : FixedDiceNG
{
    public override int RollTheDice(Player whoRollsTheDice, GameProperties.Instrument diceTarget, int diceNumbers, int rollOrderNumber, int currNumberOfRolls)
    {
        //trigger for market
        if (diceNumbers == 20)
        {
            if (GameGlobals.currGameRoundId < GameProperties.configurableProperties.numberOfAlbumsPerGame - 1)
            {
                return RollDiceFor20(diceNumbers, rollOrderNumber, currNumberOfRolls);
            }
            else
            {
                return GoodMarketRNG(diceNumbers, currNumberOfRolls);
            }
        }
        else
        {
            return RollDiceFor6(whoRollsTheDice, diceTarget, diceNumbers, rollOrderNumber, currNumberOfRolls);
        }

    }
}
public class LossDiceNG : FixedDiceNG
{
    public override int RollTheDice(Player whoRollsTheDice, GameProperties.Instrument diceTarget, int diceNumbers, int rollOrderNumber, int currNumberOfRolls)
    {

        //trigger for market
        if (diceNumbers == 20)
        {
            if (GameGlobals.currGameRoundId < GameProperties.configurableProperties.numberOfAlbumsPerGame - 1)
            {
                return RollDiceFor20(diceNumbers, rollOrderNumber, currNumberOfRolls);
            }
            else
            {
                return BadMarketRNG(diceNumbers, currNumberOfRolls);
            }
        }
        else
        {
            return RollDiceFor6(whoRollsTheDice, diceTarget, diceNumbers, rollOrderNumber, currNumberOfRolls);
        }
    }
}