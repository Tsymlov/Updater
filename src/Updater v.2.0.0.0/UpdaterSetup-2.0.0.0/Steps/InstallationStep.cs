using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;

namespace UpdaterSetup_2._0._0._0.Steps {
    public class InstallationStep:IStep {
        private readonly MainForm _mainForm;
        private ProgressBar _progressBar;
        private Label _lbState;

        public InstallationStep() {
            _mainForm = Program.MainForm;
        }

        public string Name {
            get { return "Установка компонентов"; }
        }

        public string Instruction {
            get { return "Подождите, пока установятся компоненты. Это может занять несколько минут."; }
        }

        public void OnActivate() {
            _mainForm.btnPrevious.Enabled = false;
            _mainForm.btnNext.Enabled = false;
            _mainForm.btnCancel.Enabled = false;
            _mainForm.lblInstructionForInstallation.Text = Instruction;
            _mainForm.lblNameOfInstallationPart.Text = Name;
            _progressBar = new ProgressBar {
                Minimum = 0,
                Maximum = 100,
                Location = new Point(12, 72),
                Name = "progressBar",
                Size = new Size(460, 23)
            };
            _lbState = new Label {AutoSize = true, Location = new Point(122, 122)};
            _mainForm.Controls.Add(_lbState);
            _mainForm.Controls.Add(_progressBar);
            _mainForm.worker.DoWork += Install;
            _mainForm.worker.ProgressChanged += RefreshProgress;
            _mainForm.worker.RunWorkerAsync();
        }

        private void Install(object sender, DoWorkEventArgs e) {

            if (!CreateInstallPath()) return;
            if (!TryToCopyUpdaterExe()) return;
            TryToGenerateSettingsFile();
            ShareInstallDir();
            RegisterComComponent();
            InstallAndStartService();
            TryToAddToProgramms();
            _mainForm.worker.ReportProgress(100, "Установка завершена.");
        }

        private void TryToAddToProgramms() {
            try {
                AddToPrograms();
                _mainForm.worker.ReportProgress(100, "Изменения зарегистрированы в системе.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(100, "Регистрация изменений в системе...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private void InstallAndStartService() {
            try {
                InstallService();
                _mainForm.worker.ReportProgress(80, "Установлена служба Агента обновлений.");
                TryToStartService();
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(80, "Установка службы Агента обновлений...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private void TryToStartService() {
            try {
                StartService();
                _mainForm.worker.ReportProgress(85, "Запущена служба Агента обновлений.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(85, "Запуск службы Агента обновлений...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private void RegisterComComponent() {
            try {
                Utils.RegisterComComponent(Parameters.PathToCOMConnectorDLL);
                _mainForm.worker.ReportProgress(62, "Зарегистрирована библиотека comcntr.dll.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(62, "Регистрация библиотеки comcntr.dll...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private void ShareInstallDir() {
            try {
                Utils.ShareInstallationDir("localhost", "Updater", "Этот каталог используется системой обновлений 1С");
                _mainForm.worker.ReportProgress(50, "Открыт общий доступ по сети к каталогу установки.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(50,
                    "Предоставление общего доступа по сети к каталогу установки...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private void TryToGenerateSettingsFile() {
            try {
                GenerateSettingsFile();
                _mainForm.worker.ReportProgress(37, "Сформирован файл настроек.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(37, "Формирование файла настроек...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private bool TryToCopyUpdaterExe() {
            try {
                CopyUpdaterExe();
                _mainForm.worker.ReportProgress(25, "Скопирован Updater.exe.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(100, "Копирование Updater.exe...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
                return false;
            }
            return true;
        }

        private bool CreateInstallPath() {
            try {
                if (!Directory.Exists(Parameters.InstallationPath)) {
                    Directory.CreateDirectory(Parameters.InstallationPath);
                }
                _mainForm.worker.ReportProgress(12, "Проверен каталог установки.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(100, "Создание каталога установки...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
                return false;
            }
            return true;
        }

        private static void AddToPrograms() {
            CreateBrunchInRegistry();
        }

        private static void CreateBrunchInRegistry() {
            var key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Uninstall", true);
            
            key = key.CreateSubKey("Updater");
            key.SetValue("DisplayName", "Агент обновлений информационных баз 1С");
            var updaterPath = Path.Combine(Parameters.InstallationPath, "Updater.exe");
            key.SetValue("DisplayIcon", updaterPath);
            var uninstallPath = Path.Combine(Parameters.InstallationPath, "uninstall.exe");
            key.SetValue("UninstallString", uninstallPath);
            key.SetValue("NoModify", 0x1);
            key.SetValue("NoRepair", 0x1);
        }

        private static void StartService() {
            var service = new ServiceController("Updater");
            service.Start();
            service.WaitForStatus(ServiceControllerStatus.Running);
        }

        private static void InstallService() {
            var netFrameworkPath = Utils.GetNetFrameworkPath();
            var installUtilPath = Path.Combine(netFrameworkPath, @"installutil.exe");  
            var updaterPath = Path.Combine(Parameters.InstallationPath, "Updater.exe");
            Utils.StartAndWaitProcess(installUtilPath, updaterPath);
        }

        private static void GenerateSettingsFile() {
            var path = Path.Combine(Parameters.InstallationPath, "Settings.xml");
            GenerateSettingsFile(path);
        }

        private static void GenerateSettingsFile(string path) {
            using (var xml = XmlWriter.Create(path)) {
                xml.WriteStartDocument();
                xml.WriteStartElement("UpdaterSettings");
                xml.WriteAttributeString("UpdatesDirPath", Parameters.UpdatesDirPath);
                xml.WriteAttributeString("TimeoutAfterUserNotification", "5");
                xml.WriteAttributeString("NotifyMessage",
                    "Внимание! В связи с проведение технических работ, программа 1С будет не доступна через 5 минут.");
                xml.WriteAttributeString("IsDynamicUpdate", "false");
                xml.WriteAttributeString("PathTo1C", Parameters.PathTo1Cv8EXE);
                xml.WriteAttributeString("PathToCOMConnectorDLL", Parameters.PathToCOMConnectorDLL);
                xml.WriteAttributeString("OutFilePath", Parameters.OutFilePath);
                xml.WriteAttributeString("ClusterPortNumber", "1541");
                xml.WriteAttributeString("ClusterAdminName", String.Empty);
                xml.WriteAttributeString("ClusterAdminPass", String.Empty);
                xml.WriteAttributeString("ServerName", Environment.MachineName);
                xml.WriteEndElement();
                xml.Flush();
            }
        }

        private static void CopyUpdaterExe() {
            var updater = Properties.Resources.Updater;
            var path = Path.Combine(Parameters.InstallationPath, "Updater.exe");
            File.WriteAllBytes(path, updater);
        }

        private void RefreshProgress(object sender, ProgressChangedEventArgs e) {
            _progressBar.Value = e.ProgressPercentage;
            _lbState.Text += (string)e.UserState + "\r\n";
            
            if (e.ProgressPercentage >= 100) {
                _mainForm.btnNext.Enabled = true;
            }
        }

        public void OnDeactivate() {
            _mainForm.Controls.Remove(_progressBar);
            _mainForm.Controls.Remove(_lbState);
            _progressBar.Dispose();
            _lbState.Dispose();
            _mainForm.btnCancel.Enabled = true;
        }

        public void Cancel() {
            _mainForm.worker.CancelAsync();
        }
    }
}