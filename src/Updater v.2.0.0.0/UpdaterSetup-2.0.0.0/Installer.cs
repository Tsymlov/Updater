using System.Collections.Generic;
using UpdaterSetup_2._0._0._0.Steps;

namespace UpdaterSetup_2._0._0._0 {
    internal static class Installer {
        public static bool HasError;

        private static readonly List<IStep> Steps;
        private static int _currentStepIndex;

        static Installer() {
            HasError = false;
            Steps = new List<IStep> {
                new RequestPaths(),
                new ReadyToInstallStep(),
                new InstallationStep(),
                new FinishStep()
            };
            _currentStepIndex = 0;
        }

        public static void Next() {
            if (_currentStepIndex >= (Steps.Count-1)) return;
            CurrentStep.OnDeactivate();
            _currentStepIndex++;
            CurrentStep.OnActivate();
        }

        public static void Previous() {
            if (_currentStepIndex <= 0) return;
            CurrentStep.OnDeactivate();
            _currentStepIndex--;
            CurrentStep.OnActivate();
        }

        public static IStep CurrentStep {
            get { return Steps[_currentStepIndex]; }
        }
    }
}
