using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player{
    private string name;
    private int numTokens;
    private int money;
    private Dictionary<GameProperties.Instrument,int> skillSet;

	public Player(string name)
    {
        money = 0;
        numTokens = 2;
        skillSet = new Dictionary<GameProperties.Instrument, int>();
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

    public void ReceiveMoney(int money)
    {
        this.money += money;
    }
    public int GetMoney()
    {
        return this.money;
    }
    public Dictionary<GameProperties.Instrument, int> GetSkillSet()
    {
        return this.skillSet;
    }
}
