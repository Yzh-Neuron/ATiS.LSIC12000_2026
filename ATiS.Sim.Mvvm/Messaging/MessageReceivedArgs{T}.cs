using System;

namespace ATiS.Sim.Mvvm
{
    public class MessageReceivedArgs<T> : EventArgs
    {
        public T Message { get; }

        public object[] Args { get; }

        public MessageReceivedArgs(T message, object[] args )
        {
            Message = message;
            Args = args;
        }
    }
}
