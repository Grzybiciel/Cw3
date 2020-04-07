using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Middlewares
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
      /*      httpContext.Request.Method
    [..].Path
    [..].Body
    [..].QueryString
    */
            httpContext.Request.EnableBuffering();
            var metoda = httpContext.Request.Method;
            var path = httpContext.Request.Path;
            var queryString = httpContext.Request.QueryString;
            var bodyStream = string.Empty;
            using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
            {
                bodyStream = await reader.ReadToEndAsync();
            }
            var dir = @"requestsLog.txt";
            using (StreamWriter strWriter = new StreamWriter(dir, true))
            {
                strWriter.WriteLine("===================================");
                strWriter.WriteLine(metoda);
                strWriter.WriteLine(path);
                strWriter.WriteLine(bodyStream);
                strWriter.WriteLine(queryString);
                strWriter.WriteLine("===================================");
                
             
            }

            httpContext.Request.Body.Seek(0, SeekOrigin.Begin);
            await _next(httpContext);
        }
    }
}
