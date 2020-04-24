using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request != null)
            {
                string path = context.Request.Path;
                string method = context.Request.Method;
                string queryString = context.Request.QueryString.ToString();
                string bodyStr = "";

                using(StreamReader reader=new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                }

                await using StreamWriter writer = new StreamWriter(@"C:\Users\PC\RiderProjects\WebApplication\WebApplication\logfile.txt", true);
                writer.WriteLine(path+" "+method+" "+queryString+" "+bodyStr);
            }
            
            if(_next!=null) await _next(context);
        }
    }
}