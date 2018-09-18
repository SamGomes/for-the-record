using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreenFunctionalities : MonoBehaviour
{

    public GameObject poppupPrefab;


    public Button UIRestartGameButton;
    public Button UIEndGameButton;

    public GameObject UIAlbumCollectionDisplay;
    public GameObject UIIndividualTable;
    public GameObject UIIndividualTableEntryPrefab;

    public GameObject UIVictoryOverlay;
    public GameObject UILossOverlay;
    public GameObject UIFinishedGameOverlay;

    public GameObject mainScene;


    public GameObject albumUIPrefab;

    private int StopAllAnimations()
    {
        Animator[] mainSceneAnimators = mainScene.GetComponentsInChildren<Animator>();
        for (int i = 0; i < mainSceneAnimators.Length; i++)
        {
            mainSceneAnimators[i].speed = 0;
        }
        return 0;
    }
    private int PlayAllAnimations()
    {
        Animator[] mainSceneAnimators = mainScene.GetComponentsInChildren<Animator>();
        for (int i = 0; i < mainSceneAnimators.Length; i++)
        {
            mainSceneAnimators[i].speed = 1;
        }
        return 0;
    }

    private void RestartGame()
    {
        foreach(Album album in GameGlobals.albums)
        {
            Object.Destroy(album.GetAlbumUI());
        }
        GameSceneManager.LoadStartScene();
        Debug.Log("numGamesToSimulate: " + GameProperties.numGamesToSimulate);
    }

    private IEnumerator LoadMainScreenAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (!GameProperties.isSimulation)
        {
            mainScene.SetActive(true);
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

        UIEndGameButton.onClick.AddListener(delegate () {
            //mainScene.SetActive(false);
            UIFinishedGameOverlay.SetActive(true);
            StopAllAnimations();
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

        PoppupScreenFunctionalities infoPoppupNeutralRef = new PoppupScreenFunctionalities(false,StopAllAnimations,PlayAllAnimations,poppupPrefab, mainScene, this.GetComponent<PlayerMonoBehaviourFunctionalities>(), Resources.Load<Sprite>("Textures/UI/Icons/Info"), new Color(0.9f, 0.9f, 0.9f));


        ////mock
        //GameGlobals.currSessionId = System.DateTime.Now.ToString("yyyy/MM/dd/HH-mm-ss");
        //GameGlobals.gameLogManager.InitLogs();
        //GameGlobals.albums = new System.Collections.Generic.List<Album>(GameProperties.numberOfAlbumsPerGame);
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
        //GameGlobals.players = new System.Collections.Generic.List<Player>(GameProperties.numberOfPlayersPerGame);
        //GameGlobals.players.Add(new UIPlayer("PL1"));
        //GameGlobals.players.Add(new UIPlayer("PL2"));
        //GameGlobals.players.Add(new UIPlayer("PL3"));
        //GameGlobals.currGameState = GameProperties.GameState.VICTORY;
        //GameGlobals.currGameId = 1;

        UIVictoryOverlay.SetActive(false);
        UILossOverlay.SetActive(false);
        UIFinishedGameOverlay.SetActive(false);
        mainScene.SetActive(false);

        int numAlbumsPlayed = GameGlobals.albums.Count;
        for (int i = 0; i < numAlbumsPlayed; i++)
        {
            GameGlobals.albums[i].GetAlbumUI().SetActive(false);
        }


        if (GameGlobals.currGameState == GameProperties.GameState.VICTORY)
        {
            UIVictoryOverlay.SetActive(true);
        }
        else if(GameGlobals.currGameState == GameProperties.GameState.LOSS)
        {
            UILossOverlay.SetActive(true);

        }
        else
        {
            Debug.Log("[ERROR]: Game state returned NON FINISHED on game end!");
            return;
        }
        StartCoroutine(LoadMainScreenAfterDelay(5.0f));


        GameGlobals.players.Sort(SortPlayersByMoney);
        int numPlayers = GameGlobals.players.Count;
        for (int i=0; i<numPlayers; i++)
        {
            Player currPlayer = GameGlobals.players[i];
            GameObject newTableEntry = Object.Instantiate(UIIndividualTableEntryPrefab, UIIndividualTable.transform);
            newTableEntry.GetComponentsInChildren<Text>()[0].text = currPlayer.GetName();
            newTableEntry.GetComponentsInChildren<Text>()[1].text = currPlayer.GetMoney().ToString();
        }

        GameGlobals.gameLogManager.WriteGameToLog(GameGlobals.currSessionId.ToString(),GameGlobals.currGameId.ToString(), GameGlobals.currGameState.ToString());

        GameGlobals.gameLogManager.EndLogs();

        if (GameProperties.isSimulation)
        {
            if (GameGlobals.currGameId == GameProperties.numGamesToSimulate)
            {
                RestartGame();
            }
        }
        else
        {
            if (GameProperties.isAutomaticalBriefing)
            {
                if (GameGlobals.currGameId == GameProperties.numGamesToPlay)
                {
                    infoPoppupNeutralRef.DisplayPoppup("You reached the end of the test game. You can choose to close the application or watch the credits. Thank you for your time!");
                    UIEndGameButton.interactable = true;
                    UIRestartGameButton.interactable = false;
                    //UIEndGameButton.GetComponent<Animator>().speed = 1;
                    //UIRestartGameButton.GetComponent<Animator>().speed = 0;
                }
                else
                {
                    infoPoppupNeutralRef.DisplayPoppup("You reached the end of the tutorial game. We assume that you are prepared for the test game. Good luck!");
                    UIRestartGameButton.interactable = true;
                    UIEndGameButton.interactable = false;
                    //UIRestartGameButton.GetComponent<Animator>().speed = 1;
                    //UIEndGameButton.GetComponent<Animator>().speed = 0;
                }
            
            }
        }

    }
}
    

