using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Widget;
using PolyNaviLib.BL;

namespace PolyNavi.Activities
{
    [Activity(
        Label = "PolyNavi",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public class UserTypeSelectActivity : Activity
    {
        public const string IsTeacherIntentExtraName = "is_teacher";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            MainApp.ChangeLanguage(this);

            SetTheme(Resource.Style.MyAppTheme);
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_user_type_select);

            Setup();
        }

        private void Setup()
        {
            var selectStudentButton = FindViewById<Button>(Resource.Id.button_student_user_type_select);
            selectStudentButton.Click += SelectStudentButton_Click;

            var selectTeacherButton = FindViewById<Button>(Resource.Id.button_teacher_user_type_select);
            selectTeacherButton.Click += SelectTeacherButton_Click;
        }

        private void SelectStudentButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AuthorizationActivity));
            intent.PutExtra(IsTeacherIntentExtraName, false);

            StartActivity(intent);
        }

        private void SelectTeacherButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AuthorizationActivity));
            intent.PutExtra(IsTeacherIntentExtraName, true);

            StartActivity(intent);
        }
    }
}
