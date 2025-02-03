using Newtonsoft.Json;
using Xunit;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using FluentAssertions;
using Desnz.Chmm.CreditLedger.Api;
using Desnz.Chmm.Testing.Common;
using static Desnz.Chmm.CreditLedger.ApiTests.Endpoints;
using Microsoft.AspNetCore.Http;

namespace Desnz.Chmm.CreditLedger.ApiTests;

public class DummyControllerTests : IntegrationTestsBase<LocalEntryPoint>
{
    [Fact(Skip = "TODO - DELETE")]
    public async Task Get_dummy_data_returns_200()
    {
        await CheckGet(Get.Dummy, StatusCodes.Status200OK, new Dummy[]
        {
            new Dummy(1, "AAA"),
            new Dummy(2, "BBB"),
            new Dummy(3, "CCC")
        });
    }
    public record Dummy(int Id, string Name);
}
