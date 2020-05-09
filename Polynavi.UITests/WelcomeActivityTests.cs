using NUnit.Framework;
using Xamarin.UITest;

namespace Polynavi.UITests
{
    [TestFixture]
    public class WelcomeActivityTests
    {
        private const string apkPath = "com.starovoitov.polynavi.apk";
        private IApp app;
        
        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android.ApkFile(apkPath).StartApp();
        }

        [Test]
        public void Proceeds_To_Auth_By_Tapping_Next()
        {            
            app.Tap(x => x.Id("button_welcome_next"));
            app.Tap(x => x.Id("button_welcome_next"));
            app.Tap(x => x.Id("button_welcome_finish"));
            app.WaitForElement(x => x.Id("button_auth"));
        }

        [Test]
        public void Proceeds_To_Auth_By_Tapping_Skip()
        {
            app.Tap(x => x.Id("button_welcome_skip"));
            app.WaitForElement(x => x.Id("button_auth"));
        }

        [Test]
        public void Proceeds_To_Auth_By_Swiping()
        {
            app.SwipeRightToLeft(swipeSpeed: 5000);
            app.SwipeRightToLeft(swipeSpeed: 5000);
            app.Tap(x => x.Id("button_welcome_finish"));
            app.WaitForElement(x => x.Id("button_auth"));
        }
    }
}
