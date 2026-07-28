using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Context;
using MongoDB.Driver;
using Tooyioo.UserOnboarding.Contracts;

namespace Tooyioo.UserOnboarding.Features.RetrieveUserOnboarding.Support;

public sealed class UserOnboardingProjector
    : MongoProjector<UserOnboardingDocument>
{
    public UserOnboardingProjector(IMongoDatabase database) 
        : base(database)
    {
        On<UserOnboardingDomainEvents.V1.UserOnboardingInitiated>(stream => stream.GetId(), Handle);
        On<UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated>(stream => stream.GetId(), Handle);
        On<UserOnboardingDomainEvents.V1.UserEmailVerified>(stream => stream.GetId(), Handle);
        On<UserOnboardingDomainEvents.V1.UserAliasChosen>(stream => stream.GetId(), Handle);
        On<UserOnboardingDomainEvents.V1.UserOnboardingCompleted>(stream => stream.GetId(), Handle);
        On<UserOnboardingDomainEvents.V1.UserOnboardingCanceled>(stream => stream.GetId(), Handle);
    }

    private static UpdateDefinition<UserOnboardingDocument> Handle(
        IMessageConsumeContext<UserOnboardingDomainEvents.V1.UserOnboardingInitiated> ctx, 
        UpdateDefinitionBuilder<UserOnboardingDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Name, evt.Name)
            .Set(x => x.LastName, evt.LastName)
            .Set(x => x.Email, evt.Email)
            .Set(x => x.IsEmailVerified, false)
            .Set(x => x.Status, UserOnboardingStatuses.InProgress)
            .Set(x => x.CompletedAt, null)
            .Set(x => x.CanceledAt, null)
            .Set(x => x.CancellationReason, null)
            .Set(x => x.CreatedAt, happenedAtUtc)
            .Set(x => x.LastModifiedAt, happenedAtUtc)
            .Set(x => x.Revision, ctx.StreamPosition);
    }

    private static UpdateDefinition<UserOnboardingDocument> Handle(
        IMessageConsumeContext<UserOnboardingDomainEvents.V1.UserExternalIdentityAssociated> ctx,
        UpdateDefinitionBuilder<UserOnboardingDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.ExternalIdentity, new ExternalIdentityDocument
            {
                Id = evt.Id,
                Provider = evt.Provider,
                Issuer = evt.Issuer
            })
            .Set(x => x.LastModifiedAt, happenedAtUtc)
            .Set(x => x.Revision, ctx.StreamPosition);
    }
    
    private static UpdateDefinition<UserOnboardingDocument> Handle(
        IMessageConsumeContext<UserOnboardingDomainEvents.V1.UserEmailVerified> ctx, 
        UpdateDefinitionBuilder<UserOnboardingDocument> update)
    {
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);
        
        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.IsEmailVerified, true)
            .Set(x => x.LastModifiedAt, happenedAtUtc)
            .Set(x => x.Revision, ctx.StreamPosition);
    }
    
    private static UpdateDefinition<UserOnboardingDocument> Handle(
        IMessageConsumeContext<UserOnboardingDomainEvents.V1.UserAliasChosen> ctx, 
        UpdateDefinitionBuilder<UserOnboardingDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Alias, evt.Alias)
            .Set(x => x.LastModifiedAt, happenedAtUtc)
            .Set(x => x.Revision, ctx.StreamPosition);
    }

    private static UpdateDefinition<UserOnboardingDocument> Handle(
        IMessageConsumeContext<UserOnboardingDomainEvents.V1.UserOnboardingCompleted> ctx,
        UpdateDefinitionBuilder<UserOnboardingDocument> update)
    {
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Status, UserOnboardingStatuses.Completed)
            .Set(x => x.CompletedAt, happenedAtUtc)
            .Set(x => x.LastModifiedAt, happenedAtUtc)
            .Set(x => x.Revision, ctx.StreamPosition);
    }

    private static UpdateDefinition<UserOnboardingDocument> Handle(
        IMessageConsumeContext<UserOnboardingDomainEvents.V1.UserOnboardingCanceled> ctx,
        UpdateDefinitionBuilder<UserOnboardingDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Status, UserOnboardingStatuses.Canceled)
            .Set(x => x.CanceledAt, happenedAtUtc)
            .Set(x => x.CancellationReason, evt.Reason)
            .Set(x => x.LastModifiedAt, happenedAtUtc)
            .Set(x => x.Revision, ctx.StreamPosition);
    }
}
