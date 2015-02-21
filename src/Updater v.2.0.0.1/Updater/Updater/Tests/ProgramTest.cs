#if DEBUG
using System;
using NUnit.Framework;

namespace Updater.Tests {
    [TestFixture]
    public class ProgramTest {
        [Test]
        public void GetVersionTest() {
            var ver = Program.GetVersion();
            var expectedVersion = new Version("1.0.0.0");
            Assert.AreEqual(expectedVersion.Major, ver.Major);
         }
    }
}
#endif