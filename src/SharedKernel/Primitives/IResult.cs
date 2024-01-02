namespace SharedKernel.Primitives;

public interface IResult
{
    List<Error>? Errors { get; }

    bool IsError { get => true; }
}
