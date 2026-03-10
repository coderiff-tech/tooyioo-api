using Eventuous;

namespace Tooyioo.Profile.Contracts;

public static class UniqueExternalIdDomainEvents
{
    public static class V1
    {
        private const string Prefix = "V1.UniqueExternalId";
        
        [EventType(Prefix + "Claimed")]
        public record Claimed(string ProfileId);
    }
}