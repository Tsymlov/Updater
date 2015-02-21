using System;
using System.IO;

namespace UpdaterSetup_2._0._0._0 {
    public static class Parameters {
        private const string ApplicationName = "Updater";
        private const string DefaultUpdatesDirPath = @"\\fileserver\Updates";
        private const string ComcntDLLFilename = "comcntr.dll";
// ReSharper disable InconsistentNaming
        private const string _1Cv8EXEFilename = "1cv8.exe";
// ReSharper restore InconsistentNaming
        private const string LogTXTFilename = "log.txt";
        private const string _1CBinDefaultPath = @"C:\Program Files\1cv8\8.3.4.389\bin";

        public static string PathTo1CBinDir { get; set; }
        public static string InstallationPath { get; set; }
        public static string UpdatesDirPath { get; set; }

        public static string PathToCOMConnectorDLL {
            get { return Path.Combine(PathTo1CBinDir, ComcntDLLFilename); }
        }

        public static string PathTo1Cv8EXE {
            get { return Path.Combine(PathTo1CBinDir, _1Cv8EXEFilename); }
        }

        public static string OutFilePath {
            get { return Path.Combine(InstallationPath, LogTXTFilename); }
        }

        static Parameters() {
            InstallationPath = GenerateProgrammFilesWorkDirPath();
            PathTo1CBinDir = Get1CBinDirPath();
            UpdatesDirPath = DefaultUpdatesDirPath;
        }

        private static string Get1CBinDirPath() {
            return _1CBinDefaultPath;
        }

        private static string GenerateProgrammFilesWorkDirPath() {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            return Path.Combine(programFiles, ApplicationName);
        }
    }
}