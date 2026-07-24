// ReSharper disable VirtualMemberNeverOverridden.Global
namespace Tooyioo.Tests.Support.VerticalSlices;

public abstract class VerticalSliceGivenWhenThen
{
    protected VerticalSliceTestHost Host { get; private set; } = null!;

    [Before(Test)]
    public async Task Setup()
    {
        Host = await CreateHost();
        await Given();
        await When();
    }

    [After(Test)]
    public async Task Teardown()
    {
        await Cleanup();
        await Host.DisposeAsync();
    }

    protected virtual Task<VerticalSliceTestHost> CreateHost()
        => VerticalSliceTestHost.Start();

    protected abstract Task Given();

    protected abstract Task When();

    protected virtual Task Cleanup()
        => Task.CompletedTask;
}
