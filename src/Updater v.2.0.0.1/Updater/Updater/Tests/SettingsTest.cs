#if DEBUG || Pecom
using NUnit.Framework;

namespace Updater.Tests {
    
    [TestFixture]
    public class SettingsTest {

        [Test]
        public void DefaultsTest() {
            Settings.SetToDefaults();
#if Pecom
            Assert.AreEqual(DefaultSettings.SchedulerServerExchangePath, Settings.SchedulerServerExchangePath);
            Assert.AreEqual(DefaultSettings.IsSchedulerServer, Settings.IsSchedulerServer);
            Assert.AreEqual(DefaultSettings.SchedulerClusterAdminName, Settings.SchedulerClusterAdminName);
            Assert.AreEqual(DefaultSettings.SchedulerClusterAdminPass, Settings.SchedulerClusterAdminPass);
            Assert.AreEqual(DefaultSettings.SchedulerClusterPortNumber, Settings.SchedulerClusterPortNumber);
            Assert.AreEqual(DefaultSettings.SchedulerServerName, Settings.SchedulerServerName);
#endif
            Assert.AreEqual(DefaultSettings.PathTo1C, Settings.PathTo1C);
            Assert.AreEqual(DefaultSettings.ClusterAdminName, Settings.ClusterAdminName);
            Assert.AreEqual(DefaultSettings.ClusterAdminPass, Settings.ClusterAdminPass);
            Assert.AreEqual(DefaultSettings.ClusterPortNumber, Settings.ClusterPortNumber);
            Assert.AreEqual(0, Settings.Infobases.Count);
            Assert.AreEqual(DefaultSettings.IsDynamicUpdate, Settings.IsDynamicUpdate);
            Assert.AreEqual(DefaultSettings.LockMessageText, Settings.LockMessageText);
            Assert.AreEqual(DefaultSettings.LockPermissionCode, Settings.LockPermissionCode);
            Assert.AreEqual(DefaultSettings.NotifyMessage, Settings.NotifyMessage);
            Assert.AreEqual(DefaultSettings.OutFilePath, Settings.OutFilePath);
            Assert.AreEqual(DefaultSettings.PathToCOMConnectorDLL, Settings.PathToCOMConnectorDLL);
            Assert.AreEqual(DefaultSettings.ServerName, Settings.ServerName);
            Assert.AreEqual(DefaultSettings.TimeoutAfterUserNotification, Settings.TimeoutAfterUserNotification);
            Assert.AreEqual(DefaultSettings.UpdatesDirPath, Settings.UpdatesDirPath);
        }

        [Test]
        public void LoadFakeFileTest() {
            const string fakeFile = "booooo";

            var result = Settings.Load(fakeFile);
            Assert.IsFalse(result);
        }

        [Test]
        public void LoadIncorrectRootTest() {
            const string testFile = "incorrectRoot.xml";

            var result = Settings.Load(testFile);
            Assert.IsFalse(result);
        }

        [Test]
        public void LoadEmptyAttributesTest() {
            const string testFile = "emptyAttrubutes.xml";

            var result = Settings.Load(testFile);
            Assert.IsFalse(result);
        }

        [Test]
        public void LoadTest() {
            const string testFile = "testSettings.xml";

            var result = Settings.Load(testFile);
            Assert.IsTrue(result);
#if Pecom
            Assert.AreEqual(@"\\AM-SERVER-1\Exchange\UpdaterSettings", Settings.SchedulerServerExchangePath);
            Assert.AreEqual("AM-SERVER-1", Settings.SchedulerServerName);
            Assert.AreEqual(8541, Settings.SchedulerClusterPortNumber);
            Assert.AreEqual("Test", Settings.SchedulerClusterAdminName);
            Assert.AreEqual("Test", Settings.SchedulerClusterAdminPass);
            Assert.AreEqual(false, Settings.IsSchedulerServer);
#endif

            Assert.AreEqual(@"\\mv-it-14\TestUpdate", Settings.UpdatesDirPath);
            Assert.AreEqual(300001, Settings.TimeoutAfterUserNotification);
            Assert.AreEqual(
                "Test. Внимание! В связи с проведение технических работ, программа 1С будет не доступна через 5 минут.",
                Settings.NotifyMessage);
            Assert.AreEqual(false, Settings.IsDynamicUpdate);
            Assert.AreEqual(@"C:\Program Files (x86)\1cv8\8.3.4.389\bin\1cv8.exe", Settings.PathTo1C);
            Assert.AreEqual(@"C:\Program Files (x86)\1cv8\8.3.4.389\bin\comcntr.dll", Settings.PathToCOMConnectorDLL);
            Assert.AreEqual(@"\\AM-SERVER-2\Exchange\UpdaterSettings\log.txt", Settings.OutFilePath);
            Assert.AreEqual(6, Settings.Infobases.Count);
            Assert.AreEqual("Pegasus2008", Settings.Infobases[0].InfobaseName);
            Assert.AreEqual("Администратор", Settings.Infobases[0].InfobaseAdminName);
            Assert.AreEqual("imagination", Settings.Infobases[0].InfobaseAdminPass);
            Assert.AreEqual("Pegasus2008NE", Settings.Infobases[1].InfobaseName);
            Assert.AreEqual("Администратор", Settings.Infobases[1].InfobaseAdminName);
            Assert.AreEqual("imagination", Settings.Infobases[1].InfobaseAdminPass);
            Assert.AreEqual("AM-SERVER-2", Settings.ServerName);
            Assert.AreEqual(7541, Settings.ClusterPortNumber);
            Assert.AreEqual("Test", Settings.ClusterAdminName);
            Assert.AreEqual("Test", Settings.ClusterAdminPass);
        }

