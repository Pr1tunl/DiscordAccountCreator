using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
namespace Discord_Account_Creator
{
    class Program
    {
        //Rebuilt and open-sourced by https://v3rmillion.net/member.php?action=profile&uid=855933 \\
        //Open Binary Exe by tada was leaked on v3rm by who knows who\\
        // DO NOT SKID OFF THIS LEARN AND MAKE CHANGES THIS IS BAD AND DOSENT EVEN MULTI THREAD \\
        // Reason of me doing this cuz this bot raids us alot & half the time dosen't work!! \\

        public class FingerPRINT
        {
            public long[][] assignments { get; set; }
            public string fingerprint { get; set; }
        }
        public class Login
        {
            public string email { get; set; }
            public string password { get; set; }
        }
        public class TokenResponse
        {
            public string token { get; set; }
        }

        public class GetDisCRIMINATOR
        {
            public string username { get; set; }
            public bool verified { get; set; }
            public string locale { get; set; }
            public bool mfa_enabled { get; set; }
            public string id { get; set; }
            public object phone { get; set; }
            public int flags { get; set; }
            public string avatar { get; set; }
            public string discriminator { get; set; }
            public string email { get; set; }
        }
        private static string GetFingerPrint()
        {
            string result = new HttpClient().GetAsync("https://discordapp.com/api/v6/experiments").Result.Content.ReadAsStringAsync().Result;
            Program.FingerPRINT fingerPRINT = JsonConvert.DeserializeObject<Program.FingerPRINT>(result.ToString());
            return fingerPRINT.fingerprint;
        }
        private static void RefreshProxyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            File.WriteAllText("Proxies.txt", new WebClient().DownloadString("https://proxyscra.pe/proxies/HTTP_Working_Proxies.txt"));
            Console.WriteLine("Inserted new proxies!");
        }
        public class CreationStuff
        {
            public string fingerprint { get; set; }

            public string email { get; set; }

            public string username { get; set; }

            public string password { get; set; }

            public object invite { get; set; }

            public bool consent { get; set; }

            public object captcha_key { get; set; }
        }
        private static void Error(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(message);
        }
        private static System.Timers.Timer RefreshProxyTimer = new System.Timers.Timer();
        private static Random random = new Random();

        public static string RandomString(int length)
        {
            string str = new string((
                from s in Enumerable.Repeat<string>("abcdefghijklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", length)
                select s[Program.random.Next(s.Length)]).ToArray<char>());
            return str;
        }

      

