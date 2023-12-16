namespace Application.Common.Authentication;

public interface IUserIdentifierProvider
{
    Guid UserId { get; }
}
