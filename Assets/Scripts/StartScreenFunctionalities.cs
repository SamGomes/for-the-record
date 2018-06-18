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
            FileManager.InitWriter();
        }
        GameGlobals.currGameId++;
        GameSceneManager.LoadPlayersSetupScene();
    }

	void Start () {
        this.UIStartGameButton = GameObject.Find("Canvas/StartScreen/startGameButton").gameObject.GetComponent<Button>();
        if (!GameProperties.isSimulation)
        {
            UIStartGameButton.onClick.AddListener(delegate () { StartGame(); });
        }
        else
        {
            StartGame();
        }
	}
}
