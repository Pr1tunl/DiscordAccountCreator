using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Account_Creator
{
    public class CaptchaResult
    {
        public CaptchaResult(string res, string id, string apiKey, bool hasSolved = false)
        {
            this.Response = res;
            this.ID = id;
            this.HasSolved = hasSolved;
            this._apiKey = apiKey;
        }

        public string ID;

        public string Response;

        public bool HasSolved;

        private string _apiKey;
    }
}
