using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ATiS.Sim.Wpf
{
    public static class DataGridDragSort
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(DataGridDragSort),new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject o) => (bool)o.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject o, bool v) => o.SetValue(IsEnabledProperty, v);

        private static readonly DependencyProperty DragStartProperty =
            DependencyProperty.RegisterAttached("DragStart", typeof(Point?), typeof(DataGridDragSort), new PropertyMetadata(null));

        private static readonly DependencyProperty HighlightRowProperty =
            DependencyProperty.RegisterAttached("HighlightRow", typeof(DataGridRow), typeof(DataGridDragSort), new PropertyMetadata(null, OnHighlightRowChanged));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DataGrid grid) return;

            if ((bool)e.NewValue)
            {
                grid.PreviewMouseLeftButtonDown += OnMouseDown;
                grid.PreviewMouseMove += OnMouseMove;
                grid.PreviewDragOver += OnDragOver;
                grid.Drop += OnDrop;
                grid.AllowDrop = true;
            }
            else
            {
                grid.PreviewMouseLeftButtonDown -= OnMouseDown;
                grid.PreviewMouseMove -= OnMouseMove;
                grid.PreviewDragOver -= OnDragOver;
                grid.Drop -= OnDrop;
            }
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not DataGrid grid) return;
            if (e.OriginalSource is not DependencyObject src) return;

            DataGridRow? row = FindVisualParent<DataGridRow>(src);
            // 只在行上、且不在编辑态时启动拖拽
            if (row == null || row.IsEditing) return;

            row.SetValue(DragStartProperty, e.GetPosition(grid));
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (sender is not DataGrid grid) return;
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (e.OriginalSource is not DependencyObject src) return;

            DataGridRow? row = FindVisualParent<DataGridRow>(src);
            if (row?.GetValue(DragStartProperty) is not Point start) return;

            Point cur = e.GetPosition(grid);
            if (Math.Abs(cur.Y - start.Y) < SystemParameters.MinimumVerticalDragDistance &&
                Math.Abs(cur.X - start.X) < SystemParameters.MinimumHorizontalDragDistance)
                return; // 未超过系统拖拽阈值，不启动

            row.ClearValue(DragStartProperty);

            if (grid.ItemsSource is not IList list || list.IsReadOnly) return;

            DragDrop.DoDragDrop(row, new DataObject(typeof(DataGridRow), row), DragDropEffects.Move);
        }

        private static void OnDragOver(object sender, DragEventArgs e)
        {
            if (sender is not DataGrid grid) return;
            if (!e.Data.GetDataPresent(typeof(DataGridRow)))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            e.Effects = DragDropEffects.Move;
            e.Handled = true;

            // 简单高亮当前目标行
            DataGridRow? target = FindRowAtPoint(grid, e.GetPosition(grid));
            grid.SetValue(HighlightRowProperty, target);
        }

        private static void OnDrop(object sender, DragEventArgs e)
        {
            if (sender is not DataGrid grid) return;
            grid.ClearValue(HighlightRowProperty);

            if (e.Data.GetData(typeof(DataGridRow)) is not DataGridRow sourceRow) return;
            if (sourceRow.Item == null) return;
            if (grid.ItemsSource is not IList list) return;

            DataGridRow? targetRow = FindRowAtPoint(grid, e.GetPosition(grid));

            int oldIndex = list.IndexOf(sourceRow.Item);
            int newIndex;

            if (targetRow?.Item != null)
            {
                newIndex = list.IndexOf(targetRow.Item);
            }
            else
            {
                // 拖到末尾空白处 → 追加到最后
                newIndex = list.Count - 1;
            }

            if (oldIndex < 0 || newIndex < 0 || oldIndex == newIndex) return;

            MethodInfo? move = list.GetType().GetMethod(
                nameof(ObservableCollection<object>.Move),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(int) },
                null);

            if (move != null)
            {
                move.Invoke(list, [oldIndex, newIndex]);
            }
            else
            {
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, sourceRow.Item);
            }

            e.Handled = true;
        }

        private static void OnHighlightRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is DataGridRow oldRow)
                oldRow.Background = MakeBackground(oldRow, false);
            if (e.NewValue is DataGridRow newRow)
                newRow.Background = MakeBackground(newRow, true);
        }

        private static Brush? MakeBackground(DataGridRow row, bool highlight)
        {
            // 简单做法：用系统高亮色；想保留原背景可缓存原值再还原
            return highlight
                ? new SolidColorBrush(Color.FromArgb(0x60, 0x21, 0x96, 0xF3))
                : null; // null 让模板样式接管
        }

        private static DataGridRow? FindRowAtPoint(DataGrid grid, Point pt)
        {
            HitTestResult hit = VisualTreeHelper.HitTest(grid, pt);
            return FindVisualParent<DataGridRow>(hit?.VisualHit);
        }

        private static T? FindVisualParent<T>(DependencyObject? cur) where T : DependencyObject
        {
            while (cur != null)
            {
                if (cur is T t) return t;
                cur = VisualTreeHelper.GetParent(cur);
            }
            return null;
        }
    }
}