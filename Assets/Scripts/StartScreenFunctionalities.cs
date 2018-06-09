using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScreenFunctionalities : MonoBehaviour {

    public Button UIStartGameButton;
    
	void Start () {
        if (!GameProperties.isSimulation)
        {
            UIStartGameButton.onClick.AddListener(delegate () { GameSceneManager.LoadMainScene(); });
        }
        else
        {
            GameSceneManager.LoadMainScene();
            FileManager.InitWriter();
        }
	}
}
