using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public abstract class Player
{
    private string actionLog;

    GameManager gameManagerRef;

    protected string name;
    protected int numTokens;
    protected int money;

    protected GameProperties.Instrument diceRollInstrument;
    protected GameProperties.Instrument toBeTokenedInstrument;

    protected Dictionary<GameProperties.Instrument, int> skillSet;
    protected Dictionary<GameProperties.Instrument, int> albumContributions;

    public int tokensBoughtOnCurrRound;
   
    public Player(string name)
    {
        this.name = name;

        this.tokensBoughtOnCurrRound = 0;

        this.money = 0;
        this.numTokens = 0;
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
        if (numTokens == 0)
        {
            return false;
        }

        numTokens--;
        skillSet[instrument]++;

        FileManager.WritePlayerActionToLog(GameProperties.currGameId.ToString(), this.name,"SPENT_TOKEN", instrument.ToString() , "-");
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

        FileManager.WritePlayerActionToLog(GameProperties.currGameId.ToString(), this.name,"CONVERTED_TOKENS", "-" , numTokensToConvert.ToString());
        return true;
    }
    public bool BuyTokens(int numTokensToBuy)
    {
        int moneyToSpend = numTokensToBuy * GameProperties.tokenValue;
        if (money < moneyToSpend)
        {
            return false;
        }

        money -= moneyToSpend;
        numTokens += numTokensToBuy;

        tokensBoughtOnCurrRound+=numTokensToBuy;
        FileManager.WritePlayerActionToLog(GameProperties.currGameId.ToString(), this.name,"BOUGHT_TOKENS", "-" , numTokensToBuy.ToString());
        return true;
    }

    public void ReceiveMoney(int moneyToReceive)
    {
        this.money += moneyToReceive;
        FileManager.WritePlayerActionToLog(GameProperties.currGameId.ToString(), this.name,"RECEIVED_MONEY", "-" , moneyToReceive.ToString());
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
        FileManager.WritePlayerActionToLog(GameProperties.currGameId.ToString(), this.name,"INSTRUMENT_VALUE_CHANGED", instrument.ToString(), value.ToString());
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


public class UIPlayer : Player {

    private GameObject playerUI;

    private Button UIplayerActionButton;

    private Text UInameText;
    private Text UInumTokensValue;
    private Text UImoneyValue;

    private Text UISkillTexts;
    private Text UITokensTexts;
    private Text UIContributionsTexts;

    private GameObject UILevelUpScreen;
    private GameObject UIPlayForInstrumentScreen;
    private GameObject UILastDecisionsScreen;

    private GameObject UILastDecisionsMegaHitScreen;
    protected Button UIReceiveMegaHitButton;
    protected Button UIStickWithMarktingMegaHitButton;

    private GameObject UILastDecisionsFailScreen;
    protected Button UIReceiveFailButton;


    protected Dropdown UIspendTokenDropdown;
    protected Button UIspendTokenButton;

    protected Button UIbuyTokenButton;

    protected Dropdown UIrollDicesForDropdown;


    public UIPlayer(string name) : base(name) {
    }


    public GameObject GetPlayerUI()
    {
        return this.playerUI;
    }


    public void InitUI(GameObject playerUIPrefab, GameObject canvas)
    {

        this.playerUI = Object.Instantiate(playerUIPrefab, canvas.transform);

        this.UIplayerActionButton = playerUI.transform.Find("playerActionButton").gameObject.GetComponent<Button>();

        this.UInameText = playerUI.transform.Find("nameText").gameObject.GetComponent<Text>();

        this.UInumTokensValue = playerUI.transform.Find("gameStateSection/numTokensValue").gameObject.GetComponent<Text>();
        this.UImoneyValue = playerUI.transform.Find("gameStateSection/moneyValue").gameObject.GetComponent<Text>();


        this.UISkillTexts = playerUI.transform.Find("skillTable/skillTexts").gameObject.GetComponent<Text>();
        this.UITokensTexts = playerUI.transform.Find("skillTable/tokensTexts").gameObject.GetComponent<Text>();
        this.UIContributionsTexts = playerUI.transform.Find("skillTable/albumContributionsTexts").gameObject.GetComponent<Text>();


        this.UILevelUpScreen = playerUI.transform.Find("playerActionSection/levelUpPhaseUI").gameObject;
        this.UIPlayForInstrumentScreen = playerUI.transform.Find("playerActionSection/playForInstrumentUI").gameObject;



        this.UILastDecisionsScreen = playerUI.transform.Find("playerActionSection/lastDecisionsUI").gameObject;
        this.UILastDecisionsMegaHitScreen = UILastDecisionsScreen.transform.Find("earnMoneyQuestion/megaHitQuestion").gameObject;
        this.UILastDecisionsFailScreen = UILastDecisionsScreen.transform.Find("earnMoneyQuestion/failQuestion").gameObject;

        this.UIReceiveMegaHitButton = UILastDecisionsMegaHitScreen.transform.Find("receiveButton").gameObject.GetComponent<Button>();
        this.UIStickWithMarktingMegaHitButton = UILastDecisionsMegaHitScreen.transform.Find("stickWithMarkting").gameObject.GetComponent<Button>();

        this.UIReceiveFailButton = UILastDecisionsFailScreen.transform.Find("receiveButton").gameObject.GetComponent<Button>();


        GameObject UIinstrumentSelection = UILevelUpScreen.transform.Find("spendTokenSelection").gameObject;
        this.UIspendTokenDropdown = UIinstrumentSelection.transform.Find("spendTokenDropdown").gameObject.GetComponent<Dropdown>();
        this.UIspendTokenButton = UIinstrumentSelection.transform.Find("spendTokenButton").gameObject.GetComponent<Button>();
        UIspendTokenButton.onClick.AddListener(delegate {
            GameProperties.Instrument selectedInstrument = (GameProperties.Instrument)this.UIspendTokenDropdown.value;
            SpendToken(selectedInstrument);
            UpdateUI();
        });

        GameObject UIbuyTokenSelection = playerUI.transform.Find("playerActionSection/levelUpPhaseUI/buyTokenSelection").gameObject;
        this.UIbuyTokenButton = UIbuyTokenSelection.transform.Find("buyTokenButton").GetComponent<Button>();
        UIbuyTokenButton.onClick.AddListener(delegate {
            if (tokensBoughtOnCurrRound < GameProperties.allowedPlayerTokenBuysPerRound)
            {
                BuyTokens(1);
                UpdateUI();
            }
        });

        this.UIrollDicesForDropdown = playerUI.transform.Find("playerActionSection/playForInstrumentUI/rollDicesForSelection/rollDicesForDropdown").gameObject.GetComponent<Dropdown>();


        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            UIspendTokenDropdown.options.Add(new Dropdown.OptionData(instrument.ToString()));
        }
        UIspendTokenDropdown.onValueChanged.AddListener(delegate {
            ChangeToBeTokenedInstrument((GameProperties.Instrument)UIspendTokenDropdown.value);
            UpdateUI();
        });
        UIrollDicesForDropdown.onValueChanged.AddListener(delegate {
            List<string> instrumentNames = new List<string>(System.Enum.GetNames(typeof(GameProperties.Instrument)));
            int instrIndex = instrumentNames.IndexOf(UIrollDicesForDropdown.options[UIrollDicesForDropdown.value].text);
            Debug.Log(instrumentNames.ToArray().ToString());
            ChangeDiceRollInstrument((GameProperties.Instrument)instrIndex);
            UpdateUI();
        });

        UInameText.text = this.name + " Turn:";
        UpdateUI();
    }


    public void UpdateUI()
    {
        UImoneyValue.text = money.ToString();
        UInumTokensValue.text = numTokens.ToString();

        UISkillTexts.text = "";
        UITokensTexts.text = "";
        UIContributionsTexts.text = "";
        foreach (GameProperties.Instrument instrument in skillSet.Keys)
        {
            UISkillTexts.text += " " + instrument.ToString()[0];
            UITokensTexts.text += " " + skillSet[instrument].ToString();

            int currAlbumContribution = albumContributions[instrument];
            UIContributionsTexts.text += (currAlbumContribution == 0) ? " _" : " " + currAlbumContribution.ToString();
        }

        List<GameProperties.Instrument> skillSetKeys = new List<GameProperties.Instrument>(skillSet.Keys);
        UIrollDicesForDropdown.ClearOptions();
        int firstInstrumentInDropdown = -1;
        UIrollDicesForDropdown.options.Add(new Dropdown.OptionData("(Do not role dice)"));
        for (int i = 0; i < skillSetKeys.Count - 1; i++)
        {
            GameProperties.Instrument currInstrument = skillSetKeys[i];
            if (skillSet[currInstrument] > 0 || this.toBeTokenedInstrument == currInstrument)
            {
                if (firstInstrumentInDropdown == -1)
                {
                    firstInstrumentInDropdown = i;
                }
                UIrollDicesForDropdown.options.Add(new Dropdown.OptionData(currInstrument.ToString()));
            }
        }
        ChangeDiceRollInstrument((GameProperties.Instrument)firstInstrumentInDropdown);
        UIrollDicesForDropdown.RefreshShownValue();
    }


    public override void LevelUp()
    {
        UIplayerActionButton.gameObject.SetActive(true);
        UILevelUpScreen.SetActive(true);
        UIPlayForInstrumentScreen.SetActive(false);
        UILastDecisionsScreen.SetActive(false);

        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.onClick.AddListener(delegate { SendLevelUpResponse(); UpdateUI(); });
        UpdateUI();
    }
    public override void PlayForInstrument()
    {
        UIplayerActionButton.gameObject.SetActive(true);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(true);
        UILastDecisionsScreen.SetActive(false);

        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.onClick.AddListener(delegate { SendPlayForInstrumentResponse(); UpdateUI(); });
        UpdateUI();
    }
    public override void LastDecisionsPhase(Album currAlbum)
    {
        UIplayerActionButton.gameObject.SetActive(false);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(false);
        UILastDecisionsScreen.SetActive(true);

        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            UILastDecisionsMegaHitScreen.SetActive(true);
            UILastDecisionsFailScreen.SetActive(false);
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            UILastDecisionsFailScreen.SetActive(true);
            UILastDecisionsMegaHitScreen.SetActive(false);
        }
        UIReceiveMegaHitButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(0); UpdateUI(); });
        UIStickWithMarktingMegaHitButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(1); UpdateUI(); });
        UIReceiveFailButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(2); UpdateUI(); });
        UpdateUI();
    }

}


public class SimPlayer : Player
{
    public SimPlayer(string name) : base(name)
    {
    }


    public override void LevelUp()
    {
        SpendToken(GameProperties.Instrument.GUITAR);
        SpendToken(GameProperties.Instrument.GUITAR);
        SendLevelUpResponse();
    }

    public override void PlayForInstrument()
    {
        ChangeDiceRollInstrument(GameProperties.Instrument.GUITAR);
        SendPlayForInstrumentResponse();
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            SendLastDecisionsPhaseResponse(0);
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            SendLastDecisionsPhaseResponse(1);
        }
    }

}