

using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static BO.Year GetVolunteerCurrentYear(DateTime? registrationDate)
    {
        BO.Year currYear = (BO.Year)(ClockManager.Now.Year - registrationDate?.Year!);
        return currYear > BO.Year.None ? BO.Year.None : currYear;
    }
    internal static BO.StudentInCourse GetDetailedCourseForStudent(int studentId, int courseId)
    {
        DO.Link? doLink = s_dal.Link.Read(l => l.StudentId == studentId && l.CourseId == courseId)
            ?? throw new BO.BlDoesNotExistException($"Student with ID={studentId} does Not take Course with ID={courseId}");
        DO.Course? doCourse = s_dal.Course.Read(courseId)
     ?? throw new BO.BlDoesNotExistException($"Course with ID={courseId} does Not exist");

        return new()
        {
            StudentId = studentId,
            Course = new Tuple<int, string, string>(doCourse.Id, doCourse.CourseNumber, doCourse.CourseName),
            InYear = (BO.Year?)doCourse.InYear,
            InSemester = (BO.SemesterNames?)doCourse.InSemester,
            Grade = doLink.Grade,
            Credits = doCourse.Credits
        };
    }
    internal static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock) //stage 4
{
	var list = s_dal.Volunteer.ReadAll().ToList();
    	foreach (var doVolunteer in list)
    	{
        	//if student study for more than MaxRange years
        	//then student should be automatically updated to 'not active'
        	if (ClockManager.Now.Year - doVolunteer.RegistrationDate?.Year >= s_dal.Config.RiskRange)
        	{
            	s_dal.Volunteer.Update(doVolunteer with { Active = false });
        	}
    	}
}



}
