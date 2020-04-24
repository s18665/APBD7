using System;
using WebApplication.Models;

namespace WebApplication.Services
{
    public interface IStudentServiceDb
    {
        EnrollStudentResponse EnrollStudent(EnrollStudentRequest request);
        PromoteStudentsResponse PromoteStudents(PromoteStudentsRequest request);
        void InsertRefreshToken(string indexNumber, string token);
        LoginResponse LoginRequest(string login, string password);
        LoginResponse LoginRequest(string refreshToken);
    }
}