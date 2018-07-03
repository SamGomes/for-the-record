using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public abstract class SceneTransitionEffect
{
    protected GameObject transitionScreen;

    public SceneTransitionEffect()
    {
        transitionScreen = Object.Instantiate(Resources.Load<GameObject>("Prefabs/TransitionScreen"));
        Object.DontDestroyOnLoad(transitionScreen);
    }
    public abstract void LoadSceneWithEffect(string sceneName, LoadSceneMode mode);
}


public class Fade: SceneTransitionEffect{

    private float fadeRate;
    private Color currFadeColor;

    public Fade(Color transitionColor) : base()
    {
        currFadeColor = transitionScreen.GetComponent<Image>().color;
        currFadeColor.a = 0.0f;
        fadeRate = 0.05f;
        
        //this.gameObject.SetActive(false);
    }

    public override void LoadSceneWithEffect(string sceneName, LoadSceneMode mode)
    {
        transitionScreen.gameObject.SetActive(true);
        transitionScreen.GetComponent<Image>().StartCoroutine(ApplyFade(sceneName, mode));
    }
   
    public void DisableFade()
    {
        transitionScreen.gameObject.SetActive(false);
    }

    //animation fixed update time of 24 fps
    IEnumerator ApplyFade(string sceneName, LoadSceneMode mode)
    {
        yield return ApplyFadeIn();
        SceneManager.LoadScene(sceneName, mode);
        yield return ApplyFadeOut();
    }
    IEnumerator ApplyFadeIn() {
        while (currFadeColor.a <= 1.0f)
        {
            currFadeColor.a += fadeRate;
            transitionScreen.gameObject.GetComponent<Image>().color = currFadeColor;
            yield return new WaitForSeconds(0.0417f);
        }
    }
    IEnumerator ApplyFadeOut()
    {
        while (currFadeColor.a >= 0.0f)
        {
            currFadeColor.a -= fadeRate;
            transitionScreen.gameObject.GetComponent<Image>().color = currFadeColor;
            yield return new WaitForSeconds(0.0417f);
        }
    }

}
