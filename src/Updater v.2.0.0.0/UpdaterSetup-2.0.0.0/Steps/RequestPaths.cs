using System;
using System.IO;
using System.Windows.Forms;

namespace UpdaterSetup_2._0._0._0 {
    public class RequestPaths:IStep {
        private Label _lbInstallPathInscription;
        private Button _btnChangeInstallPath;
        private Label _lbInstallPath;
        private Label _lbUpdatesPathInscription;
        private Label _lbUpdatesPath;
        private Button _btnChangeUpdatesPath;
        private Button _btnChange1CDir;
        private Label _lb1CDir;
        private Label _lb1CDirInscription;

        public string Name {
            get { return "Выбор каталогов"; }
        }

        public string Instruction {
            get { return "Укажите каталоги, которые будут использованы системой"; }
        }

        public void OnActivate() {
            ShowControls();
        }

        private void ShowControls() {
            _lbInstallPathInscription = new Label();
            _btnChangeInstallPath = new Button();
            _lbInstallPath = new Label();
            _lbUpdatesPathInscription = new Label();
            _lbUpdatesPath = new Label();
            _btnChangeUpdatesPath = new Button();
            _btnChange1CDir = new Button();
            _lb1CDir = new Label();
            _lb1CDirInscription = new Label();
            // 
            // lbInstallPathInscriptionInstallPathInscription.AutoSize = true;
            _lbInstallPathInscription.Location = new System.Drawing.Point(12, 84);
            _lbInstallPathInscription.Name = "lbInstallPathInscription";
            _lbInstallPathInscription.Size = new System.Drawing.Size(106, 13);
            _lbInstallPathInscription.TabIndex = 104;
            _lbInstallPathInscription.Text = "Каталог установки:";
            // 
            // lbInstallPath
            // 
            _lbInstallPath.AutoSize = true;
            _lbInstallPath.Location = new System.Drawing.Point(12, 103);
            _lbInstallPath.Name = "lbInstallPath";
            _lbInstallPath.Size = new System.Drawing.Size(0, 13);
            _lbInstallPath.TabIndex = 107;
            _lbInstallPath.Text = Parameters.InstallationPath;
            // 
            // lbUpdatesPathInscription
            // 
            _lbUpdatesPathInscription.AutoSize = true;
            _lbUpdatesPathInscription.Location = new System.Drawing.Point(12, 143);
            _lbUpdatesPathInscription.Name = "lbUpdatesPathInscription";
            _lbUpdatesPathInscription.Size = new System.Drawing.Size(109, 13);
            _lbUpdatesPathInscription.TabIndex = 108;
            _lbUpdatesPathInscription.Text = "Ресурс обновлений:";
            // 
            // lbUpdatesPath
            // 
            _lbUpdatesPath.AutoSize = true;
            _lbUpdatesPath.Location = new System.Drawing.Point(12, 163);
            _lbUpdatesPath.Name = "lbUpdaterPath";
            _lbUpdatesPath.Size = new System.Drawing.Size(0, 13);
            _lbUpdatesPath.TabIndex = 109;
            _lbUpdatesPath.Text = Parameters.UpdatesDirPath;
            // 
            // btnChangeInstallPath
            // 
            _btnChangeInstallPath.Location = new System.Drawing.Point(398, 98);
            _btnChangeInstallPath.Name = "btnChangeInstallPath";
            _btnChangeInstallPath.Size = new System.Drawing.Size(80, 23);
            _btnChangeInstallPath.TabIndex = 106;
            _btnChangeInstallPath.Text = "Изменить";
            _btnChangeInstallPath.UseVisualStyleBackColor = true;
            _btnChangeInstallPath.Click += new System.EventHandler(ChangeInstallPath);
            // 
            // btnChangeUpdatesPath
            // 
            _btnChangeUpdatesPath.Location = new System.Drawing.Point(398, 158);
            _btnChangeUpdatesPath.Name = "btnChangeUpdatesPath";
            _btnChangeUpdatesPath.Size = new System.Drawing.Size(80, 23);
            _btnChangeUpdatesPath.TabIndex = 110;
            _btnChangeUpdatesPath.Text = "Изменить";
            _btnChangeUpdatesPath.UseVisualStyleBackColor = true;
            _btnChangeUpdatesPath.Click += new System.EventHandler(ChangeUpdatesPath);
            // 
            // btnChange1CDir
            // 
            _btnChange1CDir.Location = new System.Drawing.Point(398, 216);
            _btnChange1CDir.Name = "btnChange1CDir";
            _btnChange1CDir.Size = new System.Drawing.Size(80, 23);
            _btnChange1CDir.TabIndex = 113;
            _btnChange1CDir.Text = "Изменить";
            _btnChange1CDir.UseVisualStyleBackColor = true;
            _btnChange1CDir.Click += new System.EventHandler(Change1CDir);
            // 
            // lb1CDir
            // 
            _lb1CDir.AutoSize = true;
            _lb1CDir.Location = new System.Drawing.Point(12, 218);
            _lb1CDir.Name = "lb1CDir";
            _lb1CDir.Size = new System.Drawing.Size(0, 13);
            _lb1CDir.TabIndex = 112;
            _lb1CDir.Text = Parameters.PathTo1CBinDir;
            // 
            // lb1CDirInscription
            // 
            _lb1CDirInscription.AutoSize = true;
            _lb1CDirInscription.Location = new System.Drawing.Point(12, 198);
            _lb1CDirInscription.Name = "lb1CDirInscription";
            _lb1CDirInscription.Size = new System.Drawing.Size(299, 13);
            _lb1CDirInscription.TabIndex = 111;
            _lb1CDirInscription.Text = "Каталог с текущей версией платформы 1С:Предприятие:";

            var mf = Program.MainForm;
            mf.Controls.Add(_lb1CDir);
            mf.Controls.Add(_lb1CDirInscription);
            mf.Controls.Add(_btnChangeUpdatesPath);
            mf.Controls.Add(_lbUpdatesPath);
            mf.Controls.Add(_lbUpdatesPathInscription);
            mf.Controls.Add(_lbInstallPath);
            mf.Controls.Add(_btnChangeInstallPath);
            mf.Controls.Add(_lbInstallPathInscription);
            mf.Controls.Add(_btnChange1CDir);

            mf.lblInstructionForInstallation.Text = Instruction;
            mf.lblNameOfInstallationPart.Text = Name;

            mf.btnPrevious.Visible = false;
        }

