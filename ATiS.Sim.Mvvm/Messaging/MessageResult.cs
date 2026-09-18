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
        public static MessageResult From(object? result, object? tag = null)
        {
            return new MessageResult(result, tag);
        }
        public static MessageResult Null()
        {
            return new MessageResult(null, null);
        }
    }
}
