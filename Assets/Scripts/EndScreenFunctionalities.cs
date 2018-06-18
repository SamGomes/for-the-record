using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public Button UIRestartGameButton;
    public GameObject UIAlbumCollectionDisplay;

    private void RestartGame()
    {
        GameSceneManager.LoadStartScene();
        Debug.Log("numGamesToSimulate: " + GameProperties.numGamesToSimulate);

        GameProperties.numGamesToSimulate--;
        if (GameProperties.numGamesToSimulate == 0)
        {
            FileManager.CloseWriter();
        }
    }

    // Use this for initialization
    void Start()
    {
        if (!(GameProperties.isSimulation && GameProperties.numGamesToSimulate > 0))
        {
            UIRestartGameButton.onClick.AddListener(delegate () {
                RestartGame();
            });
        }
        else
        {
            RestartGame();
        }
    }
}
    

