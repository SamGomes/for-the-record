using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenFunctionalities : MonoBehaviour {

    private Button UIStartGameButton;


    private void InitGameGlobals()
    {
        GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);
    }

    private void StartGame()
    {
        InitGameGlobals();
        if (GameGlobals.currGameId == 0)
        {
            GameGlobals.gameLogManager.InitLogs();
        }
        //GameGlobals.playerIdCount = 0;
        //GameGlobals.albumIdCount = 0;
        GameGlobals.currGameId++;
        GameSceneManager.LoadPlayersSetupScene();
    }

	void Start () {
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
