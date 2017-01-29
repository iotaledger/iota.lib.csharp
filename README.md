# iota.lib.charp
Iota.Lib.Csharp

Usage example:

```cs
IotaApi iotaApi = new IotaApi("node.iotawallet.info", 14265);
GetNodeInfoResponse nodeInfo = iotaApi.GetNodeInfo();
Console.WriteLine(nodeInfo.ToString());
```

