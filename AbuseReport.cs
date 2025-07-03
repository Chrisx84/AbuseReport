using log4net;
using Mono.Addins;
using Nini.Config;
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Region.Framework.Interfaces;
using OpenSim.Region.Framework.Scenes;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using System.Net;
[assembly: Addin("AbuseReportModule", "1.0")]
[assembly: AddinDependency("OpenSim.Region.Framework", OpenSim.VersionInfo.VersionNumber)]
namespace AbuseReport
{
    [Extension(Path = "/OpenSim/RegionModules", NodeName = "RegionModule", Id = "AbuseReportModule")]
    public class AbuseReport : ISharedRegionModule
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public string Name
        {
            get { return "AbuseReportModule"; }
        }
        public Type ReplaceableInterface
        {
            get { return null; }
        }
        private bool m_Enabled = false;
        private string ABUSE_URL = "";
        private List<Scene> m_SceneList = new List<Scene>();

        public void Initialise(IConfigSource source)
        {
            IConfig abuseConfig = source.Configs["AbuseReport"];
            if (abuseConfig == null)
                return;

            if (abuseConfig.GetString("ARModule", String.Empty) != Name)
                return;

            ABUSE_URL = abuseConfig.GetString("URL", String.Empty);

            m_Enabled = abuseConfig.GetBoolean("Enabled", false);

            m_log.InfoFormat("[ABUSEREPORT]: Initializing {0} with URL: {1}", Name, ABUSE_URL);
        }

        public void AddRegion(Scene scene)
        {
            m_SceneList.Add(scene);
            scene.EventManager.OnNewClient += OnNewClient;
        }

        private void OnNewClient(IClientAPI client)
        {
            if (m_Enabled)
                client.OnUserReport += OnUserReport;
        }

        private async void OnUserReport(IClientAPI client,
            string regionName,
            UUID abuserID,
            byte catagory,
            byte checkflags,
            string details,
            UUID objectID,
            Vector3 postion,
            byte reportType,
            UUID screenshotID,
            string Summary,
            UUID reporter)
        {
            string screenpic = screenshotID.ToString();

            var report = new AbuseReportJson
            {
                Details = details,
                Summary = Summary,
                ScreenshotID = screenpic
            };
            if (!string.IsNullOrEmpty(ABUSE_URL))
            {
                string json = JsonSerializer.Serialize(report);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                using HttpClient http = new HttpClient();
                var reply = await http.PutAsync(ABUSE_URL, content);
                string responseContent = await reply.Content.ReadAsStringAsync();
                if (reply.StatusCode == HttpStatusCode.OK)
                {
                    m_log.InfoFormat("[ABUSEREPORT]: Report submitted successfully: {0}", responseContent);
                }
                else
                {
                    m_log.ErrorFormat("[ABUSEREPORT]: Failed to submit report: {0} - {1}", reply.StatusCode, responseContent);
                }
            }
            else
            {
                m_log.Error("[ABUSEREPORT]: URL is not set in the configuration.");
            }
        }

        public void Close()
        {
            m_SceneList.Clear();
        }

        public void PostInitialise()
        {
            //
        }

        public void RegionLoaded(Scene scene)
        {
            //
        }

        public void RemoveRegion(Scene scene)
        {
            if (m_Enabled)
            {
                m_SceneList.Remove(scene);
                scene.EventManager.OnNewClient -= OnNewClient;
            }
        }
    }
    [Serializable]
    public class AbuseReportJson
    {
        public string Details { get; set; }
        public string ScreenshotID { get; set; }
        public string Summary { get; set; }
    }
}
