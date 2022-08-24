using System;
using API_premierductsqld.Entities;
using API_qlddata.Entity.response;
using Microsoft.EntityFrameworkCore;

namespace API_qlddata.Context
{
    public partial class QLDdatacontext : DbContext
    {
        public QLDdatacontext()
        {
        }
    
        public QLDdatacontext(DbContextOptions<QLDdatacontext> options)
        : base(options)
        {
        }
        private readonly string _connectionString;

        public QLDdatacontext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public virtual DbSet<DispatchDetail> DispatchDetails { get; set; }

        public virtual DbSet<FactoryFit> FactoryFits { get; set; }

        public virtual DbSet<FileInfo> FileInfos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FactoryFit>(entity =>
            {
                entity.ToTable("factory_fit");
                entity.HasNoKey();

            });

            modelBuilder.Entity<DispatchDetail>(entity =>
            {
                entity.ToTable("dispatch_detail");
                entity.HasNoKey();
            });

            modelBuilder.Entity<FileInfo>(entity =>
            {
                entity.ToTable("fileinfo");
                entity.HasNoKey();
            });

            OnModelCreatingPartial(modelBuilder);

        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
