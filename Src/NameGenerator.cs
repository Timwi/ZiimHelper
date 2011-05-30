using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZiimHelper
{
    sealed class NameGenerator
    {
        private string _curname;

        public NameGenerator()
        {
            _curname = "";
        }

        public string NextName()
        {
            _curname = nextName(_curname);
            return _curname;
        }

        private string nextName(string prev)
        {
            if (string.IsNullOrEmpty(prev))
                return "A";
            if (prev.Length == 1)
            {
                if (prev[0] == 'Z')
                    return "AA";
                else
                    return ((char) (prev[0] + 1)).ToString();
            }
            else
            {
                if (prev[prev.Length - 1] == 'Z')
                    return nextName(prev.Substring(0, prev.Length - 1)) + "A";
                else
                    return prev.Substring(0, prev.Length - 1) + (char) (prev[prev.Length - 1] + 1);
            }
        }
    }
}
