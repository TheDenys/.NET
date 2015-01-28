using System;
using System.Configuration;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;

namespace NET4.TestClasses
{
    public static class LDAPAuthHelper
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static string server = ConfigurationManager.AppSettings["LDAPServer"];

        public static bool Auth(string username, string password)
        {
            bool bRes = false;
            DirectoryEntry de = null;

            try
            {
                de = new DirectoryEntry(@"LDAP://" + server, username, password);
                de.RefreshCache();
                bRes = true;
            }
            catch (Exception ex)
            {
                log.Info(ex, ex);
            }
            finally
            {
                de.Dispose();
            }

            return bRes;
        }
    }

    public class ADHelper
    {
        public static void Validate(string login, string password, string domain)
        {
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domain))
            {
                var valid = pc.ValidateCredentials(login, password, ContextOptions.Negotiate);
            }
        }
    }
}
