using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSceneManager{
    public static void LoadStartScene()
    {
        SceneManager.LoadScene("startScene", LoadSceneMode.Single);
    }
    public static void LoadPlayersSetupScene()
    {
        SceneManager.LoadScene("playersSetupScene", LoadSceneMode.Single);
    }
    public static void LoadMainScene()
    {
        SceneManager.LoadScene("mainScene", LoadSceneMode.Single);
    }
    public static void LoadEndScene()
    {
        SceneManager.LoadScene("endScene", LoadSceneMode.Single);
    }
}
