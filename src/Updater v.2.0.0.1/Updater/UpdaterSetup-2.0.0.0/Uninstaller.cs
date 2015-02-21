using UpdaterSetup_2._0._0._0.Steps;

namespace UpdaterSetup_2._0._0._0 {
    internal static class Uninstaller {
        public static void Uninstall() {
            new UninstallationStep().OnActivate();
        }
    }
}