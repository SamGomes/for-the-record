using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSetupSceneFunctionalities : MonoBehaviour {

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
    

    void ConfigureAllHumanPlayers()
    {
        GameGlobals.players.Add(new UIPlayer(playerUIPrefab,playerCanvas, playerWarningPoppupRef, 0,"Player1"));
        GameGlobals.players.Add(new UIPlayer(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 1, "Player2"));
        GameGlobals.players.Add(new UIPlayer(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 2,"Player3"));
    }
    void ConfigureAITestRRH()
    {
        GameGlobals.numberOfSpeakingPlayers = 0;
        AIPlayerRandomStrategy emys = new AIPlayerRandomStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 0, "Emys", false);
        GameGlobals.players.Add(emys);
        AIPlayerRandomStrategy glin = new AIPlayerRandomStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 1, "Glin", false);
        GameGlobals.players.Add(glin);
        GameGlobals.players.Add(new UIPlayer(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 2, "Player"));
        emys.FlushAIUtterance("<gaze(Player)> Eu sou o émys!");
        glin.FlushAIUtterance("<gaze(Player)> E eu sou o Glin! Vamos lá formar uma banda e ver se conseguimos triunfar!");
    }
    void ConfigureAITestGCH()
    {
        GameGlobals.numberOfSpeakingPlayers = 2;
        AIPlayerGreedyStrategy emys = new AIPlayerGreedyStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 0, "Emys", true);
        GameGlobals.players.Add(emys);
        AIPlayerCoopStrategy glin = new AIPlayerCoopStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 1, "Glin", true);
        GameGlobals.players.Add(glin);
        GameGlobals.players.Add(new UIPlayer(playerUIPrefab, playerCanvas, playerWarningPoppupRef, 2,"Player"));
        emys.FlushAIUtterance("<gaze(Player)> Eu sou o émys!");
        glin.FlushAIUtterance("<gaze(Player)> E eu sou o Glin! Vamos lá formar uma banda e ver se conseguimos triunfar!");
    }



    void ConfigureConditionA()
    {
        ConfigureAITestGCH();
        GameGlobals.gameDiceNG = new LossDiceNG();
    }
    void ConfigureConditionB()
    {
        ConfigureAITestGCH();
        GameGlobals.gameDiceNG = new VictoryDiceNG();
    }


    void Start ()
    {
        if (!GameProperties.isSimulation)
        {
            playerWarningPoppupRef = new PoppupScreenFunctionalities(true, null, null, poppupPrefab, playerCanvas, this.GetComponent<PlayerMonoBehaviourFunctionalities>(), Resources.Load<Sprite>("Textures/UI/Icons/Info"), new Color(0.9f, 0.9f, 0.9f), "Audio/snap");
            Object.DontDestroyOnLoad(playerCanvas);

            if (GameProperties.isAutomaticalBriefing)
            {
                if (GameGlobals.currGameId == 1) //gameId starts in 1, 1 is the first game (tutorial)
                {
                    ConfigureAITestRRH();
                    GameGlobals.gameDiceNG = new RandomDiceNG();
                }
                else if (GameGlobals.currGameId == 2)
                {
                    string gameCode = GameGlobals.currSessionId;
                    //choose right parameterization for test game
                    switch (GameProperties.testGameParameterization)
                    {
                        case 'A':
                            ConfigureConditionA();
                            break;
                        case 'B':
                            ConfigureConditionB();
                            break;
                    }
                }
                StartGame();
                return;
            }

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
                GameGlobals.players.Add(new UIPlayer(playerUIPrefab, playerCanvas, playerWarningPoppupRef, GameGlobals.players.Count,UINameSelectionInputBox.text));
                CheckForAllPlayersRegistered();
            });

            for(int i=0; i < UIAIPlayerSelectionButtons.Length; i++)
            {
                GameGlobals.numberOfSpeakingPlayers++;
                Button button = UIAIPlayerSelectionButtons[i];
                button.onClick.AddListener(delegate
                {
                    int index = new List<Button>(UIAIPlayerSelectionButtons).IndexOf(button);
                    UIPlayer newPlayer = new UIPlayer(0,"");
                    switch ((GameProperties.AIPlayerType) (index+2))
                    {
                        case GameProperties.AIPlayerType.SIMPLE:
                            newPlayer = new AIPlayerSimple(playerUIPrefab, playerCanvas, playerWarningPoppupRef, GameGlobals.players.Count, "John0", true);
                            break;
                        case GameProperties.AIPlayerType.COOPERATIVE:
                            newPlayer = new AIPlayerCoopStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, GameGlobals.players.Count, "John1", true);
                            break;
                        case GameProperties.AIPlayerType.GREEDY:
                            newPlayer = new AIPlayerGreedyStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, GameGlobals.players.Count,"John2", true);
                            break;
                        case GameProperties.AIPlayerType.BALANCED:
                            newPlayer = new AIPlayerBalancedStrategy(playerUIPrefab, playerCanvas, playerWarningPoppupRef, GameGlobals.players.Count,"John3", true);
                            break;
                    }
                    GameGlobals.players.Add(newPlayer);
                    GameGlobals.gameDiceNG = new RandomDiceNG();
                    CheckForAllPlayersRegistered();
                });
            }

            for (int i = 0; i < UIConfigButtons.Length; i++)
            {
                Button button = UIConfigButtons[i];
                button.onClick.AddListener(delegate
                {
                    if (button.gameObject.name.EndsWith("1"))
                    {
                        ConfigureAllHumanPlayers();
                        GameGlobals.gameDiceNG = new RandomDiceNG();
                    }
                    else if (button.gameObject.name.EndsWith("2"))
                    {
                        ConfigureAITestRRH();
                        GameGlobals.gameDiceNG = new RandomDiceNG();
                    }
                    else if (button.gameObject.name.EndsWith("3"))
                    {
                        ConfigureConditionA();
                    }
                    else if (button.gameObject.name.EndsWith("4"))
                    {
                        ConfigureConditionB();
                    }
                    button.interactable = false;
                    CheckForAllPlayersRegistered();
                });
            }

        }
        else
        {
            GameGlobals.players.Add(new AIPlayerGreedyStrategy(GameGlobals.players.Count,"PL1"));
            GameGlobals.players.Add(new AIPlayerBalancedStrategy(GameGlobals.players.Count,"PL2"));
            GameGlobals.players.Add(new AIPlayerCoopStrategy(GameGlobals.players.Count,"PL3"));
            GameGlobals.gameDiceNG = new RandomDiceNG();
            StartGame();
        }
    }
	
	void StartGame()
    {
        GameSceneManager.LoadMainScene();
    }

    void CheckForAllPlayersRegistered()
    {
        UINameSelectionInputBox.text = "";
        if (GameGlobals.players.Count == GameProperties.numberOfPlayersPerGame)
        {
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
