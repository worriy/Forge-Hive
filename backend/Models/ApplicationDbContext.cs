using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hive.Backend.DataModels;
using Hive.Backend.Models.JoinTables;

namespace Hive.Backend.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole<string>, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<IdentityUserLogin<string>>().HasKey(e => new { e.UserId });
            builder.Entity<IdentityUserRole<string>>().HasKey(e => new { e.RoleId, e.UserId });
            builder.Entity<IdentityUserToken<string>>().HasKey(e => new { e.UserId });

            builder.Entity<Card>().HasKey(e => new { e.Id });

            //Type discriminator on the Card model to differentiate Reportings and Ideas (and later on, others)
            builder.Entity<Card>().HasDiscriminator<string>("Type");

            builder.Entity<Reporting>().HasBaseType<Card>();
            builder.Entity<Reporting>().HasMany(r => r.Results);

            builder.Entity<Idea>().HasBaseType<Card>();
            builder.Entity<Question>().HasBaseType<Card>();
            builder.Entity<Event>().HasBaseType<Card>();
            builder.Entity<Survey>().HasBaseType<Card>();
            builder.Entity<Mood>().HasBaseType<Card>();
            builder.Entity<Quote>().HasBaseType<Card>();
            builder.Entity<Suggestion>().HasBaseType<Card>();
            builder.Entity<Result>().HasKey(e => new { e.Id });
            builder.Entity<Choice>().HasKey(e => new { e.Id });
            builder.Entity<Answer>().HasKey(e => new { e.Id });
            builder.Entity<UserProfile>().HasKey(e => new { e.Id });
            builder.Entity<Group>().HasKey(e => new { e.Id });
            builder.Entity<Picture>().HasKey(e => new { e.Id });

            //Configuring join table for many to many relation between card and group
            builder.Entity<CardGroup>().HasKey(fks => new { fks.CardId, fks.GroupId });
            builder.Entity<CardGroup>().HasOne(c => c.Card)
                .WithMany(x => x.CardGroup)
                .HasForeignKey(y => y.CardId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<CardGroup>().HasOne(p => p.Group)
                .WithMany(x => x.CardGroup)
                .HasForeignKey(ci => ci.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configuring join table for many to many relation between group and user (members)
            builder.Entity<GroupUser>().HasKey(fks => new { fks.GroupId, fks.UserId });
            builder.Entity<GroupUser>().HasOne(gu => gu.Group)
                .WithMany(gr => gr.GroupUser)
                .HasForeignKey(gu => gu.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<GroupUser>().HasOne(gu => gu.User)
                .WithMany(u => u.GroupUser)
                .HasForeignKey(gu => gu.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configuring join table for many to many relation (views) between user and card
            builder.Entity<UserCardViews>().HasKey(fks => new { fks.CardId, fks.UserId });
            builder.Entity<UserCardViews>().HasOne(ucv => ucv.Card)
                .WithMany(c => c.UserCardViews)
                .HasForeignKey(ucv => ucv.CardId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserCardViews>().HasOne(ucv => ucv.User)
                .WithMany(u => u.UserCardViews)
                .HasForeignKey(ucv => ucv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //Configuring join table for many to many relation (likes) between user and card
            builder.Entity<UserCardLikes>().HasKey(fks => new { fks.CardId, fks.UserId });
            builder.Entity<UserCardLikes>().HasOne(ucv => ucv.Card)
                .WithMany(c => c.UserCardLikes)
                .HasForeignKey(ucv => ucv.CardId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<UserCardLikes>().HasOne(ucv => ucv.User)
                .WithMany(u => u.UserCardLikes)
                .HasForeignKey(ucv => ucv.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        
        public DbSet<Card> Cards { get; set; }
        public DbSet<Reporting> Reportings { get; set; }
        public DbSet<Idea> Ideas { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Mood> Moods { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<CardGroup> CardGroups { get; set; }
    }
}