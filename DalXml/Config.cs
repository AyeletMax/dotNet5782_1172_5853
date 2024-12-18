namespace Dal;
/// <summary>
/// Static class that manages system settings and global data.
/// Responsible for reading and updating values from various XML files (system settings, volunteers, assignments, calls).
/// </summary>
internal static class Config
{

    // File Names
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_volunteers_xml = "volunteers.xml";
    internal const string s_assignments_xml = "assignments.xml";
    internal const string s_calls_xml = "calls.xml";

    /// Gets the next available Assignment/Call ID from the 'data-config.xml' file and updates it.
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }
    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    /// Gets the current date and time from the 'data-config.xml' file and updates it.
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    /// Gets the current Risk Range from the 'data-config.xml' file and updates it.
    internal static TimeSpan RiskRange
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskRange");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskRange", value);
    }

    // Reset Method
    /// Resets all configuration values to their default values:
    internal static void Reset()
    {
        NextAssignmentId = 1;
        NextCallId = 1;      
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromHours(1);
    }

}