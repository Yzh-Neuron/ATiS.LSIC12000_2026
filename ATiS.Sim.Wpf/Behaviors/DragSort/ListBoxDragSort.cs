using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace ATiS.Sim.Wpf
{
    public static class ListBoxDragSort
    {
        private static readonly DependencyProperty AdornerProperty =
            DependencyProperty.RegisterAttached("Adorner", typeof(DragInsertAdorner), typeof(ListBoxDragSort), new PropertyMetadata(null));
        private static readonly DependencyProperty DragStartProperty =
            DependencyProperty.RegisterAttached("DragStart", typeof(Point?), typeof(ListBoxDragSort), new PropertyMetadata(null));
        private static readonly DependencyProperty IsSubscribedProperty =
            DependencyProperty.RegisterAttached("IsSubscribed", typeof(bool), typeof(ListBoxDragSort), new PropertyMetadata(ValueBoxes.FalseBox));

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(ListBoxDragSort), new PropertyMetadata(ValueBoxes.FalseBox, OnIsEnabledChanged));
        public static bool GetIsEnabled(DependencyObject o) => (bool)o.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject o, bool v) => o.SetValue(IsEnabledProperty, ValueBoxes.BooleanBox(v));

        #region 拖拽命令
        public static readonly DependencyProperty CanDragCommandProperty =
            DependencyProperty.RegisterAttached("CanDragCommand", typeof(ICommand), typeof(ListBoxDragSort), new PropertyMetadata(null));
        public static ICommand GetCanDragCommand(DependencyObject o) => (ICommand)o.GetValue(CanDragCommandProperty);
        public static void SetCanDragCommand(DependencyObject o, ICommand v) => o.SetValue(CanDragCommandProperty, v);

        public static readonly DependencyProperty CanDropCommandProperty =
            DependencyProperty.RegisterAttached("CanDropCommand", typeof(ICommand), typeof(ListBoxDragSort), new PropertyMetadata(null));
        public static ICommand GetCanDropCommand(DependencyObject o) => (ICommand)o.GetValue(CanDropCommandProperty);
        public static void SetCanDropCommand(DependencyObject o, ICommand v) => o.SetValue(CanDropCommandProperty, v);

        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached("DropCommand", typeof(ICommand), typeof(ListBoxDragSort), new PropertyMetadata(null));
        public static ICommand GetDropCommand(DependencyObject o) => (ICommand)o.GetValue(DropCommandProperty);
        public static void SetDropCommand(DependencyObject o, ICommand v) => o.SetValue(DropCommandProperty, v);
        #endregion

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ListBox listBox)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                listBox.Loaded += OnLoaded;
                listBox.Unloaded += OnUnloaded;
                if (listBox.IsLoaded)
                {
                    Attach(listBox);
                }
            }
            else
            {
                listBox.Loaded -= OnLoaded;
                listBox.Unloaded -= OnUnloaded;
                Detach(listBox);
            }
        }
        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListBox lb && (bool)lb.GetValue(IsEnabledProperty))
            {
                Attach(lb);
            }
        }
        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ListBox lb)
            {
                Detach(lb);
            }
        }
        private static void Attach(ListBox listBox)
        {
            if ((bool)listBox.GetValue(IsSubscribedProperty))
            {
                return;
            }

            listBox.SetValue(IsSubscribedProperty, ValueBoxes.TrueBox);
            listBox.PreviewMouseLeftButtonDown += OnMouseDown;
            listBox.PreviewMouseMove += OnMouseMove;
            listBox.PreviewDragOver += OnDragOver;
            listBox.Drop += OnDrop;
            listBox.AllowDrop = true;
        }
        private static void Detach(ListBox listBox)
        {
            if (!(bool)listBox.GetValue(IsSubscribedProperty))
            {
                return;
            }

            listBox.SetValue(IsSubscribedProperty, ValueBoxes.FalseBox);
            ClearAdorner(listBox);
            listBox.PreviewMouseLeftButtonDown -= OnMouseDown;
            listBox.PreviewMouseMove -= OnMouseMove;
            listBox.PreviewDragOver -= OnDragOver;
            listBox.Drop -= OnDrop;
            listBox.AllowDrop = false;
        }
        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not ListBox listBox || e.OriginalSource is not DependencyObject src)
            {
                return;
            }

            ListBoxItem? item = src.GetParent<ListBoxItem>();
            if (item == null)
            {
                return;
            }

            item.SetValue(DragStartProperty, e.GetPosition(listBox));
        }
        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not ListBox listBox
                || e.LeftButton != MouseButtonState.Pressed
                || e.OriginalSource is not DependencyObject src)
            {
                return;
            }

            ListBoxItem? item = src.GetParent<ListBoxItem>();
            if (item?.GetValue(DragStartProperty) is not Point start)
            {
                return;
            }

            Point cur = e.GetPosition(listBox);
            if (Math.Abs(cur.Y - start.Y) < 1 && Math.Abs(cur.X - start.X) < 1)
            {
                return;
            }

            item.ClearValue(DragStartProperty);

            if (listBox.ItemsSource is not IList list || list.IsReadOnly || list.Count <= 1)
            {
                return;
            }

            if (item.DataContext is null)
            {
                return;
            }

            ICommand? canDrag = GetCanDragCommand(listBox);
            if (canDrag != null && !canDrag.CanExecute(item.DataContext))
            {
                return;
            }

            DragDrop.DoDragDrop(item, new DataObject(typeof(ListBoxItem), item), DragDropEffects.Move);
        }
        private static void OnDragOver(object sender, DragEventArgs e)
        {
            if (sender is not ListBox listBox)
            {
                return;
            }

            if (!e.Data.GetDataPresent(typeof(ListBoxItem))
                || listBox.ItemsSource is not IList list
                || list.IsReadOnly)
            {
                DenyDrop(listBox, e);
                return;
            }

            if (e.Data.GetData(typeof(ListBoxItem)) is not ListBoxItem sourceContainer)
            {
                DenyDrop(listBox, e);
                return;
            }

            DropSpot spot = ResolveDropSpot(listBox, list, e.GetPosition(listBox));
            if (spot.Anchor == null)
            {
                DenyDrop(listBox, e);
                return;
            }

            int oldIndex = listBox.ItemContainerGenerator.IndexFromContainer(sourceContainer);
            int newIndex = ComputeNewIndex(listBox, list, spot, oldIndex);

            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
            {
                DenyDrop(listBox, e);
                return;
            }

            var args = new DragDropArgs(list[oldIndex]!, oldIndex, newIndex);

            ICommand? canDrop = GetCanDropCommand(listBox);
            if (canDrop != null && !canDrop.CanExecute(args))
            {
                DenyDrop(listBox, e);
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;

            if (listBox.GetValue(AdornerProperty) is not DragInsertAdorner adorner)
            {
                adorner = new DragInsertAdorner(listBox);
                AdornerLayer.GetAdornerLayer(listBox)?.Add(adorner);
                listBox.SetValue(AdornerProperty, adorner);
            }
            adorner.LineY = spot.LineY;
        }
        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is not ListBox listBox)
            {
                return;
            }

            ClearAdorner(listBox);

            if (e.Data.GetData(typeof(ListBoxItem)) is not ListBoxItem sourceContainer
                || sourceContainer.DataContext is null
                || listBox.ItemsSource is not IList list
                || list.IsReadOnly)
            {
                return;
            }

            DropSpot spot = ResolveDropSpot(listBox, list, e.GetPosition(listBox));
            if (spot.Anchor == null)
            {
                return;
            }

            int oldIndex = listBox.ItemContainerGenerator.IndexFromContainer(sourceContainer);
            int newIndex = ComputeNewIndex(listBox, list, spot, oldIndex);

            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
            {
                return;
            }

            var args = new DragDropArgs(list[oldIndex]!, oldIndex, newIndex);

            ICommand? canDrop = GetCanDropCommand(listBox);
            if (canDrop != null && !canDrop.CanExecute(args))
            {
                return;
            }

            ICommand? drop = GetDropCommand(listBox);
            if (drop != null)
            {
                if (drop.CanExecute(args))
                {
                    drop.Execute(args);
                }
            }
            else
            {
                DefaultMove(list, oldIndex, newIndex);
            }

            e.Handled = true;
        }
        private static void DenyDrop(ListBox listBox, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            ClearAdorner(listBox);
            e.Handled = true;
        }
        private static int ComputeNewIndex(ListBox listBox, IList list, DropSpot spot, int oldIndex)
        {
            if (spot.Below)
            {
                return list.Count - 1;
            }

            int targetIndex = listBox.ItemContainerGenerator.IndexFromContainer(spot.Anchor);
            int newIndex = targetIndex;
            if (oldIndex < targetIndex)
            {
                newIndex--;
            }
            return newIndex;
        }
        private static void DefaultMove(IList list, int oldIndex, int newIndex)
        {
            MethodInfo? move = list.GetType().GetMethod(
                nameof(ObservableCollection<object>.Move),
                BindingFlags.Public | BindingFlags.Instance,
                null, [typeof(int), typeof(int)], null);

            if (move != null)
            {
                move.Invoke(list, [oldIndex, newIndex]);
            }
            else
            {
                object? item = list[oldIndex];
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, item);
            }
        }
        private static DropSpot ResolveDropSpot(ListBox listBox, IList list, Point pt)
        {
            int count = list.Count;
            if (count == 0)
            {
                return default;
            }

            ListBoxItem? lastItem = null;
            double? lastBottom = 0;

            for (int i = 0; i < count; i++)
            {
                if (listBox.ItemContainerGenerator.ContainerFromIndex(i) is not ListBoxItem item)
                {
                    continue;
                }

                lastItem = item;
                double top = item.TranslatePoint(new Point(0, 0), listBox).Y;
                double height = item.RenderSize.Height;
                lastBottom = top + height;

                if (pt.Y < lastBottom)
                {
                    return new DropSpot(item, false, top);
                }
            }

            if (lastItem != null && lastBottom != null)
            {
                return new DropSpot(lastItem, true, lastBottom.Value);
            }
            return default;
        }
        private static void ClearAdorner(ListBox listBox)
        {
            if (listBox.GetValue(AdornerProperty) is DragInsertAdorner adorner)
            {
                AdornerLayer.GetAdornerLayer(listBox)?.Remove(adorner);
                listBox.ClearValue(AdornerProperty);
            }
        }
        private readonly record struct DropSpot(ListBoxItem? Anchor, bool Below, double LineY);
    }
}