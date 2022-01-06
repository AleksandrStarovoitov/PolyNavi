using NUnit.Framework;
using Polynavi.UITests.Pages;
using Xamarin.UITest;

namespace Polynavi.UITests.Tests
{
    public class MainBuildingPageTests : BaseTestFixture
    {
        public MainBuildingPageTests(Platform platform) : base(platform)
        {
        }

        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();
            
            new WelcomePage()
                .TapSkip();
            
            new UserSelectPage()
                .TapStudent();
            
            new AuthorizationPage()
                .TapSkip();

            new MainPage();
        }

        [Test]
        public void Enters_Rooms_And_Hides_Layout()
        {
            new MainBuildingPage()
                .EnterFromRoom("130")
                .EnterToRoom("122")
                .TapRouteButton()
                .EnsureInputLayoutIsHidden();
        }
    }
}
