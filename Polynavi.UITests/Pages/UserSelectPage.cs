using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace Polynavi.UITests.Pages
{
    public class UserSelectPage : BasePage
    {
        private readonly Query studentButton;
        private readonly Query teacherButton;

        protected override PlatformQuery Trait =>
            new PlatformQuery() { Android = x => x.Id("button_student_user_type_select") };

        public UserSelectPage()
        {
            if (OnAndroid)
            {
                studentButton = x => x.Id("button_student_user_type_select");
                teacherButton = x => x.Id("button_teacher_user_type_select");
            }
        }

        public void TapStudent()
        {
            app.Tap(studentButton);
        }
        
        public void TapTeacher()
        {
            app.Tap(teacherButton);
        }

    }
}
