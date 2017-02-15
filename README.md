[![Build Status](https://travis-ci.org/iotaledger/iota.lib.csharp.svg?branch=master)](https://travis-ci.org/iotaledger/iota.lib.csharp)
[![Build status](https://ci.appveyor.com/api/projects/status/928xuq2obg1itui7/branch/master?svg=true)](https://ci.appveyor.com/project/adrianziser/iota-lib-csharp/branch/master)

## Introduction

The Iota.Lib.CSharp library implements the [[Core API calls]](https://iota.readme.io/docs/getnodeinfo) as well as the [[proposed calls]](https://github.com/iotaledger/wiki/blob/master/api-proposal.md).

It allows to connect easily to a local or a remote [[IOTA node]](https://iota.readme.io/docs/syncing-to-the-network) using C#.

* **Latest release:** 0.9.0-beta
* **Compatibility:** fully compatible with IOTA IRI v1.2.4
* **License:** Apache License 2.0 

### Technologies & dependencies

The Iota.Lib.CSharp library has been designed to be used with .Net 4.0+.

Core dependencies:
* RestSharp 4.0.30319  [[link]](https://github.com/restsharp/RestSharp)
* Json.NET 9.0.0.0  [[link]](https://github.com/JamesNK/Newtonsoft.Json)

### Getting started

Connect to your node is quite straightforward: it requires only 2 lines of code. For example, in order to fetch the Node Info:

```cs
IotaApi iotaApi = new IotaApi("node.iotawallet.info", 14265);
GetNodeInfoResponse nodeInfo = iotaApi.GetNodeInfo();
```

### Warning
 -   This is pre-release software!
 -   There may be performance and stability issues.
 -   You may loose all your money :)
 -   Please report any issues using the <a href="https://github.com/iotaledger/iota.lib.csharp/issues">Issue Tracker</a>

### What is missing
- Multisig support