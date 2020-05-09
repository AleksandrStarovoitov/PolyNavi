using NUnit.Framework;
using Xamarin.UITest;

namespace Polynavi.UITests
{
    [TestFixture]
    public class Tests
    {
        private const string apkPath = "com.starovoitov.polynavi.apk";
        private IApp app;
        
        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android.ApkFile(apkPath).StartApp();
        }

        [Test]
        public void Test()
        {
            app.Repl();
        }
    }
}
