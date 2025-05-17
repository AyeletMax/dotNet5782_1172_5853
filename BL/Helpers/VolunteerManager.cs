using BlApi;
using BO;
using DalApi;
using DO;
using System.Security.Cryptography;
using System.Text;

namespace Helpers;

// <summary>
/// A static class responsible for managing volunteer-related operations.
/// </summary>
internal static class VolunteerManager
{
    // <summary>
    /// An instance of the data access layer (DAL).
    /// </summary>
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    internal static ObserverManager Observers = new(); //stage 5 
    /// <summary>
    /// Verifies whether the entered password matches the stored encrypted password.
    /// </summary>
    /// <param name="enteredPassword">The entered password.</param>
    /// <param name="storedPassword">The stored encrypted password.</param>
    /// <returns>True if the passwords match, otherwise false.</returns>
    internal static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword;
    }

    /// <summary>
    /// Encrypts a password using SHA-256 hashing.
    /// </summary>
    /// <param name="password">The plain text password.</param>
    /// <returns>The encrypted password as a Base64 string.</returns>
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));;
        return Convert.ToBase64String(hashedBytes!);
    }

    /// <summary>
    /// Generates a strong random password.
    /// </summary>
    /// <returns>A randomly generated strong password.</returns>

    internal static string GenerateStrongPassword()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
        return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    /// <summary>
    /// Checks if a given password meets security requirements.
    /// </summary>
    /// <param name="password">The password to check.</param>
    /// <returns>True if the password is strong, otherwise false.</returns>
    internal static bool IsPasswordStrong(string password)
    {
        if (password.Length < 8)
            return false;
        if (!password.Any(char.IsUpper))
            return false;
        if (!password.Any(char.IsLower))
            return false;
        if (!password.Any(char.IsDigit))
            return false;
        if (!password.Any(c => "!@#$%^&*".Contains(c)))
            return false;
        return true;
    }

    /// <summary>
    /// Retrieves a list of volunteers with their activity statistics.
    /// </summary>
    /// <param name="volunteers">A collection of volunteer data objects.</param>
    /// <returns>A collection of business objects representing volunteers.</returns>
    internal static IEnumerable<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        return volunteers.Select(v =>
        {
            var volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);

            var totalHandled = volunteerAssignments.Count(a => a.FinishCallType == DO.FinishCallType.TakenCareOf);
            var totalCanceled = volunteerAssignments.Count(a => a.FinishCallType == DO.FinishCallType.CanceledByManager);
            var totalExpired = volunteerAssignments.Count(a => a.FinishCallType == DO.FinishCallType.Expired);

            var currentAssignment = volunteerAssignments.FirstOrDefault(a => a.ExitTime == null);
            var assignedResponseId = currentAssignment?.CallId;

            return new BO.VolunteerInList
            {
                Id = v.Id,
                Name = v.Name,
                Active = v.Active,
                TotalResponsesHandled = totalHandled,
                TotalResponsesCancelled = totalCanceled,
                TotalExpiredResponses = totalExpired,
                AssignedResponseId = assignedResponseId,
                MyCallType = assignedResponseId.HasValue
                    ? (BO.CallType)(s_dal.Call.Read(assignedResponseId.Value)?.MyCallType ?? DO.CallType.None)
                    : BO.CallType.None
            };
        });
    }


    /// <summary>
    /// Validates the input format of a volunteer object.
    /// </summary>
    /// <param name="boVolunteer">The volunteer business object.</param>
    /// <exception cref="BO.BlInvalidFormatException">Thrown when input validation fails.</exception>
    internal static void ValidateInputFormat(BO.Volunteer boVolunteer)
    {
        if (boVolunteer == null)
            throw new BO.BlDoesNotExistException("Volunteer object cannot be null.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlInvalidFormatException("Invalid email format.");
        if (boVolunteer.Id < 0 || !IsValidId(boVolunteer.Id))
            throw new BO.BlInvalidFormatException("Invalid ID format. ID must be a valid number with a correct checksum.");
        if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Phone, @"^\d{10}$"))
            throw new BO.BlInvalidFormatException("Invalid phone number format. Phone number must have 10 digits.");
        if (boVolunteer.Name.Length < 2)
            throw new BO.BlInvalidFormatException("Volunteer name is too short. Name must have at least 2 characters.");
        if (boVolunteer.Password.Length < 6 || !Helpers.VolunteerManager.IsPasswordStrong(boVolunteer.Password))
            throw new BO.BlInvalidFormatException("Password is too weak. It must have at least 6 characters, including uppercase, lowercase, and numbers.");
    }


    /// <summary>
    /// Validates an ID number using a checksum algorithm.
    /// </summary>
    /// <param name="id">The ID number to validate.</param>
    /// <returns>True if the ID is valid, otherwise false.</returns>
    internal static bool IsValidId(int id)
    {
        string idStr = id.ToString().PadLeft(9, '0');
        if (idStr.Length != 9 || !int.TryParse(idStr, out _))
            return false;
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = idStr[i] - '0'; 
            digit *= (i % 2 == 0) ? 1 : 2; 

            if (digit > 9)
                digit -= 9;

            sum += digit;
        }
        return sum % 10 == 0;
    }

    /// <summary>
    /// Converts a business object (BO) volunteer to a data object (DO) volunteer.
    /// </summary>
    /// <param name="boVolunteer">The business object volunteer.</param>
    /// <returns>A data object volunteer.</returns>
    internal static DO.Volunteer CreateDoVolunteer(BO.Volunteer boVolunteer)
    {
        return new DO.Volunteer(
            boVolunteer.Id,
            boVolunteer.Name,
            boVolunteer.Phone,
            boVolunteer.Email,
            boVolunteer.Active,
            (DO.Role)boVolunteer.MyRole,
            EncryptPassword(boVolunteer.Password),
            boVolunteer.Address,
            boVolunteer.Latitude,
            boVolunteer.Longitude,
            boVolunteer.LargestDistance,
            (DO.DistanceType)boVolunteer.MyDistanceType
        );
    }

    /// <summary>
    /// Validates if a user has permission to update a volunteer's details.
    /// </summary>
    /// <param name="requesterId">The ID of the user requesting the update.</param>
    /// <param name="boVolunteer">The volunteer object being updated.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown when permission is denied.</exception>
    internal static void ValidatePermissions(int requesterId, BO.Volunteer boVolunteer)
    {
        bool isSelf = (requesterId == boVolunteer.Id);
        bool isAdmin = (boVolunteer.MyRole == BO.Role.Manager);

        if (!isAdmin && !isSelf)
            throw new BO.BlUnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");

        if (!isAdmin && boVolunteer.MyRole != BO.Role.Volunteer)
            throw new BO.BlUnauthorizedAccessException("Only an admin can update the volunteer's role.");
    }

    /// <summary>
    /// Determines which fields have changed between the original and updated volunteer objects.
    /// </summary>
    internal static List<string> GetChangedFields(DO.Volunteer original, BO.Volunteer updated)
    {
        var changedFields = new List<string>();

        if (original.Name != updated.Name) changedFields.Add("Name");
        if (original.Email != updated.Email) changedFields.Add("Email");
        if (original.Phone != updated.Phone) changedFields.Add("Phone");
        if (original.MyRole != (DO.Role)updated.MyRole) changedFields.Add("Role");
        if (original.Address != updated.Address) changedFields.Add("Address");

        return changedFields;
    }

    /// <summary>
    /// Determines whether a user has permission to update specific fields in a volunteer's record.
    /// </summary>
    /// <param name="requesterId">The ID of the user making the update request.</param>
    /// <param name="changedFields">A list of fields that are being updated.</param>
    /// <param name="boVolunteer">The volunteer object being updated.</param>
    /// <returns>True if the user is allowed to update the specified fields, otherwise false.</returns>
    internal static bool CanUpdateFields(int requesterId, List<string> changedFields, BO.Volunteer boVolunteer)
    {
        var restrictedFields = new List<string> { "Role" };
        if (changedFields.Contains("Role"))
        {
            bool isAdmin = boVolunteer.MyRole == BO.Role.Manager;

            if (!isAdmin)
                return false; 
        }

        return true; 
    }
}

