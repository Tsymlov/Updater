 using System;
using System.IO;
using System.Threading;

namespace Updater {
    public static class Updater {
        private static bool _isWorking;
        private static bool _connectionLost;

        #region DoWork
        public static void DoWork() {
            Test();
            UnlockAllInfobasesAndCancelNotify();
            Logger.LogVersions();
            WaitAndUpdate();
        }

        private static void Test() {
            Logger.Log("Запускаю тестирование системы...");
            Logger.Log(!Tester.TestWorkServer()
                ? "ТЕСТИРОВАНИЕ ПРОВАЛЕНО. Продолжаю работать, но лучше всё перепероверить."
                : "Тестирование прошло успешно.");
        }

        private static void UnlockAllInfobasesAndCancelNotify() {
            foreach (var infobase in Settings.Infobases) {
                UnlockAndCancelNotify(infobase);
            }
        }

        private static void UnlockAndCancelNotify(Infobase infobase) {
            UnlockIB(infobase);
            CancelNotifyUsers(infobase);
#if Pecom
            UnlockSchedulerServer(infobase);
#endif
        }

        private static void CancelNotifyUsers(Infobase infobase) {
            Logger.Log("Отключаю оповещение пользователей...");
            if (!infobase.TryToCancelNotifyAllUsers()) {
                Logger.Log("Ошибка при отключении оповещения пользователей. " +
                           "Возможны повторные сообщения о технических работах. " +
                           "Отключите оповещение в ручную, установив для константы \"споСообщениеДляВсехПользователей\" " +
                           "значение, равное пустой строке. ");
            }
        }

        #region WaitAndUpdate
        private static void WaitAndUpdate() {
            Logger.Log("Жду новых обновлений в каталоге " + Settings.UpdatesDirPath + " ...");
            _isWorking = true;
            while (_isWorking) {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                if (LoadIfSettingsWasChanged()) {
                    Test();
                    Logger.LogVersions();
                    Logger.Log("Жду новых обновлений в каталоге " + Settings.UpdatesDirPath + " ...");
                }
                var canUpdate = CheckUpdate(Settings.UpdatesDirPath);
                if (!canUpdate) continue;
                Update();
            }
            Logger.Log("Перешел в спящий режим. Чтобы возобновить работу, перезапустите сервис.");
        }

        private static bool LoadIfSettingsWasChanged() {
            if (File.GetLastWriteTime(Settings.SettingsFilePath) <= Settings.SettingsVersionTime) return false;
            Logger.Log("Настройки изменены. Загружаю новый файл с настройками...");
            LoadSettings();
            return true;
        }

        private static bool CheckUpdate(string updatesDirPath) {
            return !String.IsNullOrWhiteSpace(GetUpdateFilePath(updatesDirPath));
        }

        #region Update

        private static string GetUpdateFilePath(string updatesDirPath) {
            try {
                var updates = Directory.GetFiles(updatesDirPath, "*.cf", SearchOption.TopDirectoryOnly);
                if (_connectionLost) {
                    _connectionLost = false;
                    Logger.Log("Соединение восстановлено.");
                    Logger.Log("Жду новых обновлений в каталоге " + Settings.UpdatesDirPath + " ...");
                }
                foreach (var update in updates) {
                    if (!File.Exists(Path.Combine(Settings.Workdir, Path.GetFileName(update)))) {
                        return Path.Combine(updatesDirPath, update);
                    }
                }
            }
            catch (IOException) {
                if (_connectionLost) return String.Empty;
                _connectionLost = true;
                Logger.Log("СОЕДИНЕНИЕ С " + updatesDirPath + " ПОТЕРЯНО!!!");
            }
            catch (Exception e) {
                if (_connectionLost) return String.Empty;
                _connectionLost = true;
                Logger.Log(e.ToString());
            }
            return String.Empty;
        }

        private static void Update() {
            var commonUpdateFile = GetUpdateFilePath(Settings.UpdatesDirPath);
            Logger.Log("Найдено обновление " + commonUpdateFile);
            var localUpdateFile = Path.Combine(Settings.Workdir, Path.GetFileName(commonUpdateFile));

            if (!Download(commonUpdateFile, localUpdateFile)) {
                Logger.Log("Обновление прервано. Следующая попытка обновиться произойдет через 2 минуты.");
                Thread.Sleep(TimeSpan.FromMinutes(2));
                return;
            }
            if (LoadIfSettingsWasChanged()) {
                Test();
            }
            Logger.LogVersions();
            UpdateAllInfobases(localUpdateFile);
            Logger.Log("Обновление завершено.");
            Logger.LogInfobasesVersionNumbers();
        }

