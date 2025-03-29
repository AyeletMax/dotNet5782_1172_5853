

using DalApi;

namespace Helpers;
/// <summary>
/// A static helper class for managing assignments.
/// </summary>
internal static class AssignmentManager
{
    /// An instance of the data access layer (DAL).
    private static IDal s_dal = Factory.Get; //stage 4
}
