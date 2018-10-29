using UnityEngine;


public class AIPlayerSimpleStrategy : AIPlayer
{
    public AIPlayerSimpleStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed) : this(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, GameProperties.Instrument.NONE) { }
    public AIPlayerSimpleStrategy(GameObject playerUIPrefab, GameObject canvas, MonoBehaviourFunctionalities playerMonoBehaviourFunctionalities, PoppupScreenFunctionalities warningScreenref, int id, string name, bool isSpeechAllowed, GameProperties.Instrument likedInstrument) : base(playerUIPrefab, canvas, playerMonoBehaviourFunctionalities, warningScreenref, id, name, isSpeechAllowed, likedInstrument)
    {
        this.type = GameProperties.PlayerType.SIMPLE;
    }

    protected override GameProperties.Instrument LevelUpActions(Album currAlbum)
    {
        return GameProperties.Instrument.GUITAR;

    }
    protected override int LastDecisionsActions(Album currAlbum)
    {
        int condition = 0;
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.MEGA_HIT)
        {
            condition = 0;
        }
        if (currAlbum.GetMarketingState() == GameProperties.AlbumMarketingState.FAIL)
        {
            condition = 2;
        }
        return condition;
    }
}

