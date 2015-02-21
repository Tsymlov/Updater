using System.Windows.Forms;
using UpdaterSetup_2._0._0._0.Steps;

namespace UpdaterSetup_2._0._0._0 {
    public partial class MainForm : Form {
        private bool _uninstall;
        public MainForm() {
            InitializeComponent();
        }

        public MainForm(bool uninstall) {
            InitializeComponent();
            _uninstall = uninstall;
        }

        private void MainForm_Load(object sender, System.EventArgs e) {
            if (_uninstall) {
                Uninstaller.Uninstall();
            }
            else {
                Installer.CurrentStep.OnActivate();
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e) {
            Close();
        }

        public void MainForm_FormClosing_RequestConfirmation(object sender, FormClosingEventArgs e) {
            if (MessageBox.Show("Вы уверены, что хотите прервать установку?", Text, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No) {
                e.Cancel = true;
                return;
            }
            Installer.CurrentStep.Cancel();
        }

        private void btnNext_Click(object sender, System.EventArgs e) {
            Installer.Next();
        }

        private void btnPrevious_Click(object sender, System.EventArgs e) {
            Installer.Previous();
        }
    }
}
