namespace BlApi;
public interface IVolunteer
{
    BO.Role Login(string name, string password);
    IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortField? sortBy = null);
    void Create(BO.Volunteer boVolunteer);
    BO.Volunteer? Read(int id);
    public IEnumerable<BO.CallInList> GetCallList(
     BO.CallType? filterField = null,
     object? filterValue = null,
     BO.Status? sortField = null
 );

    void Update(BO.Volunteer boStudent);
    void Delete(int id);
    
    //bool VerifyPassword(string enteredPassword, string storedPassword);
    //IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.CallType? callType);
    BO.Volunteer GetVolunteerDetails(int volunteerId);
    void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteer);
    void DeleteVolunteer(int volunteerId);
    void AddVolunteer(BO.Volunteer volunteer);
   
}





