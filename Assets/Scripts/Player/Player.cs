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

    protected GameProperties.Instrument preferredInstrument;
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
        this.preferredInstrument = GameProperties.Instrument.NONE;
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

    public abstract void RegisterMeOnPlayersLog();

    public virtual void InitPlayer(params object[] args)
    {
        this.gameManagerRef = GameGlobals.gameManager;
        this.playerMonoBehaviourFunctionalities = gameManagerRef.GetComponent<PlayerMonoBehaviourFunctionalities>();
        RegisterMeOnPlayersLog();
    }
    
    public abstract void ResetPlayer(params object[] args);

    public abstract void ChoosePreferredInstrument(Album currAlbum);
    public abstract void LevelUp(Album currAlbum);
    public abstract void PlayForInstrument(Album currAlbum);
    public abstract void LastDecisionsPhase(Album currAlbum);

    public virtual void InformRollDicesValue(Player invoker, int maxValue, int obtainedValue) { }
    public virtual void InformAlbumResult(int albumValue, int marketValue) { }


    public int GetId()
    {
        return this.id;
    }
    public string GetName()
    {
        return this.name;
    }

    public void ChoosePreferredInstrumentRequest(Album currAlbum)
    {
        //save player state before changes
        unchangedSkillSetInstruments = new Dictionary<GameProperties.Instrument, int>();
        unchangedMoney = money;
        unchangedNumTokens = numTokens;

        this.preferredInstrument = GameProperties.Instrument.NONE;
        ChoosePreferredInstrument(currAlbum);
    }
    public void LevelUpRequest(Album currAlbum)
    {
        LevelUp(currAlbum);
    }
    public void PlayForInstrumentRequest(Album currAlbum)
    {
        tokensBoughtOnCurrRound = 0; //reset tokens bought on this round to 0
        ChangeDiceRollInstrument(GameProperties.Instrument.NONE);
        PlayForInstrument(currAlbum);
    }
    public void LastDecisionsPhaseRequest(Album currAlbum)
    {
        LastDecisionsPhase(currAlbum);
    }

    public virtual int SendChoosePreferredInstrumentResponse()
    {
        if (preferredInstrument == GameProperties.Instrument.NONE)
        {
            Debug.Log("No preferred instrumet selected!");
            return 1;
        }
        gameManagerRef.ChoosePreferredInstrumentResponse(this);
        return 0;
    }
    public virtual int SendLevelUpResponse()
    {
        if (numTokens != 0)
        {
            return 1;
        }

        //update player state saves
        unchangedSkillSetInstruments = skillSet;
        unchangedMoney = money;
        unchangedNumTokens = numTokens;

        gameManagerRef.LevelUpResponse(this);
        return 0;
    }
    public virtual int SendPlayForInstrumentResponse()
    {
        gameManagerRef.PlayerPlayForInstrumentResponse(this);
        return 0;
    }
    public virtual int SendLastDecisionsPhaseResponse(int condition)
    {
        switch (condition)
        {
            case 0:
                gameManagerRef.LastDecisionsPhaseGet3000Response(this);
                break;
            case 1:
                gameManagerRef.LastDecisionsPhaseGetMarketingResponse(this);
                break;
            case 2:
                gameManagerRef.LastDecisionsPhaseGet1000Response(this);
                break;
        }
        return 0;
    }

    public virtual int ChangePreferredInstrument(GameProperties.Instrument instrument) //returns error ids
    {
        if(instrument == GameProperties.Instrument.MARKETING)
        {
            return 1;
        }
        //check if other players have the same preferred instrument
        foreach (Player player in GameGlobals.players)
        {
            if (player == this)
            {
                continue;
            }
            if(player.preferredInstrument == instrument)
            {
                return 2;
            }
        }
        this.preferredInstrument = instrument;
        return 0;
    }
    public virtual int ChangeDiceRollInstrument(GameProperties.Instrument instrument)
    {
        if (instrument == GameProperties.Instrument.MARKETING)
        {
            Debug.Log("The dices for marketing are rolled only after knowing the album result.");
            return 1;
        }
        else if (instrument != GameProperties.Instrument.NONE && skillSet[instrument] == 0)
        {
            Debug.Log("You cannot roll dices for an unevolved skill!");
            return 2;
        }
        this.diceRollInstrument = instrument;
        return 0;
    }

    public GameProperties.Instrument GetDiceRollInstrument()
    {
        return this.diceRollInstrument;
    }
    public GameProperties.Instrument GetPreferredInstrument()
    {
        return this.preferredInstrument;
    }
    public void AddToBeTokenedInstrument(GameProperties.Instrument instrument)
    {
        this.toBeTokenedInstruments.Add(instrument);
    }
    public void RemoveToBeTokenedInstrument(GameProperties.Instrument instrument)
    {
        this.toBeTokenedInstruments.Remove(instrument);
    }

    public virtual int SpendToken(GameProperties.Instrument instrument)
    {
        //cannot spend token on last increased instruments
        if (numTokens == 0) 
        {
            Debug.Log("You have no more tokens to level up your skills!");
            return 1;
        }else if (skillSet[instrument] == GameProperties.maximumSkillLevelPerInstrument)
        {
            Debug.Log("You cannot develop the same skill more than "+ GameProperties.maximumSkillLevelPerInstrument  + " times!");
            return 2;
        }

        numTokens--;
        if (!unchangedSkillSetInstruments.ContainsKey(instrument))
        {
            unchangedSkillSetInstruments[instrument] = skillSet[instrument];
        }
        skillSet[instrument]++;

        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), this.id.ToString(), this.name,"SPENT_TOKEN", instrument.ToString() , "-");
        return 0;
    }
    public virtual int ConvertTokensToMoney(int numTokensToConvert)
    {
        if (numTokens == 0)
        {
            Debug.Log("You have no more tokens to convert!");
            return 1;
        }

        numTokens-=numTokensToConvert;
        money += numTokensToConvert * GameProperties.tokenValue;

        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), this.id.ToString(), this.name,"CONVERTED_TOKENS", "-" , numTokensToConvert.ToString());
        return 0;
    }
    public virtual int BuyTokens(int numTokensToBuy)
    {
        int moneyToSpend = numTokensToBuy * GameProperties.tokenValue;

        if (tokensBoughtOnCurrRound >= GameProperties.allowedPlayerTokenBuysPerRound)
        {
            Debug.Log("You can only convert money to one token per round!");
            return 1;
        }

        if (money < moneyToSpend)
        {
            Debug.Log("You have no money to convert!");
            return 2;
        }

        money -= moneyToSpend;
        numTokens += numTokensToBuy;

        tokensBoughtOnCurrRound+=numTokensToBuy;
        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), this.id.ToString(), this.name,"BOUGHT_TOKENS", "-" , numTokensToBuy.ToString());
        return 0;
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
        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), this.id.ToString(), this.name, "ROLL_BACK_CHANGES_TO_PHASE_START", "-", "-");
    }

    public void TakeAllMoney()
    {
        this.money = 0;
    }
    public void ReceiveMoney(int moneyToReceive)
    {
        this.money += moneyToReceive;
        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), this.id.ToString(), this.name,"RECEIVED_MONEY", "-" , moneyToReceive.ToString());
    }
    public void ReceiveTokens(int numTokensToReceive)
    {
        this.numTokens += numTokensToReceive;
    }
    public int GetMoney()
    {
        return this.money;
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
        GameGlobals.gameLogManager.WriteEventToLog(GameGlobals.currSessionId.ToString(), GameGlobals.currGameId.ToString(), GameGlobals.currGameRoundId.ToString(), this.id.ToString(), this.name,"INSTRUMENT_VALUE_CHANGED", instrument.ToString(), value.ToString());
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


