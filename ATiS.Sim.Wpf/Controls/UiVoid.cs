using System.Windows;
using System.Windows.Controls;

namespace ATiS.Sim.Wpf
{
    public class UiVoid : ContentControl
    {
        static UiVoid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UiVoid), new FrameworkPropertyMetadata(typeof(UiVoid)));
        }
    }
}
