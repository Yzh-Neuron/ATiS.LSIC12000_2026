namespace LSIC.App.Models
{
    /// <summary>
    /// 一个步次（包含多个向量）
    /// </summary>
    public class Step : ObservableValidator, ICloneable
    {
        public ObservableCollection<Vector> Vectors { get; set; }

        public object Clone()
        {

            return new Step()
            {
                Vectors = new ObservableCollection<Vector>(Vectors.Select(i => (Vector)i.Clone()))
            };
        }
    }
}
