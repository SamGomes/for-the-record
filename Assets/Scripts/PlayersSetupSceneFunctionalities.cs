using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersSetupSceneFunctionalities : MonoBehaviour {

    private InputField UINameSelectionInputBox;
    private Button UIStartGameButton;
    private Button UIAddPlayerButton;
    private Button UIResetButton;

    private GameObject UIAIPlayerSelectionButtonsObject;


    void Start ()
    {
        if (!GameProperties.isSimulation)
        {
            this.UIResetButton = GameObject.Find("Canvas/SetupScreen/resetButton").gameObject.GetComponent<Button>();
            this.UINameSelectionInputBox = GameObject.Find("Canvas/SetupScreen/nameSelectionInputBox").gameObject.GetComponent<InputField>();
            this.UIStartGameButton = GameObject.Find("Canvas/SetupScreen/startGameButton").gameObject.GetComponent<Button>();
            this.UIAddPlayerButton = GameObject.Find("Canvas/SetupScreen/addPlayerGameButton").gameObject.GetComponent<Button>();

            this.UIAIPlayerSelectionButtonsObject = GameObject.Find("Canvas/SetupScreen/addAIPlayerGameButtons").gameObject;
            Button[] UIAIPlayerSelectionButtons= UIAIPlayerSelectionButtonsObject.GetComponentsInChildren<Button>();

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
           
        }
        else
        {
            GameGlobals.players.Add(new AIPlayerBalancedStrategy("PL1"));
            GameGlobals.players.Add(new AIPlayerCoopStrategy("PL2"));
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

            UIAIPlayerSelectionButtonsObject.SetActive(false);
            UIResetButton.gameObject.SetActive(false);
        }
    }
    
}
