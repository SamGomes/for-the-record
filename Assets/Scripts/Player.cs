using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public abstract class Player
{
    private string name;
    private int numTokens;
    private int money;
    private GameProperties.Instrument preferredInstrument;
    private Dictionary<GameProperties.Instrument, int> skillSet;


    //UI stuff
    private GameObject playerUI;

    protected Dropdown UIplayerActionDropdown;
    private Button UIplayerActionButton;

    private Text UInameText;
    private Text UInumTokensValue;
    private Text UImoneyValue;

    protected bool canExecuteAction;


    public enum PlayerAction
    {
        SPEND_TOKEN,
        CONVERT_TOKEN_TO_MONEY
    }


    public Player(string name, GameObject playerUIPrefab, GameObject canvas)
    {
        this.name = name;

        this.playerUI = Object.Instantiate(playerUIPrefab,canvas.transform);

        this.canExecuteAction = false;
        this.money = 0;
        this.numTokens = 0;
        this.preferredInstrument = GameProperties.Instrument.BASS;
        this.skillSet = new Dictionary<GameProperties.Instrument, int>();

        this.UIplayerActionDropdown = playerUI.transform.Find("playerActionSection/playerActionDropdown").gameObject.GetComponent<Dropdown>();
        this.UIplayerActionButton = playerUI.transform.Find("playerActionSection/playerActionButton").gameObject.GetComponent<Button>();

        this.UInameText = playerUI.transform.Find("nameText").gameObject.GetComponent<Text>();

        this.UInumTokensValue = playerUI.transform.Find("gameStateSection/numTokensValue").gameObject.GetComponent<Text>();
        this.UImoneyValue = playerUI.transform.Find("gameStateSection/moneyValue").gameObject.GetComponent<Text>();

        UInameText.text = this.name + " Stats:";
        UIplayerActionButton.onClick.AddListener(delegate { UIplayerActionButtonOnClick(); });

        foreach(string playerActionText in System.Enum.GetNames(typeof(PlayerAction)))
        {
            UIplayerActionDropdown.options.Add(new Dropdown.OptionData(playerActionText));
        }
    }

    //main method
    public void ExecuteActionRequest()
    {
        while (this.canExecuteAction == false)
        {
            //do nothing    
        }
        PlayerAction action = ChooseAction();
        UpdateUI(); //update ui after player chooses action
    }
    public abstract PlayerAction ChooseAction();

    //aux methods
    public void UpdateUI()
    {
        UImoneyValue.text = money.ToString();
        UInumTokensValue.text = numTokens.ToString();
    }

    public void UIplayerActionButtonOnClick()
    {
        this.canExecuteAction = true;
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


public class HumanPlayer : Player {

    public HumanPlayer(string name, GameObject playerUIPrefab, GameObject canvas) : base(name, playerUIPrefab, canvas) { }

    public override PlayerAction ChooseAction()
    {
        return (PlayerAction) this.UIplayerActionDropdown.value;
    }
}