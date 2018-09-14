using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenFunctionalities : MonoBehaviour {

    private Button UIStartGameButton;


    private void InitGameGlobals()
    {
        GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);

        //only update session in first game
        if (GameGlobals.currGameId == 0)
        {
            GameGlobals.currSessionId = System.DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");
        }

    }

    private void StartGame()
    {
        GameGlobals.gameLogManager.InitLogs();
        //GameGlobals.playerIdCount = 0;
        //GameGlobals.albumIdCount = 0;
        GameGlobals.currGameId++;
        GameSceneManager.LoadPlayersSetupScene();
        
    }

	void Start () {

        InitGameGlobals();
        //generate external game code from currsessionid and lock it in place
        //gamecode is in the format ddmmyyhhmmssAAA

        // Match only digits 
        string pattern = @"\d";
        StringBuilder sb = new StringBuilder();

        foreach (Match m in Regex.Matches(GameGlobals.currSessionId, pattern))
        {
            sb.Append(m);
        }

        string generatedCode = sb.ToString();
        //generate 3 random letters in the end
        for(int i=0; i<3; i++)
        {
            generatedCode += (char)('A' + Random.Range(0, 26));
        }

        GameObject UIGameCodeDisplay = GameObject.Find("gameCodeDisplay").gameObject;
        UIGameCodeDisplay.GetComponentInChildren<Text>().text = "Game Code: " + generatedCode;
        Object.DontDestroyOnLoad(UIGameCodeDisplay);


        this.UIStartGameButton = GameObject.Find("Canvas/StartScreen/startGameButton").gameObject.GetComponent<Button>();
        if (!GameProperties.isSimulation)
        {
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
