using System.IO;
using System.Reflection;
using System.Collections;
using System.ComponentModel;

namespace InfoFetchService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            RootPath = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Parent.FullName;
            InitializeComponent();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            string DatasetPath = Path.Combine(RootPath, @"webdata.sqlite");
            string WebsiteDataPath = Path.Combine(RootPath, @"websites.txt");
            string param = DatasetPath+"\" \""+WebsiteDataPath;
            Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\" \"" + param + "\"";
            base.OnBeforeInstall(savedState);
        }

        private string RootPath;
    }
}
