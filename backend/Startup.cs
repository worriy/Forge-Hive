using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.Swagger;
using Hive.Backend.Models;
using Hive.Backend.Services;
using Hive.Backend.Infrastructure.Services;
using Hive.Backend.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;

namespace Hive.Backend
{
    public partial class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddCors(options =>
            {
                options.AddPolicy("SiteCorsPolicy",
                builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            });

            //Configure authentication services.
            ConfigureAuth(services);
            services.AddRouting();
            services.AddControllers();
            services.AddRazorPages();
            services.AddApplicationInsightsTelemetry();
            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IIdentifierRepository, IdentifierRepository>();
            services.AddScoped<ICardRepository, CardRepository>();
            services.AddScoped<IReportingRepository, ReportingRepository>();
            services.AddScoped<IIdeaRepository, IdeaRepository>();
            services.AddScoped<IResultRepository, ResultRepository>();
            services.AddScoped<IChoiceRepository, ChoiceRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IPictureRepository, PictureRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<IQuoteRepository, QuoteRepository>();
            services.AddScoped<ISuggestionRepository, SuggestionRepository>();

            services.AddScoped<IIdentifierService, IdentifierService>();
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<IReportingService, ReportingService>();
            services.AddScoped<IIdeaService, IdeaService>();
            services.AddScoped<IResultService, ResultService>();
            services.AddScoped<IChoiceService, ChoiceService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IPictureService, PictureService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<IQuoteService, QuoteService>();
            services.AddScoped<ISuggestionService, SuggestionService>();

            services.AddScoped<DbInitializer>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Hive API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
            });

            services.Configure<AuthMessageSenderOptions>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            //database migrations
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("SiteCorsPolicy");

            
            ConfigureAuth(app);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
                //endpoints.MapRazorPages();
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hive API V1");
            });
        }
    }
}