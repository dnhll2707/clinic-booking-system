using DaoNuHoangLyLy_2123110414.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DaoNuHoangLyLy_2123110414.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Specialty> Specialties => Set<Specialty>();
        public DbSet<DoctorProfile> DoctorProfiles => Set<DoctorProfile>();
        public DbSet<PatientProfile> PatientProfiles => Set<PatientProfile>();
        public DbSet<DoctorSchedule> DoctorSchedules => Set<DoctorSchedule>();
        public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();
        public DbSet<Appointment> Appointments => Set<Appointment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Specialty>()
                .HasIndex(x => x.Name)
                .IsUnique();

            builder.Entity<DoctorProfile>()
                .HasOne(x => x.Specialty)
                .WithMany(x => x.DoctorProfiles)
                .HasForeignKey(x => x.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DoctorProfile>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DoctorProfile>()
                .HasIndex(x => x.UserId)
                .IsUnique();

            builder.Entity<PatientProfile>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PatientProfile>()
                .HasIndex(x => x.UserId)
                .IsUnique();

            builder.Entity<DoctorSchedule>()
                .HasOne(x => x.DoctorProfile)
                .WithMany(x => x.DoctorSchedules)
                .HasForeignKey(x => x.DoctorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TimeSlot>()
                .HasOne(x => x.DoctorSchedule)
                .WithMany(x => x.TimeSlots)
                .HasForeignKey(x => x.DoctorScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TimeSlot>()
                .HasOne(x => x.DoctorProfile)
                .WithMany(x => x.TimeSlots)
                .HasForeignKey(x => x.DoctorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(x => x.PatientProfile)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.PatientProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(x => x.DoctorProfile)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.DoctorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(x => x.TimeSlot)
                .WithOne(x => x.Appointment)
                .HasForeignKey<Appointment>(x => x.TimeSlotId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasIndex(x => x.TimeSlotId)
                .IsUnique();

            builder.Entity<Appointment>()
                .HasIndex(x => x.AppointmentCode)
                .IsUnique();

            builder.Entity<TimeSlot>()
                .HasIndex(x => new { x.DoctorProfileId, x.SlotDate, x.StartTime, x.EndTime })
                .IsUnique();
        }
    }
}