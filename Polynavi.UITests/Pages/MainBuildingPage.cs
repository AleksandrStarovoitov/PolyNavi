using NUnit.Framework;
using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace Polynavi.UITests.Pages
{
    public class MainBuildingPage : BasePage
    {
        private readonly Query routeButton;
        
        protected override PlatformQuery Trait =>
            new PlatformQuery() { Android = x => x.Id("search_frame_mainbuilding") };

        public MainBuildingPage()
        {
            if (OnAndroid)
            {
                routeButton = x => x.Id("fab_mainbuilding");
            }
        }

        public MainBuildingPage EnterFromRoom(string text)
        {
            app.Query(x => x.Id("autoCompleteTextView_from").Invoke("setText", text));

            return this;
        }

        public MainBuildingPage EnterToRoom(string text)
        {
            app.Query(x => x.Id("autoCompleteTextView_to").Invoke("setText", text));

            return this;
        }

        public MainBuildingPage TapRouteButton()
        {
            app.Tap(routeButton);

            return this;
        }

        public MainBuildingPage EnsureInputLayoutIsHidden()
        {
            var yCoordinate = (double)app.Query(x => x.Id("appbar_mainbuilding").Invoke("getY"))[0];
            
            Assert.That(yCoordinate, Is.Negative);

            return this;
        }
    }
}
