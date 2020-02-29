using AndroidX.Fragment.App;
using PolyNavi.Fragments;

namespace PolyNavi.Adapters
{
    public class WelcomeSectionsPagerAdapter : FragmentPagerAdapter
    {
        public override int Count => 3;

        public WelcomeSectionsPagerAdapter(FragmentManager fm) : base(fm)
        {

        }

        public override Fragment GetItem(int position)
        {
            return WelcomePlaceholderFragment.NewInstance(position + 1);
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            return position switch
            {
                0 => new Java.Lang.String("SECTION 1"),
                1 => new Java.Lang.String("SECTION 2"),
                2 => new Java.Lang.String("SECTION 3"),
                _ => null
            };
        }
    }
}