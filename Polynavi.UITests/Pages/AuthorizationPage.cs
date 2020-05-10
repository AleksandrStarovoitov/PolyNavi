using NUnit.Framework;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace Polynavi.UITests.Pages
{
    public class AuthorizationPage : BasePage
    {
        private readonly Query skipAuthText;
        private readonly Query input;
        private readonly Query goButton;
        private readonly Query suggestionsDropDown;

        protected override PlatformQuery Trait => 
            new PlatformQuery() { Android = x => x.Id("textview_auth_skip") };

        public AuthorizationPage()
        {
            if (OnAndroid)
            {
                skipAuthText = x => x.Id("textview_auth_skip");
                input = x => x.Id("autocompletetextview_auth");
                goButton = x => x.Id("button_auth");
                suggestionsDropDown = x => x.Class("DropDownListView");
            }
        }

        public void TapSkip()
        {
            app.Tap(skipAuthText);
        }

        public AuthorizationPage EnterText(string text)
        {
            app.Query(x => x.Id("autocompletetextview_auth").Invoke("setText", text));

            return this;
        }

        public AuthorizationPage EnsureSuggestionsLoaded()
        {
            app.WaitForElement(suggestionsDropDown);

            var result = app.Query(x => x.Id("autocompletetextview_auth").Invoke("getAdapter").Invoke("getCount"));
            var count = (long)result[0];
            
            Assert.That(count, Is.Positive);
            
            return this;
        }
    }
}
