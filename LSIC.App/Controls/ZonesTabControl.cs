using System.Windows.Controls;

namespace LSIC.App.Controls
{
    [TemplatePart(Name = "PART_PrevButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_NextButton", Type = typeof(Button))]
    public class ZonesTabControl : TabControl
    {
        public static readonly DependencyProperty AllItemsProperty =
            DependencyProperty.Register(nameof(AllItems), typeof(IEnumerable), typeof(ZonesTabControl), new PropertyMetadata(null, OnAllItemsChanged));
        public IEnumerable AllItems
        {
            get => (IEnumerable)GetValue(AllItemsProperty);
            set => SetValue(AllItemsProperty, value);
        }
        private static void OnAllItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZonesTabControl ctrl)
            {
                ctrl._pageIndex = 0;
                ctrl.ApplyPage();
                ctrl.UpdatePager();
            }
        }

        public const int DefaultPageSize = 16;
        public int PageSize { get; set; } = DefaultPageSize;
        private Button _prev;
        private Button _next;
        private int _pageIndex;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_prev != null)
            {
                _prev.Click -= OnPrev;
            }
            if (_next != null)
            {
                _next.Click -= OnNext;
            }

            _prev = GetTemplateChild("PART_PrevButton") as Button;
            _next = GetTemplateChild("PART_NextButton") as Button;

            if (_prev != null)
            {
                _prev.Click += OnPrev;
            }
            if (_next != null)
            {
                _next.Click += OnNext;
            }

            ApplyPage();
            UpdatePager();
        }
        private void OnPrev(object sender, RoutedEventArgs e)
        {
            if (_pageIndex <= 0)
            {
                return;
            }

            _pageIndex--;
            ApplyPage();
            UpdatePager();
        }
        private void OnNext(object sender, RoutedEventArgs e)
        {
            _pageIndex++;
            ApplyPage();
            UpdatePager();
        }
        private void ApplyPage()
        {
            var source = AllItems?.Cast<object>().ToList() ?? new List<object>();
            int total = source.Count;
            int totalPages = Math.Max(1, (total + PageSize - 1) / PageSize);

            if (_pageIndex < 0)
            {
                _pageIndex = 0;
            }
            if (_pageIndex >= totalPages)
            {
                _pageIndex = totalPages - 1;
            }

            int start = _pageIndex * PageSize;
            var page = source.Skip(start).Take(PageSize).ToList();
            SetCurrentValue(ItemsSourceProperty, page);
            SetCurrentValue(SelectedItemProperty, page.FirstOrDefault());
        }
        private void UpdatePager()
        {
            var source = AllItems?.Cast<object>().ToList() ?? new List<object>();
            int total = source.Count;
            int totalPages = Math.Max(1, (total + PageSize - 1) / PageSize);
            bool multi = totalPages > 1;

            if (_prev != null)
            {
                _prev.Visibility = multi ? Visibility.Visible : Visibility.Collapsed;
                _prev.IsEnabled = _pageIndex > 0;
            }

            if (_next != null)
            {
                _next.Visibility = multi ? Visibility.Visible : Visibility.Collapsed;
                _next.IsEnabled = _pageIndex < totalPages - 1;
            }
        }
    }
}