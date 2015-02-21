using System.ServiceProcess;
using System.Threading;

namespace Updater {
    public partial class UpdaterService : ServiceBase {
        private readonly Thread _workerThread ;

        public UpdaterService() {
            InitializeComponent();
            _workerThread = new Thread(DoWork) {Name = "Updater"};
            _workerThread.SetApartmentState(ApartmentState.STA);
        }

        private static void DoWork() {
            LoadSettings();
            StartWork();
        }

        private static void LoadSettings() {
            Logger.Log("Пытаюсь загрузить файл настроек " + Settings.SettingsFilePath);
            LoadSettingsWhileNotSuccess(Settings.SettingsFilePath);
            Logger.Log("Успешно загружен файл настроек " + Settings.SettingsFilePath);
        }

        private static void StartWork() {
            Logger.Log("Начинаю работу...");
#if Pecom
            Logger.Log("Версия службы \"Агент обновлений филиалов ПЭК\": " + Program.GetVersion());
            if (Settings.IsSchedulerServer) {
                Logger.Log("РЕЖИМ СЛУЖБЫ БЛОКИРОВАНИЯ ФОНОВОГО СЕРВЕРА");
                SchedulerServerBlocker.DoWork();
            }
            else {
                Logger.Log("РЕЖИМ СЛУЖБЫ РАБОЧЕГО СЕРВЕРА");
                Updater.DoWork();
            }
#else
            Updater.DoWork();
#endif
        }

        private static void LoadSettingsWhileNotSuccess(string path) {
            while (!Settings.Load(path)) {
                Thread.Sleep(5000); // 5 sec
            }
        }

        protected override void OnStart(string[] args) {
            Logger.Log("Запуск службы.");
            _workerThread.Start();
        }

        protected override void OnStop() {
            Logger.Log("Остановка службы.");
            _workerThread.Abort();
        }
    }
}
