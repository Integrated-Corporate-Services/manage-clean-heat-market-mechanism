using AutoMapper;
using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Common.Dtos;

namespace Desnz.Chmm.Notes.Api.AutoMapper;

/// <summary>
/// Automapper config for the Notes service
/// </summary>
public class NotesAutoMapperProfile : Profile
{
    /// <summary>
    /// Default Constructor
    /// </summary>
    public NotesAutoMapperProfile()
    {
        CreateMap<ManufacturerNote, ManufacturerNoteDto>();
    }
}
