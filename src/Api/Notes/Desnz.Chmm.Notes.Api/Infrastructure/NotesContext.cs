using Desnz.Chmm.Common;
using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Api.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Notes.Api.Infrastructure;

/// <summary>
/// The DB Context for notes
/// </summary>
public class NotesContext : DbContext, INotesContext
{
    /// <summary>
    /// Notes for a Manufacturer
    /// </summary>
    public DbSet<ManufacturerNote> ManufacturerNotes { get; set; }

    /// <summary>
    /// Constructor taking all dependencies
    /// </summary>
    /// <param name="options">The options for the Notes Context</param>
    public NotesContext(DbContextOptions<NotesContext> options) : base(options)
    {
    }

    /// <summary>
    /// Configures Elements in the schema
    /// </summary>
    /// <param name="modelBuilder">The model builder to configure elements</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("uuid-ossp");

        modelBuilder.ApplyConfiguration(new ManufacturerNoteEntityTypeConfiguration());
    }
}
