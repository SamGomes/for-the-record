using UnityEngine;
using UnityEngine.UI;

//this strategy always plays for markting except in the first play where it is cooperative. In the last decision it always trusts his markting.
public class AIPlayerGreedyStrategy : AIPlayer
{
    public AIPlayerGreedyStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : this(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, GameProperties.Instrument.NONE) { }
    public AIPlayerGreedyStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, likedInstrument)
    {
        this.GetSpeechBaloonUI().GetComponentInChildren<Text>().text = "I'm using market strategy";
        this.type = GameProperties.PlayerType.GREEDY;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        return GameProperties.Instrument.MARKETING;
    }
    //protected override int LastDecisionsActions(Album currAlbum)
    //{
    //    int condition = 0;
    //    if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT && skillSet[GameProperties.Instrument.MARKETING] > 0)
    //    {
    //        condition = 1;
    //    }
    //    if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
    //    {
    //        condition = 2;
    //    }
    //    return condition;
    //}
}


