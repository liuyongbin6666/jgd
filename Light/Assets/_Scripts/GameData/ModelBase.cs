using GMVC.Core;

public class ModelBase
{
    protected void SendEvent(string eventName, params object[] args) => Game.SendEvent(eventName, args);
}
