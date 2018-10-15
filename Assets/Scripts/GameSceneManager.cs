using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSceneManager{

    private static SceneTransitionEffect transitionEffect = new Fade(Color.black);

    public static void LoadStartScene()
    {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            transitionEffect.LoadSceneWithEffect("startScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("startScene", LoadSceneMode.Single);
        }
    }
    public static void LoadPlayersSetupScene()
    {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            transitionEffect.LoadSceneWithEffect("playersSetupScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("playersSetupScene", LoadSceneMode.Single);
        }
    }
    public static void LoadMainScene()
    {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            transitionEffect.LoadSceneWithEffect("mainScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("mainScene", LoadSceneMode.Single);
        }
    }
    public static void LoadEndScene()
    {
        if (!GameProperties.configurableProperties.isSimulation)
        {
            transitionEffect.LoadSceneWithEffect("endScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("endScene", LoadSceneMode.Single);
        }
    }
}
