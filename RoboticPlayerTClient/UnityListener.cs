using CookComputing.XmlRpc;

public class UnityListener : XmlRpcListenerService, IFRTMessages
{
    private ThalamusConnector _thalamusCS;

    public UnityListener(ThalamusConnector thalamusCS)
    {
        _thalamusCS = thalamusCS;
    }

    public void PerformUtterance(string utterance, string[] tags, string[] tagsValues)
    {
        _thalamusCS.TypifiedPublisher.PerformUtteranceWithTags("", utterance, tags, tagsValues);
    }

    public void GazeAt(string target)
    {
        _thalamusCS.TypifiedPublisher.GazeAtTarget(target);
    }

    public void GlanceAt(string target)
    {
        _thalamusCS.TypifiedPublisher.GlanceAtTarget(target);
    }

    public void Dispose()
    {
    }
}
