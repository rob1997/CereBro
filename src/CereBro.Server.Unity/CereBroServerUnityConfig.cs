using System;

namespace CereBro.Server.Unity;

public class CereBroServerUnityConfig : ICereBroServerUnityConfig
{
    public Uri Url { get; init; }
}