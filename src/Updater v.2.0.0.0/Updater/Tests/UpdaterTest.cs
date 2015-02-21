#if DEBUG || Pecom
using System.Threading;
using NUnit.Framework;

namespace Updater.Tests {
    [TestFixture]
    public class UpdaterTest {
        private const int Timeout = 18000000;

        [Test]
        public void StartTest() {
            Settings.Load(Settings.SettingsFilePath);
            var testThread = new Thread(Updater.DoWork) {Name = "TestUpdater"};
            testThread.SetApartmentState(ApartmentState.STA);
            testThread.Start();
            Thread.Sleep(Timeout);
            testThread.Abort();
        }         
    }
}
#endif