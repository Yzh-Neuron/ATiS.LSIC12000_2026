namespace LSIC.App.Models
{
    public class Progress : ObservableObject
    {
        private int _total = 0;
        public int Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }

        private int _cost = 0;
        public int Cost
        {
            get => _cost;
            set => SetProperty(ref _cost, value);
        }

        private int _left = 0;
        public int Left
        {
            get => _left;
            set => SetProperty(ref _left, value);
        }

        public void Update(int total, int cost)
        {
            Total = total;
            Cost = cost;
            Left = Math.Max(total - cost, 0);
        }

        public override string ToString()
        {
            return $"{Cost}/{Total}";
        }
    }
}