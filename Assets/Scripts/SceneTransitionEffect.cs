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

    private Image transitionImage;
    private bool isFading;

    public Fade(Color transitionColor) : base()
    {
        currFadeColor = transitionColor;
        currFadeColor.a = 0.0f;
        fadeRate = 0.05f;

        transitionImage = transitionScreen.GetComponentInChildren<Image>();
    }

    public override void LoadSceneWithEffect(string sceneName, LoadSceneMode mode)
    {
        transitionScreen.gameObject.SetActive(true);
        if (!isFading)
        {
            transitionImage.StartCoroutine(ApplyFade(sceneName, mode));
        }
        else //do not fade again just load scene
        {
            SceneManager.LoadScene(sceneName, mode);
        }
    }
   
    public void DisableFade()
    {
        transitionScreen.gameObject.SetActive(false);
    }

    //animation fixed update time of 24 fps
    IEnumerator ApplyFade(string sceneName, LoadSceneMode mode)
    {
       
        isFading = true;
        yield return ApplyFadeIn();
        SceneManager.LoadScene(sceneName, mode);
        yield return ApplyFadeOut();
        isFading = false;
    }
    IEnumerator ApplyFadeIn() {
        while (currFadeColor.a <= 1.0f)
        {
            currFadeColor.a += fadeRate;
            transitionImage.color = currFadeColor;
            yield return new WaitForSeconds(0.0417f);
        }
    }
    IEnumerator ApplyFadeOut()
    {
        while (currFadeColor.a >= 0.0f)
        {
            currFadeColor.a -= fadeRate;
            transitionImage.color = currFadeColor;
            yield return new WaitForSeconds(0.0417f);
        }
    }

}

public class AnimatedEffect : SceneTransitionEffect {

    public AnimatedEffect(Animator animator) : base()
    {
        transitionScreen.gameObject.AddComponent<Animator>().runtimeAnimatorController = animator.runtimeAnimatorController;

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
        yield return PlayAnimationFirstHalf();
        SceneManager.LoadScene(sceneName, mode);
        yield return PlayAnimationSecondHalf();
    }
    IEnumerator PlayAnimationFirstHalf()
    {
        transitionScreen.gameObject.GetComponent<Animator>().Play(0);
        return null;
    }
    IEnumerator PlayAnimationSecondHalf()
    {
        transitionScreen.gameObject.GetComponent<Animator>().Play(0);
        return null;
    }


}