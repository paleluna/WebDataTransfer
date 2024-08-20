using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebDataTransfer.Models.DAL.book_excel;

public partial class bookContext : DbContext
{
    public bookContext(DbContextOptions<bookContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Book> Books { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.ToTable("Book");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BookAuthor)
                .HasMaxLength(255)
                .HasColumnName("bookAuthor");
            entity.Property(e => e.BookDataRealese)
                .HasColumnType("datetime")
                .HasColumnName("bookDataRealese");
            entity.Property(e => e.BookName)
                .HasMaxLength(255)
                .HasColumnName("bookName");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
