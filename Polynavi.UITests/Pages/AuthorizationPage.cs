using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace Polynavi.UITests.Pages
{
    public class AuthorizationPage : BasePage
    {
        private readonly Query skipAuthText;
        private readonly Query input;
        private readonly Query goButton;
        
        protected override PlatformQuery Trait => 
            new PlatformQuery() { Android = x => x.Id("textview_auth_skip") };

        public AuthorizationPage()
        {
            if (OnAndroid)
            {
                skipAuthText = x => x.Id("textview_auth_skip");
                input = x => x.Id("autocompletetextview_auth");
                goButton = x => x.Id("button_auth");
            }
        }

        public void TapSkip()
        {
            app.Tap(skipAuthText);
        }
    }
}
