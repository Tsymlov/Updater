using System.Drawing;
using System.Windows.Forms;

namespace UpdaterSetup_2._0._0._0 {
    public class FinishStep:IStep {

        private Label _lbState;

        public string Name {
            get { return "Завершение установки"; }
        }

        public string Instruction {
            get { return "Установка завершена."; }
        }

        public void OnActivate() {
            var mf = Program.MainForm;

            mf.btnCancel.Text = "Завершить";
            mf.btnNext.Visible = false;
            mf.btnPrevious.Visible = false;
            mf.lblInstructionForInstallation.Text = Instruction;
            mf.lblNameOfInstallationPart.Text = Name;
            mf.FormClosing -= mf.MainForm_FormClosing_RequestConfirmation;
            var inscription = Installer.HasError
                ? "Работа мастера завершена с ошибками. Нажмите Завершить для выхода."
                : "Мастер успешно завершил установку. Нажмите Завершить для выхода.";
            _lbState = new Label { AutoSize = true, Location = new Point(25, 70), Text = inscription };
            mf.Controls.Add(_lbState);
        }

        public void OnDeactivate() {
            Program.MainForm.Controls.Remove(_lbState);
            _lbState.Dispose();
        }

        public void Cancel() {
            //do nothing
        }
    }
}