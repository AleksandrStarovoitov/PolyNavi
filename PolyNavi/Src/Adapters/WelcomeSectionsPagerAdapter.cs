using Android.Support.V4.App;

namespace PolyNavi
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
            switch (position)
            {
                case 0:
                    Java.Lang.ICharSequence ch1 = new Java.Lang.String("SECTION 1");
                    return ch1;
                case 1:
                    Java.Lang.ICharSequence ch2 = new Java.Lang.String("SECTION 2");
                    return ch2;
                case 2:
                    Java.Lang.ICharSequence ch3 = new Java.Lang.String("SECTION 3");
                    return ch3;
            }
            return null;
        }
    }
}