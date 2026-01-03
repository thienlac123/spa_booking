namespace DoAnSPA.Models
{
    public class ResolveServiceResponse
    {
        public bool Found { get; set; }
        /// <summary> "dichvu" hoặc "combo" </summary>
        public string Type { get; set; } = "";
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}
