using Microsoft.Xaml.Behaviors;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Controls;

namespace ATiS.Sim.Wpf
{
    public class ListBoxScrollToBottomBehavior : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            ((ICollectionView)AssociatedObject.Items).CollectionChanged += OnListBoxCollectionChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();

            ((ICollectionView)AssociatedObject.Items).CollectionChanged -= OnListBoxCollectionChanged;
        }
        private void OnListBoxCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (AssociatedObject.HasItems)
            {
                AssociatedObject.ScrollIntoView(AssociatedObject.Items[AssociatedObject.Items.Count - 1]);
            }
        }
    }
}
