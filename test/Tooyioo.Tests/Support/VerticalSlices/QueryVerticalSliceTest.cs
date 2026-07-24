using Tooyioo.Tests.Support.ReadModels;

namespace Tooyioo.Tests.Support.VerticalSlices;

[ClassDataSource<MongoTestContainer>(Shared = SharedType.PerAssembly)]
[ParallelLimiter<MongoReadModelParallelLimit>]
public abstract class QueryVerticalSliceTest(MongoTestContainer mongoDb)
    : VerticalSliceGivenWhenThen
{
    protected override async Task<VerticalSliceTestHost> CreateHost()
        => await VerticalSliceTestHost.Start(await mongoDb.CreateDatabase());
}
