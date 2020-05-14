using NUnit.Framework;
using Xamarin.UITest;

namespace Polynavi.UITests
{
    [TestFixture(Platform.Android)]
    public abstract class BaseTestFixture
    {
        protected IApp app => AppManager.App;
        protected bool OnAndroid => AppManager.Platform == Platform.Android;

        protected BaseTestFixture(Platform platform)
        {
            AppManager.Platform = platform;
        }

        [SetUp]
        public virtual void BeforeEachTest()
        {
            AppManager.StartApp();
        }
    }
}
