using UnityEngine;
using UnityEngine.UI;

public class AIPlayerCoopStrategy : AIPlayer
{
    public AIPlayerCoopStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : this(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, GameProperties.Instrument.NONE) { }
    public AIPlayerCoopStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, likedInstrument)
    {
        playerMonoBehaviourFunctionalities.StartCoroutine(DisplaySpeechBalloonForAWhile("I'm Cooperative", 2.0f));
        this.type = GameProperties.PlayerType.COOPERATIVE;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        //if there is money left spend it
        //if (skillSet[preferredInstrument] < GameProperties.maximumSkillLevelPerInstrument)
        //{
        return preferredInstrument;
        //}
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

