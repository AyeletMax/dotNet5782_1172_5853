using BlApi;
using BO;
using DalApi;
using DO;
using System.Security.Cryptography;
using System.Text;

namespace Helpers
{
    internal class VolunteerManager
    {
        private static IDal s_dal = Factory.Get; //stage 4
        internal static BO.Volunteer MapVolunteer(DO.Volunteer volunteer)
        {
            return new BO.Volunteer
            {
                Id = volunteer.Id,
                Name = volunteer.Name,
                Phone = volunteer.Phone,
                Email = volunteer.Email,
                Active = volunteer.Active,
                MyRole = (BO.Role)volunteer.MyRole,

            };
        }
        internal static bool VerifyPassword(string enteredPassword, string storedPassword)
        {
            var encryptedPassword = EncryptPassword(enteredPassword);
            return encryptedPassword == storedPassword;
        }
        internal static string EncryptPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes!);
        }

        internal static string GenerateStrongPassword()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789@#$%^&*";
            return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }

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
            if (!password.Any(c => "@#$%^&*".Contains(c)))
                return false;

            return true;
        }
        internal static List<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
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
                        ? (BO.CallType)(s_dal.Call.Read(assignedResponseId.Value)?.MyCallType ?? DO.CallType.MusicPerformance)
                        : BO.CallType.MusicPerformance
                };
            }).ToList();
        }
       internal static void ValidateInputFormat(BO.Volunteer boVolunteer)
        {
            if (boVolunteer == null)
                throw new BO.BlDoesNotExistn("Volunteer object cannot be null.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new BO.InvalidFormatException("Invalid email format.");

            if (boVolunteer.Id < 0)
                throw new BO.InvalidFormatException("Invalid ID format. ID must be a non-negative number.");

            if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Phone, @"^\d{10}$"))
                throw new BO.InvalidFormatException("Invalid phone number format. Phone number must have 10 digits.");

            if (boVolunteer.Name.Length < 2)
                throw new BO.InvalidFormatException("Volunteer name is too short. Name must have at least 2 characters.");

            if (boVolunteer.Password.Length < 6)
                throw new BO.InvalidFormatException("Password is too short. Password must have at least 6 characters.");
        }



        internal static DO.Volunteer CreateDoVolunteer(BO.Volunteer boVolunteer)
        {
            return new DO.Volunteer(
                boVolunteer.Id,
                boVolunteer.Name,
                boVolunteer.Email,
                boVolunteer.PhoneN,
               (DO.Role)boVolunteer.role,
                boVolunteer.IsActive,
                boVolunteer.MaxDistanceForTask,
               EncryptPassword(boVolunteer.Password),
                boVolunteer.Address,
                boVolunteer.Longitude,
                boVolunteer.Latitude
            );
        }

   
        public static (double? Latitude, double? Longitude) logicalChecking(BO.Volunteer boVolunteer)
        {
            IsPasswordStrong(boVolunteer.Password);
            return Tools.GetCoordinatesFromAddress(boVolunteer.Address);

        }





        public static void ValidatePermissions(int requesterId, BO.Volunteer boVolunteer)
        {
            bool isSelf = requesterId == boVolunteer.Id;
            bool isAdmin = boVolunteer.role == Role.Manager;

            if (!isAdmin && !isSelf)
                throw new UnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");

            if (!isAdmin && boVolunteer.role != Role.Volunteer)
                throw new UnauthorizedAccessException("Only an admin can update the volunteer's role.");
        }

















        }
    }

