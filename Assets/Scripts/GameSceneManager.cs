using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSceneManager{

    private static SceneTransitionEffect transitionEffect = new Fade(Color.black);

    public static void LoadStartScene()
    {
        transitionEffect.LoadSceneWithEffect("startScene", LoadSceneMode.Single);
    }
    public static void LoadPlayersSetupScene()
    {
        transitionEffect.LoadSceneWithEffect("playersSetupScene", LoadSceneMode.Single);
    }
    public static void LoadMainScene()
    {
        transitionEffect.LoadSceneWithEffect("mainScene", LoadSceneMode.Single);
    }
    public static void LoadEndScene()
    {
        transitionEffect.LoadSceneWithEffect("endScene", LoadSceneMode.Single);
    }
}
