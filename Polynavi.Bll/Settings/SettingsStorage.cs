using Polynavi.Common.Services;
using Polynavi.Common.Settings;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Polynavi.Bll.Settings
{
    [ExcludeFromCodeCoverage]
    public sealed class SettingsStorage : IScheduleSettings, ILoginStateSettings, IAppInfoSettings
    {
        private const string GroupIdKey = "groupid";
        private const string TeacherIdKey = "teacherid";
        public const string GroupNumberKey = "groupnumber"; //TODO Private
        public const string TeacherNameKey = "teachername";  //TODO Private
        public const string IsUserTeacherKey = "isteacher"; //TODO Private

        private const string IsAuthCompletedKey = "auth";
        private const string IsWelcomeCompletedKey = "welcome";

        private const string StartScreenKey = "startactivity";
        private const string VersionKey = "version";
        private const string AppLanguageKey = "language";

        private readonly IKeyValueStorage keyValueStorage;

        public SettingsStorage(IKeyValueStorage keyValueStorage)
        {
            this.keyValueStorage = keyValueStorage;
        }

        public int GroupId
        {
            get => keyValueStorage.GetInt(GroupIdKey, 0);
            set => keyValueStorage.PutInt(GroupIdKey, value);
        }

        public string GroupNumber
        {
            get => keyValueStorage.GetString(GroupNumberKey, String.Empty);
            set => keyValueStorage.PutString(GroupNumberKey, value);
        }

        public int TeacherId
        {
            get => keyValueStorage.GetInt(TeacherIdKey, 0);
            set => keyValueStorage.PutInt(TeacherIdKey, value);
        }

        public string TeacherName
        {
            get => keyValueStorage.GetString(TeacherNameKey, String.Empty);
            set => keyValueStorage.GetString(TeacherNameKey, value);
        }

        public bool IsWelcomeCompleted 
        {
            get => keyValueStorage.GetBoolean(IsWelcomeCompletedKey, false);
            set => keyValueStorage.PutBoolean(IsWelcomeCompletedKey, value);
        }

        public bool IsAuthCompleted 
        {
            get => keyValueStorage.GetBoolean(IsAuthCompletedKey, false);
            set => keyValueStorage.PutBoolean(IsAuthCompletedKey, value);
        }

        public bool IsUserTeacher 
        {
            get => keyValueStorage.GetBoolean(IsUserTeacherKey, false);
            set => keyValueStorage.PutBoolean(IsUserTeacherKey, value);
        }

        public string StartScreen
        {
            get => keyValueStorage.GetString(StartScreenKey, String.Empty);
            set => keyValueStorage.PutString(StartScreenKey, value);
        }

        public int AppVersionCode
        {
            get => keyValueStorage.GetInt(VersionKey, 0);
            set => keyValueStorage.GetInt(VersionKey, value); 
        }

        public string AppLanguage 
        {
            get => keyValueStorage.GetString(AppLanguageKey, String.Empty);
            set => keyValueStorage.PutString(AppLanguageKey, value);
        }

        public bool ContainsIsTeacherKey => keyValueStorage.Contains(IsUserTeacherKey);
    }
}
