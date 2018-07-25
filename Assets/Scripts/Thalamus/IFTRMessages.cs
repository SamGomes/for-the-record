using CookComputing.XmlRpc;

public interface IFTRMessagesRpc : IFTRMessages, IXmlRpcProxy { }

public interface IFTRMessages
{
    void Dispose();

    [XmlRpcMethod]
    void PerformUtterance(string utterance);

    [XmlRpcMethod]
    void PlayAnimation(string animation);

}