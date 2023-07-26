namespace LogReg
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace YourWebAppNamespace
{
    public class Startup
    {
        private List<User> users = new List<User>();
        private const string usersFilePath = "users.json"; // Путь к файлу users.json

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("<html><body>");
                    await context.Response.WriteAsync("<h1>Welcome to the Web App</h1>");
                    await context.Response.WriteAsync("<form action='/login' method='post'><button type='submit'>Login</button></form>");
                    await context.Response.WriteAsync("<form action='/registration' method='post'><button type='submit'>Registration</button></form>");
                    await context.Response.WriteAsync("</body></html>");
                });

                endpoints.MapPost("/login", async context =>
                {
                    var username = context.Request.Form["username"];
                    var password = context.Request.Form["password"];
                    var user = users.Find(u => u.Username == username);

                    if (user != null && VerifyPassword(password, user.PasswordHash))
                    {
                        await context.Response.WriteAsync("Login successful!");
                    }
                    else
                    {
                        await context.Response.WriteAsync("Invalid username or password.");
                    }
                });

                endpoints.MapPost("/registration", async context =>
                {
                    var username = context.Request.Form["username"];
                    var password = context.Request.Form["password"];
                    var user = users.Find(u => u.Username == username);

                    if (user == null)
                    {
                        var passwordHash = HashPassword(password);
                        users.Add(new User { Username = username, PasswordHash = passwordHash });
                        SaveUsersToFile();
                        await context.Response.WriteAsync("Registration successful!");
                    }
                    else
                    {
                        await context.Response.WriteAsync("Username already exists.");
                    }
                });
            });

            LoadUsersFromFile();
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return ConvertBytesToHexString(hashedBytes);
            }
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            var hashedPassword = HashPassword(password);
            return hashedPassword == passwordHash;
        }

        private string ConvertBytesToHexString(byte[] bytes)
        {
            var builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private void LoadUsersFromFile()
        {
            if (File.Exists(usersFilePath))
            {
                var json = File.ReadAllText(usersFilePath);
                users = JsonConvert.DeserializeObject<List<User>>(json);
            }
        }

        private void SaveUsersToFile()
        {
            var json = JsonConvert.SerializeObject(users);
            File.WriteAllText(usersFilePath, json);
        }
    }

    public class User
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
    }
}
