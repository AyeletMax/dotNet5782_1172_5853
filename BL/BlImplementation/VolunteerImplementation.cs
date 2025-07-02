namespace BlImplementation;
using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;

internal class VolunteerImplementation : BlApi.IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    // Login function to authenticate a volunteer based on username and password.
    // If the username exists and the password matches, return the volunteer's role
    public BO.Role Login(int id, string password)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v => v.Id == id);
                DO.Volunteer? matchingVolunteer = volunteers.FirstOrDefault(v => VolunteerManager.VerifyPassword(password, v.Password!)) ??
                throw new BO.BlDoesNotExistException("Incorrect username or password.");
                return (BO.Role)matchingVolunteer.MyRole;
            }
        }
        catch (BO.BlDoesNotExistException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }
    // Retrieve a list of volunteers, with optional filters for activity status and sorting criteria.
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortField? sortBy = null, BO.CallType? filterField = null)
    {

        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
                !isActive.HasValue || v.Active == isActive.Value);


                var volunteerList = VolunteerManager.GetVolunteerList(volunteers)
                .Where(vol => !filterField.HasValue || vol.MyCallType == filterField.Value);

                volunteerList = sortBy.HasValue ? sortBy.Value switch
                {
                    BO.VolunteerSortField.Name => volunteerList.OrderBy(v => v.Name),
                    BO.VolunteerSortField.TotalResponsesHandled => volunteerList.OrderByDescending(v => v.TotalResponsesHandled),
                    BO.VolunteerSortField.TotalResponsesCancelled => volunteerList.OrderByDescending(v => v.TotalResponsesCancelled),
                    BO.VolunteerSortField.TotalExpiredResponses => volunteerList.OrderByDescending(v => v.TotalExpiredResponses),
                    _ => volunteerList.OrderBy(v => v.Id)
                } : volunteerList.OrderBy(v => v.Id);

                return volunteerList;
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }
    // Retrieve detailed information for a specific volunteer by ID.
    // Includes current assignments and calls in progress if any.
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                var doVolunteer = _dal.Volunteer.Read(volunteerId) ??
               throw new DO.DalDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
                var assigments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
                var currentAssignment = assigments.FirstOrDefault(a => a.ExitTime == null);
                BO.CallInProgress? callInProgress = null;
                if (currentAssignment != null)
                {
                    var callDetails = _dal.Call.Read(currentAssignment.CallId);
                    if (callDetails != null)
                    {
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
                            VolunteerResponseDistance = Tools.CalculateDistance((BO.DistanceType)doVolunteer.MyDistanceType, doVolunteer.Latitude ?? double.MaxValue, doVolunteer.Longitude ?? double.MaxValue, callDetails.Latitude, callDetails.Longitude),
                            MyStatus = Tools.CalculateStatus(callDetails)
                        };
                    }
                }
                return new BO.Volunteer
                {
                    Id = volunteerId,
                    Name = doVolunteer.Name,
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
            AdminManager.ThrowOnSimulatorIsRunning();
            DO.Volunteer doVolunteer;
            lock (AdminManager.BlMutex)
            {
                var existingVolunteer = _dal.Volunteer.Read(boVolunteer.Id) ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist");

                VolunteerManager.ValidateInputFormat(boVolunteer);

                if (boVolunteer.Password == "")
                {
                    boVolunteer.Password = existingVolunteer.Password;
                }
                else
                {
                    VolunteerManager.CheckPassword(boVolunteer.Password);
                    boVolunteer.Password = VolunteerManager.EncryptPassword(boVolunteer.Password);
                }

                if (boVolunteer.Address != null)
                {
                    boVolunteer.Latitude = null;
                    boVolunteer.Longitude = null;
                }
                else
                {
                    boVolunteer.Address = existingVolunteer.Address;
                    boVolunteer.Latitude = existingVolunteer.Latitude;
                    boVolunteer.Longitude = existingVolunteer.Longitude;
                }

                VolunteerManager.ValidatePermissions(requesterId, boVolunteer);

                var originalVolunteer = _dal.Volunteer.Read(boVolunteer.Id)!;
                var changedFields = VolunteerManager.GetChangedFields(originalVolunteer, boVolunteer);
                if (!VolunteerManager.CanUpdateFields(requesterId, changedFields, boVolunteer))
                    throw new BO.BlUnauthorizedAccessException("You do not have permission to update the Role field.");

                doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);
                _dal.Volunteer.Update(doVolunteer);
            }

            VolunteerManager.Observers.NotifyItemUpdated(boVolunteer.Id);
            VolunteerManager.Observers.NotifyListUpdated();

            if (boVolunteer.Address != null)
                _ = VolunteerManager.UpdateVolunteerCoordinatesAsync(doVolunteer);
        }
        catch (BO.BlInvalidFormatException) { throw; }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.Id} does not exist.", ex);
        }
        catch (BLTemporaryNotAvailableException ex)
        {
            throw new BO.BLTemporaryNotAvailableException(ex.Message);
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while updating the volunteer.", ex);
        }
    }

    // Delete a volunteer from the system.
    // Ensures the volunteer is not currently assigned to any tasks before deletion.
    public void DeleteVolunteer(int volunteerId)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID {volunteerId} does not exist.");
                IEnumerable<DO.Assignment> assignmentsWithVolunteer = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId);
                if (assignmentsWithVolunteer.Any())
                {
                    throw new BO.BlDeletionException($"Volunteer with ID {volunteerId} cannot be deleted because they are or have been assigned to tasks.");
                }
                _dal.Volunteer.Delete(volunteerId);//stage 4
            }
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5


        }
        catch (BLTemporaryNotAvailableException ex)
        {
            throw new BO.BLTemporaryNotAvailableException(ex.Message);
        }
        catch (BO.BlDeletionException)
        {
            throw;
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
            AdminManager.ThrowOnSimulatorIsRunning();//stage 7
            DO.Volunteer doVolunteer;
            lock (AdminManager.BlMutex) //stage 7
            {
                var existingVolunteer = _dal.Volunteer.Read(v => v.Id == boVolunteer.Id);
                if (existingVolunteer != null)
                {
                    throw new DO.DalAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists.");
                }
                VolunteerManager.ValidateInputFormat(boVolunteer);
                VolunteerManager.CheckPassword(boVolunteer.Password!);

                boVolunteer.Latitude = null;
                boVolunteer.Longitude = null;
                boVolunteer.Password = VolunteerManager.EncryptPassword(boVolunteer.Password!);

                doVolunteer = VolunteerManager.CreateDoVolunteer(boVolunteer);
                _dal.Volunteer.Create(doVolunteer);
                VolunteerManager.Observers.NotifyListUpdated();


            }
            if (boVolunteer.Address != "No Address")
                _ = VolunteerManager.UpdateVolunteerCoordinatesAsync(doVolunteer);
        }
        catch (BLTemporaryNotAvailableException ex)
        {
            throw new BO.BLTemporaryNotAvailableException(ex.Message);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.Id} already exists", ex);
        }
        catch (BO.BlInvalidFormatException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new BO.BlGeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }

    #region Stage 5
    public void AddObserver(Action listObserver) =>
            VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
            VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
            VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
            VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5
}