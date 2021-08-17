using Hive.Backend.Api;
using Hive.Backend.DataModels;
using Hive.Backend.Models;
using Hive.Backend.Models.JoinTables;
using Hive.Backend.Security;
using Hive.Backend.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hive.Backend
{
    public class DbInitializer
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole<string>> _roleManager;

        public DbInitializer(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole<string>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (!context.Groups.Any())
                await CreateDefaultGroup(context);
            else
                Program.PublicGroupId = context.Groups.Where(g => g.Name.ToUpper() == "PUBLIC").Select(g => g.Id).FirstOrDefault();

            var t = context.UserProfiles;
            if (!context.UserProfiles.Any())
                await CreateAdminUser(context);
            else
                Program.AdminId = context.UserProfiles.Where(u => u.User.Email == "admin@hive.com").Select(g => g.Id).FirstOrDefault();

            if (!context.Choices.Any())
                await CreateDefaultChoice(context);
            else
            {
                Program.DiscardChoiceId = context.Choices.Where(g => g.Name.ToUpper() == "DISCARD").Select(c => c.Id).FirstOrDefault();
                Program.StartSurveyChoiceId = context.Choices.Where(g => g.Name.ToUpper() == "STARTSURVEY").Select(c => c.Id).FirstOrDefault();
            }
            if (!context.Pictures.Any())
            {
                await CreateIdeaDefaultPic(context);
                await CreateQuestionDefaultPic(context);
                await CreateEventDefaultPic(context);
                await CreateSurveyDefaultPic(context);
                await CreateMoodDefaultPic(context);
                await CreateQuoteDefaultPic(context);
                await CreateSuggestionDefaultPic(context);
            }
            else
            {
                Program.IdeaDefaultPicId = new Guid("de90b138-ffce-4253-ee7a-08d8d9928580");
                Program.QuestionDefaultPicId = new Guid("2dc19c7f-3311-4c91-ee7b-08d8d9928580");
                Program.EventDefaultPicId = new Guid("3debe9d5-1497-4f93-ee7c-08d8d9928580");
                Program.SurveyDefaultPicId = new Guid("24867770-54eb-4bfd-ee7d-08d8d9928580");
                Program.MoodDefaultPicId = new Guid("b3edd5a6-14e1-47fb-ee7e-08d8d9928580");
                Program.QuoteDefaultPicId = new Guid("768325dd-5d8e-43fc-adb0-14317a5d715a");
                Program.SuggestionDefaultPicId = new Guid("c1dae9e2-cdcc-4fb1-b540-47378374d0b3");
            }

            if (!context.Moods.Any())
                await CreateDefaultMoodCard(context);
            else
                Program.MoodId = context.Moods.Select(c => c.Id).FirstOrDefault();
                

            Console.WriteLine("Database has been seeded");
            return;

        }

        private async Task CreateAdminUser(ApplicationDbContext context/*, UserManager<ApplicationUser> userManager*/)
        {
            Console.WriteLine("Creating Admin user");
            
            IdentityResult chkUserCreation = await this._userManager.CreateAsync(new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "admin@hive.com", Email = "admin@hive.com", FirstName = "Administrator", LastName = "Hive" });

            if (chkUserCreation.Succeeded)
            {
                var user = await this._userManager.FindByEmailAsync("admin@hive.com");

                await this._userManager.AddPasswordAsync(user, "P@ssw0rd");

                UserProfile userProfile = new UserProfile()
                {
                    City = "City",
                    Country = "Country",
                    Department = "Department",
                    Job = "Job",
                    User = user
                };
                context.UserProfiles.Add(userProfile);
                await context.SaveChangesAsync();
                

                if (_roleManager.FindByNameAsync("Admin").Result == null)
                {
                    await this.CreateRoles();
                }

                List<string> roles = new List<string>
                {
                    "Admin"
                };

                GroupUser member = new GroupUser
                {
                    GroupId = Program.PublicGroupId,
                    UserId = userProfile.Id
                };

                var group = context.Groups.Where(g => g.Id == Program.PublicGroupId).FirstOrDefault();
                group.GroupUser.Add(member);
                context.Set<GroupUser>().Add(member);

                await this._userManager.AddToRolesAsync(user, roles);
                Program.AdminId = userProfile.Id;
            }
        }

        private async Task CreateRoles()
        {
            //Initialize the roles
            var adminRole = new ApplicationRole<string>
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Admin"
            };
            var dataEditorRole = new ApplicationRole<string>
            {
                Id = Guid.NewGuid().ToString(),
                Name = "DataEditor"
            };
            var userRole = new ApplicationRole<string>
            {
                Id = Guid.NewGuid().ToString(),
                Name = "User"
            };

            //Create the role in the roles store
            await _roleManager.CreateAsync(adminRole);
            await _roleManager.CreateAsync(dataEditorRole);
            await _roleManager.CreateAsync(userRole);
        }

        private static async Task CreateDefaultGroup(ApplicationDbContext context)
        {
            Console.WriteLine("Creating public group");
            var publicGroup = new DataModels.Group
            {
                Name = "Public",
                Department = "ALL",
                City = "",
                Country = "",
                CreationDate = new DateTime(),
                GroupUser = new List<GroupUser>()
            };
            await context.Groups.AddAsync(publicGroup);
            await context.SaveChangesAsync();
            Program.PublicGroupId = publicGroup.Id;
        }

        private static async Task CreateDefaultChoice(ApplicationDbContext context)
        {
            Console.WriteLine("Creating Discard choice");
            var discardChoice = new Choice
            {
                Name = "DISCARD"
            };
            await context.Choices.AddAsync(discardChoice);
            await context.SaveChangesAsync();
            Program.DiscardChoiceId = discardChoice.Id;

            Console.WriteLine("Creating Start Survey Choice");
            var startSurveyChoice = new Choice
            {
                Name = "STARTSURVEY"
            };
            await context.Choices.AddAsync(startSurveyChoice);
            await context.SaveChangesAsync();
            Program.StartSurveyChoiceId = startSurveyChoice.Id;
        }

        private static async Task CreateDefaultMoodCard(ApplicationDbContext context)
        {
            Console.WriteLine("Creating Mood Card");
            // Create the Mood Card
            Mood mood = new Mood
            {
                // Create the content 
                Content = "What's your mood today?",
                Choices = new HashSet<Choice>(),
                /*{
                    new Choice()
                    {
                        Name = "Happy"
                    },
                    new Choice()
                    {
                        Name = "Indifferent"
                    },
                    new Choice()
                    {
                        Name = "Muted"
                    },
                    new Choice()
                    {
                        Name = "Sad"
                    },
                    new Choice()
                    {
                        Name = "Sunglasses"
                    }
                },*/

                // CreatedBy is the Admin
                CreatedById = Program.AdminId
            };

            mood.Choices.Add(new Choice()
            {
                Name = "Happy"
            });
            mood.Choices.Add(new Choice()
            {
                Name = "Indifferent"
            });
            mood.Choices.Add(new Choice()
            {
                Name = "Muted"
            });
            mood.Choices.Add(new Choice()
            {
                Name = "Sad"
            });
            mood.Choices.Add(new Choice()
            {
                Name = "Sunglasses"
            });
            var createdBy = context.UserProfiles.Find(mood.CreatedById);
            context.UserProfiles.Attach(createdBy);

            // Picture of the Mood Card
            mood.PictureId = Program.MoodDefaultPicId;
            var picture = context.Pictures.Find(mood.PictureId);
            context.Pictures.Attach(picture);

            // Publish this Mood Card in the Public Group
            mood.CardGroup = new List<CardGroup>
            {
                new CardGroup(Guid.Empty, Program.PublicGroupId)
            };
            var group = context.Groups.Find(Program.PublicGroupId);
            context.Groups.Attach(group);

            // Publication Date and End Date of Mood Card
            mood.PublicationDate = DateTime.Now.Date;
            mood.EndDate = DateTime.Now.Date.AddHours(23);
            
            /*var discardChoice = new Choice
            {
                Name = "DISCARD"
            };
            await context.Choices.AddAsync(discardChoice);
            await context.SaveChangesAsync();
            Program.discardChoiceId = discardChoice.Id;*/
            await context.Moods.AddAsync(mood);
            mood.LinkedCardId = mood.Id;
            await context.SaveChangesAsync();
            Program.MoodId = mood.Id;
        }

        private static async Task CreateIdeaDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_idea.jpg");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("de90b138-ffce-4253-ee7a-08d8d9928580"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.IdeaDefaultPicId = Picture.Id;
        }

        private static async Task CreateQuestionDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture For Question");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_question.jpg");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("2dc19c7f-3311-4c91-ee7b-08d8d9928580"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.QuestionDefaultPicId = Picture.Id;
        }

        private static async Task CreateEventDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture For Event");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_event.jpg");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("3debe9d5-1497-4f93-ee7c-08d8d9928580"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.EventDefaultPicId = Picture.Id;
        }

        private static async Task CreateSurveyDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture For Survey");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_survey.jpg");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("24867770-54eb-4bfd-ee7d-08d8d9928580"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.SurveyDefaultPicId = Picture.Id;
        }

        private static async Task CreateMoodDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture For Mood");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_mood.jpg");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("b3edd5a6-14e1-47fb-ee7e-08d8d9928580"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.MoodDefaultPicId = Picture.Id;
        }

        private static async Task CreateQuoteDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture For Quote");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_quote.png");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("768325dd-5d8e-43fc-adb0-14317a5d715a"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.QuoteDefaultPicId = Picture.Id;
        }

        private static async Task CreateSuggestionDefaultPic(ApplicationDbContext context)
        {
            Console.WriteLine("Creating default picture For Suggestion");
            Byte[] imageArray = System.IO.File.ReadAllBytes("./wwwroot/uploads/create_suggestion.jpg");
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);
            var Picture = new Picture
            {
                Id = new Guid("c1dae9e2-cdcc-4fb1-b540-47378374d0b3"),
                PicBase64 = base64ImageRepresentation
            };
            await context.Pictures.AddAsync(Picture);
            await context.SaveChangesAsync();
            Program.SuggestionDefaultPicId = Picture.Id;
        }
    }
}