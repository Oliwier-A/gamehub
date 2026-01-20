using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using praca_dyplomowa_zesp.Models;
using praca_dyplomowa_zesp.Models.Interactions.Comments;
using praca_dyplomowa_zesp.Models.Interactions.Rates;
using praca_dyplomowa_zesp.Models.Interactions.Reactions;
using praca_dyplomowa_zesp.Models.Modules.Guides;
using praca_dyplomowa_zesp.Models.Modules.Games;
using praca_dyplomowa_zesp.Models.Modules.Libraries;
using praca_dyplomowa_zesp.Models.Modules.Users;

namespace praca_dyplomowa_zesp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Guide> Guides { get; set; } = null!;
        public DbSet<Tip> Tips { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Reply> Replies { get; set; } = null!;
        public DbSet<Rate> Rates { get; set; } = null!;
        public DbSet<Reaction> Reactions { get; set; } = null!;
        public DbSet<GameInLibrary> GamesInLibraries { get; set; } = null!;
        public DbSet<UserAchievement> UserAchievements { get; set; } = null!;
        public DbSet<ToDoItem> ToDoItems { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<TicketMessage> TicketMessages { get; set; } = null!;
        public DbSet<TicketAttachment> TicketAttachments { get; set; } = null!;
        public DbSet<GameRate> GameRates { get; set; } = null!;
        public DbSet<GameMap> GameMaps { get; set; } = null!;
        public DbSet<GameReview> GameReviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Konfiguracje podstawowe ---
            modelBuilder.Entity<GameInLibrary>()
                .HasOne(g => g.User)
                .WithMany()
                .HasForeignKey(g => g.UserId);

            modelBuilder.Entity<UserAchievement>()
                .HasOne(ua => ua.User)
                .WithMany()
                .HasForeignKey(ua => ua.UserId);

            modelBuilder.Entity<Guide>()
                .HasIndex(g => g.IgdbGameId);

            // --- LOGIKA BIZNESOWA (CASCADE) ---

            // Usunięcie komentarza głównego -> usuwa odpowiedzi
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(r => r.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usunięcie komentarza -> usuwa jego lajki
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Comment)
                .WithMany(c => c.Reactions)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Usunięcie recenzji -> usuwa jej lajki
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.GameReview)
                .WithMany(gr => gr.Reactions)
                .HasForeignKey(r => r.GameReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            // 1. Reakcje pod Wskazówkami
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Tip)
                .WithMany(t => t.Reactions)
                .HasForeignKey(r => r.TipId)
                .OnDelete(DeleteBehavior.NoAction);

            // 2. Reakcje pod odpowiedziami
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.Reply)
                .WithMany(r => r.Reactions)
                .HasForeignKey(r => r.ReplyId)
                .OnDelete(DeleteBehavior.NoAction);

            // 3. Reakcje od użytkownika
            modelBuilder.Entity<Reaction>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 4. Odpowiedzi od użytkownika
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.Author)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 5. Komentarze od użytkownika
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Author)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 6. Wiadomości w ticketach od użytkownika
            modelBuilder.Entity<TicketMessage>()
                .HasOne(tm => tm.User)
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 7. Recenzje gier od użytkownika
            modelBuilder.Entity<GameReview>()
                .HasOne(gr => gr.User)
                .WithMany()
                .HasForeignKey(gr => gr.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // 8. Oceny poradników od użytkownika
            modelBuilder.Entity<Rate>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}