using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace NET4.TestClasses
{

    internal sealed class NativeMethods
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        internal static extern int WNetAddConnection2(ref NetResource pstNetRes, string psPassword, string psUsername, int piFlags);

        [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
        internal static extern int WNetCancelConnection2(string psName, int piFlags, int pfForce);


        [StructLayout(LayoutKind.Sequential)]
        internal struct NetResource
        {
            public int iScope;
            public int iType;
            public int iDisplayType;
            public int iUsage;
            public string sLocalName;
            public string sRemoteName;
            public string sComment;
            public string sProvider;
        }

        internal const int RESOURCETYPE_ANY = 0;// All resources
        internal const int RESOURCE_CONNECTED = 0;
        internal const int RESOURCEDISPLAYTYPE_GENERIC = 3;//The method used to display the object does not matter.
    }

    public sealed class ConnectorTool : IDisposable
    {
        // establish new connection with specified credentials or if user/password are empty uses default values
        // resource is an UNC path like \\hostname\some_path
        private static void _EstablishConnecion(string resource, string user, string password)
        {
            //create struct data
            NativeMethods.NetResource stNetRes = new NativeMethods.NetResource();
            stNetRes.iScope = NativeMethods.RESOURCE_CONNECTED;
            stNetRes.iType = NativeMethods.RESOURCETYPE_ANY;
            stNetRes.iDisplayType = NativeMethods.RESOURCEDISPLAYTYPE_GENERIC;
            stNetRes.sRemoteName = resource;
            stNetRes.sLocalName = null;

            string psPassword = password, psUsername = user;

            if (user == "")
            {
                psUsername = null;
            }
            if (psPassword == "")
            {
                psPassword = null;
            }
            try
            {
                _CancelConnection(resource, true); //cancellate old connections, because thay may cause collisions
            }
            catch
            {
            }
            int i = NativeMethods.WNetAddConnection2(ref stNetRes, psPassword, psUsername, 0);
            if (i > 0)
            {
                throw new System.ComponentModel.Win32Exception(i);
            }
        }

        // closes connection for specified resource
        private static void _CancelConnection(string resource, bool force)
        {
            int i = NativeMethods.WNetCancelConnection2(resource, 0, Convert.ToInt32(force));
            if (i > 0)
            {
                throw new System.ComponentModel.Win32Exception(i);
            }

        }

        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string pc;
        private string login;
        private string password;
        bool isConnected;

        public ConnectorTool(string pc, string login, string password)
        {
            this.pc = pc;
            this.login = login;
            this.password = password;
            isConnected = false;
        }

        // returns true if connected
        public bool Connect()
        {
            bool res = false;
            try
            {
                _EstablishConnecion(pc, login, password);
                isConnected = true;
                res = true;
            }
            catch (Win32Exception w32e)
            {
                log.Error(w32e, w32e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }
            return res;
        }

        // call this method after finish using of resource
        public bool Close()
        {
            bool res = false;
            try
            {
                if (isConnected)
                    _CancelConnection(pc, true);
                isConnected = false;
                res = true;
            }
            catch (Win32Exception w32e)
            {
                log.Error(w32e, w32e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }
            return res;
        }

        // the last chance to release resources
        public void Dispose()
        {
            try
            {
                _CancelConnection(pc, false);
            }
            catch (Win32Exception w32e)
            {
                log.Error(w32e, w32e);
            }
            catch (Exception e)
            {
                log.Error(e, e);
            }
        }
    }
}
