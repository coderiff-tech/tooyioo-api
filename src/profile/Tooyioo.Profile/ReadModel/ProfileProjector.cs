using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Context;
using MongoDB.Driver;
using Tooiyoo.Identity.Contracts;

namespace Tooyioo.Profile.ReadModel;

public sealed class ProfileProjector
    : MongoProjector<ProfileDocument>
{
    public ProfileProjector(
        IMongoDatabase database) 
        : base(database)
    {
        On<IdentityDomainEvents.V1.Created>(stream => stream.GetId(), Handle);
        On<IdentityDomainEvents.V1.EmailVerified>(stream => stream.GetId(), Handle);
        On<IdentityDomainEvents.V1.PhoneNumberSet>(stream => stream.GetId(), Handle);
        On<IdentityDomainEvents.V1.ExternalIdentityAssociated>(stream => stream.GetId(), Handle);
        On<IdentityDomainEvents.V1.AliasSet>(stream => stream.GetId(), Handle);
    }

    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<IdentityDomainEvents.V1.Created> ctx, 
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
        IMessageConsumeContext<IdentityDomainEvents.V1.EmailVerified> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.IsEmailVerified, true);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<IdentityDomainEvents.V1.PhoneNumberSet> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.PhoneNumber, evt.PhoneNumber);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<IdentityDomainEvents.V1.ExternalIdentityAssociated> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.ExternalId, evt.ExternalId)
            .Set(x => x.ExternalProviderName, evt.ExternalProviderName);
    }
    
    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<IdentityDomainEvents.V1.AliasSet> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;

        return update
            .SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Alias, evt.Alias)
            .Set(x => x.IsProfileComplete, true);
    }
}