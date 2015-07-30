namespace ParkenDD.Win10.Models
{
    public class MetaDataCityRow
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
