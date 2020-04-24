using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentServiceDb _service;
        
        public EnrollmentsController(IStudentServiceDb service)
        {
            _service = service;
        }
        
        [HttpPost]
        [Authorize(Roles = "employee")]
        public IActionResult EnrollStudent(EnrollStudentRequest request)
        {
            var response = _service.EnrollStudent(request);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest();
        }
    
        [HttpPost("promote")]
        [Authorize(Roles = "employee")]
        public IActionResult PromoteStudents(PromoteStudentsRequest request)
        {
            var response = _service.PromoteStudents(request);
            if (response != null)
            {
                return Ok(response);
            }
            return NotFound();
        }
        
        [HttpPost("login")]
        public IActionResult Login(LoginRequest loginRequest)
        {
            LoginResponse user;
            static string GetMd5Hash(MD5 md5Hash, string input)
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }
                return sBuilder.ToString();
            }
            
            string hash;
            
            if (loginRequest.RefreshToken == null)
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    hash = GetMd5Hash(md5Hash, loginRequest.Password);
                }
                user = _service.LoginRequest(loginRequest.Login, hash);
            }
            else
            {
                user = _service.LoginRequest(loginRequest.RefreshToken);
            }

            if (user == null)
            {
                return Unauthorized();
            }
            
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.IndexNumber),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("8du9sa8d98ausd98uas8duas8ud8asdyasd98asy9d8yas9d8ay8dsds"));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken
            (
                issuer: "Gakko",
                audience: "Students",
                claims: claims,
                expires: DateTime.Now.AddMinutes(10),
                signingCredentials: credentials
            );

           var grefreshToken = Guid.NewGuid().ToString();
            _service.InsertRefreshToken(user.IndexNumber, grefreshToken);

            return Ok(new 
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken = grefreshToken
            });

        }
    }
}