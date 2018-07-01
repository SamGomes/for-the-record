using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public abstract class Player
{
    private int id;

    private string actionLog;

    protected GameManager gameManagerRef;


    protected string name;
    protected int numTokens;
    protected int money;

    protected GameProperties.Instrument diceRollInstrument;
    protected GameProperties.Instrument toBeTokenedInstrument;

    protected List<GameProperties.Instrument> lastLeveledUpInstruments;
    private List<GameProperties.Instrument> currLeveledUpInstruments;

    protected Dictionary<GameProperties.Instrument, int> skillSet;
    protected Dictionary<GameProperties.Instrument, int> albumContributions;

    public int tokensBoughtOnCurrRound;
   
    public Player(string name)
    {

        this.id = GameGlobals.playerIdCount++;
        this.name = name;

        this.tokensBoughtOnCurrRound = 0;

        this.money = 0;
        this.numTokens = 0;

        this.lastLeveledUpInstruments = new List<GameProperties.Instrument>();
        this.currLeveledUpInstruments = new List<GameProperties.Instrument>();

        this.diceRollInstrument = GameProperties.Instrument.GUITAR;
        this.toBeTokenedInstrument = GameProperties.Instrument.GUITAR;
        this.skillSet = new Dictionary<GameProperties.Instrument, int>();
        this.albumContributions = new Dictionary<GameProperties.Instrument, int>();

        //add values to the dictionary
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            skillSet[instrument] = 0;
            albumContributions[instrument] = 0;
        }
    }

    public void InitGameData()
    {
        this.gameManagerRef = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();
    }

    public abstract void LevelUp();
    public abstract void PlayForInstrument();
    public abstract void LastDecisionsPhase(Album currAlbum);

    public int GetId()
    {
        return this.id;
    }
    public string GetName()
    {
        return this.name;
    }

    public void LevelUpRequest()
    {
        LevelUp();
    }
    public void PlayForInstrumentRequest()
    {
        PlayForInstrument();
    }
    public void LastDecisionsPhaseRequest(Album currAlbum)
    {
        LastDecisionsPhase(currAlbum);
    }


    public void SendLevelUpResponse()
    {
        lastLeveledUpInstruments = currLeveledUpInstruments;
        currLeveledUpInstruments = new List<GameProperties.Instrument>();
        gameManagerRef.LevelUpResponse(this);
    }
    public void SendPlayForInstrumentResponse()
    {
        gameManagerRef.PlayerPlayForInstrumentResponse(this);
    }
    public void SendLastDecisionsPhaseResponse(int condition)
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
    }

    public void ChangeDiceRollInstrument(GameProperties.Instrument instrument)
    {
        this.diceRollInstrument = instrument;
    }
    public void ChangeToBeTokenedInstrument(GameProperties.Instrument instrument)
    {
        this.toBeTokenedInstrument = instrument;
    }

    public bool SpendToken(GameProperties.Instrument instrument)
    {
        //cannot spend token on last increased instruments
        if (numTokens == 0) 
        {
            Debug.Log("You have no more tokens to level up your skills!");
            return false;
        }else if (lastLeveledUpInstruments.Contains(instrument))
        {
            Debug.Log("You cannot develop the same skill on two consecutive albums!");
            return false;
        }

        numTokens--;
        skillSet[instrument]++;

        currLeveledUpInstruments.Add(instrument);

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


