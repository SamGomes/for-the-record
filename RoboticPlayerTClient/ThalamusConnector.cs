using System.Threading;
using Thalamus;

public class ThalamusConnector : ThalamusClient, IFTRUnityCaller
{
    public IFTRThalamusPublisher TypifiedPublisher {  get;  private set; }
    public UnityConnector UnityConnector { private get; set; }


    public ThalamusConnector(string clientName, string character)
        : base(clientName, character)
    {
        SetPublisher<IFTRThalamusPublisher>();
        TypifiedPublisher = new ThalamusPublisher(Publisher);
    }

    public override void Dispose()
    {
        UnityConnector.Dispose();
        base.Dispose();
    }

    public void ReceiveInformation()
    {
        UnityConnector.RPCProxy.ReceiveInformation();
    }
}
