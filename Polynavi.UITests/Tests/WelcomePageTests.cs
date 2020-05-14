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
        }

        [Test]
        public void Proceeds_To_User_Select_By_Tapping_Skip()
        {
            new WelcomePage()
                .TapSkip();

            new UserSelectPage();
        }

        [Test]
        public void Proceeds_To_User_Select_By_Swiping()
        {
            new WelcomePage()
                .SwipePage()
                .SwipePage()
                .TapDone();

            new UserSelectPage();
        }
    }
}
