using Eventuous;

namespace Tooiyoo.Identity.Contracts;

public static class UniqueAliasDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.UniqueAlias";
        
        [EventType(Prefix + "Claimed")]
        public record Claimed(string IdentityId);
    }
}