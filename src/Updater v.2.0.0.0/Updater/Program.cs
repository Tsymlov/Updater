using System;
using System.Reflection;
using System.ServiceProcess;

namespace Updater {
    internal static class Program {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        private static void Main() {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] {
                new UpdaterService()
            };
            ServiceBase.Run(ServicesToRun);
        }

        public static Version GetVersion() {
            var assem = Assembly.GetExecutingAssembly();
            var assemName = assem.GetName();
            return assemName.Version;
        }
    }
}
