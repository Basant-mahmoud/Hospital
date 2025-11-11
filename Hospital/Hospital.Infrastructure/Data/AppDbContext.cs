
using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
namespace Clinic.Infrastructure.Persistence
{
    public class AppDbContext :IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions options) : base(options) { }
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<Specialization> Specializations => Set<Specialization>();
        public DbSet<Doctor> Doctors => Set<Doctor>();
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
        public DbSet<SupportTicket> SupportTickets => Set<SupportTicket>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<News> News => Set<News>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Banner> Banners => Set<Banner>();
        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));
            var timeOnlyConverter = new ValueConverter<TimeOnly, TimeSpan>(
                t => t.ToTimeSpan(),
                ts => TimeOnly.FromTimeSpan(ts));
            var genderConverter = new EnumToStringConverter<GenderType>();
            var apptStatusConverter = new EnumToStringConverter<AppointmentStatus>();
            var ticketStatusConverter = new EnumToStringConverter<TicketStatus>();
            var bannerTypeConverter = new EnumToStringConverter<BannerType>();
            model.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Gender).HasConversion(genderConverter).HasMaxLength(10);
                e.Property(u => u.DateOfBirth).HasConversion(dateOnlyConverter);
                e.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(u => u.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(u => u.Branch).WithMany(b => b.Users)
                    .HasForeignKey(u => u.BranchId).OnDelete(DeleteBehavior.SetNull);
            });
            model.Entity<Branch>(e =>
            {
                e.ToTable("Branches");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
            model.Entity<Specialization>(e =>
            {
                e.ToTable("Specializations");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
            model.Entity<Doctor>(e =>
            {
                e.ToTable("Doctors");
                e.Property(p => p.ConsultationFees).HasPrecision(10, 2);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(d => d.User).WithOne(u => u.DoctorProfile)
                    .HasForeignKey<Doctor>(d => d.UserId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(d => d.Specialization).WithMany(s => s.Doctors)
                    .HasForeignKey(d => d.SpecializationId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(d => d.Branch).WithMany(b => b.Doctors)
                    .HasForeignKey(d => d.BranchId).OnDelete(DeleteBehavior.Restrict);
            });
            model.Entity<Patient>(e =>
            {
                e.ToTable("Patients");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(p => p.User).WithOne(u => u.PatientProfile)
                    .HasForeignKey<Patient>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            });
            model.Entity<Appointment>(e =>
            {
                e.ToTable("Appointments");
                e.Property(p => p.Date).HasConversion(dateOnlyConverter);
                e.Property(p => p.Time).HasConversion(timeOnlyConverter);
                e.Property(p => p.Status).HasConversion(apptStatusConverter).HasMaxLength(20);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(a => a.Patient).WithMany(p => p.Appointments)
                    .HasForeignKey(a => a.PatientId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(a => a.Doctor).WithMany(d => d.Appointments)
                    .HasForeignKey(a => a.DoctorId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(a => a.Branch).WithMany(b => b.Appointments)
                    .HasForeignKey(a => a.BranchId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(a => a.Creator).WithMany(u => u.CreatedAppointments)
                    .HasForeignKey(a => a.CreatedBy).OnDelete(DeleteBehavior.SetNull);
            });
            model.Entity<Schedule>(e =>
            {
                e.ToTable("Schedules");
                e.Property(p => p.StartTime).HasConversion(timeOnlyConverter);
                e.Property(p => p.EndTime).HasConversion(timeOnlyConverter);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(s => s.Doctor).WithMany(d => d.Schedules)
                    .HasForeignKey(s => s.DoctorId).OnDelete(DeleteBehavior.Cascade);
            });
            model.Entity<MedicalRecord>(e =>
            {
                e.ToTable("MedicalRecords");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(m => m.Appointment).WithMany(a => a.MedicalRecords)
                    .HasForeignKey(m => m.AppointmentId).OnDelete(DeleteBehavior.Cascade);
                e.HasOne(m => m.Doctor).WithMany(d => d.MedicalRecords)
                    .HasForeignKey(m => m.DoctorId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(m => m.Patient).WithMany(p => p.MedicalRecords)
                    .HasForeignKey(m => m.PatientId).OnDelete(DeleteBehavior.Restrict);
            });
            model.Entity<SupportTicket>(e =>
            {
                e.ToTable("SupportTickets");
                e.Property(p => p.Status).HasConversion(ticketStatusConverter).HasMaxLength(20);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(t => t.User).WithMany(u => u.SupportTickets)
                    .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            });
            model.Entity<Service>(e =>
            {
                e.ToTable("Services");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(s => s.Branch).WithMany(b => b.Services)
                    .HasForeignKey(s => s.BranchId).OnDelete(DeleteBehavior.SetNull);
                e.HasOne(s => s.Specialization).WithMany(sp => sp.Services)
                    .HasForeignKey(s => s.SpecializationId).OnDelete(DeleteBehavior.SetNull);
            });
            model.Entity<News>(e =>
            {
                e.ToTable("News");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(n => n.Branch).WithMany(b => b.News)
                    .HasForeignKey(n => n.BranchId).OnDelete(DeleteBehavior.SetNull);
            });
            model.Entity<Event>(e =>
            {
                e.ToTable("Events");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(n => n.Branch).WithMany(b => b.Events)
                    .HasForeignKey(n => n.BranchId).OnDelete(DeleteBehavior.SetNull);
            });
            model.Entity<Banner>(e =>
            {
                e.ToTable("Banners");
                e.Property(p => p.Type).HasConversion(bannerTypeConverter).HasMaxLength(20);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(b => b.Branch).WithMany(br => br.Banners)
                    .HasForeignKey(b => b.BranchId).OnDelete(DeleteBehavior.SetNull);
            });
        }
        public override int SaveChanges()
        {
            ApplyTimestamps();
            return base.SaveChanges();
        }
        public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            ApplyTimestamps();
            return await base.SaveChangesAsync(ct);
        }
        private void ApplyTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State is EntityState.Added or EntityState.Modified);
            foreach (var entry in entries)
            {
                if (entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                    entry.CurrentValues["UpdatedAt"] = DateTime.UtcNow;
                if (entry.State == EntityState.Added &&
                    entry.Properties.Any(p => p.Metadata.Name == "CreatedAt"))
                    entry.CurrentValues["CreatedAt"] = DateTime.UtcNow;
            }
        }
    }
}