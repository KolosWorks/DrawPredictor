using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

[assembly: WebResource("KolosWorks.Util.Resources.jquery.min.js", "text/javascript")]
[assembly: WebResource("KolosWorks.Util.Resources.KolosWorksBase.js", "text/javascript")]
namespace KolosWorks.Util.Controls
{
    public class PageX : Page
    {
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            Emit("jQuery", "KolosWorks.Util.Resources.jquery.min.js", typeof(KolosWorksControl));
            Emit("MyUtil", "KolosWorks.Util.Resources.KolosWorksBase.js", typeof(KolosWorksControl));
        }

        protected override void OnInit(EventArgs e)
        {
            Page.Init += delegate (object sender, EventArgs e_Init)
            {
                if (ScriptManager.GetCurrent(Page) == null)
                {
                    ScriptManager scriptManager = new ScriptManager();
                    Page.Form.Controls.AddAt(0, scriptManager);

                    scriptManager.EnablePartialRendering = true;
                }
            };
            base.OnInit(e);
        }

        public void Emit (string name, string location, Type type=null) 
        {
            if (type == null)
            {
                type = GetType().BaseType;
            }

            KolosWorksControl resource = new KolosWorksControl(name, type, location);
            Form.Controls.Add(resource);
        }

        public HtmlGenericControl AddDiv(string id, string innerText = "", Control parentElement = null)
        {
            HtmlGenericControl newDiv = new HtmlGenericControl("DIV");
            newDiv.ID = id;
            newDiv.InnerText = innerText;

            AddControl(newDiv, parentElement);

            return newDiv;
        }

        public HtmlInputButton AddInputButton(string id, string value, Control parentElement = null)
        {
            HtmlInputButton newButton = new HtmlInputButton();
            newButton.ID = id;
            newButton.Value = value;

            AddControl(newButton, parentElement);

            return newButton;
        }
        public Button AddButton(string id, string text, EventHandler eventHandler, Control parentElement = null, bool isTrigger = false)
        {
            Button newButton = new Button();
            newButton.ID = id;
            newButton.Text = text;
            if (eventHandler != null)
            {
                newButton.Click += new EventHandler(eventHandler);
            }
            
            AddControl(newButton, parentElement, isTrigger);

            return newButton;
        }

        private void AddControl(Control newControl, Control parentElement, bool isTrigger = false)
        {
            if (parentElement is UpdatePanel)
            {
                UpdatePanel myParent = (UpdatePanel)parentElement;
                myParent.ContentTemplateContainer.Controls.Add(newControl);

                if (isTrigger)
                {
                    AsyncPostBackTrigger trigger = new AsyncPostBackTrigger();
                    trigger.ControlID = newControl.ID;
                    trigger.EventName = "Click";

                    myParent.Triggers.Add(trigger);
                }
            }
            else
            {
                if (parentElement == null)
                {
                    parentElement = Form;
                }

                parentElement.Controls.Add(newControl);
            }
        }

        public static object GetPropertyList(params Expression<Func<object>>[] expression)
        {
            var list = new Dictionary<string, object>();

            for (int i = 0; i < expression.Length; i++)
            {
                var body = (MemberExpression)expression[i].Body;
                var func = expression[i].Compile();

                list.Add(body.Member.Name, func());
            }

            return list;
        }

        [WebMethod]
        public static object OnFrontEndConstruct()
        {            
            return new object();
        }
    }
}
