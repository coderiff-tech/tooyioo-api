using Eventuous.Projections.MongoDB;
using Eventuous.Subscriptions.Context;
using MongoDB.Driver;
using Tooiyoo.Identity.Contracts;

namespace Tooiyoo.Identity.ReadModel;

public sealed class ProfileProjector
    : MongoProjector<ProfileDocument>
{
    public ProfileProjector(
        IMongoDatabase database) 
        : base(database)
    {
        On<IdentityDomainEvents.V1.Created>(stream => stream.GetId(), Handle);
    }

    private static UpdateDefinition<ProfileDocument> Handle(
        IMessageConsumeContext<IdentityDomainEvents.V1.Created> ctx, 
        UpdateDefinitionBuilder<ProfileDocument> update)
    {
        var evt = ctx.Message;
        
        return update.SetOnInsert(x => x.Id, ctx.Stream.GetId())
            .Set(x => x.Name, evt.Name)
            .Set(x => x.LastName, evt.LastName)
            .Set(x => x.Email, evt.Email);
    }
}