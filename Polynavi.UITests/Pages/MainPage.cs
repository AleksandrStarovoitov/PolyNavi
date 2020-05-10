namespace Polynavi.UITests.Pages
{
    public class MainPage : BasePage
    {
        protected override PlatformQuery Trait => 
            new PlatformQuery() { Android = x => x.Id("drawerlayout_main") };
    }
}
