namespace Thomsen.AccTools.SharedMemory;

public delegate void UpdatedHandler<T>(object sender, UpdatedEventArgs<T> e);

public class UpdatedEventArgs<T> : EventArgs {
    public T Data { get; private set; }

    public UpdatedEventArgs(T data) => Data = data;
}
