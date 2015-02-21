using System;
using System.ComponentModel;
using System.ServiceProcess;

namespace ServiceSwitcher {
    public class ServiceInfo {

        private bool _updatingState;
        public ServiceInfo(string serverName, string serviceName) {
            ServerName = serverName;
            ServiceName = serviceName;
            State = "Unknown";
            Checked = false;
        }

        public string ServerName { get; private set; }

        public string ServiceName { get; private set; }

        public string State { get; set; }

        public bool Checked { get; set; }

        public void UpdateStateAsync() {
            if (!_updatingState) {
                var worker = new BackgroundWorker();
                worker.DoWork += UpdateState;
                worker.RunWorkerAsync();
                _updatingState = true;
            }
        }

        private void UpdateState(object sender, DoWorkEventArgs e) {
            try {
                var sc = new ServiceController(ServiceName, ServerName);
                State = sc.Status.ToString();
                sc.Dispose();
            }
            catch {
                State = "Not available";
            }
            e.Cancel = true;
            var worker = (BackgroundWorker) sender;
            _updatingState = false;
            worker.Dispose();
        }
    }
}