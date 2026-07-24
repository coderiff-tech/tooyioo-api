namespace Tooyioo.Tests.Support.ReadModels;

[ClassDataSource<MongoReadModelTestContainer>(Shared = SharedType.PerAssembly)]
[ParallelLimiter<MongoReadModelParallelLimit>]
public abstract class MongoReadModelVerticalSliceGivenWhenThen(MongoReadModelTestContainer mongoDb)
    : VerticalSliceGivenWhenThen
{
    protected override async Task<VerticalSliceTestHost> CreateHost()
        => await VerticalSliceTestHost.Start(await mongoDb.CreateDatabase());
}
