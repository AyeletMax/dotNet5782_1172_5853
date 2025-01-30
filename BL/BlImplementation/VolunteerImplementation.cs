namespace BlImplementation;
using BlApi;
using BO;
using DO;
using Helpers;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;



    public BO.Role Login(string username, string password)
    {
        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v => v.Name == username);

            DO.Volunteer? matchingVolunteer = volunteers.FirstOrDefault(v => VolunteerManager.VerifyPassword(password, v.Password!)) ??
                throw new BO.BlDoesNotExistException("Incorrect username or password.");
            return (BO.Role)matchingVolunteer.MyRole;
        }

        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
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

            return volunteerList.ToList();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlGeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {

            var doVolunteer = _dal.Volunteer.Read(volunteerId)??
               throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
            var assigments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            var currentAssignment = assigments.FirstOrDefault(a => a.ExitTime == null);
            BO.CallInProgress? callInProgress = null;
            if (currentAssignment != null)
            {
                var callDetails = _dal.Call.Read(currentAssignment.CallId);
                if (callDetails != null) {
                    callInProgress = new BO.CallInProgress
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
                Phone = doVolunteer.Phone,
                Email = doVolunteer.Email,
                Active = doVolunteer.Active,
                MyRole = (BO.Role)doVolunteer.MyRole,
                Password = doVolunteer.Password,
                Address = doVolunteer.Address,
                Longitude = doVolunteer.Longitude,
                Latitude = doVolunteer.Latitude,
                LargestDistance = doVolunteer.LargestDistance,
                MyDistanceType = (BO.DistanceType)doVolunteer.MyDistanceType,
                TotalCallsHandled = assigments.Count(a => a.FinishCallType == DO.FinishCallType.TakenCareOf),
                TotalCallsCancelled = assigments.Count(a => a.FinishCallType == DO.FinishCallType.CanceledByVolunteer),
                TotalExpiredCallsChosen = assigments.Count(a => a.FinishCallType == DO.FinishCallType.Expired),
                CurrentCallInProgress = callInProgress

            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
        }
    }
    public void UpdateVolunteer(int requesterId, BO.Volunteer boVolunteer)
    {
        try
        {
           VolunteerManager.ValidateInputFormat(boVolunteer);
            (double? Latitude, double? Longitude) coordinates = VolunteerManager.LogicalChecking(boVolunteer);
            //if (coordinates == null)
            //    throw new BO.GeolocationNotFoundException($"Invalid address: {boVolunteer.Address}");
            boVolunteer.Latitude = coordinates.Latitude;
            boVolunteer.Longitude = coordinates.Longitude;
            VolunteerManager.ValidatePermissions(requesterId, boVolunteer);

            var originalVolunteer = _dal.Volunteer.Read(boVolunteer.Id)!;
            var changedFields =VolunteerManager.GetChangedFields(originalVolunteer, boVolunteer);
            //צריך לבדוק איזה שדות עוד א"א לעדכן ולשנות בפונ ע"פ requesterId
            if (!VolunteerManager.CanUpdateFields(requesterId, changedFields, boVolunteer))
                throw new BO.BlUnauthorizedAccessException("You do not have permission to update the Role field.");

            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);

            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while updating the volunteer.", ex);
        }
    }
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID {volunteerId} does not exist.");
            IEnumerable<DO.Assignment> assignmentsWithVolunteer = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
            if (assignmentsWithVolunteer.Any())
            {
                throw new BO.BlDeletionException($"Volunteer with ID {volunteerId} cannot be deleted because they are or have been assigned to tasks.");
            }
            _dal.Volunteer.Delete(volunteerId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Error: Volunteer with ID {volunteerId} does not exist in the database.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while updating the volunteer.", ex);
        }

    }
    public void AddVolunteer(BO.Volunteer boVolunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(v => v.Id == boVolunteer.Id);
            if (existingVolunteer != null)
            {
                throw new DO.DalAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists.");
            }
            VolunteerManager.ValidateInputFormat(boVolunteer);
            var (latitude, longitude) = VolunteerManager.LogicalChecking(boVolunteer);
            if (latitude != null && longitude != null)
            {
                boVolunteer.Latitude = latitude;
                boVolunteer.Longitude = longitude;
            }
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }

    }



}




  




