using System.Linq.Expressions;
using Slicent.Tests.Support;

namespace Slicent.Tests.Support;

public sealed class WhenExpressionsAreCombined
{
    [Test]
    public async Task Then_and_also_requires_both_expressions_to_match()
    {
        Expression<Func<TestDocument, bool>> hasAlias = x => x.Alias.Contains("jane");
        Expression<Func<TestDocument, bool>> hasEnoughScore = y => y.Score >= 10;

        var filter = hasAlias.AndAlso(hasEnoughScore).Compile();

        await Assert.That(filter(Document(alias: "jane-bloggs", score: 10))).IsTrue();
        await Assert.That(filter(Document(alias: "jane-bloggs", score: 9))).IsFalse();
        await Assert.That(filter(Document(alias: "john-bloggs", score: 10))).IsFalse();
    }

    [Test]
    public async Task Then_or_else_requires_either_expression_to_match()
    {
        Expression<Func<TestDocument, bool>> hasAlias = x => x.Alias.Contains("jane");
        Expression<Func<TestDocument, bool>> hasEnoughScore = y => y.Score >= 10;

        var filter = hasAlias.OrElse(hasEnoughScore).Compile();

        await Assert.That(filter(Document(alias: "jane-bloggs", score: 1))).IsTrue();
        await Assert.That(filter(Document(alias: "john-bloggs", score: 10))).IsTrue();
        await Assert.That(filter(Document(alias: "john-bloggs", score: 9))).IsFalse();
    }

    private static TestDocument Document(string alias, int score)
        => new("user-1")
        {
            Alias = alias,
            Name = "Jane",
            Score = score
        };
}
