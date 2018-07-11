using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSetupSceneFunctionalities : MonoBehaviour {

    private InputField UINameSelectionInputBox;
    private Button UIStartGameButton;
    private Button UIAddPlayerButton;

    private Dropdown UIAIPlayerSelectionDropdown;
    private Button UIAddAIPlayerButton;


    void Start ()
    {
        if (!GameProperties.isSimulation)
        {
            this.UINameSelectionInputBox = GameObject.Find("Canvas/SetupScreen/nameSelectionInputBox").gameObject.GetComponent<InputField>();
            this.UIStartGameButton = GameObject.Find("Canvas/SetupScreen/startGameButton").gameObject.GetComponent<Button>();
            this.UIAddPlayerButton = GameObject.Find("Canvas/SetupScreen/addPlayerGameButton").gameObject.GetComponent<Button>();

            this.UIAIPlayerSelectionDropdown = GameObject.Find("Canvas/SetupScreen/AIPlayerSelectionDropdown").gameObject.GetComponent<Dropdown>();
            this.UIAddAIPlayerButton = GameObject.Find("Canvas/SetupScreen/addAIPlayerGameButton").gameObject.GetComponent<Button>();


            UIStartGameButton.gameObject.SetActive(false);

            foreach(string type in System.Enum.GetNames(typeof(GameProperties.AIPlayerType)))
            {
                UIAIPlayerSelectionDropdown.options.Add(new Dropdown.OptionData(type));
            }

            UIStartGameButton.onClick.AddListener(delegate { StartGame(); });
            UIAddPlayerButton.onClick.AddListener(delegate
            {
                GameGlobals.players.Add(new UIPlayer(UINameSelectionInputBox.text));
                CheckForAllPlayersRegistered();
            });
            UIAddAIPlayerButton.onClick.AddListener(delegate
            {
                string playerName = UINameSelectionInputBox.text;
                AIPlayer newPlayer = new AIPlayer(playerName);
                switch ((GameProperties.AIPlayerType) UIAIPlayerSelectionDropdown.value)
                {
                    case GameProperties.AIPlayerType.SIMPLE:
                        newPlayer = new AIPlayerSimple(playerName);
                        break;
                    case GameProperties.AIPlayerType.COOPERATIVE:
                        newPlayer = new AIPlayerCoopStrategy(playerName);
                        break;
                    case GameProperties.AIPlayerType.GREEDY:
                        newPlayer = new AIPlayerGreedyStrategy(playerName);
                        break;
                    case GameProperties.AIPlayerType.BALANCED:
                        newPlayer = new AIPlayerBalancedStrategy(playerName);
                        break;
                }
                GameGlobals.players.Add(newPlayer);
                CheckForAllPlayersRegistered();
            });
        }
        else
        {
            GameGlobals.players.Add(new AIPlayerGreedyStrategy("PL1"));
            GameGlobals.players.Add(new AIPlayerGreedyStrategy("PL2"));
            GameGlobals.players.Add(new AIPlayerGreedyStrategy("PL3"));
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
            UIAddPlayerButton.gameObject.SetActive(false);
            UINameSelectionInputBox.gameObject.SetActive(false);

            UIAIPlayerSelectionDropdown.gameObject.SetActive(false);
            UIAddAIPlayerButton.gameObject.SetActive(false);
        }
    }
    
}