        private void Change1CDir(object sender, EventArgs e) {
            var selectedPath = ShowFolderBrowserDialog("Выберите каталог текущей версии платформы 1С:Предприятие...", Parameters.PathTo1CBinDir);

            if (String.IsNullOrEmpty(selectedPath)) return;
            Parameters.PathTo1CBinDir = selectedPath;
            _lb1CDir.Text = selectedPath;
        }

        private static string ShowFolderBrowserDialog(string description, string startPath) {
            var dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            dialog.Description = description;
            dialog.SelectedPath = startPath;
            var result = dialog.ShowDialog();
            var selectedPath = "";
            if (result == DialogResult.OK) {
                selectedPath = dialog.SelectedPath;
            }
            return selectedPath;
        }

        private void ChangeUpdatesPath(object sender, EventArgs e) {
            var selectedPath = ShowFolderBrowserDialog("Выберите ресурс, из которого будут браться файлы с обновлениями...", Parameters.UpdatesDirPath);

            if (String.IsNullOrEmpty(selectedPath)) return;
            Parameters.UpdatesDirPath = selectedPath;
            _lbUpdatesPath.Text = selectedPath;
        }

        private void ChangeInstallPath(object sender, EventArgs e) {
            var selectedPath = ShowFolderBrowserDialog("Выберите каталог, в который будут установлены программные файлы...", Parameters.InstallationPath);

            if (String.IsNullOrEmpty(selectedPath)) return;
            Parameters.InstallationPath = selectedPath;
            _lbInstallPath.Text = selectedPath;
        }

        public void OnDeactivate() {
            HideControls();
        }

        private void HideControls() {
            var mf = Program.MainForm;
            mf.Controls.Remove(_lb1CDir);
            mf.Controls.Add(_lb1CDirInscription);
            mf.Controls.Add(_btnChangeUpdatesPath);
            mf.Controls.Add(_lbUpdatesPath);
            mf.Controls.Add(_lbUpdatesPathInscription);
            mf.Controls.Add(_lbInstallPath);
            mf.Controls.Add(_btnChangeInstallPath);
            mf.Controls.Add(_lbInstallPathInscription);
            mf.Controls.Add(_btnChange1CDir);
            _lb1CDir.Dispose();
            _lb1CDirInscription.Dispose();
            _btnChangeUpdatesPath.Dispose();
            _lbUpdatesPath.Dispose();
            _lbUpdatesPathInscription.Dispose();
            _lbInstallPath.Dispose();
            _btnChangeInstallPath.Dispose();
            _lbInstallPathInscription.Dispose();
            _btnChange1CDir.Dispose();

            mf.btnPrevious.Visible = true;
        }

        public void Cancel() {
            //do nothing
        }
    }
}