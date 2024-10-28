namespace SettingService.Entities;

public class OperationResult
{
    public bool IsSuccess { get; protected set; }

    public bool IsFail => !IsSuccess;

    public string? Message { get; protected set; }

    protected OperationResult()
    { }

    public static OperationResult Success() => new() { IsSuccess = true };

    public static OperationResult Fail(string message) => new() { IsSuccess = false, Message = message };
}

public class OperationResult<TEntity> : OperationResult
{
    public TEntity? Entity { get; protected set; }

    private OperationResult()
    { }

    public static OperationResult<TEntity> Success(TEntity entity) => new() { IsSuccess = true, Entity = entity };
    
    public static new OperationResult<TEntity> Fail(string message) => new() { IsSuccess = false, Message = message };
}