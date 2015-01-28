namespace LightIndexerGUI.Classes.Models
{
    public class AboutModel : IModel
    {
        public string Description
        {
            get
            {
                {
                    var version = this.GetType().Assembly.GetName().Version;
                    return string.Format(FormResources.AboutModel_Text, version);
                }
            }
        }
    }
}