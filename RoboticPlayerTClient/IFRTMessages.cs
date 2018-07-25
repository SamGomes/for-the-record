using System;
using CookComputing.XmlRpc;

public interface IFRTMessagesRpc : IFRTMessages, IXmlRpcProxy { }

public interface IFRTMessages
{
    void Dispose();

    [XmlRpcMethod]
    void PerformUtterance(string utterance);

    [XmlRpcMethod]
    void PlayAnimation(string animation);
        
}