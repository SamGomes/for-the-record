using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player
{
    private string name;
    private int numTokens;
    private int money;
    private GameProperties.Instrument preferredInstrument;
    private Dictionary<GameProperties.Instrument, int> skillSet;

    public Player(string name)
    {
        money = 0;
        numTokens = 0;
        preferredInstrument = GameProperties.Instrument.BASS;
        skillSet = new Dictionary<GameProperties.Instrument, int>();
    }

    //main method
    public abstract bool ExecuteActionRequest();

    //aux methods
    public void ChangePreferredInstrument(GameProperties.Instrument instrument)
    {
        this.preferredInstrument = instrument;
    }

    public bool SpendToken(GameProperties.Instrument instrument)
    {
        if (numTokens == 0)
        {
            return false;
        }

        numTokens--;
        if (skillSet.ContainsKey(instrument))
        {
            skillSet.Add(instrument, 1);
        }
        else
        {
            skillSet[instrument]++;
        }
        return true;
    }
    public bool ConvertTokensToMoney(int numTokensToConvert)
    {
        if (numTokens == 0)
        {
            return false;
        }

        numTokens-=numTokensToConvert;
        money += numTokensToConvert * GameProperties.tokenValue;

        return true;
    }

    public void ReceiveMoney(int moneyToReceive)
    {
        this.money += moneyToReceive;
    }
    public void ReceiveTokens(int numTokensToReceive)
    {
        this.numTokens += numTokensToReceive;
    }
    public int GetMoney()
    {
        return this.money;
    }
    public GameProperties.Instrument GetPreferredInstrument()
    {
        return this.preferredInstrument;
    }
    public Dictionary<GameProperties.Instrument, int> GetSkillSet()
    {
        return this.skillSet;
    }
}


public class GreedyPlayer : Player {

    public GreedyPlayer(string name) : base(name) { }

    public override bool ExecuteActionRequest()
    {
        return true;
    }
}