using System.Drawing;
using System.Reflection.Emit;
using System.Security.AccessControl;
using Label = System.Windows.Forms.Label;

namespace UpdaterSetup_2._0._0._0 {
    public class ReadyToInstallStep : IStep {
        private Label _label;

        public string Name {
            get { return "Всё готово для начала установки"; }
        }

        public string Instruction {
            get { return "Мастер готов начать установку."; }
        }

        public void OnActivate() {
            Program.MainForm.btnNext.Text = "Установить";
            _label = new Label {
                Text =
                    "Нажмите Установить для начала установки.\r\n\r\nЕсли вы хотите проверить или изменить установочные параметры, нажмите Назад.\r\n" +
                    "Нажмите Отмена для завершения работы мастера установки.",
                Location = new Point(25, 70),
                AutoSize = true
            };
            Program.MainForm.Controls.Add(_label);

            Program.MainForm.lblInstructionForInstallation.Text = Instruction;
            Program.MainForm.lblNameOfInstallationPart.Text = Name;

        }

        public void OnDeactivate() {
            Program.MainForm.Controls.Remove(_label);
            Program.MainForm.btnNext.Text = "Далее >";
            _label.Dispose();
        }

        public void Cancel() {
            //do nothing
        }
    }
}