namespace BlApi;
public interface IVolunteer
{

    void Create(BO.Volunteer boVolunteer);
    BO.Volunteer? Read(int id);

    //IEnumerable<BO.VolunteerInList> ReadAll(BO.VolunteerFieldSort? sort = null, BO.StudentFieldFilter? filter = null, object? value = null);
    void Update(BO.Volunteer boStudent);
    void Delete(int id);
    BO.Role Login(string name, string password);
    //bool VerifyPassword(string enteredPassword, string storedPassword);
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.CallType? callType);
    BO.Volunteer GetVolunteerDetails(int volunteerId);
    void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer);
    void DeleteVolunteer(int volunteerId);
    void AddVolunteer(BO.Volunteer volunteer);
   
}





