namespace BlImplementation;

using BlApi;



//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Security.Cryptography;
//using System.Text;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public BO.Role Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Name == username);

        if (volunteer == null)
            throw new BO.DalAlreadyExistsException("Invalid username.");

        if (!VerifyPassword(password,volunteer.Password))
            throw new BO.DalAlreadyExistsException("Invalid password.");


        return (BO.Role)Enum.Parse(typeof(BO.Role), volunteer.MyRole);

    }

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive, BO.CallType? callType)
{
    // קבלת כל המתנדבים
    var volunteers = _dal.Volunteer.ReadAll().Select(v => new BO.VolunteerInList
    {
        Id = v.Id,
        Name = v.Name,
        Active = v.Active,
        MyCallType = (BO.CallType)Enum.Parse(typeof(BO.CallType), v.MyCallType)
    });

    // סינון לפי פעילים/לא פעילים
    if (isActive.HasValue)
    {
        volunteers = volunteers.Where(v => v.Active == isActive.Value);
    }

    // מיון לפי קריטריון callType או ת.ז
    if (callType.HasValue)
    {
        volunteers = volunteers.OrderBy(v => v.CallType == callType.Value ? 0 : 1).ThenBy(v => v.Id);
    }
    else
    {
        volunteers = volunteers.OrderBy(v => v.Id);
    }

    return volunteers;
}


    public Volunteer GetVolunteerDetails(int volunteerId)
    {
        var volunteerDo = _dal.Volunteer.GetById(volunteerId);

        var callInProgress = _dal.Call.GetAll()
            .FirstOrDefault(c => c.VolunteerId == volunteerId && c.Status == CallStatus.InProgress);

        return new Volunteer
        {
            Id = volunteerDo.Id,
            Name = volunteerDo.Name,
            Email = volunteerDo.Email,
            CallInProgress = callInProgress != null ? new CallInProgress
            {
                Id = callInProgress.Id,
                Description = callInProgress.Description
            } : null
        };
    }

    public void UpdateVolunteerDetails(int requesterId, Volunteer volunteer)
    {
        var requester = _dal.Volunteer.GetById(requesterId);
        if (requester.Role != "Admin" && requester.Id != volunteer.Id)
            throw new BO.LogicException("Permission denied.");

        ValidateVolunteerDetails(volunteer);

        var existingVolunteer = _dal.Volunteer.GetById(volunteer.Id);
        if (requester.Role != "Admin" && existingVolunteer.Role != volunteer.Role)
            throw new BO.LogicException("Only admin can change roles.");

        if (!string.IsNullOrWhiteSpace(volunteer.Password))
        {
            if (!IsPasswordStrong(volunteer.Password))
                throw new BO.LogicException("Password is not strong enough.");

            existingVolunteer.Password = EncryptPassword(volunteer.Password);
        }

        _dal.Volunteer.Update(new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Email = volunteer.Email,
            Role = volunteer.Role.ToString(),
            IsActive = volunteer.IsActive,
            Password = existingVolunteer.Password
        });
    }

    public void DeleteVolunteer(int volunteerId)
    {
        var volunteer = _dal.Volunteer.GetById(volunteerId);

        var hasActiveCalls = _dal.Call.GetAll()
            .Any(c => c.VolunteerId == volunteerId);

        if (hasActiveCalls)
            throw new BO.LogicException("Cannot delete a volunteer with active or past calls.");

        _dal.Volunteer.Delete(volunteerId);
    }

    public void AddVolunteer(Volunteer volunteer)
    {
        ValidateVolunteerDetails(volunteer);

        var initialPassword = GenerateStrongPassword();
        var encryptedPassword = EncryptPassword(initialPassword);

        // Notify admin about the initial password for the volunteer.
        NotifyAdminInitialPassword(volunteer.Email, initialPassword);

        _dal.Volunteer.Add(new DO.Volunteer
        {
            Id = volunteer.Id,
            Name = volunteer.Name,
            Email = volunteer.Email,
            Role = volunteer.Role.ToString(),
            IsActive = volunteer.IsActive,
            Password = encryptedPassword
        });
    }

    private void ValidateVolunteerDetails(Volunteer volunteer)
    {
        if (string.IsNullOrWhiteSpace(volunteer.Email) || !volunteer.Email.Contains("@"))
            throw new BO.LogicException("Invalid email format.");

        if (volunteer.Id <= 0)
            throw new BO.LogicException("Invalid ID.");

        // Add further validations as required.
    }

    private string EncryptPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword;
    }

    private string GenerateStrongPassword()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
        return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private bool IsPasswordStrong(string password)
    {
        if (password.Length < 8)
            return false;
        if (!password.Any(char.IsUpper))
            return false;
        if (!password.Any(char.IsLower))
            return false;
        if (!password.Any(char.IsDigit))
            return false;
        if (!password.Any(c => "@#$%^&*".Contains(c)))
            return false;

        return true;
    }

    private void NotifyAdminInitialPassword(string email, string initialPassword)
    {
        // Example logic to notify the admin. Replace with actual implementation.
        Console.WriteLine($"Admin notification: Initial password for {email} is {initialPassword}");
    }

    public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive, CallType? callType)
    {
        throw new NotImplementedException();
    }

    Volunteer IVolunteer.GetVolunteerDetails(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteerDetails(int requesterId, Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void AddVolunteer(Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}
