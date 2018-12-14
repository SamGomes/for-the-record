using System.Collections.Generic;
using System.Threading;
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


    void Start ()
    {
        if (!GameProperties.isSimulation)
        {
            
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
                GameGlobals.players.Add(new UIPlayer(UINameSelectionInputBox.text));
                CheckForAllPlayersRegistered();
            });

            for(int i=0; i < UIAIPlayerSelectionButtons.Length; i++)
            {
                Button button = UIAIPlayerSelectionButtons[i];
                button.onClick.AddListener(delegate
                {
                    int index = new List<Button>(UIAIPlayerSelectionButtons).IndexOf(button);
                    UIPlayer newPlayer = new UIPlayer("");
                    switch ((GameProperties.AIPlayerType) (index+1))
                    {
                        case GameProperties.AIPlayerType.SIMPLE:
                            newPlayer = new AIPlayerSimple("John0");
                            break;
                        case GameProperties.AIPlayerType.COOPERATIVE:
                            newPlayer = new AIPlayerCoopStrategy("John1");
                            break;
                        case GameProperties.AIPlayerType.GREEDY:
                            newPlayer = new AIPlayerGreedyStrategy("John2");
                            break;
                        case GameProperties.AIPlayerType.BALANCED:
                            newPlayer = new AIPlayerBalancedStrategy("John3");
                            break;
                    }
                    GameGlobals.players.Add(newPlayer);
                    button.interactable = false;
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
                        GameGlobals.players.Add(new UIPlayer("Player1"));
                        GameGlobals.players.Add(new UIPlayer("Player2"));
                        GameGlobals.players.Add(new UIPlayer("Player3"));
                        GameGlobals.gameDiceNG = new RandomDiceNG();
                    }
                    else if (button.gameObject.name.EndsWith("2"))
                    {
                        GameGlobals.numberOfSpeakingPlayers = 2;
                        GameGlobals.players.Add(new RoboticPlayerGreedyStrategy(0, "Emys"));
                        GameGlobals.players.Add(new RoboticPlayerCoopStrategy(1, "Glin"));
                        GameGlobals.players.Add(new UIPlayer("Player"));
                        GameGlobals.gameDiceNG = new RandomDiceNG();
                    }
                    else if (button.gameObject.name.EndsWith("3"))
                    {
                        GameGlobals.numberOfSpeakingPlayers = 2;
                        RoboticPlayerGreedyStrategy emys = new RoboticPlayerGreedyStrategy(0, "Emys");
                        GameGlobals.players.Add(emys);
                        RoboticPlayerCoopStrategyPunisher glin = new RoboticPlayerCoopStrategyPunisher(1, "Glin");
                        GameGlobals.players.Add(glin);
                        GameGlobals.players.Add(new UIPlayer("Player"));
                        GameGlobals.gameDiceNG = new LossDiceNG();
                        emys.FlushRobotUtterance("<gaze(Player)> Eu sou o émys!");
                        Thread.Sleep(1000);
                        glin.FlushRobotUtterance("<gaze(Player)> E eu sou o Glin! Vamos lá formar uma banda e ver se conseguimos triunfar!");
                    }
                    else if (button.gameObject.name.EndsWith("4"))
                    {
                        GameGlobals.numberOfSpeakingPlayers = 2;
                        RoboticPlayerGreedyStrategyPunisher emys = new RoboticPlayerGreedyStrategyPunisher(0, "Emys");
                        GameGlobals.players.Add(emys);
                        RoboticPlayerCoopStrategy glin = new RoboticPlayerCoopStrategy(1, "Glin");
                        GameGlobals.players.Add(glin);
                        GameGlobals.players.Add(new UIPlayer("Player"));
                        GameGlobals.gameDiceNG = new LossDiceNG();
                        emys.FlushRobotUtterance("<gaze(Player)> Eu sou o émys!");
                        Thread.Sleep(1000);
                        glin.FlushRobotUtterance("<gaze(Player)> E eu sou o Glin! Vamos lá formar uma banda e ver se conseguimos triunfar!");
                    }
                    /*else if (button.gameObject.name.EndsWith("3"))
                    {
                        GameGlobals.numberOfSpeakingPlayers = 2;
                        RoboticPlayerGreedyStrategy emys = new RoboticPlayerGreedyStrategy(0, "Emys");
                        GameGlobals.players.Add(emys);
                        RoboticPlayerCoopStrategy glin = new RoboticPlayerCoopStrategy(1, "Glin");
                        GameGlobals.players.Add(glin);
                        GameGlobals.players.Add(new UIPlayer("Player"));
                        GameGlobals.gameDiceNG = new VictoryDiceNG();
                        emys.FlushRobotUtterance("<gaze(Player)> Eu sou o émys!");
                        Thread.Sleep(1000);
                        glin.FlushRobotUtterance("<gaze(Player)> E eu sou o Glin! Vamos lá formar uma banda e ver se conseguimos triunfar!");
                    }
                    else if (button.gameObject.name.EndsWith("4"))
                    {
                        GameGlobals.numberOfSpeakingPlayers = 2;
                        RoboticPlayerGreedyStrategy emys = new RoboticPlayerGreedyStrategy(0, "Emys");
                        GameGlobals.players.Add(emys);
                        RoboticPlayerCoopStrategy glin = new RoboticPlayerCoopStrategy(1, "Glin");
                        GameGlobals.players.Add(glin);
                        GameGlobals.players.Add(new UIPlayer("Player"));
                        GameGlobals.gameDiceNG = new LossDiceNG();
                        emys.FlushRobotUtterance("<gaze(Player)> Eu sou o émys!");
                        Thread.Sleep(1000);
                        glin.FlushRobotUtterance("<gaze(Player)> E eu sou o Glin! Vamos lá formar uma banda e ver se conseguimos triunfar!");
                    }*/
                    button.interactable = false;
                    CheckForAllPlayersRegistered();
                });
            }

        }
        else
        {
            GameGlobals.players.Add(new AIPlayerCoopStrategy("PL1"));
            GameGlobals.players.Add(new AIPlayerCoopStrategy("PL2"));
            GameGlobals.players.Add(new AIPlayerCoopStrategy("PL3"));
            GameGlobals.gameDiceNG = new VictoryDiceNG();
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
