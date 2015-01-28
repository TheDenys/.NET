using System.Xml.XPath;
using System.Configuration;
using System.Web.Configuration;
using System.IO;

namespace NET4.TestClasses
{

    public interface ISettings
    {
        string this[string key]
        {
            get;
        }
    }

    public class AppSettings : ISettings
    {
        XPathNavigator xpnav;

        public AppSettings(XPathNavigator xpnav)
        {
            this.xpnav = xpnav;
        }

        public string this[string key]
        {
            get
            {
                XPathNodeIterator navig = xpnav.Select("//add[@key='" + key + "']/@value");
                navig.MoveNext();
                return navig.Count != 0 ? navig.Current.Value : null;
            }
        }
    }

    public class ConnectionStrings : ISettings
    {
        XPathNavigator xpnav;

        public ConnectionStrings(XPathNavigator xpnav)
        {
            this.xpnav = xpnav;
        }

        public string this[string key]
        {
            get
            {
                XPathNodeIterator navig = xpnav.Select("//add[@name='" + key + "']/@connectionString");
                navig.MoveNext();
                return navig.Count != 0 ? navig.Current.Value : null;
            }
        }
    }

    public class WebCfgHelper
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        XPathNavigator xnav;

        public WebCfgHelper()
        {
            string web_config_path = ConfigurationManager.AppSettings["web_config_path"];

            XPathDocument xdoc = new XPathDocument(web_config_path);
            xnav = xdoc.CreateNavigator();
        }

        public ISettings AppSettings
        {
            get
            {
                XPathNodeIterator navig = xnav.Select("/configuration/appSettings");
                navig.MoveNext();
                return new AppSettings(navig.Current);
            }
        }

        public ISettings ConnectionStrings
        {
            get
            {
                XPathNodeIterator navig = xnav.Select("/configuration/connectionStrings");
                navig.MoveNext();
                return new ConnectionStrings(navig.Current);
            }
        }

        public Configuration GetConfig()
        {
            WebConfigurationFileMap fm = getfm();
            return WebConfigurationManager.OpenMappedWebConfiguration(fm, "/");
        }

        private WebConfigurationFileMap getfm()
        {
            WebConfigurationFileMap fm = new WebConfigurationFileMap();
            string web_config_path = ConfigurationManager.AppSettings["web_config_path"];
            web_config_path = Path.GetDirectoryName(web_config_path);
            VirtualDirectoryMapping vdm = new VirtualDirectoryMapping(web_config_path, true, "Web.config");
            fm.VirtualDirectories.Add("/", vdm);
            return fm;
        }


    }


}
