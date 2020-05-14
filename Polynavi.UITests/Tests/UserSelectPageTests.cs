using NUnit.Framework;
using Polynavi.UITests.Pages;
using Xamarin.UITest;

namespace Polynavi.UITests.Tests
{
    public class UserSelectPageTests : BaseTestFixture
    {
        public UserSelectPageTests(Platform platform) : base(platform)
        {
        }

        [SetUp]
        public override void BeforeEachTest()
        {
            base.BeforeEachTest();

            new WelcomePage()
                .TapSkip();
        }

        [Test]
        public void Proceeds_To_Student_Auth()
        {
            new UserSelectPage()
                .TapStudent();

            new AuthorizationPage();
        }
        
        [Test]
        public void Proceeds_To_Teacher_Auth()
        {
            new UserSelectPage()
                .TapTeacher();

            new AuthorizationPage();
        }
    }
}
