namespace BlApi;
using BO;
public interface IVolunteer
{
    public Role Login(string name, string password)
    {
        Volunteer volunteer = volunteers.FirstOrDefault(v => v.name == name);

        if (volunteer == null)
        {
            throw new InvalidOperationException("משתמש לא קיים");
        }

        if (volunteer.Password != password)
        {
            throw new InvalidOperationException("סיסמה לא נכונה");
        }

        return volunteer.MyRole; 
    }
}

}
