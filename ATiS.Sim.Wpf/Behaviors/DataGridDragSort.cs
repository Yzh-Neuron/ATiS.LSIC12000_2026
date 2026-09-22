using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    public static class DataGridDragSort
    {
        private static readonly DependencyProperty AdornerProperty =
            DependencyProperty.RegisterAttached("Adorner", typeof(DragInsertAdorner), typeof(DataGridDragSort), new PropertyMetadata(null));
        private static readonly DependencyProperty DragStartProperty =
            DependencyProperty.RegisterAttached("DragStart", typeof(Point?), typeof(DataGridDragSort), new PropertyMetadata(null));
        private static readonly DependencyProperty IsSubscribedProperty =
            DependencyProperty.RegisterAttached("IsSubscribed", typeof(bool), typeof(DataGridDragSort), new PropertyMetadata(ValueBoxes.FalseBox));
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DataGridDragSort), new PropertyMetadata(ValueBoxes.FalseBox, OnIsEnabledChanged));
        public static bool GetIsEnabled(DependencyObject o) => (bool)o.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject o, bool v) => o.SetValue(IsEnabledProperty, v);
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid)
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                grid.Loaded += OnLoaded;
                grid.Unloaded += OnUnloaded;
                if (grid.IsLoaded)
                {
                    Attach(grid);
                }
            }
            else
            {
                grid.Loaded -= OnLoaded;
                grid.Unloaded -= OnUnloaded;
                Detach(grid);
            }
        }
        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid g && (bool)g.GetValue(IsEnabledProperty))
            {
                Attach(g);
            }
        }
        private static void OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (sender is DataGrid g)
            {
                Detach(grid: g);
            }
        }
        private static void Attach(DataGrid grid)
        {
            if ((bool)grid.GetValue(IsSubscribedProperty))
            {
                return;
            }

            grid.SetValue(IsSubscribedProperty, ValueBoxes.TrueBox);
            grid.PreviewMouseLeftButtonDown += OnMouseDown;
            grid.PreviewMouseMove += OnMouseMove;
            grid.PreviewDragOver += OnDragOver;
            grid.Drop += OnDrop;
            grid.AllowDrop = true;
        }
        private static void Detach(DataGrid grid)
        {
            if (!(bool)grid.GetValue(IsSubscribedProperty))
            {
                return;
            }

            grid.SetValue(IsSubscribedProperty, false);
            ClearAdorner(grid);
            grid.PreviewMouseLeftButtonDown -= OnMouseDown;
            grid.PreviewMouseMove -= OnMouseMove;
            grid.PreviewDragOver -= OnDragOver;
            grid.Drop -= OnDrop;
            grid.AllowDrop = false;
        }
        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGrid grid)
            {
                return;
            }
            if (e.OriginalSource is not DependencyObject src)
            {
                return;
            }

            DataGridRow? row = src.GetParent<DataGridRow>();
            if (row == null || row.IsEditing)
            {
                return;
            }
            row.SetValue(DragStartProperty, e.GetPosition(grid));
        }
        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not DataGrid grid)
            {
                return;
            }

            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            if (e.OriginalSource is not DependencyObject src)
            {
                return;
            }

            DataGridRow? row = src.GetParent<DataGridRow>();
            if (row?.GetValue(DragStartProperty) is not Point start)
            {
                return;
            }

            Point cur = e.GetPosition(grid);
            if (Math.Abs(cur.Y - start.Y) < 1 && Math.Abs(cur.X - start.X) < 1)
            {
                return;
            }

            row.ClearValue(DragStartProperty);

            if (grid.ItemsSource is not IList list || list.IsReadOnly || list.Count <= 1)
            {
                return;
            }

            DragDrop.DoDragDrop(row, new DataObject(typeof(DataGridRow), row), DragDropEffects.Move);
        }
        private static void OnDragOver(object sender, DragEventArgs e)
        {
            if (sender is not DataGrid grid)
            {
                return;
            }

            if (!e.Data.GetDataPresent(typeof(DataGridRow)) || grid.ItemsSource is not IList list || list.IsReadOnly)
            {
                e.Effects = DragDropEffects.None;
                ClearAdorner(grid);
                e.Handled = true;
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;

            DropSpot spot = ResolveDropSpot(grid, list, e.GetPosition(grid));
            if (spot.Anchor == null)
            {
                ClearAdorner(grid);
                return;
            }

            if (grid.GetValue(AdornerProperty) is not DragInsertAdorner adorner)
            {
                adorner = new DragInsertAdorner(grid);
                AdornerLayer.GetAdornerLayer(grid)?.Add(adorner);
                grid.SetValue(AdornerProperty, adorner);
            }
            adorner.LineY = spot.LineY;
        }
        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is not DataGrid grid)
            {
                return;
            }
            ClearAdorner(grid);

            if (e.Data.GetData(typeof(DataGridRow)) is not DataGridRow sourceRow)
            {
                return;
            }
            if (sourceRow.Item == null)
            {
                return;
            }
            if (grid.ItemsSource is not IList list || list.IsReadOnly)
            {
                return;
            }

            DropSpot spot = ResolveDropSpot(grid, list, e.GetPosition(grid));
            if (spot.Anchor == null)
            {
                return;
            }

            int oldIndex = list.IndexOf(sourceRow.Item);
            int count = list.Count;
            int newIndex;

            if (spot.Below)
            {
                newIndex = count - 1;
            }
            else
            {
                int targetIndex = grid.ItemContainerGenerator.IndexFromContainer(spot.Anchor);
                newIndex = targetIndex;
                if (oldIndex < targetIndex)
                {
                    newIndex--;
                }
            }

            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex)
            {
                return;
            }

            MethodInfo? move = list.GetType().GetMethod(nameof(ObservableCollection<object>.Move), BindingFlags.Public | BindingFlags.Instance, null, [typeof(int), typeof(int)], null);

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

            e.Handled = true;
        }
        private static DropSpot ResolveDropSpot(DataGrid grid, IList list, Point pt)
        {
            int count = list.Count;
            if (count == 0)
            {
                return default;
            }

            DataGridRow? lastRow = null;
            double? lastBottom = 0;

            for (int i = 0; i < count; i++)
            {
                if (grid.ItemContainerGenerator.ContainerFromIndex(i) is not DataGridRow row)
                {
                    continue;
                }

                lastRow = row;
                double top = row.TranslatePoint(new Point(0, 0), grid).Y;
                double height = row.RenderSize.Height;
                lastBottom = top + height;

                if (pt.Y < lastBottom)
                {
                    return new DropSpot(row, false, top);
                }
            }

            if (lastRow != null && lastBottom != null)
            {
                return new DropSpot(lastRow, true, lastBottom.Value);
            }
            return default;
        }
        private static void ClearAdorner(DataGrid grid)
        {
            if (grid.GetValue(AdornerProperty) is DragInsertAdorner a)
            {
                AdornerLayer.GetAdornerLayer(grid)?.Remove(a);
                grid.ClearValue(AdornerProperty);
            }
        }
        private readonly record struct DropSpot(DataGridRow? Anchor, bool Below, double LineY);
    }

    public sealed class DragInsertAdorner : Adorner
    {
        private static readonly Pen Pen = CreatePen();
        private double _lineY;

        public DragInsertAdorner(UIElement adornedElement) : base(adornedElement)
        {
            IsHitTestVisible = false;
        }

        public double LineY
        {
            get => _lineY;
            set
            {
                if (Math.Abs(_lineY - value) > 0.01)
                {
                    _lineY = value;
                    InvalidateVisual();
                }
            }
        }

        private static Pen CreatePen()
        {
            //ForegroundBrush PrimaryBrush
            Brush brush = (Application.Current.TryFindResource("ForegroundBrush") as Brush) ?? new SolidColorBrush(Colors.Black);
            var pen = new Pen(brush, 1);
            return pen;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double width = AdornedElement.RenderSize.Width;
            drawingContext.DrawLine(Pen, new Point(0, _lineY + 0.5), new Point(width, _lineY + 0.5));
        }
    }
}
