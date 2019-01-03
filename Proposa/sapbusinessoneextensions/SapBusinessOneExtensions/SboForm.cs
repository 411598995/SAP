using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NLog;
using SAPbouiCOM;
using System.Runtime.InteropServices;

namespace SapBusinessOneExtensions
{
    public class SboForm
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        protected SboForm(string formType, string formUid = null)
        {
            FormType = formType;
            FormUid = formUid;

            SboAddon.Instance.Events.ItemEvent += (string uid, ref ItemEvent val, out bool @event) =>
                {
                    if (FormUid == null
                            ? FormType.Equals(val.FormTypeEx)
                            : FormType.Equals(val.FormTypeEx) && uid.Equals(FormUid))
                        try
                        {
                            if (ItemEvent != null)
                                ItemEvent(uid, ref val, out @event);
                            else
                                @event = true;
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, "Error handling form item event");
                            @event = true;
                        }
                    else
                        @event = true;
                };
        }

        public string FormUid { get; set; }
        public string FormType { get; set; }
        public event _IApplicationEvents_ItemEventEventHandler ItemEvent;

        public static void LoadFromXml(Stream stream, string formUid = null, ResourceManager resources = null)
        {
            if (resources == null)
                resources = Properties.Resources.ResourceManager;

            var xmlDocument = XDocument.Load(stream);
            if (resources != null)
            {
                var xmlNodeList = xmlDocument.XPathSelectElements("//*");
                foreach (var node in xmlNodeList)
                {
                    foreach (var attribute in node.Attributes())
                    {
                        if (attribute.Value.StartsWith("%%"))
                        {
                            string str = resources.GetString(attribute.Value.Substring(2));
                            attribute.Value = String.IsNullOrEmpty(str) ? attribute.Value.Substring(2) : str;
                        }
                    }
                }
            }
            if (formUid != null)
            {
                var formNode = xmlDocument.XPathSelectElement("//form");
                if (formNode != null)
                    formNode.SetAttributeValue("uid", formUid);
            }

            SboAddon.Instance.Application.LoadBatchActions(xmlDocument.ToString());
        }

        public static void LoadFromXml(string filename, string formUid = null, ResourceManager resources = null)
        {
            using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                LoadFromXml(stream, formUid, resources);
        }

        public static void LoadFormUserSettings(Form form)
        {
            if (SboAddon.Instance.UserSettings.GetValueOrDefault<bool>(string.Format("formsettings.{0}.maximized", form.TypeEx)))
                form.State = BoFormStateEnum.fs_Maximized;
            else
            {
                form.Resize(
                    SboAddon.Instance.UserSettings.GetValueOrDefault<int>(string.Format("formsettings.{0}.width", form.TypeEx), form.Width),
                    SboAddon.Instance.UserSettings.GetValueOrDefault<int>(string.Format("formsettings.{0}.height", form.TypeEx), form.Height));
            }
        }

        public static void SaveFormUserSettings(Form form)
        {
            SboAddon.Instance.UserSettings.SetValue<bool>(string.Format("formsettings.{0}.maximized", form.TypeEx), form.State == BoFormStateEnum.fs_Maximized);
            SboAddon.Instance.UserSettings.SetValue<int>(string.Format("formsettings.{0}.width", form.TypeEx), form.Width);
            SboAddon.Instance.UserSettings.SetValue<int>(string.Format("formsettings.{0}.height", form.TypeEx), form.Height);
        }

        public static FrozenForm FreezeForm(Form form)
        {
            return new FrozenForm(form);
        }
    }

    public sealed class FrozenForm : IDisposable
    {
        private Form Form { get; set; }
        public FrozenForm(Form form)
        {
            Form = form;
            Form.Freeze(true);
        }

        public void Dispose()
        {
            if (Form != null)
            {
                Form.Freeze(false);
                Form = null;

                GC.SuppressFinalize(this);
            }
        }
    }
}