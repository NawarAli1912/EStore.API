namespace Application.Common.Authentication.Jwt;

public interface IJwtTokenGenerator
{
    string Generate(string userId, string firstName, string lastName, string email);
}
