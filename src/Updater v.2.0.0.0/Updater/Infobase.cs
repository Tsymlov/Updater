using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if !DEBUG
using System.Threading;
#endif
using System.Text;
using V83;

namespace Updater {
    public class Infobase {
        private const int ScheduledJobsWaitingTimeoutMins = 4; //

        public Infobase(string infobaseName, string infobaseAdminName, string infobaseAdminPass) {
            InfobaseAdminPass = infobaseAdminPass;
            InfobaseAdminName = infobaseAdminName;
            InfobaseName = infobaseName;
        }

        public string InfobaseName { get; private set; }
        public string InfobaseAdminName { get; private set; }
        public string InfobaseAdminPass { get; private set; }

        public bool TryToLoadAndUpdateCfgFrom(string updateFilePath) {
            try {
                Logger.Log("Загружаю конфигурацию из файла " + updateFilePath);
                var exitCode = LoadCfg(updateFilePath);
                if (exitCode != 0) {
                    Logger.Log("ExitCode = " + exitCode);
                }
                Log1COut();
                Delete1COutFile();

                Logger.Log("Обновляю конфигурацию БД...");
                exitCode = UpdateDBCfg();
                if (exitCode != 0) {
                    Logger.Log("ExitCode = " + exitCode);
                }
                Log1COut();
                Delete1COutFile();
            }
            catch (Exception ex) {
                Logger.Log(ex.Message);
                return false;
            }
            return true;
        }

        private int LoadCfg(string path) {
            if (!File.Exists(Settings.PathTo1C)) {
                Logger.Log("Обновление прервано. Ошибка при проверке пути к платформе 1С. Проверьте настройки. Файл " +
                           Settings.PathTo1C + " не существует.");
                throw new Exception("Ошибка при проверке пути к платформе 1С. Проверьте настройки. Файл " +
                                    Settings.PathTo1C + " не существует.");
            }

            var loadCfgArgument = " /LoadCfg \"" + path + "\"";
            var argumentList = ArgumentListFor1C + loadCfgArgument;
            var exitCode = Utils.StartAndWaitProcess(Settings.PathTo1C, argumentList);
            return exitCode;
        }

        private string ArgumentListFor1C {
            get {
                var argList = "DESIGNER "
                              + " /S" + Settings.ServerName + @"\" + InfobaseName
                              + " /N" + InfobaseAdminName
                              + " /P" + InfobaseAdminPass
                              + " /UC" + Settings.LockPermissionCode
                              + " /WA-"
                              + " /DisableStartupMessages"
                              + " /Out" + _1COutFilePath + " -NoTruncate";
                return argList;
            }
        }

        private static string _1COutFilePath {
            get {
                const string _1COutFileName = "1c_out.txt";
                return Path.Combine(Settings.Workdir, _1COutFileName);
            }
        }

        private static void Log1COut() {
            Logger.Log("Считываю вывод из 1С...");
            var outFileContent = Get1COutFileContent();
            Logger.Log("Вывод из 1С: \r\n" +
                       "--------------------------------------------------------------------------------------\r\n" +
                       outFileContent + "\r\n" +
                       "--------------------------------------------------------------------------------------");
        }

        private static string Get1COutFileContent() {
            try {
                return File.ReadAllText(_1COutFilePath, Encoding.Default);
            }
            catch (Exception e) {
                Logger.Log(e.ToString());
                return String.Empty;
            }
        }

        private static void Delete1COutFile() {
            Logger.Log("Удаляю файл вывода из 1С " + _1COutFilePath + "...");
            try {
                File.Delete(_1COutFilePath);
            }
            catch (Exception e) {
                Logger.Log(e.ToString());
                Logger.Log("ПРОВАЛЕНО удаление файла вывода из 1С " + _1COutFilePath);
            }
        }

        private int UpdateDBCfg() {
            const string updateCfgArgument = " /UpdateDBCfg";
            var argumentList = ArgumentListFor1C + updateCfgArgument;
            var exitCode = Utils.StartAndWaitProcess(Settings.PathTo1C, argumentList);
            return exitCode;
        }

#if Pecom
        private static void RestartRAgentOnSchedulerServer() {
            const string serviceName = "1C:Enterprise 8.3 Server Agent";
            const int timeoutMilliseconds = 40000; // 40 secs
            Utils.RestartService(serviceName, Settings.SchedulerServerName, timeoutMilliseconds);
        }
#endif

