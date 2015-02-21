using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace UpdaterSetup_2._0._0._0 {
    public static class Utils {
        public enum NetError : uint {
            NERR_Success = 0,
            NERR_BASE = 2100,
            NERR_UnknownDevDir = (NERR_BASE + 16),
            NERR_DuplicateShare = (NERR_BASE + 18),
            NERR_BufTooSmall = (NERR_BASE + 23),
        }

        private enum SHARE_TYPE : uint {
            STYPE_DISKTREE = 0,
            STYPE_PRINTQ = 1,
            STYPE_DEVICE = 2,
            STYPE_IPC = 3,
            STYPE_TEMPORARY = 0x40000000,
            STYPE_SPECIAL = 0x80000000,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct ShareInfo502 {
            [MarshalAs(UnmanagedType.LPWStr)] public string shi502_netname;
            public SHARE_TYPE shi502_type;
            [MarshalAs(UnmanagedType.LPWStr)] public string shi502_remark;
            public Int32 shi502_permissions;
            public Int32 shi502_max_uses;
            public Int32 shi502_current_uses;
            [MarshalAs(UnmanagedType.LPWStr)] public string shi502_path;
            [MarshalAs(UnmanagedType.LPWStr)] public string shi502_passwd;
            public Int32 shi502_reserved;
            public IntPtr shi502_security_descriptor;
        }

        [DllImport("Netapi32.dll")]
        private static extern uint NetShareAdd(
            [MarshalAs(UnmanagedType.LPWStr)] string strServer,
            Int32 dwLevel,
            ref ShareInfo502 buf,
            out uint parmErr
            );

        public static void ShareInstallationDir(string server, string shareName, string shareDesc) {
            var path = Parameters.InstallationPath; // do not append comma, it'll fail

            var info = new ShareInfo502 {
                shi502_netname = shareName,
                shi502_type = SHARE_TYPE.STYPE_DISKTREE,
                shi502_remark = shareDesc,
                shi502_permissions = 0,
                shi502_max_uses = -1,
                shi502_current_uses = 0,
                shi502_path = path,
                shi502_passwd = null,
                shi502_reserved = 0,
                shi502_security_descriptor = IntPtr.Zero
            };

            uint error = 0;
            var result = NetShareAdd(server, 502, ref info, out error);
        }

        public static int RegisterComComponent(string pathToDLL) {
            const string regsvr32 = "regsvr32.exe";
            var arguments = "\"" + pathToDLL + "\" /s";
            return StartAndWaitProcess(regsvr32, arguments);
        }

        public static int StartAndWaitProcess(string fileName, string argumentList) {
            var si = new ProcessStartInfo {FileName = fileName, Arguments = argumentList};
            var process = new Process {StartInfo = si,};

            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }

        public static string GetNetFrameworkPath() {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), @"Microsoft.NET\Framework\v4.0.30319");
        }
    }
}