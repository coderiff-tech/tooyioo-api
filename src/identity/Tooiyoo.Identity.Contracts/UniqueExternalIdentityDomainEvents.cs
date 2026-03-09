using Eventuous;

namespace Tooiyoo.Identity.Contracts;

public static class UniqueExternalIdentityDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.UniqueExternalIdentity";
        
        [EventType(Prefix + "Claimed")]
        public record Claimed(string IdentityId);
    }
}