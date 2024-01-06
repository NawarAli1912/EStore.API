namespace Application.Common.FriendlyIdentifiers;
public interface IFriendlyIdGenerator
{
    Task<List<string>> GenerateOrderFriendlyId(int count);

    Task<List<string>> GenerateProductFriendlyId(int count);
}
