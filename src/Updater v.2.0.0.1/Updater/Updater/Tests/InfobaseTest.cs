#if DEBUG
using NUnit.Framework;

namespace Updater.Tests
{
    [TestFixture]
    class InfobaseTest
    {
        [Test]
        public void TryToUpdateSQLIndexesTest() {
            Settings.Load("s-qs-1c-01 Settings.xml");
            var testIB = Settings.Infobases[0];
            var result = testIB.TryToUpdateSQLIndexes();
            Assert.IsTrue(result);
        }
    }
}
#endif