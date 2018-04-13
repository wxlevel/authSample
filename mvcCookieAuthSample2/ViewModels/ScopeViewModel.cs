namespace mvcCookieAuthSample.ViewModels
{
    public class ScopeViewModel
    {
        public string Name { get; set; }
        public string DisplayName{ get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; } //强调
        public bool Required { get; set; }
        public bool Checked { get; set; }
    }
}
