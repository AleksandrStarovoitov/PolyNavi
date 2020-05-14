using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace Polynavi.UITests
{
    public class PlatformQuery
    {
        private Func<AppQuery, AppQuery> current;
        
        public Func<AppQuery, AppQuery> Current =>
            current ?? throw new NullReferenceException("Trait not set for current platform");

        public Func<AppQuery, AppQuery> Android
        {
            set
            {
                if (AppManager.Platform == Platform.Android)
                {
                    current = value;
                }
            }
        }
    }
}
