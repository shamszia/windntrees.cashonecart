using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace DataAccessNET5.Models
{
    public partial class ApplicationContext : DbContext
    {
        public ApplicationContext()
        {
        }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Codsale> Codsales { get; set; }
        public virtual DbSet<Color> Colors { get; set; }
        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<CompanyType> CompanyTypes { get; set; }
        public virtual DbSet<Expense> Expenses { get; set; }
        public virtual DbSet<ExpenseType> ExpenseTypes { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<OrderStatusType> OrderStatusTypes { get; set; }
        public virtual DbSet<OrderType> OrderTypes { get; set; }
        public virtual DbSet<Place> Places { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<SalesPayment> SalesPayments { get; set; }
        public virtual DbSet<SalesTransaction> SalesTransactions { get; set; }
        public virtual DbSet<ServiceType> ServiceTypes { get; set; }
        public virtual DbSet<StockPayment> StockPayments { get; set; }
        public virtual DbSet<StockPurchased> StockPurchaseds { get; set; }
        public virtual DbSet<StockSold> StockSolds { get; set; }
        public virtual DbSet<StockTransaction> StockTransactions { get; set; }
        public virtual DbSet<Unit> Units { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<City>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Codsale>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<CompanyType>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.ExpenseNavigation)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.ExpenseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Expense_ExpenseType");

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.Expenses)
                    .HasForeignKey(d => d.PlaceId)
                    .HasConstraintName("FK_Expense_Place");
            });

            modelBuilder.Entity<ExpenseType>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.OrderNo).ValueGeneratedOnAdd();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.ItemId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderItem_Product");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_OrderItem_Order");
            });

            modelBuilder.Entity<OrderStatusType>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<OrderType>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<SalesPayment>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK_ExportPayment");

                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.SalesPayments)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_SalesPayment_SalesTransaction");
            });

            modelBuilder.Entity<SalesTransaction>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK_dm_SaleTransaction");

                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RefNumber).ValueGeneratedOnAdd();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Buyer)
                    .WithMany(p => p.SalesTransactions)
                    .HasForeignKey(d => d.BuyerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SalesTransaction_Company");
            });

            modelBuilder.Entity<ServiceType>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            modelBuilder.Entity<StockPayment>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Transaction)
                    .WithMany(p => p.StockPayments)
                    .HasForeignKey(d => d.TransactionId)
                    .HasConstraintName("FK_StockPayment_StockTransaction");
            });

            modelBuilder.Entity<StockPurchased>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.StockPurchaseds)
                    .HasForeignKey(d => d.PlaceId)
                    .HasConstraintName("FK_StockPurchased_Place");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.StockPurchaseds)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StockPurchased_Product");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.StockPurchaseds)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_StockPurchased_StockTransaction");
            });

            modelBuilder.Entity<StockSold>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK_dm_Stock");

                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Place)
                    .WithMany(p => p.StockSolds)
                    .HasForeignKey(d => d.PlaceId)
                    .HasConstraintName("FK_StockSold_Place");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.StockSolds)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StockSold_Product");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.StockSolds)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_StockSold_SalesTransaction");
            });

            modelBuilder.Entity<StockTransaction>(entity =>
            {
                entity.HasKey(e => e.Uid)
                    .HasName("PK_dm_StockTransaction");

                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RefNumber).ValueGeneratedOnAdd();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.StockTransactions)
                    .HasForeignKey(d => d.SupplierId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StockTransaction_Company");
            });

            modelBuilder.Entity<Unit>(entity =>
            {
                entity.Property(e => e.Uid).ValueGeneratedNever();

                entity.Property(e => e.RowVersion)
                    .IsRowVersion()
                    .IsConcurrencyToken();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
