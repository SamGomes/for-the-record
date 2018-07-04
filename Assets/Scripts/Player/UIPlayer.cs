using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : Player
{

    private GameObject playerUI;

    private Button UIplayerActionButton;

    private Text UInameText;
    private Text UImoneyValue;
    
    private GameObject UISkillLevelsTexts;
    //private Text UIContributionsTexts;

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
    private Text UInumTokensValue;

    protected Dropdown UIrollDicesForDropdown;


    private WarningScreenFunctionalities warningScreenRef;


    public UIPlayer(string name) : base(name)
    {
    }


    public GameObject GetPlayerUI()
    {
        return this.playerUI;
    }


    public void InitUI(GameObject playerUIPrefab, GameObject canvas, WarningScreenFunctionalities warningScreenRef)
    {
        this.warningScreenRef = warningScreenRef;

        this.playerUI = Object.Instantiate(playerUIPrefab, canvas.transform);

        this.UIplayerActionButton = playerUI.transform.Find("playerActionSection/playerActionButton").gameObject.GetComponent<Button>();

        this.UInameText = playerUI.transform.Find("nameText").gameObject.GetComponent<Text>();
        this.UImoneyValue = playerUI.transform.Find("playerStateSection/moneyValue").gameObject.GetComponent<Text>();
        

        this.UISkillLevelsTexts = playerUI.transform.Find("skillTable/skillLevels").gameObject;
        //this.UIContributionsTexts = playerUI.transform.Find("skillTable/albumContributionsTexts").gameObject.GetComponent<Text>();

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
        UIspendTokenButton.onClick.AddListener(delegate () {
            GameProperties.Instrument selectedInstrument = (GameProperties.Instrument)this.UIspendTokenDropdown.value;
            SpendToken(selectedInstrument);
            UpdateCommonUIElements();
        });

        GameObject UILevelUpPhase = playerUI.transform.Find("playerActionSection/levelUpPhaseUI").gameObject;
        this.UInumTokensValue = UILevelUpPhase.transform.Find("numTokensValue").GetComponent<Text>();
        this.UIbuyTokenButton = UILevelUpPhase.transform.Find("buyTokenSelection/buyTokenButton").GetComponent<Button>();
        UIbuyTokenButton.onClick.AddListener(delegate () {
            BuyTokens(1);
            UpdateCommonUIElements();
        });

        this.UIrollDicesForDropdown = playerUI.transform.Find("playerActionSection/playForInstrumentUI/rollDicesForSelection/rollDicesForDropdown").gameObject.GetComponent<Dropdown>();


        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            if(instrument != GameProperties.Instrument.NONE)
            {
                UIspendTokenDropdown.options.Add(new Dropdown.OptionData(instrument.ToString()));
            }
        }
        UIspendTokenDropdown.onValueChanged.AddListener(delegate {
            ChangeToBeTokenedInstrument((GameProperties.Instrument)UIspendTokenDropdown.value);
            UpdateCommonUIElements();
        });
        UIrollDicesForDropdown.onValueChanged.AddListener(delegate {
            
            List<string> instrumentNames = new List<string>(System.Enum.GetNames(typeof(GameProperties.Instrument)));
            int instrIndex = instrumentNames.IndexOf(UIrollDicesForDropdown.options[UIrollDicesForDropdown.value].text);
            Debug.Log(instrumentNames.ToArray().ToString());
            if (instrIndex != -1) //workaround to be able to not roll dice
            {
                ChangeDiceRollInstrument((GameProperties.Instrument) instrIndex);
            }
            else
            {
                ChangeDiceRollInstrument(GameProperties.Instrument.NONE);
            }
            UpdateCommonUIElements();
        });

        UInameText.text = this.name + " Turn:";
        UpdateCommonUIElements();
    }


    public void UpdateCommonUIElements()
    {
        UImoneyValue.text = money.ToString();
        UInumTokensValue.text = numTokens.ToString();

        //UISkillTexts.text = "";
        //UIContributionsTexts.text = "";
        foreach (GameProperties.Instrument instrument in skillSet.Keys)
        {
            //UISkillTexts.text += " " + instrument.ToString()[0];
            UISkillLevelsTexts.GetComponentsInChildren<Text>()[(int)instrument].text = "  " + skillSet[instrument].ToString();

            //int currAlbumContribution = albumContributions[instrument];
            //UIContributionsTexts.text += (currAlbumContribution == 0) ? " _" : " " + currAlbumContribution.ToString();
        }
    }

    public new bool SpendToken(GameProperties.Instrument instrument)
    {
        bool result = base.SpendToken(instrument);
        if (result == false) //handle the error in the UI
        {
            if (numTokens == 0)
            {
                warningScreenRef.DisplayWarning("You have no more tokens to level up your skills!");
            }
            else if (instrument != GameProperties.Instrument.MARKTING && lastLeveledUpInstruments.Contains(instrument))
            {
                warningScreenRef.DisplayWarning("You cannot develop the same skill on two consecutive albums!");
            }
        }
        return result;
    }

    public new bool ConvertTokensToMoney(int numTokensToConvert)
    {
        bool result = base.ConvertTokensToMoney(numTokensToConvert);
        if (result == false) //handle the error in the UI
        {
            warningScreenRef.DisplayWarning("You have no more tokens to convert!");
        }
        return result;
    }

    public new bool BuyTokens(int numTokensToBuy)
    {
        bool result = base.BuyTokens(numTokensToBuy);
        if (result == false) //handle the error in the UI
        {
            int moneyToSpend = numTokensToBuy * GameProperties.tokenValue;
            if (tokensBoughtOnCurrRound >= GameProperties.allowedPlayerTokenBuysPerRound)
            {
                warningScreenRef.DisplayWarning("You can only convert money to one token per round!");
            }

            if (money < moneyToSpend)
            {
                warningScreenRef.DisplayWarning("You have no money to convert!");
            }
        }
        return result;
    }


    public new bool SendLevelUpResponse()
    {
        bool success = base.SendLevelUpResponse();
        if (!success) //handle the error in the ui
        {
            if (numTokens != 0)
            {
                warningScreenRef.DisplayWarning("You have to spend all your tokens before you finish leveling up!");
            }
        }
        return success;
    }



    public override void LevelUp()
    {
        UIplayerActionButton.gameObject.SetActive(true);
        UILevelUpScreen.SetActive(true);
        UIPlayForInstrumentScreen.SetActive(false);
        UILastDecisionsScreen.SetActive(false);

        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.onClick.AddListener(delegate { SendLevelUpResponse(); UpdateCommonUIElements(); });
        UpdateCommonUIElements();
    }
    public override void PlayForInstrument()
    {
        UIplayerActionButton.gameObject.SetActive(true);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(true);
        UILastDecisionsScreen.SetActive(false);

        //setup dropdown
        List<GameProperties.Instrument> skillSetKeys = new List<GameProperties.Instrument>(skillSet.Keys);
        UIrollDicesForDropdown.ClearOptions();
        for (int i = 0; i < skillSetKeys.Count; i++)
        {
            GameProperties.Instrument currInstrument = skillSetKeys[i];
            if (currInstrument != GameProperties.Instrument.MARKTING && skillSet[currInstrument] > 0)
            {
                if (UIrollDicesForDropdown.options.Count == 0)
                {
                    ChangeDiceRollInstrument(currInstrument);
                }
                UIrollDicesForDropdown.options.Add(new Dropdown.OptionData(currInstrument.ToString()));
            }
        }
        UIrollDicesForDropdown.options.Add(new Dropdown.OptionData("(Do not role dice)"));
        UIrollDicesForDropdown.RefreshShownValue();
        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.onClick.AddListener(delegate { SendPlayForInstrumentResponse(); UpdateCommonUIElements(); });
        UpdateCommonUIElements();
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

        UIReceiveMegaHitButton.onClick.RemoveAllListeners();
        UIStickWithMarktingMegaHitButton.onClick.RemoveAllListeners();
        UIReceiveFailButton.onClick.RemoveAllListeners();
        UIReceiveMegaHitButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(0); UpdateCommonUIElements(); });
        UIStickWithMarktingMegaHitButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(1); UpdateCommonUIElements(); });
        UIReceiveFailButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(2); UpdateCommonUIElements(); });
        UpdateCommonUIElements();
    }

}
