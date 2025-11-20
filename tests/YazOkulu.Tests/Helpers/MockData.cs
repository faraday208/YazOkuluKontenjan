using YazOkulu.Application.DTOs.Courses;
using YazOkulu.Domain.Entities;
using YazOkulu.Domain.Enums;

namespace YazOkulu.Tests.Helpers;

public static class MockData
{
    public static Student CreateStudent(int id = 1, string phone = "5001234567")
    {
        return new Student
        {
            Id = id,
            PhoneNumber = phone,
            FirstName = "Test",
            LastName = "Student",
            Email = $"{phone}@test.com",
            StudentNumber = $"STU{id:000}",
            Department = "Computer Engineering"
        };
    }

    public static Course CreateCourse(int id = 1, string code = "CS101", int quota = 30)
    {
        return new Course
        {
            Id = id,
            Code = code,
            Name = $"Test Course {code}",
            Quota = quota,
            Department = "Computer Engineering",
            Faculty = "Engineering",
            Instructor = "Dr. Test",
            Credits = 3,
            Applications = new List<CourseApplication>()
        };
    }

    public static CourseApplication CreateApplication(
        int id = 1,
        int studentId = 1,
        int courseId = 1,
        ApplicationStatus status = ApplicationStatus.Pending)
    {
        return new CourseApplication
        {
            Id = id,
            StudentId = studentId,
            CourseId = courseId,
            Status = status,
            AppliedAt = DateTime.UtcNow,
            Student = CreateStudent(studentId),
            Course = CreateCourse(courseId)
        };
    }

    public static OtpCode CreateOtpCode(
        string phone = "5001234567",
        string code = "123456",
        OtpStatus status = OtpStatus.Active,
        DateTime? expiresAt = null)
    {
        return new OtpCode
        {
            Id = 1,
            PhoneNumber = phone,
            Code = code,
            Status = status,
            ExpiresAt = expiresAt ?? DateTime.UtcNow.AddMinutes(5),
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<Course> CreateCourseList()
    {
        return new List<Course>
        {
            CreateCourse(1, "CS101", 30),
            CreateCourse(2, "CS201", 25),
            CreateCourse(3, "MAT101", 40)
        };
    }

    public static UpdateCourseDto CreateUpdateCourseDto(
        string code = "CS101",
        string name = "Updated Course",
        int quota = 30,
        string department = "Computer Engineering",
        string faculty = "Engineering",
        string instructor = "Dr. Updated",
        int credits = 3)
    {
        return new UpdateCourseDto
        {
            Code = code,
            Name = name,
            Quota = quota,
            Department = department,
            Faculty = faculty,
            Instructor = instructor,
            Credits = credits
        };
    }
}
