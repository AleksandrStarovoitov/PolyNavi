using NUnit.Framework;
using Polynavi.UITests.Pages;
using Xamarin.UITest;

namespace Polynavi.UITests.Tests
{
    public class WelcomePageTests : BaseTestFixture 
    {
        public WelcomePageTests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void Proceeds_To_User_Select_By_Tapping_Next()
        {
            new WelcomePage()
                .TapNext()
                .TapNext()
                .TapDone();

            new UserSelectPage();
            // app.Tap(x => x.Id("button_welcome_next"));
            // app.Tap(x => x.Id("button_welcome_next"));
            // app.Tap(x => x.Id("button_welcome_finish"));
            // app.WaitForElement(x => x.Id("button_student_user_type_select"));
        }

        [Test]
        public void Proceeds_To_User_Select_By_Tapping_Skip()
        {
            new WelcomePage()
                .TapSkip();

            new UserSelectPage();
            // app.Tap(x => x.Id("button_welcome_skip"));
            // app.WaitForElement(x => x.Id("button_student_user_type_select"));
        }

        [Test]
        public void Proceeds_To_User_Select_By_Swiping()
        {
            new WelcomePage()
                .SwipePage()
                .SwipePage()
                .TapDone();

            new UserSelectPage();
            // app.SwipeRightToLeft(swipeSpeed: 5000);
            // app.SwipeRightToLeft(swipeSpeed: 5000);
            // app.Tap(x => x.Id("button_welcome_finish"));
            // app.WaitForElement(x => x.Id("button_student_user_type_select"));
        }
    }
}
