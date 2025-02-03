using Desnz.Chmm.Common;
using Desnz.Chmm.Notes.Api.Entities;
using System.Linq.Expressions;

namespace Desnz.Chmm.Notes.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Definition for for repository for getting Manufacturer Notes
    /// </summary>
    public interface IManufacturerNotesRepository : IRepository
    {
        /// <summary>
        /// Creates a Manufacturer Note
        /// </summary>
        /// <param name="manufacturerNote">The note to create</param>
        /// <param name="saveChanges">Whether to save changes</param>
        /// <returns>Returns the ID of the created Note</returns>
        Task<Guid> Create(ManufacturerNote manufacturerNote, bool saveChanges = true);
        Task<List<ManufacturerNote>> GetAll(Expression<Func<ManufacturerNote, bool>>? condition = null, bool withTracking = false);
        Task<ManufacturerNote?> GetById(Guid id, bool withTracking = false);
    }
}
