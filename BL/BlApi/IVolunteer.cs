namespace BlApi;
using BO;
public interface IVolunteer
{
    Role Login(string name, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive,  CallType? callType);
    BO.Volunteer GetVolunteerDetails(int volunteerId);
    void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer);
    void DeleteVolunteer(int volunteerId);
    void AddVolunteer(BO.Volunteer volunteer);
}


