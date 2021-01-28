using System;
using System.Web.UI;

namespace KolosWorks.Util.Controls
{
    public class KolosWorksControl : Control
    {
        public string Name { get; set; }
        public Type BaseType { get; set; }
        public string Location { get; set; }

        public KolosWorksControl(string name, Type baseType, string location)
        {
            Name = name;
            BaseType = baseType;
            Location = location;
        }
        protected override void OnPreRender(EventArgs e)
        {
            Page.ClientScript.RegisterClientScriptInclude(Name, Page.ClientScript.GetWebResourceUrl(BaseType, Location));

            base.OnPreRender(e);
        }
    }
}