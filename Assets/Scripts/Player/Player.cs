using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public abstract class Player
{
    protected PlayerMonoBehaviourFunctionalities playerMonoBehaviourFunctionalities;

    protected int id;
    private string actionLog;
    protected GameManager gameManagerRef;

    protected string name;

    protected int numTokens;
    protected int money;

    protected GameProperties.Instrument diceRollInstrument;
    protected List<GameProperties.Instrument> toBeTokenedInstruments;

    protected Dictionary<GameProperties.Instrument, int> skillSet;
    protected Dictionary<GameProperties.Instrument, int> albumContributions;

    protected int unchangedMoney;
    protected int unchangedNumTokens;
    protected Dictionary<GameProperties.Instrument, int> unchangedSkillSetInstruments;

    public int tokensBoughtOnCurrRound;
   
    public Player(string name)
    {
        this.id = GameGlobals.playerIdCount++;
        this.name = name;

        this.tokensBoughtOnCurrRound = 0;

        this.money = 0;
        this.numTokens = 0;

        this.diceRollInstrument = GameProperties.Instrument.NONE;
        this.toBeTokenedInstruments = new List<GameProperties.Instrument>();
        this.skillSet = new Dictionary<GameProperties.Instrument, int>();
        this.albumContributions = new Dictionary<GameProperties.Instrument, int>();

        //add values to the dictionary
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            if (instrument == GameProperties.Instrument.NONE)
            {
                continue;
            }
            skillSet[instrument] = 0;
            albumContributions[instrument] = 0;
        }

    }

    public void InitGameData()
    {
        this.gameManagerRef = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
        this.playerMonoBehaviourFunctionalities = gameManagerRef.GetComponent<PlayerMonoBehaviourFunctionalities>();
    }

    public abstract void LevelUp(Album currAlbum);
    public abstract void PlayForInstrument(Album currAlbum);
    public abstract void LastDecisionsPhase(Album currAlbum);


    public int GetId()
    {
        return this.id;
    }
    public string GetName()
    {
        return this.name;
    }

    public void LevelUpRequest(Album currAlbum)
    {
        //save player state before changes
        unchangedSkillSetInstruments = new Dictionary<GameProperties.Instrument, int>();
        unchangedMoney = money;
        unchangedNumTokens = numTokens;

        LevelUp(currAlbum);
    }
    public void PlayForInstrumentRequest(Album currAlbum)
    {
        tokensBoughtOnCurrRound = 0; //reset tokens bought on this round to 0
        PlayForInstrument(currAlbum);
    }
    public void LastDecisionsPhaseRequest(Album currAlbum)
    {
        LastDecisionsPhase(currAlbum);
    }


    public bool SendLevelUpResponse()
    {
        if (numTokens != 0)
        {
            return false;
        }

        //update player state saves
        unchangedSkillSetInstruments = skillSet;
        unchangedMoney = money;
        unchangedNumTokens = numTokens;

        gameManagerRef.LevelUpResponse(this);
        return true;
    }
    public bool SendPlayForInstrumentResponse()
    {
        gameManagerRef.PlayerPlayForInstrumentResponse(this);
        return true;
    }
    public bool SendLastDecisionsPhaseResponse(int condition)
    {
        switch (condition)
        {
            case 0:
                gameManagerRef.LastDecisionsPhaseGet3000Response(this);
                break;
            case 1:
                gameManagerRef.LastDecisionsPhaseGetMarktingResponse(this);
                break;
            case 2:
                gameManagerRef.LastDecisionsPhaseGet1000Response(this);
                break;
        }
        return true;
    }

    public bool ChangeDiceRollInstrument(GameProperties.Instrument instrument)
    {
        if (skillSet[instrument] == 0 || instrument == GameProperties.Instrument.MARKETING)
        {
            return false;
        }

        this.diceRollInstrument = instrument;
        return true;
    }
    public void AddToBeTokenedInstrument(GameProperties.Instrument instrument)
    {
        this.toBeTokenedInstruments.Add(instrument);
    }
    public void RemoveToBeTokenedInstrument(GameProperties.Instrument instrument)
    {
        this.toBeTokenedInstruments.Remove(instrument);
    }

    public bool SpendToken(GameProperties.Instrument instrument)
    {
        //cannot spend token on last increased instruments
        if (numTokens == 0) 
        {
            Debug.Log("You have no more tokens to level up your skills!");
            return false;
        }else if (skillSet[instrument] == GameProperties.maximumSkillLevelPerInstrument)
        {
            Debug.Log("You cannot develop the same skill more than "+ GameProperties.maximumSkillLevelPerInstrument  + " times!");
            return false;
        }

        numTokens--;
        if (!unchangedSkillSetInstruments.ContainsKey(instrument))
        {
            unchangedSkillSetInstruments[instrument] = skillSet[instrument];
        }
        skillSet[instrument]++;

        FileManager.WritePlayerActionToLog(GameGlobals.currGameId.ToString(), gameManagerRef.GetCurrGameRound().ToString(), this.id.ToString(), this.name,"SPENT_TOKEN", instrument.ToString() , "-");
        return true;
    }
    public bool ConvertTokensToMoney(int numTokensToConvert)
    {
        if (numTokens == 0)
        {
            Debug.Log("You have no more tokens to convert!");
            return false;
        }

        numTokens-=numTokensToConvert;
        money += numTokensToConvert * GameProperties.tokenValue;

        FileManager.WritePlayerActionToLog(GameGlobals.currGameId.ToString(), gameManagerRef.GetCurrGameRound().ToString(), this.id.ToString(), this.name,"CONVERTED_TOKENS", "-" , numTokensToConvert.ToString());
        return true;
    }
    public bool BuyTokens(int numTokensToBuy)
    {
        int moneyToSpend = numTokensToBuy * GameProperties.tokenValue;

        if (tokensBoughtOnCurrRound >= GameProperties.allowedPlayerTokenBuysPerRound)
        {
            Debug.Log("You can only convert money to one token per round!");
            return false;
        }

        if (money < moneyToSpend)
        {
            Debug.Log("You have no money to convert!");
            return false;
        }

        money -= moneyToSpend;
        numTokens += numTokensToBuy;

        tokensBoughtOnCurrRound+=numTokensToBuy;
        FileManager.WritePlayerActionToLog(GameGlobals.currGameId.ToString(), gameManagerRef.GetCurrGameRound().ToString(), this.id.ToString(), this.name,"BOUGHT_TOKENS", "-" , numTokensToBuy.ToString());
        return true;
    }
    public void RollBackChangesToPhaseStart()
    {
        foreach(GameProperties.Instrument skill in unchangedSkillSetInstruments.Keys)
        {
            skillSet[skill] = unchangedSkillSetInstruments[skill];
        }
        money = unchangedMoney;
        numTokens = unchangedNumTokens;
        tokensBoughtOnCurrRound = 0;
    }

    public void ReceiveMoney(int moneyToReceive)
    {
        this.money += moneyToReceive;
        FileManager.WritePlayerActionToLog(GameGlobals.currGameId.ToString(), gameManagerRef.GetCurrGameRound().ToString(), this.id.ToString(), this.name,"RECEIVED_MONEY", "-" , moneyToReceive.ToString());
    }
    public void ReceiveTokens(int numTokensToReceive)
    {
        this.numTokens += numTokensToReceive;
    }
    public int GetMoney()
    {
        return this.money;
    }
    public GameProperties.Instrument GetDiceRollInstrument()
    {
        return this.diceRollInstrument;
    }


    public Dictionary<GameProperties.Instrument, int> GetSkillSet()
    {
        return this.skillSet;
    }

    public void SetAlbumContributions(Dictionary<GameProperties.Instrument, int>  albumContributions)
    {
        this.albumContributions = albumContributions;
    }
    public void InitAlbumContributions()
    {
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            albumContributions[instrument] = 0;
        }
    }
    public void SetAlbumContribution(GameProperties.Instrument instrument, int value)
    {
        this.albumContributions[instrument] = value;
        FileManager.WritePlayerActionToLog(GameGlobals.currGameId.ToString(), gameManagerRef.GetCurrGameRound().ToString(), this.id.ToString(), this.name,"INSTRUMENT_VALUE_CHANGED", instrument.ToString(), value.ToString());
    }
    public Dictionary<GameProperties.Instrument, int> GetAlbumContributions()
    {
        return this.albumContributions;
    }


    public void WriteToActionLog(string newAtomicContent)
    {
        actionLog += "\n"+newAtomicContent;
    }
    public string GetActionLog()
    {
        return this.actionLog;
    }

}


