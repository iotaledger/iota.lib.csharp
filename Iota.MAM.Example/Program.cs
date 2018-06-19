using System;
using Iota.Api;

namespace Iota.MAM.Example
{
    class Program
    {
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            PublishPublic("POTATO");
        }

        private static async void PublishPublic(string packet)
        {
            var iota = new IotaApi("node.iotawallet.info", 14265);

        }
    }
}
