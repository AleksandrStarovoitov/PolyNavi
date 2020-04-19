using AndroidX.Fragment.App;
using Java.Lang;
using PolyNavi.Fragments;

namespace PolyNavi.Adapters
{
    public class WelcomeSectionsPagerAdapter : FragmentPagerAdapter
    {
        public override int Count => 3;

        public WelcomeSectionsPagerAdapter(FragmentManager fm) : base(fm, BehaviorResumeOnlyCurrentFragment)
        {
        }

        public override Fragment GetItem(int position)
        {
            return WelcomePlaceholderFragment.NewInstance(position + 1);
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return position switch
            {
                0 => new String("SECTION 1"),
                1 => new String("SECTION 2"),
                2 => new String("SECTION 3"),
                _ => null
            };
        }
    }
}
