namespace ATiS.Sim.Mvvm
{
    public class MsgArgs<T> : EventArgs
    {
        public T Message { get; }

        public object[] Args { get; }

        public MsgArgs(T message, object[] args)
        {
            Message = message;
            Args = args;
        }
    }

    public class MsgArgs
    {
        public static MsgArgs<T> From<T>(T message, object[] args)
        {
            return new MsgArgs<T>(message, args);
        }
    }
}
