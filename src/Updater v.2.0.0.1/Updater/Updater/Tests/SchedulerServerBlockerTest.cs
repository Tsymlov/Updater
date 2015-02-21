#if Pecom
using System.Threading;
using NUnit.Framework;

namespace Updater.Tests {
    [TestFixture]
    public class SchedulerServerBlockTest {
        private const int Timeout = 1800000;

        [Test]
        public void StartTest() {
            Settings.Load(Settings.SettingsFilePath);
            var testThread = new Thread(SchedulerServerBlocker.DoWork) {Name = "TestBlocker"};
            testThread.SetApartmentState(ApartmentState.STA);
            testThread.Start();
            Thread.Sleep(Timeout);
            testThread.Abort();
        }
    }
}
#endif