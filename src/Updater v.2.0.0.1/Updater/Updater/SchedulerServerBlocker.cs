#if Pecom
using System;
using System.IO;
using System.Threading;

namespace Updater {
    public static class SchedulerServerBlocker {
        private const string BlockCommand = "blockCommand";
        private const string UnblockCommand = "unblockCommand";
        private static bool _isWorking;

        public static void DoWork() {
            Test();
            Logger.LogAgentVersionNumber();
            WaitAndBlock();
        }

        private static void Test() {
            Logger.Log("Запускаю тестирование системы...");
            Logger.Log(!Tester.TestSchedulerServer()
                ? "ТЕСТИРОВАНИЕ ПРОВАЛЕНО. Продолжаю работать, но лучше всё перепероверить."
                : "Тестирование прошло успешно.");
        }

        private static void WaitAndBlock() {
            Logger.Log("Рабочий каталог:" + Settings.Workdir);
            Logger.Log("Жду команд...");
            _isWorking = true;
            while (_isWorking) {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                var commandExist = CheckCommand(Settings.Workdir);
                if (!commandExist) continue;
                Block();
            }
            Logger.Log("Перешел в спящий режим. Чтобы возобновить работу, перезапустите сервис.");
        }

        private static bool CheckCommand(string path) {
            var blockCommandFileName = Path.Combine(path, BlockCommand);
            var unblockCommandFileName = Path.Combine(path, UnblockCommand);
            return File.Exists(blockCommandFileName) || File.Exists(unblockCommandFileName);
        }

        private static void Block() {
            var block = GetCommandValue(Settings.Workdir);
            Logger.Log("Получена команда. Block=" + block);

            var infobaseName = GetInfobaseName(Settings.Workdir);
            var infobase = GetInfobase(infobaseName);
            if (infobase == null) {
                Logger.Log("Ошибка! В настройках не найдена ИБ с наименованием " + infobaseName +
                           ". Процесс остановлен.");
                return;
            }
            Logger.Log("Работаю с базой " + infobase.InfobaseName);
            Logger.Log(block ? "Блокирую сервер..." : "Снимаю блокировку сервера...");
            LoadSettings();
            Test();
            Logger.LogAgentVersionNumber();
            BlockShedulerServer(infobase, block);
            DeleteCommandFile(block);
            Logger.Log(block ? "Сервер заблокирован." : "Блокировка снята.");
        }

        private static bool GetCommandValue(string path) {
            var blockCommandFileName = Path.Combine(path, BlockCommand);
            return File.Exists(blockCommandFileName);
        }

        private static string GetInfobaseName(string path) {
            var blockCommandFileName = Path.Combine(path, BlockCommand);
            var unblockCommandFileName = Path.Combine(path, UnblockCommand);
            var filename = File.Exists(blockCommandFileName) ? blockCommandFileName : unblockCommandFileName;

            string infobaseName;
            try {
                infobaseName = File.ReadAllText(filename);
            }
            catch (Exception e) {
                Logger.Log(e.ToString());
                Logger.Log("Ошибка считывания нименования ИБ.");
                return String.Empty;
            }
            return infobaseName;
        }

        private static Infobase GetInfobase(string infobaseName) {
            foreach (var infobase in Settings.Infobases) {
                if (infobase.InfobaseName == infobaseName) {
                    return infobase;
                }
            }
            return null;
        }

        private static void LoadSettings() {
            if (!Settings.Load(Settings.SettingsFilePath)) {
                Logger.Log("Ошибка при загрузке файла настроек " + Settings.SettingsFilePath +
                           " Проверьте наличие файла и доступ к нему на чтение для системной учетной записи. " +
                           " Процесс продолжается с настройками, загруженными в предыдущий раз.");
            }
            else {
                Logger.Log("Загружен файл настроек " + Settings.SettingsFilePath);
            }
        }

        private static void BlockShedulerServer(Infobase infobase, bool block) {
            Logger.Log(infobase.TryToBlockSchedulerServer(block)
                ? "Процесс завершился успешно."
                : "Ошибка! Процесс завершился не удачно.");
        }

        private static void DeleteCommandFile(bool block) {
            Logger.Log("Удаляю командный файл...  ");
            Logger.Log(TryToDeleteCommandFile(block)
                ? "Командный файл удален успешно."
                : "Ошибка! Командный файл не был удален. Это серьезная проблема. " +
                  "Служба может уйти в бесконечный цикл. " +
                  "Удалите файл в ручную и перепроверьте права и настройки.");
        }

        private static bool TryToDeleteCommandFile(bool block) {
            var blockCommandFileName = Path.Combine(Settings.Workdir, BlockCommand);
            var unblockCommandFileName = Path.Combine(Settings.Workdir, UnblockCommand);
            try {
                File.Delete(block ? blockCommandFileName : unblockCommandFileName);
            }
            catch (Exception e) {
                Logger.Log(e.ToString());
                return false;
            }
            return true;
        }
    }
}
#endif