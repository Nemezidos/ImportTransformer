using System;
using System.Collections.Generic;
using System.Text;

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

        //    public SantensReport (string company, DateTime parentTime, string parentContainer, string idSupply, string warehouse, List<Child> children)
        //    {
        //        this.Company = company;
        //        this.ParentTime = parentTime;
        //        this.ParentContainer = parentContainer;
        //        this.IdSupply = idSupply;
        //        this.Warehouse = warehouse;
        //        this.Children = children;
        //    }

        //    public SantensReport()
        //    {
        //        this.Children = new List<Child>();
        //    }

        //    public string Company { get; set; }
        //    public DateTime ParentTime { get; set; }
        //    public string ParentContainer { get; set; }
        //    public string IdSupply { get; set; }
        //    public string Warehouse { get; set; }
        //    public List<Child> Children { get; set; }
        //}

        //public class Child
        //{
        //    public Child (DateTime childTime, string ssccOrSn, string user)
        //    {
        //        this.ChildTime = childTime;
        //        this.SsccOrSn = ssccOrSn;
        //        this.User = user;
        //    }

        //    public Child()
        //    {

        //    }

        //    public DateTime ChildTime { get; set; }
        //    public string SsccOrSn { get; set; }
        //    public string User { get; set; }
    }
}
