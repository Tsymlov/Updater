using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Updater {
    public static class Logger {
        private const string SourceName = "Updater";
        private static readonly EventLog EventLog;

        static Logger() {
            EventLog = new EventLog();
        }

        public static void Log(string message) {
            WriteToWinEventLog(message);
            WriteToFile(message);
        }

        private static void WriteToFile(string message) {
            const int moscowTimeZone = 4;
            var nowInMoscow = DateTime.Now.ToUniversalTime().AddHours(moscowTimeZone);
            message = nowInMoscow.ToString("dd.MM.yyyy HH:mm:ss") + " МСК " + message + "\r\n";
            try {
                File.AppendAllText(Settings.OutFilePath, message, Encoding.UTF8);
            }
            catch (Exception e) {
                WriteToWinEventLog(e.ToString());
            }

        }

        private static void WriteToWinEventLog(string message) {
            try {
                if (!EventLog.SourceExists(SourceName)) {
                    EventLog.CreateEventSource(SourceName, SourceName);
                }
                EventLog.Source = SourceName;
                EventLog.WriteEntry(message);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch {
// ReSharper restore EmptyGeneralCatchClause
                //do nothing
            }
        }

        public static void LogVersions() {
            LogAgentVersionNumber();
            LogInfobasesVersionNumbers();
        }

// ReSharper disable MemberCanBePrivate.Global
        public static void LogAgentVersionNumber() {
// ReSharper restore MemberCanBePrivate.Global
            Log("Версия службы \"Агент обновлений филиалов ПЭК\": " + Program.GetVersion());
            WriteAgentVersionToFile(Path.Combine(Settings.Workdir, "Version.txt"));
        }

        private static void WriteAgentVersionToFile(string path) {
            try {
                File.WriteAllText(path, Program.GetVersion().ToString(), Encoding.UTF8);
            }
            catch (Exception e) {
                Log("Ошибка при попытке записать файл с версией Агента!!!");
                Log(e.ToString());
            }
        }

        public static void LogInfobasesVersionNumbers() {
            foreach (var infobase in Settings.Infobases) {
                Log("Версия информационной базы " + infobase.InfobaseName + ": " + infobase.GetVersion());
                WriteInfobaseVersionToFile(Path.Combine(Settings.Workdir, infobase.InfobaseName + ".ver"), infobase);
            }
        }

        private static void WriteInfobaseVersionToFile(string path, Infobase infobase) {
            try {
                File.WriteAllText(path, infobase.GetVersion());
            }
            catch (Exception e) {
                Log("Ошибка при попытке записать файл с версией информационной базы " + infobase.InfobaseName + "!!!");
                Log(e.ToString());
            }
        }
    }
}