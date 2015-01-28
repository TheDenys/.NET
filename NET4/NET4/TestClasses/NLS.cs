using System;
using System.Collections.Generic;

namespace NET4.TestClasses
{

    /// <summary>
    /// abstract class for fetching translations from some storage
    /// with caching ability
    /// </summary>
    public abstract class ChacheableNls
    {
        private IDictionary<string, string> cache;

        protected ChacheableNls()
        {
            cache = new Dictionary<string, string>();
        }

        // fetch nls data by keyword and languag id
        // with caching it value
        // cache expiring will be configurable

        // return appropriate entry from databaze according to given
        // keyword and language identifier
        // language identifier could be an int od enumeration
        // with a worldwide used acronyms (en, cz, fr, ...)
        public string Get<I>(string keyword, I language_identifier)
        {
            string res = null;
            if (cache.ContainsKey(keyword))
            {
                res = cache[keyword];
            }
            else
            {
                res = InternalGet<I>(keyword, language_identifier);
                cache[keyword] = res;
            }
            return res;
        }

        // session_param is an object representing for instance NHibernate ssession or 
        // whatever else
        public string Get<S, I>(S session_param, string keyword, I language_identifier)
        {
            throw new NotImplementedException();
        }

        protected abstract string InternalGet<I>(string keyword, I language_identifier);

        protected abstract string InternalGet<S, I>(S session_param, string keyword, I language_identifier);

    }

    /*------------------------------- concrete implementation in Elearning------------------------------*/

    public enum Langs
    {
        Undefined = 0,
        CZ = 1,
        EN = 2,
        DE = 3,
        RU = 4,
        FR = 5,
        SK = 6
    }

    /// <summary>
    /// INls concrete implementation
    /// </summary>
    public class ElsNls : ChacheableNls
    {

        protected override string InternalGet<I>(string keyword, I language_identifier)
        {
            return _Get<I>(keyword, language_identifier);
        }

        private string _Get<I>(string keyword, I language_identifier)
        {
            string res = null;
            if (typeof(I) == typeof(int))
            {
                res = _Get((keyword as string), Convert.ToInt32(language_identifier));
            }
            else if (typeof(I) == typeof(Langs))
            {
                res = _Get(keyword as string, (Langs) Enum.ToObject(typeof(Langs), language_identifier));
            }
            else
            {
                throw new NotImplementedException("there no implementation for type " + typeof(I).Name);
            }
            return res;
        }

        protected override string InternalGet<S, I>(S session_param, string keyword, I language_identifier)
        {
            string res = null;
            if (typeof(S) == typeof(object))// SessionProxy instead of object
            {
            }
            else
            {
                throw new NotImplementedException("there no implementation for type " + typeof(S).Name);
            }
            return res;
        }

        private string _Get(string keyword, int lang_id)
        {
            // routine for fetcing from db
            throw new NotImplementedException("stub");
        }

        private string _Get(string keyword, Langs lang_id)
        {
            // routine for fetcing from db
            throw new NotImplementedException("stub");
        }

        private string _Get(object session, string keyword, Langs lang_id)
        {
            // routine for fetcing from db
            throw new NotImplementedException("stub");
        }

    }

    public class LangUtil
    {

        private static ChacheableNls nls;

        private static object locker = new object();

        private static ChacheableNls Instance
        {
            get
            {
                lock (locker)
                {
                    if (nls == null)
                    {
                        nls = new ElsNls();
                    }
                }
                return nls;
            }
        }

        public static string Get(string keyword, int lang_id)
        {
            return Instance.Get<int>(keyword, lang_id);
        }

        public static string Get(object session, string keyword, int lang_id)
        {
            return Instance.Get<object, int>(session, keyword, lang_id);
        }

        public static string Get(string keyword, Langs lang_id)
        {
            return Instance.Get<Langs>(keyword, lang_id);
        }

        public static string Get(object session, string keyword, Langs lang_id)
        {
            return Instance.Get<object, Langs>(session, keyword, lang_id);
        }

    }

    // Example of usage

    public class Sample
    {

        public static void SampleUse()
        {

            string translation_cz = LangUtil.Get("User.Name", 1);

            string translation_de = LangUtil.Get("User.Name", Langs.DE);

        }

    }

}
