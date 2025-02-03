using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Api.Infrastructure;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace Desnz.Chmm.Notes.UnitTests.Infrastructure.Repositories;

public class ManufacturerNotesRepositoryTests
{
    private readonly Mock<ILogger<ManufacturerNotesRepository>> logger;
    private readonly Mock<INotesContext> context;

    private readonly List<ManufacturerNote> notes;
    private readonly ManufacturerNotesRepository repository;

    public ManufacturerNotesRepositoryTests()
    {
        logger = new Mock<ILogger<ManufacturerNotesRepository>>();
        context = new Mock<INotesContext>();

        notes = new List<ManufacturerNote>
        {
            new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "ONE"),
            new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "TWO"),
            new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "TWO"),
            new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "THREE"),
            new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "THREE"),
            new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "THREE")
        };

        context.Setup(x => x.ManufacturerNotes)
            .Returns(notes.AsQueryable().BuildMockDbSet().Object);

        repository = new ManufacturerNotesRepository(context.Object, logger.Object);
    }

    [Fact]
    public async Task Query_Without_ConditionAsync()
    {
        var response = await repository.GetAll();
        Assert.Equal(6, response.Count);
    }

    [Fact]
    public async Task Query_With_ConditionAsync()
    {
        var response = await repository.GetAll(x => x.Details == "TWO");
        Assert.Equal(2, response.Count);
    }

    [Fact]
    public async Task Query_With_TrackingAsync()
    {
        var response = await repository.GetAll(x => x.Details == "ONE", withTracking: true);
        Assert.Single(response);
    }

    [Fact]
    public void Create_Without_Saving_Changes()
    {
        var note = new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "TEST");
        var response = repository.Create(note, saveChanges: false);
        context.Verify(x => x.SaveChangesAsync(default), Times.Never);
    }

    [Fact]
    public void Create_With_Saving_Changes()
    {
        var note = new ManufacturerNote(Guid.NewGuid(), Guid.NewGuid(), "TEST");
        var response = repository.Create(note, saveChanges: true);
        context.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