        private static async void CreateDiscordAccount(string proxyUrl, string email, string username, string password, string invonCreation = null)
        {
            try
            {
                string Fingermeprint = Program.GetFingerPrint();
                WebProxy proxy = new WebProxy(proxyUrl, false)
                {
                    UseDefaultCredentials = true
                };
                HttpClient cl = null;
                HttpClientHandler httpClientHandler = new HttpClientHandler
                {
                    Proxy = proxy,
                    PreAuthenticate = true,
                    UseDefaultCredentials = true
                };
                cl = new HttpClient(httpClientHandler);
                cl.DefaultRequestHeaders.Clear();
                cl.DefaultRequestHeaders.Add("Accept-Language", "en-US");
                cl.DefaultRequestHeaders.Add("X-Fingerprint", string.Format("{0}", Fingermeprint));
                Task<HttpResponseMessage> response = cl.PostAsync("https://discordapp.com/api/v6/auth/register", new StringContent(JsonConvert.SerializeObject(new Program.CreationStuff
                {
                    fingerprint = string.Format("{0}", Fingermeprint),
                    email = email,
                    username = username,
                    password = password,
                    invite = invonCreation,
                    consent = true,
                    captcha_key = null
                }), Encoding.UTF8, "application/json"));
                Console.WriteLine("Sent request!");
                bool flag = response.Result.StatusCode == HttpStatusCode.NoContent || response.Result.StatusCode == HttpStatusCode.OK;
                if (flag)
                {
                    Console.WriteLine("-- " + response.Result.Content.ReadAsStringAsync().Result + string.Format(" -- ({0})", response.Result.StatusCode));
                    Console.WriteLine(string.Format("Successfully registered [{0}]{1}:{2}!", proxyUrl, email, password));
                    Task<HttpResponseMessage> responselol = cl.PostAsync("https://discordapp.com/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Program.Login
                    {
                        email = email,
                        password = password
                    }), Encoding.UTF8, "application/json"));
                    Program.TokenResponse resp = JsonConvert.DeserializeObject<Program.TokenResponse>(responselol.Result.Content.ReadAsStringAsync().Result);
                    Console.WriteLine(string.Format("-- {0} --", resp.token));
                    cl.DefaultRequestHeaders.Add("Authorization", resp.token);
                    Task<HttpResponseMessage> responsexd = cl.GetAsync("https://discordapp.com/api/v6/users/@me");
                    Program.GetDisCRIMINATOR discrim = JsonConvert.DeserializeObject<Program.GetDisCRIMINATOR>(responsexd.Result.Content.ReadAsStringAsync().Result);
                    File.AppendAllText("Results.txt", string.Format("{0}:{1}:{2}", email, password, discrim.discriminator) + Environment.NewLine);
                    File.AppendAllText("OutputTokens.txt", resp.token + "\n");
                    responselol = null;
                    resp = null;
                    responsexd = null;
                    discrim = null;
                }
                else
                {
                    TwoCaptcha solve = new TwoCaptcha(Config.TwoCaptchaKey, "6Lef5iQTAAAAAKeIvIY-DeexoO3gj7ryl9rLMEnn", "https://discordapp.com/api/v6/auth/register");
                    bool flag2 = response.Result.Content.ReadAsStringAsync().Result.Contains("captcha-required");
                    if (flag2)
                    {
                        Console.WriteLine(string.Format("[{0}/{1}/{2}]Captcha required, attempting to solve captcha...", proxyUrl, email, password));
                        solve.AccessProxy = proxy;
                        CaptchaResult responsexd2 = solve.SolveCaptcha();
                        Console.WriteLine("g-recaptcha-response token:  " + responsexd2.Response);
                        await cl.PostAsync("https://discordapp.com/api/v6/auth/register", new StringContent(JsonConvert.SerializeObject(new Program.CreationStuff
                        {
                            fingerprint = string.Format("{0}", Fingermeprint),
                            email = email,
                            username = username,
                            password = password,
                            invite = invonCreation,
                            consent = true,
                            captcha_key = responsexd2.Response
                        }), Encoding.UTF8, "application/json"));
                        Console.WriteLine(string.Format("Successfully registered [{0}]{1}:{2}()!", proxyUrl, email, password));
                        Task<HttpResponseMessage> responselol2 = cl.PostAsync("https://discordapp.com/api/auth/login", new StringContent(JsonConvert.SerializeObject(new Program.Login
                        {
                            email = email,
                            password = password
                        }), Encoding.UTF8, "application/json"));
                        Program.TokenResponse resp2 = JsonConvert.DeserializeObject<Program.TokenResponse>(responselol2.Result.Content.ReadAsStringAsync().Result);
                        Console.WriteLine(string.Format("-- {0} --", resp2.token));
                        cl.DefaultRequestHeaders.Add("Authorization", resp2.token);
                        Task<HttpResponseMessage> responsexdx = cl.GetAsync("https://discordapp.com/api/v6/users/@me");
                        Program.GetDisCRIMINATOR discrim2 = JsonConvert.DeserializeObject<Program.GetDisCRIMINATOR>(responsexdx.Result.Content.ReadAsStringAsync().Result);
                        File.AppendAllText("Results.txt", string.Format("{0}:{1}:{2}", email, password, discrim2.discriminator) + Environment.NewLine);
                        File.AppendAllText("OutputTokens.txt", resp2.token + "\n");
                        responsexd2 = null;
                        responselol2 = null;
                        resp2 = null;
                        responsexdx = null;
                        discrim2 = null;
                    }
                    else if (response.Result.Content.ReadAsStringAsync().Result.Contains("limited"))
                    {
                        Program.Error("Rate limited.REEEEEEEEEEEEEEEEEEEEEEE");
                        Thread.Sleep(35000);
                        Console.WriteLine("ok, lets try again.");
                    }
                    else if (response.Result.Content.ReadAsStringAsync().Result.Contains("registered"))
                    {
                        Console.WriteLine("Email is registered REEEEEEEEEEEEEEEEE");
                    }
                    solve = null;
                }
                Fingermeprint = null;
                proxy = null;
                cl = null;
                httpClientHandler = null;
                response = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void Main(string[] args)
        {

            bool flag = !File.Exists("Proxies.txt");
            if (flag)
            {
                Console.WriteLine("Proxy file doesn't exist. Creating one right now and inserting proxies");
                FileStream fileStream = File.Create("Proxies.txt");
                fileStream.Close();
                File.WriteAllText("Proxies.txt", new WebClient().DownloadString("https://proxyscra.pe/proxies/HTTP_Working_Proxies.txt"));
                Console.WriteLine("Done!");
            }
            else
            {
                Console.WriteLine("Proxy file exists. Creating one right now and inserting proxies");
                File.WriteAllText("Proxies.txt", new WebClient().DownloadString("https://proxyscra.pe/proxies/HTTP_Working_Proxies.txt"));
                Console.WriteLine("Done!");
            }
            Console.WriteLine("Logged in!");
            bool randomName = Config.RandomName;
            if (randomName)
            {
                for (; ; )
                {
                    try
                    {
                        string[] array = File.ReadAllLines(Config.Location);
                        Random random = new Random();
                        int num = random.Next(0, array.Length - 1);
                        string FinalName = array[num];
                        int num2 = new Random().Next(1, File.ReadAllLines("Proxies.txt").Count<string>());
                        string proxy = File.ReadAllLines("Proxies.txt")[num2];
                        Console.WriteLine("CONNECTING TO " + proxy + " ...");
                        new Thread(delegate ()
                        {
                            Program.CreateDiscordAccount(proxy, string.Format("{0}@gmail.com", Program.RandomString(7)), FinalName.ToString(), string.Format("{0}", Program.RandomString(10)), Config.Invite);
                        }).Start();
                        Console.WriteLine("Waiting 1 second, as we just tried to create a discord account and don't wanna be classified as DDOSers and get sued noob.");
                        Thread.Sleep(Config.delay);
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                bool flag2 = !Config.RandomName;
                if (!flag2)
                {
                    return;
                }
                for (; ; )
                {
                    try
                    {
                        int num3 = new Random().Next(1, File.ReadAllLines("Proxies.txt").Count<string>());
                        string proxy = File.ReadAllLines("Proxies.txt")[num3];
                        Console.WriteLine("CONNECTING TO " + proxy + " ...");
                        new Thread(delegate ()
                        {
                            Program.CreateDiscordAccount(proxy, string.Format("{0}@gmail.com", Program.RandomString(7)), string.Format("{0}{1}", Config.Name, new Random().Next(1, 9999)), string.Format("{0}", Program.RandomString(10)), Config.Invite);
                        }).Start();
                        Console.WriteLine("Waiting 1 second, as we just tried to create a discord account and don't wanna be classified as DDOSers and get sued noob.");
                        Thread.Sleep(Config.delay);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}
