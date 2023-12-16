namespace SharedKernel;

public interface IResult
{
    List<Error>? Errors { get; }

    bool IsError { get; }
}
