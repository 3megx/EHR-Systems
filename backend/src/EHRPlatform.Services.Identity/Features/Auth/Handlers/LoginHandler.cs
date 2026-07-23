using MediatR;
using EHRPlatform.Services.Identity.Features.Auth.Commands;

namespace EHRPlatform.Services.Identity.Features.Auth.Handlers;

/// <summary>
/// Handler for LoginCommand.
/// </summary>
public class LoginHandler : IRequestHandler<LoginCommand, object>
{
    public Task<object> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Implementation would go here
        throw new NotImplementedException();
    }
}
