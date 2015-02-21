using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace UpdaterSetup_2._0._0._0 {
    public class CheckSystemStep : IStep {
        public CheckSystemStep() {
            _mf = Program.MainForm;
        }

        public string Name {
            get { return "Проверка установленных компонент."; }
        }

        public string Instruction {
            get { return "Подождите, пока программа установки проверит установленные компоненты..."; }
        }

        private Label _lbDonNetCheckingStatus;
        private readonly MainForm _mf;

        public void OnActivate() {
            _mf.lblInstructionForInstallation.Text = Instruction;
            _mf.lblNameOfInstallationPart.Text = Name;
            _mf.btnPrevious.Visible = false;
            _mf.btnNext.Visible = false;

            _lbDonNetCheckingStatus = new Label {
                AutoSize = true,
                Text = ".NET Framework 4",
                Location = new Point(100, 100)
            };
            _mf.Controls.Add(_lbDonNetCheckingStatus);
            _mf.worker.DoWork += CheckDotNet;
            _mf.worker.RunWorkerCompleted += CheckDotNetComplete;
            _mf.worker.RunWorkerAsync();

        }

        private void CheckDotNetComplete(object sender, RunWorkerCompletedEventArgs e) {
            _lbDonNetCheckingStatus.Text += "..................................................Ok";
            _mf.btnNext.Visible = true;
        }

        private static void CheckDotNet(object sender, DoWorkEventArgs doWorkEventArgs) {
            Thread.Sleep(5000);
        }

        public void OnDeactivate() {
            _mf.Controls.Remove(_lbDonNetCheckingStatus);
        }

        public void Cancel() {
            _mf.worker.CancelAsync();
        }

    }
}