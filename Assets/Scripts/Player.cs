using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public abstract class Player
{
    GameManager gameManagerRef;

    private string name;
    private int numTokens;
    private int money;
    private GameProperties.Instrument preferredInstrument;
    private Dictionary<GameProperties.Instrument, int> skillSet;
    private Dictionary<GameProperties.Instrument, int> albumContributions;


    //UI stuff
    private GameObject playerUI;

    protected Dropdown UIplayerActionDropdown;
    private Button UIplayerActionButton;

    private Text UInameText;
    private Text UInumTokensValue;
    private Text UImoneyValue;

    private Text UISkillTexts;
    private Text UITokensTexts;
    private Text UIContributionsTexts;

    protected GameObject UIinstrumentSelection;
    protected Dropdown UIinstrumentDropdown;

    protected GameObject UImoneyConversionSelection;
    protected Text UImoneyConversionValue;



    public enum PlayerAction
    {
        SPEND_TOKEN,
        CONVERT_MONEY_TO_TOKEN
    }


    public Player(string name, GameObject playerUIPrefab, GameObject canvas)
    {
        this.name = name;

        this.playerUI = Object.Instantiate(playerUIPrefab,canvas.transform);

        this.money = 0;
        this.numTokens = 0;
        this.preferredInstrument = GameProperties.Instrument.BASS;
        this.skillSet = new Dictionary<GameProperties.Instrument, int>();
        this.albumContributions = new Dictionary<GameProperties.Instrument, int>();

        //add values to the dictionary
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            skillSet[instrument] = 0;
            albumContributions[instrument] = 0;
        }

        this.gameManagerRef = GameObject.Find("GameManager").gameObject.GetComponent<GameManager>();

        this.UIplayerActionDropdown = playerUI.transform.Find("playerActionSection/playerActionDropdown").gameObject.GetComponent<Dropdown>();
        this.UIplayerActionButton = playerUI.transform.Find("playerActionButton").gameObject.GetComponent<Button>();

        this.UInameText = playerUI.transform.Find("nameText").gameObject.GetComponent<Text>();

        this.UInumTokensValue = playerUI.transform.Find("gameStateSection/numTokensValue").gameObject.GetComponent<Text>();
        this.UImoneyValue = playerUI.transform.Find("gameStateSection/moneyValue").gameObject.GetComponent<Text>();


        this.UISkillTexts = playerUI.transform.Find("skillTable/skillTexts").gameObject.GetComponent<Text>();
        this.UITokensTexts = playerUI.transform.Find("skillTable/tokensTexts").gameObject.GetComponent<Text>();
        this.UIContributionsTexts = playerUI.transform.Find("skillTable/albumContributionsTexts").gameObject.GetComponent<Text>();


        this.UISkillTexts = playerUI.transform.Find("skillTable/skillTexts").gameObject.GetComponent<Text>();
        this.UITokensTexts = playerUI.transform.Find("skillTable/tokensTexts").gameObject.GetComponent<Text>();
        this.UIContributionsTexts = playerUI.transform.Find("skillTable/albumContributionsTexts").gameObject.GetComponent<Text>();


        this.UIinstrumentSelection = playerUI.transform.Find("playerActionSection/instrumentSelection").gameObject;
        this.UIinstrumentDropdown = UIinstrumentSelection.transform.Find("instrumentDropdown").gameObject.GetComponent<Dropdown>();


        this.UImoneyConversionSelection = playerUI.transform.Find("playerActionSection/moneyConversionSelection").gameObject;
        this.UImoneyConversionValue = UImoneyConversionSelection.transform.Find("moneyConversionValue/Text").gameObject.GetComponent<Text>();


        foreach (string instrumentText in System.Enum.GetNames(typeof(GameProperties.Instrument)))
        {
            UIinstrumentDropdown.options.Add(new Dropdown.OptionData(instrumentText));
        }

        UInameText.text = this.name + " Stats:";
        UIplayerActionButton.onClick.AddListener(delegate { ExecuteAction(); });

        foreach(string playerActionText in System.Enum.GetNames(typeof(PlayerAction)))
        {
            UIplayerActionDropdown.options.Add(new Dropdown.OptionData(playerActionText));
        }
        UIplayerActionDropdown.onValueChanged.AddListener(delegate { SpawnActionSpecificScreens((PlayerAction) UIplayerActionDropdown.value); });


        this.UIinstrumentSelection.SetActive(true);
        this.UImoneyConversionSelection.SetActive(false);

    }

    public GameObject GetPlayerUI()
    {
        return this.playerUI;
    }

    //main method
    public void ExecuteActionRequest() //actions choosen
    {
    }

    public abstract PlayerAction ChooseAction();

    //aux methods
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
            UIContributionsTexts.text += " " + albumContributions[instrument].ToString();
        }
    }

    public void SpawnActionSpecificScreens(Player.PlayerAction action)
    {
        this.UIinstrumentSelection.SetActive(false);
        this.UImoneyConversionSelection.SetActive(false);

        switch (action)
        {
            case PlayerAction.CONVERT_MONEY_TO_TOKEN:
                this.UImoneyConversionSelection.SetActive(true);
                break;

            case PlayerAction.SPEND_TOKEN:
                this.UIinstrumentSelection.SetActive(true);
                break;
        }
    }

    public void ExecuteAction() //actions choosen
    {
        ChangePreferredInstrument((GameProperties.Instrument) UIinstrumentDropdown.value);

        //ask gameManager to resume game thread
        PlayerAction action = ChooseAction();
        switch (action)
        {
            case PlayerAction.CONVERT_MONEY_TO_TOKEN:
                ConvertMoneyToTokens(int.Parse(UImoneyConversionValue.text));
                break;
            
            case PlayerAction.SPEND_TOKEN:
                SpendToken(this.preferredInstrument);
                break;
        }

        gameManagerRef.CurrPlayerActionExecuted(this);
    }

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
        skillSet[instrument]++;
        UpdateUI(); 

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
        UpdateUI();

        return true;
    }
    public bool ConvertMoneyToTokens(int moneyToConvert)
    {
        if (numTokens == 0)
        {
            return false;
        }

        money -= moneyToConvert;
        numTokens += (int) (moneyToConvert / GameProperties.tokenValue);
        UpdateUI();

        return true;
    }

    public void ReceiveMoney(int moneyToReceive)
    {
        this.money += moneyToReceive;
        UImoneyValue.text = money.ToString();
        UpdateUI();
    }
    public void ReceiveTokens(int numTokensToReceive)
    {
        this.numTokens += numTokensToReceive;
        UInumTokensValue.text = numTokens.ToString();
        UpdateUI();
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

    public void SetAlbumContributions(Dictionary<GameProperties.Instrument, int>  albumContributions)
    {
        this.albumContributions = albumContributions;
        UpdateUI();
    }
    public void InitAlbumContributions()
    {
        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            albumContributions[instrument] = 0;
        }
        UpdateUI(); 
    }
    public void SetAlbumContribution(GameProperties.Instrument instrument, int value)
    {
        this.albumContributions[instrument] = value;
        UpdateUI();
    }
    public Dictionary<GameProperties.Instrument, int> GetAlbumContributions()
    {
        return this.albumContributions;
    }

}


public class HumanPlayer : Player {

    public HumanPlayer(string name, GameObject playerUIPrefab, GameObject canvas) : base(name, playerUIPrefab, canvas) { }

    public override PlayerAction ChooseAction()
    {
        return (PlayerAction) this.UIplayerActionDropdown.value;
    }
}