using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public Button UIRestartGameButton;

    public GameObject UIAlbumCollectionDisplay;
    public GameObject UIIndividualTable;
    public GameObject UIIndividualTableEntryPrefab;

    public GameObject UIVictoryOverlay;
    public GameObject UILossOverlay;


    public GameObject albumUIPrefab;

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
        if (!GameProperties.isSimulation)
        {
            LoadEndScreenUIElements();
        }
    }

    private void LoadEndScreenUIElements()
    {
        if (GameGlobals.albums != null)
        {
            //disable rendering of some albums and selecting others
            int numAlbumsPlayed = GameGlobals.albums.Count;
            for (int i = 0; i < numAlbumsPlayed; i++)
            {
                Album currAlbum = GameGlobals.albums[i];
                GameObject currAlbumUI = currAlbum.GetAlbumUI();
                if ((numAlbumsPlayed - i) > 5) //maximum of 5 last albums rendered albums
                {
                    currAlbumUI.SetActive(false);
                    continue;
                }
                currAlbumUI.SetActive(true);

                Animator animator = currAlbumUI.GetComponentInChildren<Animator>();
                animator.Rebind();
                animator.Play(0);
                animator.speed = (i * 0.1f < animator.speed) ? animator.speed - i * 0.1f : animator.speed;

                currAlbumUI.transform.SetParent(UIAlbumCollectionDisplay.transform);
                currAlbumUI.transform.localPosition = new Vector3(0, 0, 0);
                currAlbumUI.transform.localScale = new Vector3(1, 1, 1);

                currAlbumUI.transform.Translate(new Vector3(i * 50.0f, 0, 0));
            }
        }

        UIRestartGameButton.onClick.AddListener(delegate () {
            RestartGame();
        });

    }

    //in order to sort the players list by money earned
    public int SortPlayersByMoney(Player p1, Player p2)
    {

        return -1*(p1.GetMoney()).CompareTo(p2.GetMoney());
    }


    // Use this for initialization
    void Start()
    {
        //mock
        //GameGlobals.albums = new List<Album>(GameProperties.numberOfAlbumsPerGame);
        //Album newAlbum = new Album("1", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("2", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("3", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("4", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("5", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("6", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //newAlbum = new Album("7", albumUIPrefab);
        //GameGlobals.albums.Add(newAlbum);
        //GameGlobals.players = new List<Player>(GameProperties.numberOfPlayersPerGame);
        //GameGlobals.players.Add(new UIPlayer("PL1"));
        //GameGlobals.players.Add(new UIPlayer("PL2"));
        //GameGlobals.players.Add(new UIPlayer("PL3"));


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
        StartCoroutine(HideAfterDelay(UILossOverlay, 0.0f));

        //else
        //{
        //    Debug.Log("[ERROR]: Game state returned NON FINISHED on game end!");
        //    return;
        //}

        GameGlobals.players.Sort(SortPlayersByMoney);
        int numPlayers = GameGlobals.players.Count;
        for (int i=0; i<numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            GameObject newTableEntry = Object.Instantiate(UIIndividualTableEntryPrefab, UIIndividualTable.transform);
            newTableEntry.GetComponentsInChildren<Text>()[0].text = currPlayer.GetName();
            newTableEntry.GetComponentsInChildren<Text>()[1].text = currPlayer.GetMoney().ToString();
        }


        if (GameProperties.isSimulation)
        {
            RestartGame();
        }

    }
}
    

