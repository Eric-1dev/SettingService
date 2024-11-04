export interface OperationResultGeneric<T> extends OperationResult {
    entity?: T;
}

export interface OperationResult {
    isSuccess: boolean;
    message?: string;
}
