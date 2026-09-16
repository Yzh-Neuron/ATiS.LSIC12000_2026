using System;

namespace ATiS.Sim.Mvvm
{
    public class MessageResult
    {
        public Type? RecipientType { get; internal set; }
        public object? Result { get; }
        public object? Tag { get; }
        private MessageResult(object? result, object? tag)
        {
            Result = result;
            Tag = tag;
        }
        public static MessageResult From(object? result)
        {
            return new MessageResult(result, null);
        }

        public static MessageResult From(object? result, object? tag)
        {
            return new MessageResult(result, tag);
        }
    }
}
