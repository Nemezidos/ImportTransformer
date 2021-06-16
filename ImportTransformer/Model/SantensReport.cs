using System.Collections.Generic;

namespace ImportTransformer.Model
{
    public class SantensReport
    {
        public SantensReport(string parentContainer, List<string> content)
        {
            this.ParentContainer = parentContainer;
            this.Content = content;
        }
        public SantensReport()
        {
            Content = new List<string>();
        }

        public string ParentContainer { get; set; }
        public List<string> Content { get; set; }
    }
}
