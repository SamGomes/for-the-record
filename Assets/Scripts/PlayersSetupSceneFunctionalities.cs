using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSetupSceneFunctionalities : MonoBehaviour {

    private bool isErrorEncountered;
    private GameObject customizeLabel;

    private InputField UINameSelectionInputBox;
    private Button UIStartGameButton;
    private Button UIAddPlayerButton;
    private Button UIResetButton;

    private GameObject UIAIPlayerSelectionButtonsObject;
    private GameObject configSelectionButtonsObject;

    
    public GameObject poppupPrefab;
    public GameObject playerUIPrefab;
    public GameObject playerCanvas;
    private PoppupScreenFunctionalities playerWarningPoppupRef;
    private PoppupScreenFunctionalities setupWarningPoppupRef;

    void SetUpParameterization(GameParameterization parameterization)
    {
        GameGlobals.players.Clear();
        int currPlayerId = 0;
        for (int i = 0; i < parameterization.playerParameterizations.Count; i++)
        {
            PlayerParameterization currParam = parameterization.playerParameterizations[i];

            GameProperties.Instrument currLikedInstrument = GameProperties.Instrument.NONE;

            switch (currParam.likedInstrument)
            {
                case "GUITAR":
                    currLikedInstrument = GameProperties.Instrument.GUITAR;
                    break;
                case "DRUMS":
                    currLikedInstrument = GameProperties.Instrument.DRUMS;
                    break;
                case "VOCALS":
                    currLikedInstrument = GameProperties.Instrument.VOCALS;
                    break;
                case "KEYBOARD":
                    currLikedInstrument = GameProperties.Instrument.KEYBOARD;
                    break;
                case "BASS":
                    currLikedInstrument = GameProperties.Instrument.BASS;
                    break;
                default:
                    currLikedInstrument = GameProperties.Instrument.GUITAR;
                    //isErrorEncountered = true;
                    //setupWarningPoppupRef.DisplayPoppup("did not parse the liked instrument of player. Guitar assumed..." + currParam.name);
                    break;
            }

            switch (currParam.playerType)
            {
                case "HUMAN":
                    GameGlobals.players.Add(new UIPlayer(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name));
                    break;
                case "SIMPLE":
                    GameGlobals.players.Add(new AIPlayerSimpleStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                case "RANDOM":
                    GameGlobals.players.Add(new AIPlayerRandomStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                case "COOPERATIVE":
                    GameGlobals.players.Add(new AIPlayerCoopStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                case "GREEDY":
                    GameGlobals.players.Add(new AIPlayerGreedyStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                case "BALANCED":
                    GameGlobals.players.Add(new AIPlayerBalancedStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                case "UNBALANCED":
                    GameGlobals.players.Add(new AIPlayerUnbalancedStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                case "TITFORTAT":
                    GameGlobals.players.Add(new AIPlayerTitForTatStrategy(playerUIPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, playerWarningPoppupRef, currPlayerId++, currParam.name, currParam.isSpeechAllowed, currLikedInstrument));
                    break;
                default:
                    isErrorEncountered = true;
                    setupWarningPoppupRef.DisplayPoppup("Error on parsing the player type of " + currParam.name);
                    Debug.Log("[ERROR]: Error on parsing the player type of " + currParam.name);
                    break;
            }
        }

        switch (parameterization.ngType)
        {
            case "RANDOM":
                GameGlobals.gameDiceNG = new RandomDiceNG();
                break;
            case "FIXED:LOSS":
                GameGlobals.gameDiceNG = new LossDiceNG();
                break;
            case "FIXED:VICTORY":
                GameGlobals.gameDiceNG = new VictoryDiceNG();
                break;
            default:
                isErrorEncountered = true;
                setupWarningPoppupRef.DisplayPoppup("Error on parsing the NG Type of parameterization " + parameterization.ngType);
                Debug.Log("[ERROR]: Error on parsing the NG Type of parameterization " + parameterization.ngType);
                break;
        }
        
        //Game.gameParameterizations.Add(parameterization);
        //string json = JsonUtility.ToJson(GameProperties.configurableProperties);
    }

    void Start ()
    {
        isErrorEncountered = false;
        playerWarningPoppupRef = new PoppupScreenFunctionalities(true, null, null, poppupPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, Resources.Load<Sprite>("Textures/UI/Icons/Info"), new Color(0.9f, 0.9f, 0.9f), "Audio/snap");
        setupWarningPoppupRef = new PoppupScreenFunctionalities(true, null, null, poppupPrefab, playerCanvas, GameGlobals.monoBehaviourFunctionalities, Resources.Load<Sprite>("Textures/UI/Icons/Info"), new Color(0.9f, 0.9f, 0.9f), "Audio/snap");
        Object.DontDestroyOnLoad(playerCanvas);

       
        if (!GameProperties.configurableProperties.isSimulation)
        {

            if (GameProperties.configurableProperties.isAutomaticalBriefing)
            {
                GameProperties.currGameParameterization = GameProperties.currSessionParameterization.gameParameterizations[GameGlobals.currGameId - 1];
                StartGame();
                return;
            }


            GameParameterization manualGameParam = new GameParameterization();
            manualGameParam.playerParameterizations = new List<PlayerParameterization>();


            this.customizeLabel = GameObject.Find("Canvas/SetupScreen/customizeLabel").gameObject;

            this.UIResetButton = GameObject.Find("Canvas/SetupScreen/resetButton").gameObject.GetComponent<Button>();
            this.UINameSelectionInputBox = GameObject.Find("Canvas/SetupScreen/nameSelectionInputBox").gameObject.GetComponent<InputField>();
            this.UIStartGameButton = GameObject.Find("Canvas/SetupScreen/startGameButton").gameObject.GetComponent<Button>();
            this.UIAddPlayerButton = GameObject.Find("Canvas/SetupScreen/addPlayerGameButton").gameObject.GetComponent<Button>();

            this.UIAIPlayerSelectionButtonsObject = GameObject.Find("Canvas/SetupScreen/addAIPlayerGameButtons").gameObject;
            Button[] UIAIPlayerSelectionButtons= UIAIPlayerSelectionButtonsObject.GetComponentsInChildren<Button>();

            this.configSelectionButtonsObject = GameObject.Find("Canvas/SetupScreen/configButtons").gameObject;
            Button[] UIConfigButtons = configSelectionButtonsObject.GetComponentsInChildren<Button>();

            UIResetButton.onClick.AddListener(delegate {
                GameGlobals.players.Clear();
                foreach (Button button in UIAIPlayerSelectionButtons)
                {
                    button.interactable = true;
                }
            });


            UIStartGameButton.gameObject.SetActive(false);

            UIStartGameButton.onClick.AddListener(delegate { StartGame(); });
            UIAddPlayerButton.onClick.AddListener(delegate {
                manualGameParam.playerParameterizations.Add(new PlayerParameterization("Sam", "HUMAN"));
                CheckForAllPlayersRegistered(manualGameParam);
            });


            manualGameParam.ngType = "RANDOM";
            for (int i=0; i < UIAIPlayerSelectionButtons.Length; i++)
            {
                GameGlobals.numberOfSpeakingPlayers++;
                Button button = UIAIPlayerSelectionButtons[i];
                button.onClick.AddListener(delegate
                {
                    int index = new List<Button>(UIAIPlayerSelectionButtons).IndexOf(button);
                    switch ((GameProperties.PlayerType) (index+4))
                    {
                        //case GameProperties.PlayerType.SIMPLE:
                        //    manualGameParam.playerParameterizations.Add(new PlayerParameterization("Sam", "SIMPLE", false));
                        //    break;
                        case GameProperties.PlayerType.COOPERATIVE:
                            manualGameParam.playerParameterizations.Add(new PlayerParameterization("Cristoph", "COOPERATIVE", false));
                            break;
                        case GameProperties.PlayerType.GREEDY:
                            manualGameParam.playerParameterizations.Add(new PlayerParameterization("Giovanni", "GREEDY", false));
                            break;
                        case GameProperties.PlayerType.BALANCED:
                            manualGameParam.playerParameterizations.Add(new PlayerParameterization("Brian", "BALANCED", false));
                            break;
                        case GameProperties.PlayerType.UNBALANCED:
                            manualGameParam.playerParameterizations.Add(new PlayerParameterization("Ulrich", "UNBALANCED", false));
                            break;
                        case GameProperties.PlayerType.TITFORTAT:
                            manualGameParam.playerParameterizations.Add(new PlayerParameterization("Tim", "TITFORTAT", false));
                            break;
                    }
                    CheckForAllPlayersRegistered(manualGameParam);
                });
            }

            for (int i = 0; i < UIConfigButtons.Length; i++)
            {
                Button button = UIConfigButtons[i];
                button.onClick.AddListener(delegate
                {
                    if (button.gameObject.name.EndsWith("1"))
                    {
                        manualGameParam.playerParameterizations.Add(new PlayerParameterization("Player", "HUMAN"));
                        manualGameParam.playerParameterizations.Add(new PlayerParameterization("Player", "HUMAN"));
                        manualGameParam.playerParameterizations.Add(new PlayerParameterization("Player", "HUMAN"));
                        manualGameParam.ngType = "RANDOM";
                    }
                    else if (button.gameObject.name.EndsWith("2"))
                    {
                        manualGameParam.playerParameterizations.Add(new PlayerParameterization("Player", "COOPERATIVE", false));
                        manualGameParam.playerParameterizations.Add(new PlayerParameterization("Player", "GREEDY", false));
                        manualGameParam.playerParameterizations.Add(new PlayerParameterization("Player", "HUMAN"));
                        manualGameParam.ngType = "RANDOM";
                    }
                    button.interactable = false;
                    CheckForAllPlayersRegistered(manualGameParam);
                });
            }

        }
        else
        {
            //fetch config for simulation
            StartGame();
        }
    }
	
	void StartGame()
    {
        SetUpParameterization(GameProperties.currGameParameterization);

        if (isErrorEncountered)
        {
            return;
        }

        //write players in log before starting the game
        Player currPlayer = null;
        for (int i = 0; i < GameProperties.configurableProperties.numberOfPlayersPerGame; i++)
        {
            currPlayer = GameGlobals.players[i];
            currPlayer.RegisterMeOnPlayersLog();
        }

        string json = JsonUtility.ToJson(GameProperties.configurableProperties);

        GameSceneManager.LoadMainScene();
    }

    void CheckForAllPlayersRegistered(GameParameterization param)
    {
        UINameSelectionInputBox.text = "";
        if (param.playerParameterizations.Count == GameProperties.configurableProperties.numberOfPlayersPerGame)
        {
            GameProperties.currGameParameterization = param;

            UIStartGameButton.gameObject.SetActive(true);
            customizeLabel.gameObject.SetActive(false);
            UIAddPlayerButton.gameObject.SetActive(false);
            UINameSelectionInputBox.gameObject.SetActive(false);

            UIAIPlayerSelectionButtonsObject.SetActive(false);
            configSelectionButtonsObject.SetActive(false);
            UIResetButton.gameObject.SetActive(false);
        }
    }
    
}
