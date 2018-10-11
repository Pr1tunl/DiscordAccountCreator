using System;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Threading;

namespace Discord_Account_Creator
{

    public class TwoCaptcha
    {
        public WebProxy AccessProxy { get; set; }

        public TwoCaptcha(string apiKey, string siteKey, string siteUrl) : this(apiKey, siteKey, siteUrl, null)
        {
        }

        public TwoCaptcha(string apiKey, string siteKey, string siteUrl, WebProxy accessProxy)
        {
            this.SiteKey = siteKey;
            this.SiteURL = siteUrl;
            this.APIKey = apiKey;
            this.AccessProxy = accessProxy;
        }


        public CaptchaResult SolveCaptcha()
        {
            CaptchaResult captchaID = this.GetCaptchaID();
            bool flag = captchaID == null;
            CaptchaResult result;
            if (flag)
            {
                result = null;
            }
            else
            {
                Thread.Sleep(5000);
                result = this.PollCaptchaID(captchaID);
            }
            return result;
        }

        private CaptchaResult PollCaptchaID(CaptchaResult captchaId)
        {
            int maxPollTries = this.MaxPollTries;
            while (maxPollTries-- > 0)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        string text = webClient.DownloadString(string.Format("{0}?key={1}&action=get&id={2}", "http://2captcha.com/res.php", this.APIKey, captchaId.ID));
                        bool flag = text.Contains("OK|");
                        if (flag)
                        {
                            captchaId.HasSolved = true;
                            captchaId.Response = text.Split(new char[]
                            {
                            '|'
                            })[1];
                            return captchaId;
                        }
                        bool flag2 = text == "ERROR_CAPTCHA_UNSOLVABLE";
                        if (flag2)
                        {
                            return captchaId;
                        }
                    }
                }
                catch (Exception arg)
                {
                    Console.WriteLine("PollCaptchaID {0}: {1}", maxPollTries, arg);
                }
                Thread.Sleep(5000);
            }
            return captchaId;
        }

        private CaptchaResult GetCaptchaID()
        {
            NameValueCollection nameValueCollection = new NameValueCollection();
            string value = string.Format("{0}:{1}", this.AccessProxy.Address.DnsSafeHost, this.AccessProxy.Address.Port);
            nameValueCollection.Add("key", this.APIKey);
            nameValueCollection.Add("method", "userrecaptcha");
            nameValueCollection.Add("googlekey", this.SiteKey);
            nameValueCollection.Add("proxy", value);
            nameValueCollection.Add("proxytype", "http");
            nameValueCollection.Add("pageurl", this.SiteURL);
            int maxSubmitTries = this.MaxSubmitTries;
            while (maxSubmitTries-- > 0)
            {
                try
                {
                    using (WebClient webClient = new WebClient())
                    {
                        string @string = Encoding.UTF8.GetString(webClient.UploadValues(string.Format("{0}", "http://2captcha.com/in.php"), nameValueCollection));
                        bool flag = @string.StartsWith("OK|");
                        if (flag)
                        {
                            return new CaptchaResult(null, @string.Split(new char[]
                            {
                            '|'
                            })[1], this.APIKey, false);
                        }
                        return null;
                    }
                }
                catch (Exception arg)
                {
                    Console.WriteLine("GetCaptchaID: {0}", arg);
                }
                Thread.Sleep(1500);
            }
            return null;
        }

        public const string IN_URL = "http://2captcha.com/in.php";

        public const string RES_URL = "http://2captcha.com/res.php";

        public string SiteKey;

        public string APIKey;

        public string SiteURL;

        public int MaxPollTries = 15;

        public int MaxSubmitTries = 15;
    }

}
