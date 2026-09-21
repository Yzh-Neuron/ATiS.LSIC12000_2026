using System.Windows;
using System.Windows.Input;

namespace ATiS.Sim.Wpf
{
    public sealed class MessagePop : ChromeWindow
    {
        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(nameof(Message), typeof(string), typeof(MessagePop), new PropertyMetadata(default(string)));
        public MessageType Type
        {
            get { return (MessageType)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(MessageType), typeof(MessagePop), new PropertyMetadata(MessageType.Info));

        static MessagePop()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(MessagePop), new FrameworkPropertyMetadata(typeof(MessagePop)));
        }
        private MessagePop()
        {
            CommandBindings.Add(new CommandBinding(ControlCommands.Confirm, Confirm));
            CommandBindings.Add(new CommandBinding(ControlCommands.Cancel, Cancel));
        }

        private void Confirm(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = true;
        }
        private void Cancel(object sender, ExecutedRoutedEventArgs e)
        {
            DialogResult = false;
        }
        public static bool? Show(string message, MessageType messageType = MessageType.Info, Window? owner = null)
        {
            Window? ownerWindow = owner ?? WindowExtension.GetActiveWindow();
            bool ownerIsNull = ownerWindow is null;

            var pop = new MessagePop
            {
                Message = message,
                Type = messageType,
                Owner = WindowExtension.GetActiveWindow(),
                WindowStartupLocation = ownerIsNull ? WindowStartupLocation.CenterScreen : WindowStartupLocation.CenterOwner,
            };

            return pop.ShowDialog();
        }
    }
}
