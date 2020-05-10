using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace Polynavi.UITests.Pages
{
    public class WelcomePage : BasePage
    {
        private readonly Query nextButton;
        private readonly Query skipButton;
        private readonly Query doneButton;

        protected override PlatformQuery Trait =>
            new PlatformQuery() { Android = x => x.Id("coordinatorlayout_welcome") };

        public WelcomePage()
        {
            if (OnAndroid)
            {
                nextButton = x => x.Id("button_welcome_next");
                skipButton = x => x.Id("button_welcome_skip");
                doneButton = x => x.Id("button_welcome_finish");
            }
        }

        public WelcomePage TapNext()
        {
            app.Tap(nextButton);

            return this;
        }

        public void TapDone()
        {
            app.Tap(doneButton);
        }

        public void TapSkip()
        {
            app.Tap(skipButton);
        }

        public WelcomePage SwipePage()
        {
            app.SwipeRightToLeft(swipeSpeed: 5000);

            return this;
        }
    }
}
