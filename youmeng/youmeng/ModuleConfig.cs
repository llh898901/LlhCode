using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UMeng.Model;

namespace UMeng
{
    public class ModuleConfig
    {
        XmlDocument xml;
        public ModuleConfig()
            : this("") { }

        public ModuleConfig(string xmlPath)
        {
            LoadXml(xmlPath);
        }
        void LoadXml(string xmlPath = "")
        {
            if (string.IsNullOrEmpty(xmlPath))
            {
                xmlPath = "config.xml";
            }
            xml = new XmlDocument();

            var xmlInDebug = Path.Combine(Environment.CurrentDirectory, xmlPath);
            var xmlInDebugPre = Path.Combine(Path.GetFullPath(".."), xmlPath);
            var xmlInDebugPrePre = Path.Combine(Path.GetFullPath("../.."), xmlPath);
            if (File.Exists(xmlInDebug))
            {
                xml.Load(xmlInDebug);
            }
            else if (File.Exists(xmlInDebugPre))
            {
                xml.Load(xmlInDebugPre);
            }
            else if (File.Exists(xmlInDebugPrePre))
            {
                xml.Load(xmlInDebugPrePre);
            }
        }
        /// <summary>
        /// 任务集合
        /// </summary>
        List<TaskModule> TaskModuleList;
        public TaskModule GetModule(string moduleName)
        {
            if (TaskModuleList == null)
            {
                TaskModuleList = new List<TaskModule>();
            }
            if (!TaskModuleList.Any(o => o.Name == moduleName))
            {
                TaskModuleList.Add(FillTask(moduleName));
            }
            return TaskModuleList.SingleOrDefault(o => o.Name == moduleName);
        }
        public List<TaskModule> GetModules()
        {
            List<TaskModule> list = new List<TaskModule>();
            var nodes = xml.SelectNodes(string.Format("/modules/module[@isRun={0}]", 1));
            foreach (var objNode in nodes)
            {
                var node = objNode as XmlNode;
                TaskModule model = new TaskModule();

                model.IsRun = node.Attributes["isRun"] != null ? int.Parse(node.Attributes["isRun"].Value) == 1 ? true : false : false;
                model.Name = node.Attributes["name"].Value;
                model.AppKey = node.SelectSingleNode("appKey") == null ? "" : node.SelectSingleNode("appKey").InnerText;
                model.UserKey = node.SelectSingleNode("userKey") == null ? "" : node.SelectSingleNode("userKey").InnerText;
                model.Secretkey = node.SelectSingleNode("secretKey") == null ? "" : node.SelectSingleNode("secretKey").InnerText;
                model.Manual_Run = node.SelectSingleNode("manual") != null ? int.Parse(node.SelectSingleNode("manual").Attributes["isRun"].Value) == 1 ? true : false : false;
                model.StartDate = node.SelectSingleNode("manual/startDate") == null ? DateTime.Now.AddDays(-1) : DateTime.Parse(node.SelectSingleNode("manual/startDate").InnerText);
                model.EndDate = node.SelectSingleNode("manual/endDate") == null ? DateTime.Now.AddDays(-1) : DateTime.Parse(node.SelectSingleNode("manual/endDate").InnerText);

                list.Add(model);
            }
            return list;
        }
        TaskModule FillTask(string moduleName)
        {
            var node = xml.SelectSingleNode(string.Format("/modules/module[@name='{0}']", moduleName));
            TaskModule model = new TaskModule();
            model.IsRun = node.Attributes["isRun"] != null ? int.Parse(node.Attributes["isRun"].Value) == 1 ? true : false : false;
            model.Name = node.Attributes["name"].Value;
            model.AppKey = node.SelectSingleNode("appKey") == null ? "" : node.SelectSingleNode("appKey").InnerText;
            model.AppKey = node.SelectSingleNode("userKey") == null ? "" : node.SelectSingleNode("userKey").InnerText;
            model.AppKey = node.SelectSingleNode("secretkey") == null ? "" : node.SelectSingleNode("secretkey").InnerText;

            model.Manual_Run = node.SelectSingleNode("manual") != null ? int.Parse(node.SelectSingleNode("manual").Attributes["isRun"].Value) == 1 ? true : false : false;
            model.StartDate = node.SelectSingleNode("manual/startDate") == null ? DateTime.Now.AddDays(-1) : DateTime.Parse(node.SelectSingleNode("manual/startDate").InnerText);
            model.EndDate = node.SelectSingleNode("manual/endDate") == null ? DateTime.Now.AddDays(-1) : DateTime.Parse(node.SelectSingleNode("manual/endDate").InnerText);

            return model;
        }
    }
}
