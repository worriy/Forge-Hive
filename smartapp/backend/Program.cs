
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hive.Backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Hive.Backend
{
    public class Program
    {
        private static Guid publicGroupId;
        private static Guid discardChoiceId;
        private static Guid moodId;

        private static Guid ideaDefaultPicId;
        private static Guid questionDefaultPicId;
        private static Guid eventDefaultPicId;
        private static Guid moodDefaultPicId;
        private static Guid suggestionDefaultPicId;
        private static Guid defaultPicId;

        private static Guid adminId;

        public static Guid SuggestionDefaultPicId { get => suggestionDefaultPicId; set => suggestionDefaultPicId = value; }
        public static Guid MoodDefaultPicId { get => moodDefaultPicId; set => moodDefaultPicId = value; }
        public static Guid EventDefaultPicId { get => eventDefaultPicId; set => eventDefaultPicId = value; }
        public static Guid QuestionDefaultPicId { get => questionDefaultPicId; set => questionDefaultPicId = value; }
        public static Guid IdeaDefaultPicId { get => ideaDefaultPicId; set => ideaDefaultPicId = value; }
        public static Guid DefaultPicId { get => defaultPicId; set => defaultPicId = value; }
        public static Guid MoodId { get => moodId; set => moodId = value; }
        public static Guid DiscardChoiceId { get => discardChoiceId; set => discardChoiceId = value; }
        public static Guid PublicGroupId { get => publicGroupId; set => publicGroupId = value; }
        public static Guid AdminId { get => adminId; set => adminId = value; }

        public static void Main()
        {
            var config = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true)
                        .Build();


            string url = "http://*:8080";
            url = config.GetSection("appUrl")?.Value;
            var log = config.GetSection("Logging");

            var host = new HostBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls(url)
                    .UseStartup<Startup>()
                    .ConfigureLogging((hostingContext, logging) =>
                    {
                        logging.AddConfiguration(log);
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddConsole();
                    });
                })
                
                .Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                Console.WriteLine("trying to initialize the database");
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                //var roleManager 
                var Dbinit = services.GetRequiredService<DbInitializer>();

                Dbinit.Initialize(context).Wait();

            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occured while seeding the database.");
            }

            host.Run();
        }
    }
}