        private static bool Download(string commonUpdateFile, string localUpdateFile) {
            Logger.Log("Копирую файл обновлений " + commonUpdateFile + " в рабочую директорию " + Settings.Workdir);
            if (!Copy(commonUpdateFile, localUpdateFile)) {
                Logger.Log("Копирование файла обновлений " + commonUpdateFile +
                           " в рабочую директорию " + Settings.Workdir + " не удалось.");
                return false;
            }
            Logger.Log("Файл обновлений " + commonUpdateFile + " успешно скопирован в рабочую директорию.");
            return true;
        }

        private static bool Copy(string file, string destination) {
            var tempFileName = Guid.NewGuid().ToString();
            var tempFilePath = Path.Combine(Path.GetDirectoryName(destination), tempFileName);
            try {
                File.Copy(file, tempFilePath, true);
                File.Move(tempFilePath, destination);
            }
            catch (Exception e) {
                Logger.Log("Не удалось скопировать файл " + file + " в директорию " + destination +
                           " Дополнительная информация: " + e.Message);
                return false;
            }
            return true;
        }

        private static void LoadSettings() {
            if (!Settings.Load(Settings.SettingsFilePath)) {
                Logger.Log("Ошибка при загрузке файла настроек " + Settings.SettingsFilePath +
                           " Проверьте наличие файла и доступ к нему на чтение для системной учетной записи. " +
                           " Обновление продолжается с настройками, загруженными в предыдущий раз.");
            }
            else {
                Logger.Log("Загружен файл настроек " + Settings.SettingsFilePath);
            }
        }

        private static void UpdateAllInfobases(string localUpdateFile) {
            Logger.Log("Количество информационных баз: " + Settings.Infobases.Count);
            foreach (var infobase in Settings.Infobases) {
                if (!infobase.Exist()) {
                    Logger.Log("ОШИБКА!!! ИБ С ИМЕНЕМ "+ infobase.InfobaseName + " НЕ НАЙДЕНА НА СЕРВЕРЕ "+ Settings.ServerName + "!!!");
                    Logger.Log("Обновление ИБ " + infobase.InfobaseName + " произведено не будет.");
                    continue;
                }
                Logger.Log("Начинаю обновление информационной базы " + infobase.InfobaseName);
                Update(infobase, localUpdateFile);
                Logger.Log("Обновление информационной базы " + infobase.InfobaseName + " закончено.");
            }
        }

        private static void Update(Infobase infobase, string updateFilePath) {
            Logger.Log(Settings.IsDynamicUpdate ? "ДИНАМИЧЕСКОЕ ОБНОВЛЕНИЕ" : "ОБЫЧНОЕ ОБНОВЛЕНИЕ");
#if Pecom
            var blockSchedulerServerError = !BlockSchedulerServer(infobase);
#endif
            if (Settings.IsDynamicUpdate) {
                DisconnectDesigner(infobase);
            }
            else {
                NotifyUsersIfItNeed(infobase);
                BlockIB(infobase);
                DisconnectAllUsers(infobase);
            }
#if Pecom
            if (!blockSchedulerServerError) {
                WaitIfSchedulerServerNotBlocked();
            }
#endif
            LoadAndUpdateCfg(infobase, updateFilePath);
            if (!Settings.IsDynamicUpdate) {
                UnlockIB(infobase);
            }
#if Pecom
            UnlockSchedulerServer(infobase);
#endif
            if (Settings.IsDynamicUpdate) return;
            UpdateSQLIndexes(infobase);
        }

        private static void UpdateSQLIndexes(Infobase infobase) {
            Logger.Log("Обновление индексов SQL...");
            Logger.Log(infobase.TryToUpdateSQLIndexes()
                ? "Обновление индексов SQL выполнено."
                : "Ошибка при обновлении индексов!!!");
        }

#if Pecom
        private static void UnlockSchedulerServer(Infobase infobase) {
            Logger.Log("Снимаю блокировку фонового сервера...");
            if (!infobase.TryToSendBlockCommandToSchedulerServer(false)) {
                Logger.Log("Ошибка при попытке снять блокировку фонового сервера. " +
                           "Возможно блокировка не снята и тогда фоновые процессы не будут выполняться. " +
                           "Снимите блокировку в ручную с помощью Консоли администрирования сервера 1С. " +
                           "Если 1С сообщает об ошибке совместного доступа к файлу, " +
                           "завершите процессы rphost.exe и перезапустите Агента сервера 1С:Предприятие.");
            }
        }
#endif

