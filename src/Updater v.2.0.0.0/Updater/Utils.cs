#if Pecom
using System;
using System.ServiceProcess;
#endif

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Updater {
    public static class Utils {
        public static int StartAndWaitProcess(string fileName, string argumentList) {
            var si = new ProcessStartInfo {FileName = fileName, Arguments = argumentList};
            var process = new Process {StartInfo = si,};

            process.Start();
            process.WaitForExit();
            return process.ExitCode;
        }

#if Pecom
        public static void RestartService(string serviceName, string machineName, int timeoutMilliseconds) {
            var service = new ServiceController(serviceName, machineName);
            int millisec1 = Environment.TickCount;
            TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

            // count the rest of the timeout
            int millisec2 = Environment.TickCount;
            timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running, timeout);
        }
        
        public static void KillProcess(string processName) {
            var processes = Process.GetProcessesByName(processName);
            foreach (var process in processes) {
                Logger.Log("Завершаю процесс " + process.ProcessName + " PID:" + process.Id + "... ");
                process.Kill();
            }
        }
#endif

        public static int RegisterComComponent(string pathToDLL) {
            const string regsvr32 = "regsvr32.exe";
            var arguments = "\"" + pathToDLL + "\" /s";
            return StartAndWaitProcess(regsvr32, arguments);
        }
    }
}