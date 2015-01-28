using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace PDNUtils.IO
{
    public class LongPath
    {

        public static long GetFileSize(string path)
        {
            long size = 0;

            using (var h = GetFileHandle(NormalizeLongPath(path), FileMode.Open, FileAccess.Read, FileShare.Read, FileOptions.None))
            {
                NativeMethods.GetFileSizeEx(h, ref size);
            }

            return size;
        }

        public static string GetShortPath(string path)
        {
            var shortPath = new StringBuilder(255);
            var res = NativeMethods.GetShortPathName(NormalizeLongPath(path), shortPath, shortPath.Capacity);
            return shortPath.ToString();
        }

        public static bool StartProcess(string path)
        {
            NativeMethods.STARTUPINFO si = new NativeMethods.STARTUPINFO();
            NativeMethods.PROCESS_INFORMATION pi = new NativeMethods.PROCESS_INFORMATION();
            string normalizeLongPath = NormalizeLongPath(path);
            bool res = NativeMethods.CreateProcess(null, normalizeLongPath, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, out pi);

            if (!res)
            {
                throw GetExceptionFromLastWin32Error();
            }

            return true;
        }

        public static void ShellExecute(string path)
        {
            const uint SEE_MASK_INVOKEIDLIST = 16384;

            NativeMethods.SHELLEXECUTEINFO info = new NativeMethods.SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "open";
            info.lpFile = path;
            info.nShow = (int)NativeMethods.ShowCommands.SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            NativeMethods.ShellExecuteEx(ref info);
        }

        public static void ShowProperties(string path)
        {
            const uint SEE_MASK_INVOKEIDLIST = 12;

            NativeMethods.SHELLEXECUTEINFO info = new NativeMethods.SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = path;
            info.nShow = (int)NativeMethods.ShowCommands.SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            NativeMethods.ShellExecuteEx(ref info);
        }

        private static SafeFileHandle GetFileHandle(string normalizedPath, FileMode mode, FileAccess access, FileShare share, FileOptions options)
        {
            NativeMethods.EFileAccess underlyingAccess = GetUnderlyingAccess(access);
            var file = NativeMethods.CreateFile(normalizedPath, underlyingAccess, (uint)share, IntPtr.Zero, (uint)mode, (uint)options, IntPtr.Zero);
            if (file.IsInvalid)
                throw GetExceptionFromLastWin32Error();
            else
                return file;
        }

        internal static string NormalizeLongPath(string path)
        {
            return NormalizeLongPath(path, "path");
        }

        internal static string NormalizeLongPath(string path, string parameterName)
        {
            if (path == null)
                throw new ArgumentNullException(parameterName);
            if (path.Length == 0)
            {
                throw new ArgumentException(string.Format((IFormatProvider)CultureInfo.CurrentCulture, "'{0}' cannot be an empty string.", new object[1]
        {
          (object) parameterName
        }), parameterName);
            }
            else
            {
                StringBuilder lpBuffer = new StringBuilder(path.Length + 1);
                uint fullPathName = NativeMethods.GetFullPathName(path, (uint)lpBuffer.Capacity, lpBuffer, IntPtr.Zero);
                if ((long)fullPathName > (long)lpBuffer.Capacity)
                {
                    lpBuffer.Capacity = (int)fullPathName;
                    fullPathName = NativeMethods.GetFullPathName(path, fullPathName, lpBuffer, IntPtr.Zero);
                }
                if ((int)fullPathName == 0)
                    throw GetExceptionFromLastWin32Error(parameterName);
                if (fullPathName > 32000U)
                    throw GetExceptionFromWin32Error(206, parameterName);
                else
                    return AddLongPathPrefix(((object)lpBuffer).ToString());
            }
        }

        private static string AddLongPathPrefix(string path)
        {
            return "\\\\?\\" + path;
        }

        internal static Exception GetExceptionFromLastWin32Error()
        {
            return GetExceptionFromLastWin32Error("path");
        }

        internal static Exception GetExceptionFromLastWin32Error(string parameterName)
        {
            return GetExceptionFromWin32Error(Marshal.GetLastWin32Error(), parameterName);
        }

        private static string GetMessageFromErrorCode(int errorCode)
        {
            StringBuilder lpBuffer = new StringBuilder(512);
            NativeMethods.FormatMessage(12800, IntPtr.Zero, errorCode, 0, lpBuffer, lpBuffer.Capacity, IntPtr.Zero);
            return ((object)lpBuffer).ToString();
        }

        internal static Exception GetExceptionFromWin32Error(int errorCode, string parameterName)
        {
            string messageFromErrorCode = GetMessageFromErrorCode(errorCode);
            switch (errorCode)
            {
                case 123:
                    return (Exception)new ArgumentException(messageFromErrorCode, parameterName);
                case 206:
                    return (Exception)new PathTooLongException(messageFromErrorCode);
                case 995:
                    return (Exception)new OperationCanceledException(messageFromErrorCode);
                case 2:
                    return (Exception)new FileNotFoundException(messageFromErrorCode);
                case 3:
                    return (Exception)new DirectoryNotFoundException(messageFromErrorCode);
                case 5:
                    return (Exception)new UnauthorizedAccessException(messageFromErrorCode);
                case 15:
                    return (Exception)new DriveNotFoundException(messageFromErrorCode);
                default:
                    return (Exception)new IOException(messageFromErrorCode, NativeMethods.MakeHRFromErrorCode(errorCode));
            }
        }

        private static NativeMethods.EFileAccess GetUnderlyingAccess(FileAccess access)
        {
            switch (access)
            {
                case FileAccess.Read:
                    return NativeMethods.EFileAccess.GenericRead;
                case FileAccess.Write:
                    return NativeMethods.EFileAccess.GenericWrite;
                case FileAccess.Read | FileAccess.Write:
                    return NativeMethods.EFileAccess.GenericRead | NativeMethods.EFileAccess.GenericWrite;
                default:
                    throw new ArgumentOutOfRangeException("access");
            }
        }

    }
}