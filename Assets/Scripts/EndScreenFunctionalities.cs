using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public Button UIRestartGameButton;
    public GameObject UIAlbumCollectionDisplay;

    public GameObject UIVictoryOverlay;
    public GameObject UILossOverlay;



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

    private IEnumerator HideAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }


    // Use this for initialization
    void Start()
    {
        UIVictoryOverlay.SetActive(false);
        UILossOverlay.SetActive(false);

        if (GameGlobals.currGameState == GameProperties.GameState.VICTORY)
        {
            UIVictoryOverlay.SetActive(true);
            StartCoroutine(HideAfterDelay(UIVictoryOverlay, 5.0f));
        }
        else if(GameGlobals.currGameState == GameProperties.GameState.LOSS)
        {
            UILossOverlay.SetActive(true);
            StartCoroutine(HideAfterDelay(UILossOverlay, 5.0f));

        }
        else
        {
            Debug.Log("[ERROR]: Game state returned NON FINISHED on game end!");
            return;
        }

        if (!GameProperties.isSimulation || (GameProperties.isSimulation && GameProperties.numGamesToSimulate > 0))
        {
            foreach (Album album in GameGlobals.albums)
            {
                album.GetAlbumUI().transform.SetParent(UIAlbumCollectionDisplay.transform);
                album.GetAlbumUI().transform.localPosition = new Vector3(0, 0, 0);
            }
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
    

