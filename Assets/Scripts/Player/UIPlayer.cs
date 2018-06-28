using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayer : Player
{

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


    public UIPlayer(string name) : base(name)
    {
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
        UIspendTokenButton.onClick.AddListener(delegate () {
            GameProperties.Instrument selectedInstrument = (GameProperties.Instrument)this.UIspendTokenDropdown.value;
            SpendToken(selectedInstrument);
            UpdateCommonUIElements();
        });

        GameObject UIbuyTokenSelection = playerUI.transform.Find("playerActionSection/levelUpPhaseUI/buyTokenSelection").gameObject;
        this.UIbuyTokenButton = UIbuyTokenSelection.transform.Find("buyTokenButton").GetComponent<Button>();
        UIbuyTokenButton.onClick.AddListener(delegate () {
            if (tokensBoughtOnCurrRound < GameProperties.allowedPlayerTokenBuysPerRound)
            {
                BuyTokens(1);
                UpdateCommonUIElements();
            }
        });

        this.UIrollDicesForDropdown = playerUI.transform.Find("playerActionSection/playForInstrumentUI/rollDicesForSelection/rollDicesForDropdown").gameObject.GetComponent<Dropdown>();


        foreach (GameProperties.Instrument instrument in System.Enum.GetValues(typeof(GameProperties.Instrument)))
        {
            UIspendTokenDropdown.options.Add(new Dropdown.OptionData(instrument.ToString()));
        }
        UIspendTokenDropdown.onValueChanged.AddListener(delegate {
            ChangeToBeTokenedInstrument((GameProperties.Instrument)UIspendTokenDropdown.value);
            UpdateCommonUIElements();
        });
        UIrollDicesForDropdown.onValueChanged.AddListener(delegate {
            List<string> instrumentNames = new List<string>(System.Enum.GetNames(typeof(GameProperties.Instrument)));
            int instrIndex = instrumentNames.IndexOf(UIrollDicesForDropdown.options[UIrollDicesForDropdown.value].text);
            Debug.Log(instrumentNames.ToArray().ToString());
            ChangeDiceRollInstrument((GameProperties.Instrument)instrIndex);
            UpdateCommonUIElements();
        });

        UInameText.text = this.name + " Turn:";
        UpdateCommonUIElements();
    }


    public void UpdateCommonUIElements()
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