        [Test]
        public void LockSettingsTest() {
            Settings.SetToDefaults();
            Assert.AreEqual("Технические работы", Settings.LockMessageText);
            Assert.AreEqual("123321", Settings.LockPermissionCode);
        }

        [Test]
        public void LoadFrom1CTest()
        {
            const string testFile = "from1CSettings.xml";//Файл каждый раз требуется выгружать из 1С.

            var result = Settings.Load(testFile);
            Assert.IsTrue(result);
#if Pecom
            Assert.AreEqual(@"\\CHB-SERVER-1\Exchange\UpdaterSettings", Settings.SchedulerServerExchangePath); 
            Assert.AreEqual("CHB-SERVER-1", Settings.SchedulerServerName);
            Assert.AreEqual(1541, Settings.SchedulerClusterPortNumber);
            Assert.AreEqual("", Settings.SchedulerClusterAdminName);
            Assert.AreEqual("", Settings.SchedulerClusterAdminPass);
            Assert.AreEqual(false, Settings.IsSchedulerServer);
#endif
            Assert.AreEqual(@"\\mv-it-14\TestUpdate", Settings.UpdatesDirPath);
            Assert.AreEqual(300000, Settings.TimeoutAfterUserNotification);
            Assert.AreEqual(
                "Внимание! В связи с проведение технических работ, программа 1С будет не доступна через 5 минут.",
                Settings.NotifyMessage);
            Assert.AreEqual(false, Settings.IsDynamicUpdate);
            Assert.AreEqual(@"C:\Program Files (x86)\1cv8\8.3.4.389\bin\1cv8.exe", Settings.PathTo1C);
            Assert.AreEqual(@"C:\Program Files (x86)\1cv8\8.3.4.389\bin\comcntr.dll", Settings.PathToCOMConnectorDLL);
            Assert.AreEqual(@"\\CHB-SERVER-2\Exchange\UpdaterSettings\log.txt", Settings.OutFilePath);
            Assert.AreEqual(2, Settings.Infobases.Count);
            Assert.AreEqual("Pegasus2008", Settings.Infobases[0].InfobaseName);
            Assert.AreEqual("Администратор", Settings.Infobases[0].InfobaseAdminName);
            Assert.AreEqual("imagination", Settings.Infobases[0].InfobaseAdminPass);
            Assert.AreEqual("Pegasus2008NE", Settings.Infobases[1].InfobaseName);
            Assert.AreEqual("Администратор", Settings.Infobases[1].InfobaseAdminName);
            Assert.AreEqual("imagination", Settings.Infobases[1].InfobaseAdminPass);
            Assert.AreEqual("CHB-SERVER-2", Settings.ServerName);
            Assert.AreEqual(1541, Settings.ClusterPortNumber);
            Assert.AreEqual("", Settings.ClusterAdminName);
            Assert.AreEqual("", Settings.ClusterAdminPass);
        }

#if Pecom
        [Test]
        public void LoadTestSettingsForSchedulerServer() {
            const string testFile = "schedulerServerTestSettings.xml";

            var result = Settings.Load(testFile);
            Assert.IsTrue(result);
            Assert.AreEqual(true, Settings.IsSchedulerServer);
        }

        [Test]
        public void LoadSettingsForSchedulerServerFrom1C() {
            const string testFile = "schedulerServerTestSettingsFrom1C.xml";

            var result = Settings.Load(testFile);
            Assert.IsTrue(result);
            Assert.AreEqual(true, Settings.IsSchedulerServer);
        }
#endif

    }
}
#endif