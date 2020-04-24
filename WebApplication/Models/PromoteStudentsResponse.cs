using System;

namespace WebApplication.Models
{
    public class PromoteStudentsResponse
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public string Study { get; set; }
        public DateTime StartDate { get; set; }
    }
}