using AssetManagerPackage;
using FAtiMAScripts;

using IntegratedAuthoringTool;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenFunctionalities : MonoBehaviour {

    private Button UIStartGameButton;
    public GameObject UIGameCodeDisplayPrefab;

    private void InitGameGlobals()
    {
        GameGlobals.numberOfSpeakingPlayers = 0;
        GameGlobals.currGameId++;
        GameGlobals.currGameRoundId = 0;
        GameGlobals.albumIdCount = 0;

        GameGlobals.gameLogManager = new DebugLogManager();
        GameGlobals.audioManager = new AudioManager();


        GameGlobals.gameLogManager.InitLogs();
        //GameGlobals.playerIdCount = 0;
        //GameGlobals.albumIdCount = 0;

        GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        
        //destroy UIs if any
        if (GameGlobals.players!=null && GameGlobals.players.Count > 0)
        {
            UIPlayer firstUIPlayer = null;
            int pIndex = 0;
            while (firstUIPlayer == null && pIndex < GameGlobals.players.Count)
            {
                firstUIPlayer = (UIPlayer)GameGlobals.players[pIndex++];
                if (firstUIPlayer != null)
                {
                    firstUIPlayer.GetWarningScreenRef().DestroyPoppupPanel();
                    Destroy(firstUIPlayer.GetPlayerCanvas());
                }
            }

        }
        GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);


        //only generate session data in the first game
        if (GameGlobals.currGameId == 1)
        {

            string date = System.DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");


            //generate external game code from currsessionid and lock it in place
            //gamecode is in the format ddmmyyhhmmssAAA

            // Match only digits 
            string pattern = @"\d";
            StringBuilder sb = new StringBuilder();

            foreach (Match m in Regex.Matches(date, pattern))
            {
                sb.Append(m);
            }

            string generatedCode = sb.ToString();
            //generate 3 random letters in the end
            for (int i = 0; i < 3; i++)
            {
                generatedCode += (char)('A' + Random.Range(0, 26));
            }
            GameGlobals.currSessionId = generatedCode;

            //update the gamecode UI
            GameObject UIGameCodeDisplay = Object.Instantiate(UIGameCodeDisplayPrefab);
            UIGameCodeDisplay.GetComponentInChildren<Text>().text = "Game Code: " + GameGlobals.currSessionId;
            Object.DontDestroyOnLoad(UIGameCodeDisplay);
        }

        //init fatima strings
        GameGlobals.FAtiMAScenarioPath = "/Scenarios/ForTheRecord.iat";

        Application.ExternalEval("console.log(\"GameGlobals.FAtiMAScenarioPath: " + GameGlobals.FAtiMAScenarioPath + "\")");

        AssetManager.Instance.Bridge = new AssetManagerBridge();
        GameGlobals.FAtiMAIat = IntegratedAuthoringToolAsset.LoadFromFile(GameGlobals.FAtiMAScenarioPath);
    }




    private void StartGame()
    {
        GameSceneManager.LoadPlayersSetupScene();
    }

	void Start()
    {
        InitGameGlobals();

        
        this.UIStartGameButton = GameObject.Find("Canvas/StartScreen/startGameButton").gameObject.GetComponent<Button>();
        if (!GameProperties.isSimulation)
        {
            if (GameProperties.isAutomaticalBriefing)
            {
                Text startButtonText = UIStartGameButton.GetComponentInChildren<Text>();
                if (GameGlobals.currGameId == 1)
                {
                    startButtonText.text = "Start Tutorial Game";
                }
                else
                {
                    startButtonText.text = "Start Experiment Game";
                }
            }


            //play theme song
            //GameGlobals.audioManager.PlayInfinitClip("Audio/theme/themeIntro", "Audio/theme/themeLoop");
            UIStartGameButton.onClick.AddListener(delegate () { StartGame(); });
        }
        else
        {
            StartGame();
        }
	}
}
