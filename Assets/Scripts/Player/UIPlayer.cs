using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : Player
{

    private GameObject playerUI;
    private GameObject playerMarkerUI;
    private GameObject playerDisablerUI;
    private GameObject playerSelfDisablerUI;

    private Button UIplayerActionButton;

    private Text UInameText;
    private Text UImoneyValue;
    
    private Text[] UISkillLevelsTexts;
    private List<Button> UISkillIconsButtons;
    

    private GameObject UIChooseDiceRollInstrumentScreen;
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
    protected Button UIdiscardChangesButton;

    protected Button UIspendTokenInInstrumentButton;
    protected Button UIspendTokenInMarketingButton;

    protected Button UInotRollDicesButton;
    protected Button UIrollForPreferredInstrumentButton;

    private PoppupScreenFunctionalities warningScreenRef;


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


    public virtual void InitUI(GameObject playerUIPrefab, GameObject canvas, PoppupScreenFunctionalities warningScreenRef)
    {
        this.warningScreenRef = warningScreenRef;

        this.playerUI = Object.Instantiate(playerUIPrefab, canvas.transform);
        this.playerMarkerUI = playerUI.transform.Find("marker").gameObject;
        this.playerDisablerUI = playerUI.transform.Find("disabler").gameObject;
        this.playerSelfDisablerUI = playerUI.transform.Find("selfDisabler").gameObject;
        playerSelfDisablerUI.SetActive(false); //provide interaction by default

        this.UIplayerActionButton = playerUI.transform.Find("playerActionSection/playerActionButton").gameObject.GetComponent<Button>();

        this.UInameText = playerUI.transform.Find("nameText").gameObject.GetComponent<Text>();
        this.UImoneyValue = playerUI.transform.Find("playerStateSection/moneyValue").gameObject.GetComponent<Text>();

        this.UISkillLevelsTexts = playerUI.transform.Find("playerStateSection/skillTable/skillLevels").GetComponentsInChildren<Text>();
        this.UISkillIconsButtons = new List<Button>(playerUI.transform.Find("playerStateSection/skillTable/skillIcons").GetComponentsInChildren<Button>());
        foreach(Button button in UISkillIconsButtons)
        {
            button.GetComponent<Outline>().enabled = false;
        }
        this.UIChooseDiceRollInstrumentScreen = playerUI.transform.Find("playerActionSection/chooseDiceRollInstrumentPhaseUI").gameObject;




        this.UILevelUpScreen = playerUI.transform.Find("playerActionSection/levelUpPhaseUI").gameObject;

        //--------------------this is disabled in this version-----------------------------
        this.UIbuyTokenButton = UILevelUpScreen.transform.Find("buyTokenSelection/buyTokenButton").GetComponent<Button>();
        UIbuyTokenButton.onClick.AddListener(delegate ()
        {
            BuyTokens(1);
            UpdateCommonUIElements();
        });
        this.UIdiscardChangesButton = UILevelUpScreen.transform.Find("buyTokenSelection/discardChangesButton").GetComponent<Button>();
        UIdiscardChangesButton.onClick.AddListener(delegate ()
        {
            RollBackChangesToPhaseStart();
            UpdateCommonUIElements();
        });
        this.UIbuyTokenButton.enabled = false;
        this.UIdiscardChangesButton.enabled = false;
        //--------------------------------------------------------------------------------


        this.UIspendTokenInInstrumentButton = UILevelUpScreen.transform.Find("spendTokenSelection/spendTokenInPreferredInstrumentButton").GetComponent<Button>();
        UIspendTokenInInstrumentButton.onClick.AddListener(delegate ()
        {
            if (money >= GameProperties.tokenValue)
            {
                BuyTokens(1);
            }
            SpendToken(preferredInstrument);
            UpdateCommonUIElements();
        });
        this.UIspendTokenInMarketingButton = UILevelUpScreen.transform.Find("spendTokenSelection/spendTokenInMarketingButton").GetComponent<Button>();
        UIspendTokenInMarketingButton.onClick.AddListener(delegate ()
        {
            if (money >= GameProperties.tokenValue)
            {
                BuyTokens(1);
            }
            SpendToken(GameProperties.Instrument.MARKETING);
            UpdateCommonUIElements();
        });



        this.UIPlayForInstrumentScreen = playerUI.transform.Find("playerActionSection/playForInstrumentUI").gameObject;

        this.UInotRollDicesButton = UIPlayForInstrumentScreen.transform.Find("notRollDicesButton").GetComponent<Button>();
        UInotRollDicesButton.onClick.AddListener(delegate ()
        {
            ChangeDiceRollInstrument(GameProperties.Instrument.NONE);
            UpdateCommonUIElements();
            SendPlayForInstrumentResponse();
        });
        this.UIrollForPreferredInstrumentButton = UIPlayForInstrumentScreen.transform.Find("rollForPreferredInstrumentButton").GetComponent<Button>();
        UIrollForPreferredInstrumentButton.onClick.AddListener(delegate ()
        {
            ChangeDiceRollInstrument(this.preferredInstrument);
            UpdateCommonUIElements();
            SendPlayForInstrumentResponse();
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

    public override void RegisterMeOnPlayersLog()
    {
        GameProperties.gameLogManager.WritePlayerToLog("0", GameGlobals.currGameId.ToString(), this.id.ToString(), this.name, "-");
    }

    public void UpdateCommonUIElements()
    {
        UImoneyValue.text = money.ToString();

        foreach (GameProperties.Instrument instrument in skillSet.Keys)
        {
            int currSkillLevel = skillSet[instrument];
            UISkillLevelsTexts[(int)instrument].text = (currSkillLevel>0)? "  " + skillSet[instrument].ToString() : "";
        }
    }

    public override int SpendToken(GameProperties.Instrument instrument)
    {
        int result = base.SpendToken(instrument);
        if (result != 0) //handle the error in the UI
        {
            switch (result)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("You cannot level up more skills in this round!");
                    break;
                case 2:
                    warningScreenRef.DisplayPoppup("You cannot develop the same skill more than " + GameProperties.maximumSkillLevelPerInstrument + " times!");
                    break;
            }
        }
        return result;
    }
    public override int ConvertTokensToMoney(int numTokensToConvert)
    {
        int result = base.ConvertTokensToMoney(numTokensToConvert);
        if (result != 0) //handle the error in the UI
        {
            switch (result)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("You have no more skill levels to convert to money!");
                    break;
            }
        }
        return result;
    }
    public override int BuyTokens(int numTokensToBuy)
    {
        int result = base.BuyTokens(numTokensToBuy);
        if (result != 0) //handle the error in the UI
        {
            switch (result)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("You can only convert money to one skill level per round!");
                    break;

                case 2:
                    warningScreenRef.DisplayPoppup("You have no more money to convert to skill levels!");
                    break;
            }
        }
        return result;
    }

    public override int SendChoosePreferredInstrumentResponse()
    {
        int success = base.SendChoosePreferredInstrumentResponse();
        if (success!=0) //handle the error in the ui
        {
            switch (success)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("No preferred instrument selected!");
                    break;
            }
        }
        else
        {
            //disable instrument button renderer objects except the chosen one. Also remove its outline
            foreach (GameProperties.Instrument instrument in skillSet.Keys)
            {
                Button currButton = UISkillIconsButtons[(int)instrument];
                if(instrument == preferredInstrument || instrument == GameProperties.Instrument.MARKETING)
                {
                    currButton.GetComponent<Outline>().enabled = false;
                    continue;
                }
                currButton.transform.gameObject.SetActive(false); //take off the other instruments
                UISkillLevelsTexts[(int)instrument].transform.gameObject.SetActive(false);
            }
            UpdateCommonUIElements();
        }
        return success;
    }
    public override int SendLevelUpResponse()
    {
        int success = base.SendLevelUpResponse();
        if (success!=0) //handle the error in the ui
        {
            switch (success)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("You have to spend all your tokens before you finish leveling up!");
                    break;
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
    public override int SendPlayForInstrumentResponse()
    {
        int success = base.SendPlayForInstrumentResponse();
        if (success!=0) //handle the error in the ui
        {
            //do nothing, no possible errors yet!
        }
        else
        {
            UIplayerActionButton.interactable = false;
            UInotRollDicesButton.interactable = false;
            UpdateCommonUIElements();
            if (diceRollInstrument != GameProperties.Instrument.NONE)
            {
                UISkillIconsButtons[(int) diceRollInstrument].GetComponent<Outline>().enabled = false;
            }
        }
        return success;
    }
    public override int SendLastDecisionsPhaseResponse(int condition)
    {
        int success = base.SendLastDecisionsPhaseResponse(condition);
        if (success != 0) //handle the error in the ui
        {
            //do nothing, no possible errors yet!
        }
        else
        {
            UIplayerActionButton.interactable = false;
            UIReceiveFailButton.interactable = false;
            UIReceiveMegaHitButton.interactable = false;
            UIStickWithMarketingMegaHitButton.interactable = false;
            UpdateCommonUIElements();
        }
        return success;
    }


    public override int ChangeDiceRollInstrument(GameProperties.Instrument instrument)
    {
        int success = base.ChangeDiceRollInstrument(instrument);
        if (success!=0) //handle the error in the ui
        {
            switch(success)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("The dices for marketing are rolled only after knowing the album result.");
                    break;
                case 2:
                    warningScreenRef.DisplayPoppup("You cannot roll dices for an unevolved skill!");
                    break;
            }
        }
        else
        {
            //enable outline only in this button
            for (int j = 0; j < UISkillIconsButtons.Count; j++)
            {
                Button button = UISkillIconsButtons[j];
                button.GetComponent<Outline>().enabled = ((int)instrument == j);
            }
        }
        return success;
    }
    public override int ChangePreferredInstrument(GameProperties.Instrument instrument)
    {
        int success = base.ChangePreferredInstrument(instrument);
        if (success!=0) //handle the error in the ui
        {
            switch(success)
            {
                case 1:
                    warningScreenRef.DisplayPoppup("Marketing is not a playable instrument, it's a skill!");
                    break;
                case 2:
                    warningScreenRef.DisplayPoppup("This instrument is already played by someone!");
                    break;
            }
        }
        else
        {
            //enable outline only in this button
            for (int j = 0; j < UISkillIconsButtons.Count; j++)
            {
                Button button = UISkillIconsButtons[j];
                button.GetComponent<Outline>().enabled = ((int)instrument == j);
            }
        }
        return success;
    }

    public override void ChoosePreferredInstrument(Album currAlbum) {
        
        UIplayerActionButton.gameObject.SetActive(true);
        UIChooseDiceRollInstrumentScreen.SetActive(true);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(false);
        UILastDecisionsScreen.SetActive(false);

        for (int i = 0; i < UISkillIconsButtons.Count; i++)
        {
            Button currButton = UISkillIconsButtons[i];
            currButton.onClick.RemoveAllListeners();
            currButton.onClick.AddListener(delegate {
                int currButtonIndex = UISkillIconsButtons.IndexOf(currButton);
                ChangePreferredInstrument((GameProperties.Instrument) currButtonIndex);
                UpdateCommonUIElements();
            });
        }

        UIplayerActionButton.onClick.RemoveAllListeners();
        UIplayerActionButton.interactable = true;
        UIplayerActionButton.onClick.AddListener(delegate {
            SendChoosePreferredInstrumentResponse();
        });
    }
    public override void LevelUp(Album currAlbum)
    {
        UIplayerActionButton.gameObject.SetActive(true);
        UIChooseDiceRollInstrumentScreen.SetActive(false);
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
    public override void PlayForInstrument(Album currAlbum)
    {
        UIplayerActionButton.gameObject.SetActive(false);
        UIChooseDiceRollInstrumentScreen.SetActive(false);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(true);
        UILastDecisionsScreen.SetActive(false);


        if (skillSet[preferredInstrument] == 0)
        {
            UIrollForPreferredInstrumentButton.gameObject.SetActive(false);
            UInotRollDicesButton.gameObject.SetActive(true);
        }
        else
        {
            UIrollForPreferredInstrumentButton.gameObject.SetActive(true);
            UInotRollDicesButton.gameObject.SetActive(false);
        }

        for (int i = 0; i < UISkillIconsButtons.Count; i++)
        {
            Button currButton = UISkillIconsButtons[i];
            currButton.onClick.RemoveAllListeners();
            currButton.onClick.AddListener(delegate {
                int currButtonIndex = UISkillIconsButtons.IndexOf(currButton);
                ChangeDiceRollInstrument((GameProperties.Instrument)currButtonIndex);
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
        UIChooseDiceRollInstrumentScreen.SetActive(false);
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

    public void ClearActionUI()
    {
        UIChooseDiceRollInstrumentScreen.SetActive(false);
        UILevelUpScreen.SetActive(false);
        UIPlayForInstrumentScreen.SetActive(false);
        UILastDecisionsScreen.SetActive(false);
        UIplayerActionButton.gameObject.SetActive(false);
    }

    public void DisableAllInputs()
    {
        playerSelfDisablerUI.SetActive(true);
    }
    public void EnableAllInputs()
    {
        playerSelfDisablerUI.SetActive(false);
    }

    public override void InitPlayer(params object[] args)
    {
        base.InitPlayer(args);
        InitUI((GameObject)args[0], (GameObject)args[1], (PoppupScreenFunctionalities)args[2]);
    }
    public override void ResetPlayer(params object[] args)
    {
        ClearActionUI();
    }
}
