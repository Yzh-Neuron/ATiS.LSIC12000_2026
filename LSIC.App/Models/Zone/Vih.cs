namespace LSIC.App.Models
{
    public class Vih : ObservableObject
    {
        public Vih(int id)
        {
            Id = id;
        }

        public int Id { get; private set; }

        private int setVoltage;
        public int SetVoltage
        {
            get => setVoltage;
            set => SetProperty(ref setVoltage, value);
        }

        private int outputVoltage;
        public int OutputVoltage
        {
            get => outputVoltage;
            set => SetProperty(ref outputVoltage, value);
        }
    }
}
