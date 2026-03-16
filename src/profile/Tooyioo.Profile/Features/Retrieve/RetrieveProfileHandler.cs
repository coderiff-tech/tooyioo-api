using Eventuous;
using Slicent.Application.Queries;
using Slicent.EventStore;
using Tooyioo.Profile.Domain;
// ReSharper disable ConvertToPrimaryConstructor
// ReSharper disable UnusedType.Global
// ReSharper disable ClassNeverInstantiated.Global

namespace Tooyioo.Profile.Features.Retrieve;

public sealed class RetrieveProfileHandler
    : IQueryHandler<RetrieveProfileQuery, RetrieveProfileQueryResultOkResult>
{
    private readonly IEventReader _eventReader;

    public RetrieveProfileHandler(IEventReader eventReader)
    {
        _eventReader = eventReader;
    }

    public async Task<RetrieveProfileQueryResultOkResult> Handle(
        RetrieveProfileQuery query, 
        CancellationToken cancellationToken = default)
    {
        var profileAggregate =
            await _eventReader.LoadAggregateOrNew<ProfileAggregate, ProfileState, ProfileId>(
                query.ProfileId, cancellationToken);
        
        return new RetrieveProfileQueryResultOkResult(
            profileAggregate.State.Id,
            profileAggregate.State.Alias,
            profileAggregate.State.Email,
            profileAggregate.State.PhoneNumber,
            profileAggregate.State.IsProfileComplete);
    }
}

public sealed record RetrieveProfileQuery(ProfileId ProfileId)
    : IQuery<RetrieveProfileQueryResultOkResult>;

public sealed record RetrieveProfileQueryResultOkResult(
    ProfileId ProfileId, 
    string? Alias,
    string Email,
    string? PhoneNumber,
    bool IsComplete);