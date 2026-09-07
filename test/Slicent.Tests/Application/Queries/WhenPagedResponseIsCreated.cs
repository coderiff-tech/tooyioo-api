using Slicent.Application.Queries;

namespace Slicent.Tests.Application.Queries;

public sealed class WhenPagedResponseIsCreated
{
    [Test]
    public async Task Then_pagination_metadata_and_items_are_preserved()
    {
        var response = new TestPagedResponse
        {
            PageNumber = 2,
            PageSize = 10,
            TotalPages = 3,
            TotalCount = 25,
            Items =
            [
                new TestPagedResponseItem
                {
                    Id = "user-1",
                    Alias = "jane-bloggs"
                }
            ]
        };

        await Assert.That(response.PageNumber).IsEqualTo(2);
        await Assert.That(response.PageSize).IsEqualTo(10);
        await Assert.That(response.TotalPages).IsEqualTo(3);
        await Assert.That(response.TotalCount).IsEqualTo(25);
        await Assert.That(response.Items.Single().Id).IsEqualTo("user-1");
        await Assert.That(response.Items.Single().Alias).IsEqualTo("jane-bloggs");
    }

    private sealed record TestPagedResponse
        : PagedResponse<TestPagedResponseItem>;

    private sealed record TestPagedResponseItem
    {
        public required string Id { get; init; }
        public required string Alias { get; init; }
    }
}
