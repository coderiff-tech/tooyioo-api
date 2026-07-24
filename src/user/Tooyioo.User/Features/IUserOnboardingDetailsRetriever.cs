using Funzo;
using Tooyioo.User.Features.Support;

namespace Tooyioo.User.Features;

public interface IUserOnboardingDetailsRetriever
{
    Task<Option<UserOnboardingDetails>> Retrieve(string userOnboardingId, CancellationToken cancellationToken);
}