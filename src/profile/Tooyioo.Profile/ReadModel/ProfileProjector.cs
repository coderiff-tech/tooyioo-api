using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Context;
using MongoDB.Driver;
using Tooyioo.Profile.Contracts;

namespace Tooyioo.Profile.ReadModel;

public sealed class ProfileProjector
    : MongoProjector<ProfileDocument>
{
    public ProfileProjector(IMongoDatabase database) 
        : base(database)
    {
        On<ProfileDomainEvents.V1.Created>(stream => stream.GetId(), Handle);
        On<ProfileDomainEvents.V1.EmailVerified>(stream => stream.GetId(), Handle);
        On<ProfileDomainEvents.V1.PhoneNumberSet>(stream => stream.GetId(), Handle);
        On<ProfileDomainEvents.V1.ExternalIdAssociated>(stream => stream.GetId(), Handle);
        On<ProfileDomainEvents.V1.AliasSet>(stream => stream.GetId(), Handle);
        On<ProfileDomainEvents.V1.Completed>(stream => stream.GetId(), Handle);
    }

    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.Created> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);
        
        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Name, evt.Name)
            .Set(x => x.LastName, evt.LastName)
            .Set(x => x.Email, evt.Email)
            .Set(x => x.CreatedAt, happenedAtUtc)
            .Set(x => x.LastModifiedAt, happenedAtUtc);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.EmailVerified> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);
        
        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.IsEmailVerified, true)
            .Set(x => x.LastModifiedAt, happenedAtUtc);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.PhoneNumberSet> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.PhoneNumber, evt.PhoneNumber)
            .Set(x => x.LastModifiedAt, happenedAtUtc);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.ExternalIdAssociated> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.ExternalId, evt.ExternalId)
            .Set(x => x.ExternalIdProvider, evt.ExternalIdProvider)
            .Set(x => x.LastModifiedAt, happenedAtUtc);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.AliasSet> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Alias, evt.Alias)
            .Set(x => x.LastModifiedAt, happenedAtUtc);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.Completed> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var happenedAtUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.IsComplete, true)
            .Set(x => x.CompletedAt, happenedAtUtc)
            .Set(x => x.LastModifiedAt, happenedAtUtc);
    }
}