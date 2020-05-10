using NUnit.Framework;
using Polynavi.UITests.Pages;
using Xamarin.UITest;

namespace Polynavi.UITests.Tests
{
    public class MainBuildingPageTests : InstalledAppTestFixture
    {
        public MainBuildingPageTests(Platform platform) : base(platform)
        {
        }

        [OneTimeSetUp]
        public override void OneTimeSetUp()
        {
            base.OneTimeSetUp();
            
            new WelcomePage()
                .TapSkip();
            
            new UserSelectPage()
                .TapStudent();
            
            new AuthorizationPage()
                .TapSkip();

            new MainPage();
        }

        [Test]
        public void Test()
        {
            new MainBuildingPage();
        }
    }
}
