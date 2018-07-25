using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RoboticPlayerCoopStrategy : AIPlayerCoopStrategy
{
    private ThalamusConnector thalamusConnector = null;

    public RoboticPlayerCoopStrategy(string name) : base(name)
    {
        thalamusConnector = new ThalamusConnector(7000);
    }

    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        Debug.Log("Rotobic player did LevelUp.");
        thalamusConnector.PerformUtterance("Primeira fase do Glin");
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        base.PlayForInstrument(currAlbum);
        Debug.Log("Rotobic player did PlayForInstrument.");
        thalamusConnector.PerformUtterance("Segunda fase do Glin");
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        base.LastDecisionsPhase(currAlbum);
        Debug.Log("Rotobic player did LastDecisionsPhase.");
        thalamusConnector.PerformUtterance("Terceira fase do Glin");
    }
}

public class RoboticPlayerGreedyStrategy : AIPlayerGreedyStrategy
{
    private ThalamusConnector thalamusConnector = null;

    public RoboticPlayerGreedyStrategy(string name) : base(name)
    {
        thalamusConnector = new ThalamusConnector(7002);
    }

    public override void LevelUp(Album currAlbum)
    {
        base.LevelUp(currAlbum);
        Debug.Log("Rotobic player did LevelUp.");
        thalamusConnector.PerformUtterance("Primeira fase do Emys");
    }

    public override void PlayForInstrument(Album currAlbum)
    {
        base.PlayForInstrument(currAlbum);
        Debug.Log("Rotobic player did PlayForInstrument.");
        thalamusConnector.PerformUtterance("Segunda fase do Emys");
    }

    public override void LastDecisionsPhase(Album currAlbum)
    {
        base.LastDecisionsPhase(currAlbum);
        Debug.Log("Rotobic player did LastDecisionsPhase.");
        thalamusConnector.PerformUtterance("Terceira fase do Emys");
    }
}