        private void DisconnectAllUsers() {
            dynamic comConnector = new COMConnector();
            dynamic serverAgent = comConnector.ConnectAgent(Settings.ServerName);
            var clusters = serverAgent.GetClusters();
            foreach (IClusterInfo cluster in clusters) {
                if (cluster.MainPort == Settings.ClusterPortNumber) {
                    serverAgent.Authenticate(cluster, Settings.ClusterAdminName, Settings.ClusterAdminPass);
                    var workingProcesses = serverAgent.GetWorkingProcesses(cluster);
                    foreach (IWorkingProcessInfo workingProcess in workingProcesses) {
                        if (workingProcess.Running == 1) {
                            var workProcessConnection =
                                comConnector.ConnectWorkingProcess("tcp://" + workingProcess.HostName + ":" +
                                                                   workingProcess.MainPort);
                            workProcessConnection.AuthenticateAdmin(Settings.ClusterAdminName, Settings.ClusterAdminPass);
                            workProcessConnection.AddAuthentication(InfobaseAdminName,
                                InfobaseAdminPass);
                            IInfoBaseInfo ib = null;
                            bool findInfobase = false;
                            if (!findInfobase) {
                                var infobases = workProcessConnection.GetInfoBases();
                                foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                                    if (infoBaseInfo.Name.ToUpper() ==
                                        InfobaseName.ToUpper()) {
                                        findInfobase = true;
                                        ib = infoBaseInfo;
                                        break;
                                    }
                                }
                                if (!findInfobase) {
                                    break;
                                }
                                if (findInfobase) {
                                    var connections = workProcessConnection.GetInfoBaseConnections(ib);
                                    foreach (IInfoBaseConnectionInfo connection in connections) {
                                        if (connection.AppID != "SrvrConsole") {
                                            Logger.Log("Завершен сеанс " + connection.AppID + " пользователя " +
                                                       connection.userName +
                                                       " на " + connection.HostName);
                                            workProcessConnection.Disconnect(connection);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BlockIB(bool value) {
            dynamic comConnector = new COMConnector();
            dynamic serverAgent = comConnector.ConnectAgent(Settings.ServerName);
            var clusters = serverAgent.GetClusters();
            foreach (IClusterInfo cluster in clusters) {
                if (cluster.MainPort == Settings.ClusterPortNumber) {
                    serverAgent.Authenticate(cluster, Settings.ClusterAdminName, Settings.ClusterAdminPass);
                    var workingProcesses = serverAgent.GetWorkingProcesses(cluster);
                    foreach (IWorkingProcessInfo workingProcess in workingProcesses) {
                        if (workingProcess.Running == 1) {
                            var workProcessConnection =
                                comConnector.ConnectWorkingProcess("tcp://" + workingProcess.HostName + ":" +
                                                                   workingProcess.MainPort);
                            workProcessConnection.AuthenticateAdmin(Settings.ClusterAdminName, Settings.ClusterAdminPass);
                            workProcessConnection.AddAuthentication(InfobaseAdminName,
                                InfobaseAdminPass);
                            IInfoBaseInfo ib = null;
                            bool findInfobase = false;
                            if (!findInfobase) {
                                var infobases = workProcessConnection.GetInfoBases();
                                foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                                    if (infoBaseInfo.Name.ToUpper() == InfobaseName.ToUpper()) {
                                        findInfobase = true;
                                        ib = infoBaseInfo;
                                        break;
                                    }
                                }
                                if (!findInfobase) {
                                    break;
                                }
                                ib.ConnectDenied = value;
                                //ib.ScheduledJobsDenied = value;
                                ib.DeniedMessage = Settings.LockMessageText;
                                ib.DeniedFrom = DateTime.Now;
                                ib.DeniedTo = DateTime.Now.AddHours(2);
                                ib.PermissionCode = Settings.LockPermissionCode;
                                workProcessConnection.UpdateInfoBase(ib);
                            }
                        }
                    }
                }
            }
        }

        private void CancelNotifyAllUsers() {
            SetNotificationConstantIn1C("");
        }

        private void SetNotificationConstantIn1C(string notifyMessage) {
            dynamic comConnector = new COMConnector();

            string сonnectionString = "Srvr=" + Settings.ServerName + ";Ref=" + InfobaseName + ";Usr=" +
                                      InfobaseAdminName + ";Pwd=" + InfobaseAdminPass;
            dynamic connection = comConnector.Connect(сonnectionString);
            connection.Константы.споСообщениеДляВсехПользователей.Установить(notifyMessage);
        }

        private void NotifyAllUsers() {
            SetNotificationConstantIn1C(Settings.NotifyMessage);
        }

        public int GetLiveConnectionsCount() {
            var liveConnectionsCount = 0;

            try {
                var connections = GetConnections();
                foreach (IInfoBaseConnectionInfo connection in connections) {
                    if (connection.AppID == "1CV8") {
                        liveConnectionsCount++;
                    }
                }
            }
            catch (Exception e) {
                Logger.Log("GetLiveConnectionCount(): " + e.Message);
                return 0;
            }
            return liveConnectionsCount;
        }

        private IEnumerable<object> GetConnections() {
            var connections = new object[0];

            dynamic comConnector = new COMConnector();
            dynamic serverAgent = comConnector.ConnectAgent(Settings.ServerName);
            var clusters = serverAgent.GetClusters();
            foreach (IClusterInfo cluster in clusters) {
                if (cluster.MainPort == Settings.ClusterPortNumber) {
                    serverAgent.Authenticate(cluster, Settings.ClusterAdminName, Settings.ClusterAdminPass);
                    var workingProcesses = serverAgent.GetWorkingProcesses(cluster);
                    foreach (IWorkingProcessInfo workingProcess in workingProcesses) {
                        if (workingProcess.Running == 1) {
                            var workingProcessConnection =
                                comConnector.ConnectWorkingProcess("tcp://" + workingProcess.HostName + ":" +
                                                                   workingProcess.MainPort);
                            workingProcessConnection.AuthenticateAdmin(Settings.ClusterAdminName,
                                Settings.ClusterAdminPass);
                            workingProcessConnection.AddAuthentication(InfobaseAdminName,
                                InfobaseAdminPass);

                            IInfoBaseInfo ib = null;
                            var findInfoBase = false;
                            var infobases = workingProcessConnection.GetInfoBases();
                            foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                                if (!String.Equals(infoBaseInfo.Name, InfobaseName,
                                    StringComparison.CurrentCultureIgnoreCase)) continue;
                                findInfoBase = true;
                                ib = infoBaseInfo;
                                break;
                            }
                            if (!findInfoBase) continue;
                            connections = workingProcessConnection.GetInfoBaseConnections(ib);
                        }
                    }
                }
            }
            return connections;
        }

        public bool TryToNotifyAllUsers() {
            try {
                NotifyAllUsers();
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

        public bool TryToCancelNotifyAllUsers() {
            try {
                CancelNotifyAllUsers();
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

        public bool TryToBlockIB(bool value) {
            try {
                BlockIB(value);
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

        public bool TryToDisconnectAllUsers() {
            try {
                DisconnectAllUsers();
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

#if Pecom
        public bool TryToSendBlockCommandToSchedulerServer(bool value) {
            try {
                SendBlockCommandToSchedulerServer(value);
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

        private void SendBlockCommandToSchedulerServer(bool value) {
            const string blockCommand = "blockCommand";
            const string unblockCommand = "unblockCommand";
            var commandFileName = value ? blockCommand : unblockCommand;
            var path = Path.Combine(Settings.SchedulerServerExchangePath, commandFileName);
            File.WriteAllText(path, InfobaseName);
        }
#endif

        public bool TryToUpdateSQLIndexes() {
            try {
                UpdateSQLIndexes();
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

        private void UpdateSQLIndexes() {
            dynamic comConnector = new COMConnector();
            var сonnectionString = "Srvr=" + Settings.ServerName + ";Ref=" + InfobaseName + ";Usr=" +
                                   InfobaseAdminName + ";Pwd=" + InfobaseAdminPass;
            dynamic connection = comConnector.Connect(сonnectionString);
            dynamic handler = connection.Обработки.споОбновлениеИБ.Создать();
            handler.ОбновитьИндексыКонфигурации();
        }

#if Pecom
        public bool TryToBlockSchedulerServer(bool value) {
            try {
                BlockSchedulerServer(value);
            }
            catch (Exception e) {
                Logger.Log(e.Message);
                return false;
            }
            return true;
        }

        private void BlockSchedulerServer(bool value) {
            dynamic comConnector = new COMConnector();
            dynamic serverAgent = comConnector.ConnectAgent(Settings.SchedulerServerName);
            var clusters = serverAgent.GetClusters();
            foreach (IClusterInfo cluster in clusters) {
                if (cluster.MainPort == Settings.SchedulerClusterPortNumber) {
                    serverAgent.Authenticate(cluster, Settings.SchedulerClusterAdminName,
                        Settings.SchedulerClusterAdminPass);
                    var workingProcesses = serverAgent.GetWorkingProcesses(cluster);
                    foreach (IWorkingProcessInfo workingProcess in workingProcesses) {
                        if (workingProcess.Running == 1) {
                            var workProcessConnection =
                                comConnector.ConnectWorkingProcess("tcp://" + workingProcess.HostName + ":" +
                                                                   workingProcess.MainPort);
                            workProcessConnection.AuthenticateAdmin(Settings.SchedulerClusterAdminName,
                                Settings.SchedulerClusterAdminPass);
                            workProcessConnection.AddAuthentication(InfobaseAdminName,
                                InfobaseAdminPass);
                            IInfoBaseInfo ib = null;
                            bool findInfobase = false;
                            if (!findInfobase) {
                                var infobases = workProcessConnection.GetInfoBases();
                                foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                                    if (infoBaseInfo.Name.ToUpper() == InfobaseName.ToUpper()) {
                                        findInfobase = true;
                                        ib = infoBaseInfo;
                                        break;
                                    }
                                }
                                if (!findInfobase) {
                                    break;
                                }

                                Logger.Log((value ? "Устанавливаю" : "Снимаю") + " блокировку начала фоновых сеансов...");
                                ib.ScheduledJobsDenied = value;
                                workProcessConnection.UpdateInfoBase(ib);
                                if (value != true) continue;
                                Logger.Log("Жду завершения фоновых сеансов " + ScheduledJobsWaitingTimeoutMins +
                                           " минут...");
#if !DEBUG
                                Thread.Sleep(TimeSpan.FromMinutes(ScheduledJobsWaitingTimeoutMins));
#endif
                                Array connections = workProcessConnection.GetInfoBaseConnections(ib);

                                if (connections.Length <= 0) continue;
                                Logger.Log("Завершаю процессы \"rphost.exe\"...");
                                KillRPHost();

                                Logger.Log("Перезапускаю службу \"Агент сервера 1С:Предприятие 8.3\"...");
                                RestartRAgentOnSchedulerServer();
                            }
                        }
                    }
                }
            }
        }

        private static void KillRPHost() {
            const string rphost = "rphost.exe";
            Utils.KillProcess(rphost);
        }
#endif

        public bool Exist() {
            var infobases = GetInfobases();
            return infobases.Count > 0;
        }

        private ICollection GetInfobases() {
            var result = new List<object>();

            dynamic comConnector = new COMConnector();
            dynamic serverAgent = comConnector.ConnectAgent(Settings.ServerName);
            var clusters = serverAgent.GetClusters();
            foreach (IClusterInfo cluster in clusters) {
                if (cluster.MainPort != Settings.ClusterPortNumber) continue;
                serverAgent.Authenticate(cluster, Settings.ClusterAdminName, Settings.ClusterAdminPass);
                var workingProcesses = serverAgent.GetWorkingProcesses(cluster);
                foreach (IWorkingProcessInfo workingProcess in workingProcesses) {
                    if (workingProcess.Running != 1) continue;
                    var workingProcessConnection =
                        comConnector.ConnectWorkingProcess("tcp://" + workingProcess.HostName + ":" +
                                                           workingProcess.MainPort);
                    workingProcessConnection.AuthenticateAdmin(Settings.ClusterAdminName,
                        Settings.ClusterAdminPass);
                    workingProcessConnection.AddAuthentication(InfobaseAdminName,
                        InfobaseAdminPass);

                    var infobases = workingProcessConnection.GetInfoBases();
                    foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                        if (!String.Equals(infoBaseInfo.Name, InfobaseName,
                            StringComparison.CurrentCultureIgnoreCase)) continue;
                        result.Add(infoBaseInfo);
                        break;
                    }
                }
            }
            return result;
        }

        public void DisconnectDesigner() {
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
                    workProcessConnection.AuthenticateAdmin(Settings.ClusterAdminName, Settings.ClusterAdminPass);
                    workProcessConnection.AddAuthentication(InfobaseAdminName,
                        InfobaseAdminPass);
                    IInfoBaseInfo ib = null;
                    var findInfobase = false;
                    var infobases = workProcessConnection.GetInfoBases();
                    foreach (IInfoBaseInfo infoBaseInfo in infobases) {
                        if (infoBaseInfo.Name.ToUpper() != InfobaseName.ToUpper()) continue;
                        findInfobase = true;
                        ib = infoBaseInfo;
                        break;
                    }
                    if (!findInfobase) {
                        break;
                    }
                    var connections = workProcessConnection.GetInfoBaseConnections(ib);
                    foreach (IInfoBaseConnectionInfo connection in connections) {
                        if (connection.AppID != "Designer") continue;
                        Logger.Log("Завершен сеанс " + connection.AppID + " пользователя " +
                                   connection.userName +
                                   " на " + connection.HostName);
                        workProcessConnection.Disconnect(connection);
                    }
                }
            }
        }

        public string GetVersion() {
            return TryToGetVersionViaCOM();
        }

        private string TryToGetVersionViaCOM() {
            try {
                return GetVersionViaCOM();
            }
            catch (Exception e) {
                Logger.Log("Ошибка при попытке получить версию информационной базы " + InfobaseName + "!!!");
                Logger.Log(e.ToString());
                return String.Empty;
            }
        }

        private string GetVersionViaCOM() {
            dynamic comConnector = new COMConnector();
            var сonnectionString = "Srvr=" + Settings.ServerName + ";Ref=" + InfobaseName + ";Usr=" +
                                   InfobaseAdminName + ";Pwd=" + InfobaseAdminPass;
            var connection = comConnector.Connect(сonnectionString);
            return connection.Метаданные.Версия;
        }
    }
}