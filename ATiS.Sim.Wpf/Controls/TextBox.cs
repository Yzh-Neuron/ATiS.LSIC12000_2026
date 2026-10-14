using System.Windows.Input;

namespace ATiS.Sim.Wpf
{
    public class TextBox : System.Windows.Controls.TextBox
    {
        public TextBox()
        {
            CommandBindings.Add(new CommandBinding(ControlCommands.Clear, (s, e) =>
            {
                SetCurrentValue(TextProperty, string.Empty);
                GetBindingExpression(TextProperty)?.UpdateSource();
            }));
        }
    }
}
