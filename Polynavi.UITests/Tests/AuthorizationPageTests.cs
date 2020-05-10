using NUnit.Framework;
using Polynavi.UITests.Pages;
using Xamarin.UITest;

namespace Polynavi.UITests.Tests
{
    public class AuthorizationPageTests : BaseTestFixture
    {
        public AuthorizationPageTests(Platform platform) : base(platform)
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
        }

        [Test]
        public void Skips_Auth()
        {
            new AuthorizationPage()
                .TapSkip();

            new MainPage();
        }
    }
}
