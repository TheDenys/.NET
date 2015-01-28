using System.Web;

namespace NET4.TestClasses
{
    class Encoder
    {

        public static string URLEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }

    }
}
