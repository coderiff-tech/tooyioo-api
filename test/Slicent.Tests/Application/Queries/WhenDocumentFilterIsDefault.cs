using Slicent.Application.Queries;
using Slicent.Tests.Support;

namespace Slicent.Tests.Application.Queries;

public sealed class WhenDocumentFilterIsDefault
{
    [Test]
    public async Task Then_filter_matches_documents()
    {
        var filter = new TestDocumentFilter().Filter.Compile();

        var document = new TestDocument("user-1")
        {
            Alias = "jane-bloggs",
            Name = "Jane",
            Score = 10
        };

        await Assert.That(filter(document)).IsTrue();
    }

    [Test]
    public async Task Then_pagination_defaults_are_stable()
    {
        var filter = new TestDocumentFilter();

        await Assert.That(filter.PageNumber).IsEqualTo(1);
        await Assert.That(filter.PageSize).IsEqualTo(20);
    }

    [Test]
    public async Task Then_sort_defaults_to_document_id()
    {
        var filter = new TestDocumentFilter();

        await Assert.That(filter.Sort.IsEmpty).IsFalse();
        await Assert.That(filter.Sort.Definitions).Count().IsEqualTo(1);
    }

    private sealed record TestDocumentFilter
        : DocumentFilter<TestDocument>;
}
