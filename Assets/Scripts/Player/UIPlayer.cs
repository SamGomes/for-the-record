using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : Player
{

    private GameObject playerUI;
    private GameObject playerMarkerUI;
    private GameObject playerDisablerUI;

    private Button UIplayerActionButton;

    private Text UInameText;
    private Text UImoneyValue;
    
    private Text[] UISkillLevelsTexts;
    private List<Button> UISkillIconsButtons;
    //private Text UIContributionsTexts;

    private GameObject UILevelUpScreen;
    private GameObject UIPlayForInstrumentScreen;
    private GameObject UILastDecisionsScreen;

    private GameObject UILastDecisionsMegaHitScreen;
    protected Button UIReceiveMegaHitButton;
    protected Button UIStickWithMarketingMegaHitButton;

    private GameObject UILastDecisionsFailScreen;
    protected Button UIReceiveFailButton;

    protected Button UIspendTokenButton;

    protected Button UIbuyTokenButton;
    private Text UInumTokensValue;
    protected Button UIdiscardChangesButton;

    private Text UICurrInstrumentToRollText;
    protected Button UInotRollDicesButton;


    private WarningScreenFunctionalities warningScreenRef;


    public UIPlayer(string name) : base(name)
    {
    }


    public GameObject GetPlayerUI()
    {
        return this.playerUI;
    }
    public GameObject GetPlayerMarkerUI()
    {
        return this.playerMarkerUI;
    }
    public GameObject GetPlayerDisablerUI()
    {
        return this.playerDisablerUI;
    }

    public void InitUI(GameObject playerUIPrefab, GameObject canvas, WarningScreenFunctionalities warningScreenRef)
    {
        this.warningScreenRef = warningScreenRef;

        this.playerUI = Object.Instantiate(playerUIPrefab, canvas.transform);
        this.playerMarkerUI = playerUI.transform.Find("marker").gameObject;
        this.playerDisablerUI = playerUI.transform.Find("disabler").gameObject;

        this.UIplayerActionButton = playerUI.transform.Find("playerActionSection/playerActionButton").gameObject.GetComponent<Button>();

        this.UInameText = playerUI.transform.Find("nameText").gameObject.GetComponent<Text>();
        this.UImoneyValue = playerUI.transform.Find("playerStateSection/moneyValue").gameObject.GetComponent<Text>();

        this.UISkillLevelsTexts = playerUI.transform.Find("playerStateSection/skillTable/skillLevels").GetComponentsInChildren<Text>();
        this.UISkillIconsButtons = new List<Button>(playerUI.transform.Find("playerStateSection/skillTable/skillIcons").GetComponentsInChildren<Button>());
        

        this.UILevelUpScreen = playerUI.transform.Find("playerActionSection/levelUpPhaseUI").gameObject;
        this.UInumTokensValue = UILevelUpScreen.transform.Find("numTokensValue").GetComponent<Text>();
        this.UIbuyTokenButton = UILevelUpScreen.transform.Find("buyTokenSelection/buyTokenButton").GetComponent<Button>();
        UIbuyTokenButton.onClick.AddListener(delegate () {
            BuyTokens(1);
            UpdateCommonUIElements();
        });
        this.UIdiscardChangesButton = UILevelUpScreen.transform.Find("buyTokenSelection/discardChangesButton").GetComponent<Button>();
        UIdiscardChangesButton.onClick.AddListener(delegate ()
        {
            RollBackChangesToPhaseStart();
            UpdateCommonUIElements();
        });



        this.UIPlayForInstrumentScreen = playerUI.transform.Find("playerActionSection/playForInstrumentUI").gameObject;
        this.UICurrInstrumentToRollText = UIPlayForInstrumentScreen.transform.Find("currInstrumentToRollValue").GetComponent<Text>();

        this.UInotRollDicesButton = UIPlayForInstrumentScreen.transform.Find("notRollDicesButton").GetComponent<Button>();
        UInotRollDicesButton.onClick.AddListener(delegate ()
        {
            this.diceRollInstrument = GameProperties.Instrument.NONE;
            UpdateCommonUIElements();
        });


        this.UILastDecisionsScreen = playerUI.transform.Find("playerActionSection/lastDecisionsUI").gameObject;
        this.UILastDecisionsMegaHitScreen = UILastDecisionsScreen.transform.Find("earnMoneyQuestion/megaHitQuestion").gameObject;
        this.UILastDecisionsFailScreen = UILastDecisionsScreen.transform.Find("earnMoneyQuestion/failQuestion").gameObject;

        this.UIReceiveMegaHitButton = UILastDecisionsMegaHitScreen.transform.Find("receiveButton").gameObject.GetComponent<Button>();
        this.UIStickWithMarketingMegaHitButton = UILastDecisionsMegaHitScreen.transform.Find("stickWithMarketing").gameObject.GetComponent<Button>();

        this.UIReceiveFailButton = UILastDecisionsFailScreen.transform.Find("receiveButton").gameObject.GetComponent<Button>();



        UInameText.text = this.name + " Control Panel:";
        UpdateCommonUIElements();
    }
    

    public void UpdateCommonUIElements()
    {
        UImoneyValue.text = money.ToString();
        UInumTokensValue.text = numTokens.ToString();

        foreach (GameProperties.Instrument instrument in skillSet.Keys)
        {
            UISkillLevelsTexts[(int)instrument].text = "  " + skillSet[instrument].ToString();
        }
        UICurrInstrumentToRollText.text = this.diceRollInstrument.ToString();

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
            else if (skillSet[instrument] == GameProperties.maximumSkillLevelPerInstrument)
            {
                warningScreenRef.DisplayWarning("You cannot develop the same skill more than " + GameProperties.maximumSkillLevelPerInstrument + " times!");
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
        else
        {
            UIplayerActionButton.interactable = false;
            UIdiscardChangesButton.interactable = false;
            UIbuyTokenButton.interactable = false;
            UpdateCommonUIElements();
        }
        return success;
    }
    public new bool SendPlayForInstrumentResponse()
    {
        bool success = base.SendPlayForInstrumentResponse();
        if (success)
        {
            UIplayerActionButton.interactable = false;
            UInotRollDicesButton.interactable = false;
            UpdateCommonUIElements();
        }
        return success;
    }
    public new bool SendLastDecisionsPhaseResponse(int condition)
    {
        bool success = base.SendLastDecisionsPhaseResponse(condition);
        if (success)
        {
            UIplayerActionButton.interactable = false;
            UIReceiveFailButton.interactable = false;
            UIReceiveMegaHitButton.interactable = false;
            UIStickWithMarketingMegaHitButton.interactable = false;
            UpdateCommonUIElements();
        }
        return success;
    }


    public new bool ChangeDiceRollInstrument(GameProperties.Instrument instrument)
    {
        bool success = base.ChangeDiceRollInstrument(instrument);
        if (!success) //handle the error in the ui
        {
            if (instrument == GameProperties.Instrument.MARKETING)
            {
                warningScreenRef.DisplayWarning("The dices for marketing are rolled only after knowing the album result.");
            }else if (skillSet[instrument] == 0)
            {
                warningScreenRef.DisplayWarning("You cannot roll dices for an unevolved skill!");
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

        for (int i = 0; i < UISkillIconsButtons.Count; i++)
        {
            Button currButton = UISkillIconsButtons[i];
            currButton.onClick.RemoveAllListeners();
            currButton.onClick.AddListener(delegate {
                SpendToken((GameProperties.Instrument) UISkillIconsButtons.IndexOf(currButton));
                UpdateCommonUIElements();
            });
        }
        
        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.interactable = true;
        UIdiscardChangesButton.interactable = true;
        UIbuyTokenButton.interactable = true;
        UIplayerActionButton.onClick.AddListener(delegate {
            SendLevelUpResponse();
        });


        if (this.money == 0)
        {
            UIbuyTokenButton.interactable = false;
        }
        else
        {
            UIbuyTokenButton.interactable = true;
        }


        UpdateCommonUIElements();
    }
    public override void PlayForInstrument()
    {
        UIplayerActionButton.gameObject.SetActive(true);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(true);
        UILastDecisionsScreen.SetActive(false);


        for (int i = 0; i < UISkillIconsButtons.Count; i++)
        {
            Button currButton = UISkillIconsButtons[i];
            currButton.onClick.RemoveAllListeners();
            currButton.onClick.AddListener(delegate {
                ChangeDiceRollInstrument((GameProperties.Instrument) UISkillIconsButtons.IndexOf(currButton));
                UpdateCommonUIElements();
            });
        }


        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.interactable = true;
        UInotRollDicesButton.interactable = true;
        UIplayerActionButton.onClick.AddListener(delegate {
            SendPlayForInstrumentResponse();
        });
        UpdateCommonUIElements();
    }
    public override void LastDecisionsPhase(Album currAlbum)
    {
        UIplayerActionButton.gameObject.SetActive(false);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(false);
        UILastDecisionsScreen.SetActive(true);

        for (int i = 0; i < UISkillIconsButtons.Count; i++)
        {
            Button currButton = UISkillIconsButtons[i];
            currButton.onClick.RemoveAllListeners();
        }


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

        UIplayerActionButton.interactable = true;
        UIReceiveFailButton.interactable = true;
        UIReceiveMegaHitButton.interactable = true;
        if (skillSet[GameProperties.Instrument.MARKETING] == 0)
        {
            UIStickWithMarketingMegaHitButton.interactable = false;
        }
        else
        {
            UIStickWithMarketingMegaHitButton.interactable = true;
        }


        UIReceiveMegaHitButton.onClick.RemoveAllListeners();
        UIStickWithMarketingMegaHitButton.onClick.RemoveAllListeners();
        UIReceiveFailButton.onClick.RemoveAllListeners();

        UIReceiveMegaHitButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(0); });
        UIStickWithMarketingMegaHitButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(1); });
        UIReceiveFailButton.onClick.AddListener(delegate { SendLastDecisionsPhaseResponse(2); });
        UpdateCommonUIElements();
    }

}
