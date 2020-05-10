using NUnit.Framework;
using Xamarin.UITest;

namespace Polynavi.UITests
{
    public abstract class InstalledAppTestFixture : BaseTestFixture
    {
        public InstalledAppTestFixture(Platform platform) : base(platform)
        {
        }

        [OneTimeSetUp]
        public virtual void OneTimeSetUp()
        {
            AppManager.StartApp();
        }
        
        [SetUp]
        public override void BeforeEachTest()
        {
            AppManager.StartInstalledApp();
        }
    }
}
