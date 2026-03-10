using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Context;
using MongoDB.Driver;
using Tooyioo.Profile.Contracts;

namespace Tooyioo.Profile.ReadModel;

public sealed class ProfileProjector
    : MongoProjector<ProfileDocument>
{
    public ProfileProjector(
        IMongoDatabase database) 
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
        
        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Name, evt.Name)
            .Set(x => x.LastName, evt.LastName)
            .Set(x => x.Email, evt.Email);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.EmailVerified> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.IsEmailVerified, true);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.PhoneNumberSet> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.PhoneNumber, evt.PhoneNumber);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.ExternalIdAssociated> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.ExternalId, evt.ExternalId)
            .Set(x => x.ExternalIdProvider, evt.ExternalIdProvider);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.AliasSet> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Alias, evt.Alias);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<ProfileDomainEvents.V1.Completed> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var createdUtc = DateTime.SpecifyKind(ctx.Created, DateTimeKind.Utc);

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.IsComplete, true)
            .Set(x => x.CompletedAt, createdUtc);
    }
}