using Desnz.Chmm.Common;
using Desnz.Chmm.Notes.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Desnz.Chmm.Notes.Api.Infrastructure;

public interface INotesContext : IUnitOfWork
{
    DbSet<ManufacturerNote> ManufacturerNotes { get; }
}
