using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.ServiceProcess;

namespace ServiceSwitcher {
    public partial class MainForm : Form {
        private const string UpdaterServiceName = "Updater";
        private const string SQLServiceName = "MSSQLSERVER";
        private const string AgentOf1CServiceName = "1C:Enterprise 8.3 Server Agent";
        private const string RPHost = "rphost";
        private readonly BindingList<ServiceInfo> _updaterServices;
        private readonly BindingList<ServiceInfo> _sqlServices;
        private readonly BindingList<ServiceInfo> _1CServices; 
        private readonly string _serversFilePath = Path.Combine(Application.StartupPath, "servers.txt");

        public MainForm() {
            _updaterServices = new BindingList<ServiceInfo>();
            _sqlServices = new BindingList<ServiceInfo>();
            _1CServices = new BindingList<ServiceInfo>();

            InitializeComponent();

            dgvUpdaterServices.DataSource = _updaterServices;
            dgvSQLServices.DataSource = _sqlServices;
            dgv1CServices.DataSource = _1CServices;

            dgvUpdaterServices.Columns[1].Visible = false;
            dgvSQLServices.Columns[1].Visible = false;
            dgv1CServices.Columns[1].Visible = false;
        }

        private void MainForm_Load(object sender, EventArgs e) {
            RefillServices();
            tmrRefresher.Enabled = true;
        }

        private void RefillServices() {
            _updaterServices.Clear();
            _1CServices.Clear();
            _sqlServices.Clear();

            var servers = ReadAllLines(_serversFilePath);
            if (servers.Count == 0) {
                MessageBox.Show(
                   "Список рабочих серверов пуст. \r\nПроверьте файл servers.txt в папке с программой.");
                return;
            }

            Fill(_updaterServices, UpdaterServiceName, servers);
            Fill(_1CServices, AgentOf1CServiceName, servers);
            Fill(_sqlServices, SQLServiceName, servers);
        }

        private void Fill(ICollection<ServiceInfo> services, string serviceName, IEnumerable<string> servers) {
            foreach (var server in servers) {
                var serverName = server.Trim();
                services.Add(new ServiceInfo(serverName, serviceName));
            }
        }

        private IList<string> ReadAllLines(string filePath) {
            IList<string> lines;
            try {
                lines = ReadFromFile(filePath);
            }
            catch {
                lines = new string[0];
            }
            return lines;
        }

        private IList<string> ReadFromFile(string serversFilePath) {
            var fileContent = File.ReadAllText(serversFilePath);
            var lines = fileContent.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            return lines;
        }

        private void UpdateAllStatesAsync() {
            for (int index = 0; index < _updaterServices.Count; index++) {
                _updaterServices[index].UpdateStateAsync();
                _1CServices[index].UpdateStateAsync();
                _sqlServices[index].UpdateStateAsync();
            }
        }

        private void btUpdaterRun_Click(object sender, EventArgs e) {
            RunSelectedServices(_updaterServices);
        }

        private void RunSelectedServices(IEnumerable<ServiceInfo> services) {
            foreach (var service in services) {
                if (!service.Checked) continue;
                try {
                    var sc = new ServiceController(service.ServiceName, service.ServerName);
                    sc.Start();
                    sc.Refresh();
                    service.State = sc.Status.ToString();
                    Refresh();
                }
                catch {
                }
            }
        }

        private void bt1CRun_Click(object sender, EventArgs e) {
            RunSelectedServices(_1CServices);
        }

        private void btSQLRun_Click(object sender, EventArgs e) {
            RunSelectedServices(_sqlServices);
        }

        private void btUpdaterStop_Click(object sender, EventArgs e) {
            StopSelectedServices(_updaterServices);
        }

        private void StopSelectedServices(IEnumerable<ServiceInfo> services) {
            foreach (var service in services) {
                if (!service.Checked) continue;
                try {
                    var sc = new ServiceController(service.ServiceName, service.ServerName);
                    sc.Stop();
                    sc.Refresh();
                    service.State = sc.Status.ToString();
                    Refresh();
                }
                catch {
                }
            }
        }

        private void bt1CStop_Click(object sender, EventArgs e) {
            StopSelectedServices(_1CServices);
        }

        private void btSQLStop_Click(object sender, EventArgs e) {
            StopSelectedServices(_sqlServices);
        }

        private void btUpdaterSelectAll_Click(object sender, EventArgs e) {
            SelectAll(_updaterServices);
            dgvUpdaterServices.Refresh();
        }

        private static void SelectAll(IEnumerable<ServiceInfo> services) {
            foreach (var service in services) {
                service.Checked = true;
            }
        }

        private void bt1CSelectAll_Click(object sender, EventArgs e) {
            SelectAll(_1CServices);
            dgv1CServices.Refresh();
        }

        private void btSQLSelectAll_Click(object sender, EventArgs e) {
            SelectAll(_sqlServices);
            dgvSQLServices.Refresh();
        }

        private void btUpdaterUnselectAll_Click(object sender, EventArgs e) {
            UnselectAll(_updaterServices);
            dgvUpdaterServices.Refresh();
        }

        private static void UnselectAll(IEnumerable<ServiceInfo> services) {
            foreach (var service in services) {
                service.Checked = false;
            }
        }

        private void bt1CUnselectAll_Click(object sender, EventArgs e) {
            UnselectAll(_1CServices);
            dgv1CServices.Refresh();
        }

        private void btSQLUnselectAll_Click(object sender, EventArgs e) {
            UnselectAll(_sqlServices);
            dgvSQLServices.Refresh();
        }

        private void dgvUpdaterServices_Scroll(object sender, ScrollEventArgs e) {
            dgv1CServices.FirstDisplayedScrollingRowIndex = dgvUpdaterServices.FirstDisplayedScrollingRowIndex;
            dgvSQLServices.FirstDisplayedScrollingRowIndex = dgvUpdaterServices.FirstDisplayedScrollingRowIndex;
        }

        private void tmrRefresher_Tick(object sender, EventArgs e) {
            UpdateAllStatesAsync();
            Refresh();
        }

        private void btDeleteUpdates_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Уверен?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation)== DialogResult.No) {
                return;
            }
            var success  = DeleteUpdates();
            MessageBox.Show(success ? "Успешно." : "Завершено с ошибками", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool DeleteUpdates() {
            var success = true;
            foreach (var service in _updaterServices) {
                if (service.Checked) {
                    if (!DeleteUpdateFiles(service))
                        success = false;
                }
            }
            return success;
        }

        private bool DeleteUpdateFiles(ServiceInfo service) {
            var success = true;
            var serverPath = @"\\" + service.ServerName;
            var updaterSettingsPath = @"Exchange\UpdaterSettings";
            updaterSettingsPath = Path.Combine(serverPath, updaterSettingsPath);
            try {
                DeleteUpdateFiles(updaterSettingsPath);
            }
            catch {
                success = false;
            }
            return success;
        }

        private static void DeleteUpdateFiles(string updaterSettingsPath) {
            var updates = Directory.GetFiles(updaterSettingsPath, "*.cf", SearchOption.TopDirectoryOnly);
            foreach (var update in updates) {
                File.Delete(update);
            }
        }
    }
}
