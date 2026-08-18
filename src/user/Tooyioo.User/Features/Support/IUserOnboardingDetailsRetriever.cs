using Funzo;

namespace Tooyioo.User.Features.Support;

public interface IUserOnboardingDetailsRetriever
{
    Task<Option<UserOnboardingDetails>> Retrieve(string userOnboardingId, CancellationToken cancellationToken);
}