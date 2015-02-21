using System;
using System.IO;
using V83;

namespace Updater {
    public static class Tester {

        #region TestWorkServer

        public static bool TestWorkServer() {
            var success = true;
            Logger.Log("Тестирую настройки...");
            if (!Settings.CheckItself()) {
                Logger.Log("ПРОВАЛЕНО.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }
            Logger.Log("Тестирую доступ к \"comcntr.dll\"...");
            if (!TestComCntr()) {
                Logger.Log("ПРОВАЛЕНО. Нет доступа к библиотеке \"comcntr.dll\". Проверьте путь " +
                           Settings.PathToCOMConnectorDLL);
                success = false;
            }
            else {
                Logger.Log("Ок.");
                Logger.Log(
                    !TryToRegisterComCntr()
                        ? "Ошибка при попытке регистрации COM-компоненты 1С."
                        : "COM-компонента успешно зарегистрирована в системе.");
            }

            Logger.Log("Тестирую COM-компоненту...");
            if (!TestCOM()) {
                Logger.Log("ПРОВАЛЕНО. COM-компонента не работает.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }

            Logger.Log("Тестирую доступ к \"1cv8.exe\"...");
            if (!TestPathTo1C()) {
                Logger.Log("ПРОВАЛЕНО. Нет доступа к файлу \"1cv8.exe\". Проверьте путь " + Settings.PathTo1C);
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }

            Logger.Log("Тестирую доступ к каталогу обновлений \"" + Settings.UpdatesDirPath + "\"...");
            if (!TestUpdatesDir()) {
                Logger.Log("ПРОВАЛЕНО.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }

            Logger.Log("Тестирую доступ к рабочему каталогу \"" + Settings.Workdir + "\"...");
            if (!TestWorkdir()) {
                Logger.Log("ПРОВАЛЕНО.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }
#if Pecom
            Logger.Log("Тестирую доступ к каталогу Агента фонового сервера \"" + Settings.SchedulerServerExchangePath + "\"...");
            if (!TestSchedulerClusterExchangePath()) {
                Logger.Log("ПРОВАЛЕНО.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }
#endif

            return success;
        }

        private static bool TestWorkdir() {
            return TestDir(Settings.Workdir);
        }

        private static bool TestDir(string workdir) {
            var testFileName = Guid.NewGuid().ToString();
            var testFilePath = Path.Combine(workdir, testFileName);
            try {
                File.WriteAllText(testFilePath, "test");
                File.Delete(testFilePath);
            }
            catch (Exception) {
                return false;
            }
            return true;
        }

#if Pecom
        private static bool TestSchedulerClusterExchangePath() {
            return TestDir(Settings.SchedulerServerExchangePath);
        }
#endif


        #region TestCOM

        private static bool TestCOM() {
            var success = true;
            foreach (var infobase in Settings.Infobases) {
                if (!TestCOMConnection(infobase)) {
                    success = false;
                }
            }
            return success;
        }

        private static bool TestCOMConnection(Infobase infobase) {
            try {
                dynamic comConnector = new COMConnector();
                dynamic serverAgent = comConnector.ConnectAgent(Settings.ServerName);
                var clusters = serverAgent.GetClusters();
                foreach (IClusterInfo cluster in clusters) {
                    if (cluster.MainPort != Settings.ClusterPortNumber) continue;
                    serverAgent.Authenticate(cluster, Settings.ClusterAdminName, Settings.ClusterAdminPass);
                    var workingProcesses = serverAgent.GetWorkingProcesses(cluster);
                    foreach (IWorkingProcessInfo workingProcess in workingProcesses) {
                        if (workingProcess.Running != 1) continue;
                        var workProcessConnection =
                            comConnector.ConnectWorkingProcess("tcp://" + workingProcess.HostName + ":" +
                                                               workingProcess.MainPort);
                        workProcessConnection.AuthenticateAdmin(Settings.ClusterAdminName,
                            Settings.ClusterAdminPass);
                        workProcessConnection.AddAuthentication(infobase.InfobaseAdminName,
                            infobase.InfobaseAdminPass);
                        IInfoBaseInfo ib = null;
                        var findInfobase = false;
                        var infobases = workProcessConnection.GetInfoBases();
                        foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                            if (infoBaseInfo.Name.ToUpper() != infobase.InfobaseName.ToUpper()) continue;
                            findInfobase = true;
                            ib = infoBaseInfo;
                            break;
                        }
                        if (!findInfobase) {
                            break;
                        }
                        workProcessConnection.GetInfoBaseConnections(ib);
                    }
                }
            }
            catch (Exception e) {
                Logger.Log("Ошибка при подключении к ИБ " + infobase.InfobaseName);
                Logger.Log(e.ToString());
                return false;
            }
            return true;
        }

        #endregion TestCOM

        private static bool TestComCntr() {
            return File.Exists(Settings.PathToCOMConnectorDLL);
        }

        private static bool TryToRegisterComCntr() {
            int exitCode;
            try {
                exitCode = Utils.RegisterComComponent(Settings.PathToCOMConnectorDLL);
            }
            catch (Exception exception) {
                Logger.Log(exception.Message);
                return false;
            }
            return exitCode == 0;
        }

        private static bool TestPathTo1C() {
            return File.Exists(Settings.PathTo1C);
        }

        private static bool TestUpdatesDir() {
            return Directory.Exists(Settings.UpdatesDirPath);
        }

        #endregion TestWorkServer

#if Pecom
        #region TestSchedulerServer
        public static bool TestSchedulerServer() {
            var success = true;
            Logger.Log("Тестирую настройки...");
            if (!Settings.CheckItself()) {
                Logger.Log("ПРОВАЛЕНО.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }
            Logger.Log("Тестирую доступ к \"comcntr.dll\"...");
            if (!TestComCntr()) {
                Logger.Log("ПРОВАЛЕНО. Нет доступа к библиотеке \"comcntr.dll\". Проверьте путь " +
                           Settings.PathToCOMConnectorDLL);
                success = false;
            }
            else {
                Logger.Log("Ок.");
                Logger.Log(
                    !TryToRegisterComCntr()
                        ? "Ошибка при попытке регистрации COM-компоненты 1С."
                        : "COM-компонента успешно зарегистрирована в системе.");
            }

            Logger.Log("Тестирую COM-компоненту...");
            if (!TestCOM()) {
                Logger.Log("ПРОВАЛЕНО. COM-компонента не работает.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }

            Logger.Log("Тестирую доступ к рабочему каталогу \"" + Settings.Workdir + "\"...");
            if (!TestWorkdir()) {
                Logger.Log("ПРОВАЛЕНО.");
                success = false;
            }
            else {
                Logger.Log("Ок.");
            }

            return success;
        }
        #endregion TestSchedulerServer
#endif
    }
}
