namespace ATiS.Sim.Wpf
{
    /// <summary>
    /// 消息类型 决定了消息弹窗的图标和默认标题
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 致命
        /// </summary>
        Fatal,
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 询问
        /// </summary>
        Question
    }
}
