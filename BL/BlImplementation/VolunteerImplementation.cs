
namespace BlImplementation;
using Helpers;
using BlApi;
using BO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using DO;
using System.Net;
using DalApi;


internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    public BO.Role Login(string username, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll().Select(v => VolunteerManager.MapVolunteer(v)).FirstOrDefault(v => v.Name == username);

        if (volunteer! == null)
            throw new BO.DalAlreadyExistsException("Invalid username.");

        if (!VolunteerManager.VerifyPassword(password, volunteer.Password!))
            throw new BO.DalAlreadyExistsException("Invalid password.");

        return volunteer.MyRole;

    }
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortField? sortBy = null)
    {
       
        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
                !isActive.HasValue || v.Active == isActive.Value);

            var volunteerList = VolunteerManager.GetVolunteerList(volunteers);

            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
                BO.VolunteerSortField.Name => volunteerList.OrderBy(v => v.Name).ToList(),
                BO.VolunteerSortField.TotalResponsesHandled => volunteerList.OrderByDescending(v => v.TotalResponsesHandled).ToList(),
                BO.VolunteerSortField.TotalResponsesCancelled => volunteerList.OrderByDescending(v => v.TotalResponsesCancelled).ToList(),
                BO.VolunteerSortField.TotalExpiredResponses => volunteerList.OrderByDescending(v => v.TotalExpiredResponses).ToList(),
                BO.VolunteerSortField.SumOfCalls => volunteerList.OrderBy(v => v.TotalResponsesHandled).ToList(),
                BO.VolunteerSortField.SumOfCancellation => volunteerList.OrderBy(v => v.TotalResponsesCancelled).ToList(),
                BO.VolunteerSortField.SumOfExpiredCalls => volunteerList.OrderBy(v => v.TotalExpiredResponses).ToList(),
                _ => volunteerList.OrderBy(v => v.Id).ToList()
            } : volunteerList.OrderBy(v => v.Id).ToList();

            return volunteerList;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.GeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {

            var doVolunteer = _dal.Volunteer.ReadAll(v => v.Id == volunteerId).FirstOrDefault() ??
               throw new BO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");

            var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.ExitTime == null).FirstOrDefault();
            BO.CallInProgress? callInProgress = null;
            if (currentAssignment != null)
            {
                var callDetails = _dal.Call.Read(currentAssignment.CallId);
                if (callDetails != null) {
                    callInProgress = new CallInProgress
                    {
                        Id = currentAssignment.Id,
                        CallId = currentAssignment.CallId,
                        MyCallType = (BO.CallType)callDetails.MyCallType,
                        VerbalDescription = callDetails.VerbalDescription,
                        Address = callDetails.Address,
                        OpenTime = callDetails.OpenTime,
                        MaxFinishTime = callDetails.MaxFinishTime,
                        EntranceTime = currentAssignment.EntranceTime,
                        VolunteerResponseDistance = Tools.CalculateDistance(doVolunteer.Latitude, doVolunteer.Longitude, callDetails.Latitude, callDetails.Longitude),
                        MyStatus = Tools.CalculateStatus(currentAssignment, callDetails, 30)
                    };
                }
            }

            return new BO.Volunteer
            {
                Id = volunteerId,
                Name= doVolunteer.Name,
                Email = doVolunteer.Email,
                Phone = doVolunteer.Phone,
                MyRole = (BO.Role)doVolunteer.MyRole,
                Active = doVolunteer.Active,
                LargestDistance = doVolunteer.LargestDistance,
                Password = doVolunteer.Password,
                Address = doVolunteer.Address,
                Longitude = doVolunteer.Longitude,
                Latitude = doVolunteer.Latitude,
                MyDistanceType = (BO.DistanceType)doVolunteer.MyDistanceType,
                CurrentCallInProgress = callInProgress
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.DalDoesNotExistException("Volunteer not found in data layer.");
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
        }
    }

}





