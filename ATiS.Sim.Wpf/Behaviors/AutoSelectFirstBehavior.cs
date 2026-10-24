using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace ATiS.Sim.Wpf
{
    public class AutoSelectFirstBehavior : Behavior<ItemsControl>
    {
        public static readonly DependencyProperty AutoSelectFirstProperty =
            DependencyProperty.Register(nameof(AutoSelectFirst), typeof(bool), typeof(AutoSelectFirstBehavior), new PropertyMetadata(true));
        public bool AutoSelectFirst
        {
            get => (bool)GetValue(AutoSelectFirstProperty);
            set => SetValue(AutoSelectFirstProperty, value);
        }
        private ICollectionView? _collectionView;
        private bool _firstSelectionDone = false;
        protected override void OnAttached()
        {
            base.OnAttached();

            _collectionView = AssociatedObject.Items;
            if (_collectionView != null)
            {
                _collectionView.CollectionChanged += OnCollectionChanged;
            }

            if (AssociatedObject.HasItems)
            {
                if (AutoSelectFirst)
                {
                    SelectFirst();
                }
                else
                {
                    if (!_firstSelectionDone)
                    {
                        SelectFirst();
                        _firstSelectionDone = true;
                    }
                }
            }
        }
        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (AssociatedObject == null)
            {
                return;
            }

            if (AutoSelectFirst)
            {
                if (AssociatedObject.HasItems)
                {
                    SelectFirst();
                }
                else
                {
                    ClearSelection();
                }
            }
            else
            {
                if (!_firstSelectionDone && AssociatedObject.HasItems)
                {
                    SelectFirst();
                    _firstSelectionDone = true;
                }
            }
        }
        private void SelectFirst()
        {
            if (AssociatedObject is Selector selector)
            {
                selector.SelectedIndex = 0;
            }
        }
        private void ClearSelection()
        {
            if (AssociatedObject is Selector selector)
            {
                selector.SelectedIndex = -1;
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (_collectionView != null)
            {
                _collectionView.CollectionChanged -= OnCollectionChanged;
                _collectionView = null;
            }
        }
    }
}