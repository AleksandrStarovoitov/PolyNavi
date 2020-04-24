using Polynavi.Common.Models;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface ISuggestionsService
    {
        Task<GroupRoot> GetSuggestedGroupsAsync(string groupName);
        Task<TeachersRoot> GetSuggestedTeachersAsync(string teacherName);
    }
}
