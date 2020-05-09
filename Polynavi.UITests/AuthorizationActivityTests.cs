using NUnit.Framework;
using Xamarin.UITest;

namespace Polynavi.UITests
{
    [TestFixture]
    public class AuthorizationActivityTests
    {
        private IApp app;

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp();

            ProcceedToAuth();
        }

        private void ProcceedToAuth()
        {
            app.Tap(x => x.Id("button_welcome_skip"));
            app.Tap(x => x.Id("button_student_user_type_select"));
            app.WaitForElement(x => x.Id("textview_auth_skip"));
        }

        [Test]
        public void Skip_Auth()
        {
            app.Tap(x => x.Marked("textview_auth_skip"));
            app.WaitForElement(x => x.Id("drawerlayout_main"));
        }
    }
}
