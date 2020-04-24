using Polynavi.Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Polynavi.Common.Services
{
    public interface ISuggestionsService
    {
        Task<Dictionary<string, int>> GetSuggestedGroupsAsync(string groupName);
        Task<Dictionary<string, int>> GetSuggestedTeachersAsync(string teacherName);
    }
}
