using Desnz.Chmm.Common;
using Desnz.Chmm.Notes.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Desnz.Chmm.Notes.Api.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for Manufacturer Notes
    /// </summary>
    public class ManufacturerNotesRepository : IManufacturerNotesRepository
    {
        private readonly ILogger<ManufacturerNotesRepository> _logger;
        private readonly INotesContext _context;

        public IUnitOfWork UnitOfWork => _context;

        /// <summary>
        /// Constructor taking all args
        /// </summary>
        /// <param name="context">The DBContext for the repository to use</param>
        /// <param name="logger">Logger</param>
        public ManufacturerNotesRepository(INotesContext context, ILogger<ManufacturerNotesRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Return all manufacturer notes matching the condition
        /// </summary>
        /// <param name="condition">Filter manufacturer notes</param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A collection of manufacturer notes</returns>
        public async Task<List<ManufacturerNote>> GetAll(Expression<Func<ManufacturerNote, bool>>? condition = null, bool withTracking = false)
            => await Query(condition, withTracking).ToListAsync();

        /// <summary>
        /// Gets a single manufacturer note
        /// </summary>
        /// <param name="id"></param>
        /// <param name="withTracking">Should track?</param>
        /// <returns>A single licence holder</returns>
        public async Task<ManufacturerNote?> GetById(Guid id, bool withTracking = false)
            => await Query(u => u.Id == id, withTracking).SingleOrDefaultAsync();

        /// <summary>
        /// Creates a new Note for a Manufacturer
        /// </summary>
        /// <param name="manufacturerNote">The note to create</param>
        /// <param name="saveChanges">Whether to save changes</param>
        /// <returns>The ID of the newly created note</returns>
        public async Task<Guid> Create(ManufacturerNote manufacturerNote, bool saveChanges = true)
        {
            await _context.ManufacturerNotes.AddAsync(manufacturerNote);
            if (saveChanges)
            {
                await _context.SaveChangesAsync();
            }
            return manufacturerNote.Id;
        }

        private IQueryable<ManufacturerNote> Query(Expression<Func<ManufacturerNote, bool>>? condition = null, bool withTracking = false)
        {
            var query = (condition == null) ?
                _context.ManufacturerNotes :
                _context.ManufacturerNotes.Where(condition);

            if (!withTracking)
            {
                query = query.AsNoTracking();
            }

            return query;
        }
    }
}
