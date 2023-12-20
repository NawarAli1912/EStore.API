using Application.Authentication.Common;
using Application.Authentication.Register;
using Contracts.Authentication;
using Mapster;

namespace Presentation.Common.Mapping;

public class AuthenticationMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<RegisterRequest, RegisterCommand>();

        config.NewConfig<AuthenticationResult, AuthenticationResponse>();
    }
}
