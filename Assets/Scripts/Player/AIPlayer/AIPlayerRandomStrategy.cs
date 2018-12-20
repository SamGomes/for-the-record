using UnityEngine;
using UnityEngine.UI;



public class AIPlayerRandomStrategy : AIPlayer
{
    public AIPlayerRandomStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : this(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, GameProperties.Instrument.NONE) { }
    public AIPlayerRandomStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, likedInstrument)
    {
        playerMonoBehaviourFunctionalities.StartCoroutine(DisplaySpeechBalloonForAWhile("", 2.0f));
        this.type = GameProperties.PlayerType.RANDOM;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        GameProperties.Instrument instrumentToLevelUp = GameProperties.Instrument.NONE;
        instrumentToLevelUp = (Random.Range(0, 2) > 0) ? preferredInstrument : GameProperties.Instrument.MARKETING;
        return instrumentToLevelUp;
    }
    //protected override int LastDecisionsActions(Album currAlbum)
    //{
    //    int condition = 0;
    //    if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
    //    {
    //        condition = 0;
    //    }
    //    if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
    //    {
    //        condition = 2;
    //    }
    //    return condition;
    //}
}

