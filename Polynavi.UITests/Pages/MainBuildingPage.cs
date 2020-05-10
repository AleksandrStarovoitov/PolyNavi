namespace Polynavi.UITests.Pages
{
    public class MainBuildingPage : BasePage
    {
        protected override PlatformQuery Trait =>
            new PlatformQuery() { Android = x => x.Id("search_frame_mainbuilding") };
    }
}
