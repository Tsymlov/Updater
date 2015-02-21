using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Updater {
    internal static class Settings {
        static Settings() {
            Initialize();
        }

        private static void Initialize() {
#if Pecom
            SchedulerServerExchangePath = DefaultSettings.SchedulerServerExchangePath;
            SchedulerServerName = DefaultSettings.SchedulerServerName;
            SchedulerClusterPortNumber = DefaultSettings.SchedulerClusterPortNumber;
            SchedulerClusterAdminName = DefaultSettings.SchedulerClusterAdminName;
            SchedulerClusterAdminPass = DefaultSettings.SchedulerClusterAdminPass;
#endif
            Workdir = DefaultSettings.Workdir;
            SettingsFilePath = DefaultSettings.SettingsFilePath; 
            Infobases = new List<Infobase>();
            PathTo1C = DefaultSettings.PathTo1C;
            ClusterAdminName = DefaultSettings.ClusterAdminName;
            ClusterAdminPass = DefaultSettings.ClusterAdminPass;
            ClusterPortNumber = DefaultSettings.ClusterPortNumber;
            ServerName = DefaultSettings.ServerName;
            OutFilePath = DefaultSettings.OutFilePath;
            IsDynamicUpdate = DefaultSettings.IsDynamicUpdate;
            TimeoutAfterUserNotification = DefaultSettings.TimeoutAfterUserNotification;
            NotifyMessage = DefaultSettings.NotifyMessage;
            LockMessageText = DefaultSettings.LockMessageText;
            LockPermissionCode = DefaultSettings.LockPermissionCode;
            PathToCOMConnectorDLL = DefaultSettings.PathToCOMConnectorDLL;
            UpdatesDirPath = DefaultSettings.UpdatesDirPath;
            SettingsVersionTime = new DateTime();
        }

        public static bool Load(string path) {
            Infobases.Clear();
            try {
                LoadFromXml(path);
            }
            catch(Exception e) {
                Logger.Log(e.ToString());
                return false;
            }
            return true;
        }

        private static void LoadFromXml(string path) {
            var doc = new XmlDocument();
            doc.Load(path);
            var us = doc.SelectSingleNode("UpdaterSettings");

// ReSharper disable PossibleNullReferenceException
            UpdatesDirPath = us.Attributes["UpdatesDirPath"].Value;
            NotifyMessage = us.Attributes["NotifyMessage"].Value;
            PathTo1C = us.Attributes["PathTo1C"].Value;
            PathToCOMConnectorDLL = us.Attributes["PathToCOMConnectorDLL"].Value;
            OutFilePath = us.Attributes["OutFilePath"].Value;
            var nodeList = us.SelectNodes("Infobase");
            foreach (XmlNode infobase in nodeList) {
                var infobaseName = infobase.Attributes["InfobaseName"].Value;
                var infobaseAdminName = infobase.Attributes["InfobaseAdminName"].Value;
                var infobaseAdminPass = infobase.Attributes["InfobaseAdminPass"].Value;
                Infobases.Add(new Infobase(infobaseName, infobaseAdminName, infobaseAdminPass));
            }
            ServerName = us.Attributes["ServerName"].Value;
            ClusterAdminName = us.Attributes["ClusterAdminName"].Value;
            ClusterAdminPass = us.Attributes["ClusterAdminPass"].Value;
            int parsedInt;
            if (Int32.TryParse(us.Attributes["TimeoutAfterUserNotification"].Value, out parsedInt)) {
                TimeoutAfterUserNotification = parsedInt;
            }
            if (Int32.TryParse(us.Attributes["ClusterPortNumber"].Value, out parsedInt)) {
                ClusterPortNumber = parsedInt;
            }

            bool parsedBool;
            if (Boolean.TryParse(us.Attributes["IsDynamicUpdate"].Value, out parsedBool)) {
                IsDynamicUpdate = parsedBool;
            }
#if Pecom
// ReSharper restore PossibleNullReferenceException
            if (Boolean.TryParse(us.Attributes["IsSchedulerServer"].Value, out parsedBool)) {
                IsSchedulerServer = parsedBool;
            }            
            if (Int32.TryParse(us.Attributes["SchedulerClusterPortNumber"].Value, out parsedInt)) {
                SchedulerClusterPortNumber = parsedInt;
            }
            SchedulerServerName = us.Attributes["SchedulerServerName"].Value;
            SchedulerClusterAdminName = us.Attributes["SchedulerClusterAdminName"].Value;
            SchedulerClusterAdminPass = us.Attributes["SchedulerClusterAdminPass"].Value;
#endif
            SettingsVersionTime = File.GetLastWriteTime(path);
        }

#if Pecom
        public static string SchedulerServerExchangePath {
            get {
                if (String.IsNullOrEmpty(SchedulerServerName)) {
                    return DefaultSettings.SchedulerServerExchangePath;
                }
                var serverPath = @"\\" + SchedulerServerName;
                const string updaterPath = @"Exchange\UpdaterSettings";
                return Path.Combine(serverPath, updaterPath);
            }
// ReSharper disable ValueParameterNotUsed
            private set { /*do noting*/ }
// ReSharper restore ValueParameterNotUsed
        }
#endif

        public static string SettingsFilePath { get; private set; }
        public static string Workdir { get; private set; }
        public static string PathTo1C { get; private set; }
        public static string ServerName { get; private set; }
        public static List<Infobase> Infobases { get; private set; }
        public static string OutFilePath { get; private set; }
        public static bool IsDynamicUpdate { get; private set; }
        public static int TimeoutAfterUserNotification { get; private set; }
        public static int ClusterPortNumber { get; private set; }
        public static string ClusterAdminName { get; private set; }
        public static string ClusterAdminPass { get; private set; }
        public static string NotifyMessage { get; private set; }
        public static string LockMessageText { get; private set; }
        public static string LockPermissionCode { get; private set; }
#if Pecom
        public static bool IsSchedulerServer { get; private set; }
        public static string SchedulerServerName { get; private set; }
        public static int SchedulerClusterPortNumber { get; private set; }
        public static string SchedulerClusterAdminName { get; private set; }
        public static string SchedulerClusterAdminPass { get; private set; }
#endif
        public static string PathToCOMConnectorDLL { get; private set; }
        public static string UpdatesDirPath { get; private set; }

        public static DateTime SettingsVersionTime { get; private set; }

        public static bool CheckItself() {
            var isFatal = false;
            if (Infobases.Count < 1) {
                Logger.Log("В настройках нет информации ни об одной ИБ.");
                isFatal = true;
            }
            if (Infobases.Count > 5) {
                Logger.Log("Ого! Сколько баз на одном сервере! )))");
            }
            if (String.IsNullOrEmpty(PathTo1C)) {
                Logger.Log("Нет информации о расположении файла \"1cv8.exe\"!!!");
                isFatal = true;
            }
            if ((ClusterPortNumber < 1) || (ClusterPortNumber > 65535)) {
                Logger.Log("Не корректное значение номера порта рабочего кластера 1С!!!");
                isFatal = true;
            }
            if (String.IsNullOrEmpty(LockMessageText)) {
                Logger.Log("Не заполнен текст сообщения при блокировке начала сеансов ИБ.");
            }
            if (String.IsNullOrEmpty(LockPermissionCode)) {
                Logger.Log("Пустой код разрешения при блокировке начала сеансов ИБ.");
            }
            if (String.IsNullOrEmpty(NotifyMessage)) {
                Logger.Log("Не заполнен текст оповещения!!! Пользователи не будут информированы об обновлении.");
                isFatal = true;
            }
            if (String.IsNullOrEmpty(OutFilePath)) {
                Logger.Log(
                    "Нет информации о пути к файлу лога. Лог можно будет посмотреть в журнале приложений Windows.");
            }
            if (String.IsNullOrEmpty(PathToCOMConnectorDLL)) {
                Logger.Log("Нет информации о расположении файла \"comcntr.dll\"!!!");
                isFatal = true;
            }
#if Pecom
            if ((SchedulerClusterPortNumber < 1) || (SchedulerClusterPortNumber > 65535)) {
                Logger.Log("Не корректное значение номера порта фонового кластера 1С!!!");
                isFatal = true;
            }
#endif
            if (String.IsNullOrEmpty(ServerName)) {
                Logger.Log("Не заполнено значение наименования рабочего сервера!!!");
                isFatal = true;
            }
            if (TimeoutAfterUserNotification < 0) {
                Logger.Log("Не корректное значение таймаута после оповещение пользователей!!! Значение: " +
                           TimeoutAfterUserNotification);
                isFatal = true;
            }
            if (TimeoutAfterUserNotification == 0) {
                Logger.Log(
                    "Значение таймаута после оповещения пользователей равно 0. Возможно пользователи не успеют получить уведомление об обновлении.");
            }
            if (TimeoutAfterUserNotification > TimeSpan.FromMinutes(15).TotalMilliseconds) {
                Logger.Log("Ого! Слишком большое значение таймаута после оповещения пользователей. Значение: " +
                           TimeSpan.FromMilliseconds(TimeoutAfterUserNotification).TotalMinutes + " мин.");
            }
            if (String.IsNullOrEmpty(UpdatesDirPath)) {
                Logger.Log("Не заполнено значение пути к общему ресурсу обновлений!!!");
                isFatal = true;
            }
            foreach (var infobase in Infobases) {
                if (!String.IsNullOrEmpty(infobase.InfobaseName)) continue;
                Logger.Log("Не заполнено значение наименования ИБ!!!");
                isFatal = true;
            }

            return !isFatal;
        }

        public static void SetToDefaults() {
            Initialize();
        }
    }

    internal static class DefaultSettings {
        private const string SettingsFileName = "Settings.xml";
#if Pecom
        private static readonly string DefaultSchedulerServerExchangePath = String.Empty;
        private static readonly string DefaultSchedulerServerName = String.Empty;
        private static readonly string DefaultSchedulerClusterAdminName = String.Empty;
        private static readonly string DefaultSchedulerClusterAdminPass = String.Empty;
#endif
        private const string DefaultWorkdir = @"\\localhost\Exchange\UpdaterSettings";
        private static readonly string DefaultSettingsFilePath = Path.Combine(DefaultWorkdir, SettingsFileName);
        private static readonly string DefaultPathTo1C = String.Empty;
        private static readonly string DefaultServerName = String.Empty;
        private static readonly string DefaultUpdatesDirPath = String.Empty;
        private static readonly string DefaultOutFilePath = String.Empty;
        private static readonly string DefaultClusterAdminName = String.Empty;
        private static readonly string DefaultClusterAdminPass = String.Empty;
        private static readonly string DefaultPathToCOMConnectorDLL = String.Empty;
        private const int DefaultPortNumber = 1541;
        private const int DefaultTimeout = 300000; //5 min

        private const string DefaultNotifyMessage =
            "Внимание! В связи с проведение технических работ, программа 1С будет не доступна через несколько минут.";

        private const string DefaultLockMessageText = "Технические работы";
        private const string DefaultLockPermissionCode = "123321";
        private const bool DefaultIsDynamicUpdate = false;

        static DefaultSettings() {
            Initialize();
        }

        private static void Initialize() {
#if Pecom
            SchedulerServerExchangePath = DefaultSchedulerServerExchangePath;
            SchedulerServerName = DefaultSchedulerServerName;
            SchedulerClusterPortNumber = DefaultPortNumber;
            SchedulerClusterAdminName = DefaultSchedulerClusterAdminName;
            SchedulerClusterAdminPass = DefaultSchedulerClusterAdminPass;
#endif
            Workdir = DefaultWorkdir;
            SettingsFilePath = DefaultSettingsFilePath;
            PathTo1C = DefaultPathTo1C;
            ServerName = DefaultServerName;
            UpdatesDirPath = DefaultUpdatesDirPath;
            OutFilePath = DefaultOutFilePath;
            IsDynamicUpdate = DefaultIsDynamicUpdate;
            TimeoutAfterUserNotification = DefaultTimeout;
            ClusterPortNumber = DefaultPortNumber;
            ClusterAdminName = DefaultClusterAdminName;
            ClusterAdminPass = DefaultClusterAdminPass;
            NotifyMessage = DefaultNotifyMessage;
            LockMessageText = DefaultLockMessageText;
            LockPermissionCode = DefaultLockPermissionCode;
            PathToCOMConnectorDLL = DefaultPathToCOMConnectorDLL;
        }

#if Pecom
        public static string SchedulerServerExchangePath { get; private set; }
        public static bool IsSchedulerServer { get; set; }
        public static string SchedulerServerName { get; private set; }
        public static int SchedulerClusterPortNumber { get; private set; }
        public static string SchedulerClusterAdminName { get; private set; }
        public static string SchedulerClusterAdminPass { get; private set; }
#endif
        public static string SettingsFilePath { get; private set; }
        public static string Workdir { get; private set; }
        public static string PathTo1C { get; private set; }
        public static string ServerName { get; private set; }
        public static string OutFilePath { get; private set; }
        public static bool IsDynamicUpdate { get; private set; }
        public static int TimeoutAfterUserNotification { get; private set; }
        public static int ClusterPortNumber { get; private set; }
        public static string ClusterAdminName { get; private set; }
        public static string ClusterAdminPass { get; private set; }
        public static string NotifyMessage { get; private set; }
        public static string LockMessageText { get; private set; }
        public static string LockPermissionCode { get; private set; }
        public static string PathToCOMConnectorDLL { get; private set; }
        public static string UpdatesDirPath { get; private set; }
    }
}