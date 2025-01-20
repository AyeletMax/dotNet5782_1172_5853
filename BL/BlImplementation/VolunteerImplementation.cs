
namespace BlImplementation;
using Helpers;
using BlApi;
using BO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;


internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
          

    public BO.Role Login(string username, string password)
    {
        Volunteer? volunteer = _dal.Volunteer.ReadAll().Select(v => VolunteerManager.MapVolunteer(v)).FirstOrDefault(v => v.Name == username);

        if (volunteer! == null)
            throw new BO.DalAlreadyExistsException("Invalid username.");

        if (!VolunteerManager.VerifyPassword(password, volunteer.Password!))
            throw new BO.DalAlreadyExistsException("Invalid password.");


        return volunteer.MyRole;

    }

   

    private void NotifyAdminInitialPassword(string email, string initialPassword)
    {
        // Example logic to notify the admin. Replace with actual implementation.
        Console.WriteLine($"Admin notification: Initial password for {email} is {initialPassword}");
    }

    public void Create(Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer boStudent)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive, CallType? callType)
    {
        throw new NotImplementedException();
    }

    public Volunteer GetVolunteerDetails(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public void UpdateVolunteerDetails(int requesterId, Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public void AddVolunteer(Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void Create(Volunteer boVolunteer)
    {
        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer boStudent)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
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

    public void DeleteVolunteer(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public Volunteer GetVolunteerDetails(int volunteerId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive, CallType? callType)
    {
        throw new NotImplementedException();
    }

}

