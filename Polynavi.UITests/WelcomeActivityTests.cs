using NUnit.Framework;
using Xamarin.UITest;

namespace Polynavi.UITests
{
    [TestFixture]
    public class WelcomeActivityTests
    {
        private IApp app;
        
        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp();
        }

        [Test]
        public void Proceeds_To_User_Select_By_Tapping_Next()
        {            
            app.Tap(x => x.Id("button_welcome_next"));
            app.Tap(x => x.Id("button_welcome_next"));
            app.Tap(x => x.Id("button_welcome_finish"));
            app.WaitForElement(x => x.Id("button_student_user_type_select"));
        }

        [Test]
        public void Proceeds_To_User_Select_By_Tapping_Skip()
        {
            app.Tap(x => x.Id("button_welcome_skip"));
            app.WaitForElement(x => x.Id("button_student_user_type_select"));
        }

        [Test]
        public void Proceeds_To_User_Select_By_Swiping()
        {
            app.SwipeRightToLeft(swipeSpeed: 5000);
            app.SwipeRightToLeft(swipeSpeed: 5000);
            app.Tap(x => x.Id("button_welcome_finish"));
            app.WaitForElement(x => x.Id("button_student_user_type_select"));
        }
    }
}
