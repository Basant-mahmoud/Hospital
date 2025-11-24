using Hospital.Domain.Enum;
using Hospital.Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Clinic.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<User>
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
        public DbSet<PasswordResetCode> PasswordResetCodes => Set<PasswordResetCode>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);

            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(DateTime.SpecifyKind(dt, DateTimeKind.Utc)));

            var genderConverter = new EnumToStringConverter<GenderType>();
            var apptStatusConverter = new EnumToStringConverter<AppointmentStatus>();
            var ticketStatusConverter = new EnumToStringConverter<TicketStatus>();
            var bannerTypeConverter = new EnumToStringConverter<BannerType>();
            var paymentStatusConverter = new EnumToStringConverter<PaymentStatus>();

            // User
            model.Entity<User>(e =>
            {
                e.ToTable("Users");
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Gender).HasConversion(genderConverter).HasMaxLength(10);
                e.Property(u => u.DateOfBirth).HasConversion(dateOnlyConverter);
                e.Property(u => u.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(u => u.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Branch
            model.Entity<Branch>(e =>
            {
                e.ToTable("Branches");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasMany(b => b.Doctors)
                 .WithMany(d => d.Branches);
            });

            // Specialization
            model.Entity<Specialization>(e =>
            {
                e.ToTable("Specializations");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasMany(s => s.Branches)
                 .WithMany(b => b.Specializations)
                 .UsingEntity<Dictionary<string, object>>(
                    "BranchSpecialization",
                    j => j.HasOne<Branch>().WithMany().HasForeignKey("BranchId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Specialization>().WithMany().HasForeignKey("SpecializationId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("BranchId", "SpecializationId");
                        j.ToTable("BranchSpecializations");
                    });
            });

            // Service
            model.Entity<Service>(e =>
            {
                e.ToTable("Services");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasMany(s => s.Branches)
                 .WithMany(b => b.Services)
                 .UsingEntity<Dictionary<string, object>>(
                    "BranchService",
                    j => j.HasOne<Branch>().WithMany().HasForeignKey("BranchId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<Service>().WithMany().HasForeignKey("ServiceId").OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey("BranchId", "ServiceId");
                        j.ToTable("BranchServices");
                    });
            });

            // Doctor
            model.Entity<Doctor>(e =>
            {
                e.ToTable("Doctors");
                e.Property(p => p.ConsultationFees).HasPrecision(10, 2);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(d => d.User)
                    .WithOne(u => u.DoctorProfile)
                    .HasForeignKey<Doctor>(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(d => d.Specialization)
                    .WithMany(s => s.Doctors)
                    .HasForeignKey(d => d.SpecializationId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(d => d.Branches)
                    .WithMany(b => b.Doctors)
                    .UsingEntity(j => j.ToTable("DoctorBranches"));
            });

            // Patient
            model.Entity<Patient>(e =>
            {
                e.ToTable("Patients");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(p => p.User).WithOne(u => u.PatientProfile)
                    .HasForeignKey<Patient>(p => p.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // Appointment
            model.Entity<Appointment>(e =>
            {
                e.ToTable("Appointments");
                e.Property(p => p.Date).HasConversion(dateOnlyConverter);
                e.Property(p => p.Shift).HasConversion<string>().HasMaxLength(20).IsRequired();
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

                // 1:1 Appointment–Payment (principal side)
                e.HasOne(a => a.Payment)
                    .WithOne(p => p.Appointment)
                    .HasForeignKey<Payment>(p => p.AppointmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Schedule
            model.Entity<Schedule>(e =>
            {
                e.ToTable("Schedules");
                e.Property(p => p.StartTime).IsRequired();
                e.Property(p => p.EndTime).IsRequired();
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                e.HasOne(s => s.Doctor)
                    .WithMany(d => d.Schedules)
                    .HasForeignKey(s => s.DoctorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // MedicalRecord
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

            // SupportTicket
            model.Entity<SupportTicket>(e =>
            {
                e.ToTable("SupportTickets");
                e.Property(p => p.Status).HasConversion(ticketStatusConverter).HasMaxLength(20);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(t => t.User).WithMany(u => u.SupportTickets)
                    .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
            });

            // News
            model.Entity<News>(e =>
            {
                e.ToTable("News");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Event
            model.Entity<Event>(e =>
            {
                e.ToTable("Events");
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Banner
            model.Entity<Banner>(e =>
            {
                e.ToTable("Banners");
                e.Property(p => p.Type).HasConversion(bannerTypeConverter).HasMaxLength(20);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Payment
            model.Entity<Payment>(e =>
            {
                e.ToTable("Payments");
                e.Property(p => p.Amount).HasPrecision(10, 2);
                e.Property(p => p.Currency).HasMaxLength(3);
                e.Property(p => p.Status)
                    .HasConversion(paymentStatusConverter)
                    .HasMaxLength(20);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                // enforce 1:1 at DB level
                e.HasIndex(p => p.AppointmentId).IsUnique();
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
