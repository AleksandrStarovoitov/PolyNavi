namespace Polynavi.Common.Settings
{
    public interface IScheduleSettings
    {
        int GroupId { get; set; }

        string GroupNumber { get; set; }

        int TeacherId { get; set; }

        string TeacherName { get; set; }

        bool IsUserTeacher { get; set; }

        bool ContainsIsTeacherKey { get; }
    }
}
