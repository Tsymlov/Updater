using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.ServiceProcess;
using System.Windows.Forms;
using Microsoft.Win32;

namespace UpdaterSetup_2._0._0._0.Steps {
    public class UninstallationStep : IStep {
        private readonly MainForm _mainForm;
        private ProgressBar _progressBar;
        private Label _lbState;

        public UninstallationStep() {
            _mainForm = Program.MainForm;
        }

        public string Name {
            get { return "Удаление компонентов"; }
        }

        public string Instruction {
            get { return "Подождите, пока компоненты удаляться. Это может занять несколько минут."; }
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
            _lbState = new Label {AutoSize = true, Location = new Point(122, 172)};
            _mainForm.Controls.Add(_lbState);
            _mainForm.Controls.Add(_progressBar);
            _mainForm.worker.DoWork += Uninstall;
            _mainForm.worker.ProgressChanged += RefreshProgress;
            _mainForm.worker.RunWorkerAsync();
        }

        private void Uninstall(object sender, DoWorkEventArgs e) {
            DeleteInstallDir();
            StopAndUninstallService();
            TryToRemoveFromProgramms();
            _mainForm.worker.ReportProgress(100, "Удаление завершено.");
        }

        private void DeleteInstallDir() {
            try {
                Directory.Delete(Parameters.InstallationPath, true);
                _mainForm.worker.ReportProgress(40, "Программные файлы удалены.");
            }
            catch (Exception e) {
                _mainForm.worker.ReportProgress(40, "Удаление программных файлов...ОШИБКА!");
                Console.WriteLine(e);
                Installer.HasError = true;
            }
        }

        private void TryToRemoveFromProgramms() {
            try {
                RemoveFromPrograms();
                _mainForm.worker.ReportProgress(100, "Изменения зарегистрированы в системе.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(100, "Регистрация изменений в системе...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private static void RemoveFromPrograms() {
            RemoveBrunchFromRegistry();
        }

        private static void RemoveBrunchFromRegistry() {
            Registry.LocalMachine.DeleteSubKeyTree(@"Software\Microsoft\Windows\CurrentVersion\Uninstall\Updater");
        }

        private void StopAndUninstallService() {
            try {
                StopService();
                _mainForm.worker.ReportProgress(80, "Служба Агента обновлений остановлена.");
                TryToUninstallService();
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(80, "Остановка службы Агента обновлений...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private static void StopService() {
            var service = new ServiceController("Updater");
            service.Stop();
            service.WaitForStatus(ServiceControllerStatus.Stopped);
        }

        private void TryToUninstallService() {
            try {
                UninstallService();
                _mainForm.worker.ReportProgress(85, "Служба Агента обновлений удалена.");
            }
            catch (Exception ex) {
                _mainForm.worker.ReportProgress(85, "Удаление службы Агента обновлений...ОШИБКА!");
                Console.WriteLine(ex);
                Installer.HasError = true;
            }
        }

        private static void UninstallService() {
            var netFrameworkPath = Utils.GetNetFrameworkPath();
            var installUtilPath = Path.Combine(netFrameworkPath, @"installutil.exe");
            var updaterPath = Path.Combine(Parameters.InstallationPath, "Updater.exe");
            Utils.StartAndWaitProcess(installUtilPath, updaterPath + " /u");
        }

        private void RefreshProgress(object sender, ProgressChangedEventArgs e) {
            _progressBar.Value = e.ProgressPercentage;
            _lbState.Text += (string) e.UserState + "\r\n";

            if (e.ProgressPercentage < 100) return;
            _mainForm.btnCancel.Enabled = true;
            _mainForm.btnCancel.Text = "Завершить";
            _mainForm.FormClosing -= _mainForm.MainForm_FormClosing_RequestConfirmation;
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