        private static void UnlockIB(Infobase infobase) {
            Logger.Log("Снимаю блокировку ИБ " + infobase.InfobaseName + "...");
            if (!infobase.TryToBlockIB(false)) {
                Logger.Log("Ошибка при попытке снять блокировку ИБ " + infobase.InfobaseName + ". " +
                           "Возможно блокировка не была снята и тогда пользователи не смогут войти в программу. " +
                           "Снимите блокировку в ручную с помощью Консоли администрирования сервера 1С");
            }
        }

        private static void LoadAndUpdateCfg(Infobase infobase, string updateFilePath) {
            Logger.Log("Начинаю обновлять...");
            if (!infobase.TryToLoadAndUpdateCfgFrom(updateFilePath)) {
                Logger.Log("Ошибка при обновлении базы " + infobase.InfobaseName + ". Проверьте настройки.");
            }
        }

#if Pecom
        private static void WaitIfSchedulerServerNotBlocked() {
            Logger.Log("Проверяю, заблокирован ли фоновый сервер...");
            if (IsSchedulerServerBlocked()) {
                Logger.Log("Фоновый сервер успешно заблокирован.");
            }
            else {
                const int minsWaiting = 6;
                for (var i = 0; i < minsWaiting; i++) {
                    Logger.Log("Фоновый сервер еще не заблокирован. Жду одну минуту...");
#if !DEBUG
                        Thread.Sleep(TimeSpan.FromMinutes(1));
#endif
                    if (IsSchedulerServerBlocked()) {
                        Logger.Log("Фоновый сервер успешно заблокирован.");
                        break;
                    }
                    if (i == (minsWaiting - 1)) {
                        Logger.Log("Слишком долго блокируется фоновый сервер. Продолжаю обновление, не ожидая его.");
                    }
                }
            }
        }
#endif

        private static void DisconnectAllUsers(Infobase infobase) {
            Logger.Log("Завершаю все сеансы на рабочем сервере...");
            if (infobase.TryToDisconnectAllUsers()) return;
            Logger.Log("Ошибка при попытке завершить сеансы на рабочем сервере! Попытаюсь еще раз...");
            if (!infobase.TryToDisconnectAllUsers()) {
                Logger.Log("Снова ошибка при попытке завершить сеансы на рабочем сервере! Обновление продолжается.");
            }
        }

        private static void DisconnectDesigner(Infobase infobase) {
            Logger.Log("Проверяю, работает ли кто-то с Конфигуратором, и завершаю его сеанс...");
            infobase.DisconnectDesigner();
        }

        private static void BlockIB(Infobase infobase) {
            Logger.Log("Блокирую ИБ " + infobase.InfobaseName + "...");
            if (!infobase.TryToBlockIB(true)) {
                Logger.Log("Ошибка при попытке установить блокировку ИБ " + infobase.InfobaseName + ".");
            }
        }

        private static void NotifyUsersIfItNeed(Infobase infobase) {
            var liveConnectionsCount = infobase.GetLiveConnectionsCount();
            if (liveConnectionsCount > 0) {
                Logger.Log("Оповещаю пользователей... Количество пользовательских сеансов: " + liveConnectionsCount);
                NotifyUsers(infobase);
            }
            else {
                Logger.Log("Пользовательские сеансы отсутствуют. Оповещение не требуется.");
            }
        }

#if Pecom
        private static bool BlockSchedulerServer(Infobase infobase) {
            Logger.Log("Блокирую фоновый сервер...");
            if (infobase.TryToSendBlockCommandToSchedulerServer(true)) return true;
            Logger.Log("Ошибка при попытке заблокировать фоновый сервер. Обновление продолжается.");
            return false;
        }
#endif

        private static void NotifyUsers(Infobase infobase) {
            if (!infobase.TryToNotifyAllUsers()) {
                Logger.Log("Ошибка при оповещении пользователей. " +
                           "Возможно некоторые или все пользователи не получили сообщения.");
            }
            Logger.Log("Жду окончания работы пользователей " +
                       TimeSpan.FromMilliseconds(Settings.TimeoutAfterUserNotification).TotalMinutes +
                       " минут...");
#if !DEBUG
            Thread.Sleep(Settings.TimeoutAfterUserNotification);
#endif
            CancelNotifyUsers(infobase);
        }

#if Pecom
        private static bool IsSchedulerServerBlocked() {
            return !ExistCommandFile();
        }

        private static bool ExistCommandFile() {
            const string commandFileName = "blockCommand";
            var path = Path.Combine(Settings.SchedulerServerExchangePath, commandFileName);
            return File.Exists(path);
        }
#endif

        #endregion Update

        #endregion WaitAndUpdate

        #endregion DoWork
    }
}