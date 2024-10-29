
using DynamicData;
using Embedded_Home_Screen.Models.Update;
using Embedded_Home_Screen.ViewModels;
using Newtonsoft.Json;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Version = Embedded_Home_Screen.Models.Update.Version;


namespace Embedded_Home_Screen.Controller
{
    public class UpdateController
    {

        public List<Version> OtherVersionsToUpdate { get; set; }
        public Version NextVersion { get; set; }
        public Version ThisVersion { get; set; }

        public Version LatestVersion { get; set; }  

        private GitHubClient client;
        private MainViewModel mainViewModel;

        private string FetchedFileName = string.Empty;
        public UpdateController(MainViewModel model)
        {
            ThisVersion = Version.getVersionFromString("v1.0.2");
            mainViewModel = model;
            client = new GitHubClient(new ProductHeaderValue("BoTech.dev-Embedded_Home_Screen"));

            // Delete old Directory after Update.
            CleanUp();


        }
        private void CleanUp()
        {
            string? dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName;
            mainViewModel.Status = "Cleaning up: Dir: " + dir;
            if (dir != null)
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dir);
                foreach (DirectoryInfo directory in dirInfo.GetDirectories())
                {
                    // Dir has the Format: vx.y.z.?p
                    if (directory.Name.IndexOf('v') != -1)
                    {
                        if (directory.Name.IndexOf('.') != -1)
                        {
                            if (!directory.Name.Equals(ThisVersion.ToString()))
                            {
                                directory.Delete();
                            }
                        }
                    }
                }
            }
        }
    
        /// <summary>
        /// This Method checks if there is a newer Version to thisVersion available, in the related Repository to this App on github.com.
        /// 
        /// </summary>
        /// <returns>
        /// Returns true if there are Updates available or false when this Version is the Current Version.
        /// </returns>
        public bool CheckForUpdates(bool includePreRelease)
        {
            mainViewModel.Status = "Get: api.github.com (Releases for Embedded_Home_Screen)";
            mainViewModel.IsIndeterminate = false;
           
            IReadOnlyList<Release> allReleases = client.Repository.Release.GetAll("BoTech-Development", "Embedded_Home_Screen").Result;

            mainViewModel.Percentage = 33;
            if (allReleases.Count > 0)
            {
                if (ThisVersion.Equals(allReleases[0]))
                {
                    // Nothing to Update.
                    return false;
                }
                else
                {
                    LatestVersion = Version.getVersionFromRelease(allReleases[0]);

                    //Get the next Version from thisVersion for Instance :
                    // thisVersion is v1.0.1
                    // The next Version to Update to will be v1.0.2
                    mainViewModel.Status = "Process: Get the next higher Version...";
                    Version lastVersion = null;
                    // NextVersion => Version to Update to ...
                    // Checking from the newest to the oldest version:

                    foreach (Release release in allReleases)
                    {

                        if (Version.getVersionFromRelease(release).Equals(ThisVersion) && lastVersion != null)
                        {
                            NextVersion = lastVersion;
                            break;
                        }
                        lastVersion = Version.getVersionFromRelease(release);
                    }
                    mainViewModel.Percentage = 66;
                    mainViewModel.Status = "Process: Convert other Releases... (Model Convert)";

                    List<Version> otherVersionsToUpdate = new List<Version>();
                    foreach (Release release in allReleases)
                    {
                        if (!Version.getVersionFromRelease(release).Equals(NextVersion) && !Version.getVersionFromRelease(release).Equals(ThisVersion))
                        {
                            otherVersionsToUpdate.Add(Version.getVersionFromRelease(release));
                        }
                    }
                    mainViewModel.Percentage = 100;
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public void AplyUpdate()
        {
            ReleaseAsset? updateFile;
            if ((updateFile = NextVersion.RelatedRelease.Assets.Where(a => a.Name.Equals("update.zip")).FirstOrDefault()) != null)
            {
                string destPath = AppDomain.CurrentDomain.BaseDirectory;

                string newDir =
                    destPath.Replace(ThisVersion.ToString() + "/",
                        NextVersion.ToString() + "/"); // Remove for e.g. the /v1.0.1/ Folder.


                Directory.CreateDirectory(newDir); // Create new Version Folder
                destPath = newDir + "/" + updateFile.Name; // Create new File
                DownloadFile(updateFile.BrowserDownloadUrl, destPath);


                // Extract the Downloaded update.zip File in the new Directory
                ZipFile.ExtractToDirectory(destPath, newDir);

                // Change Startup.sh which will be called by a Service
                // Change to the correct Path
                string startupContent = "#!/bin/bash";
                startupContent += "\n cd /";
                startupContent += "\n cd " + newDir;
                startupContent += "\n dotnet Embedded_Home_Screen.Desktop.dll";
                string? parentDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
                if (parentDir != null)
                {
                    File.WriteAllText(Path.Combine(parentDir, "Startup.sh"), startupContent);
                }

                // Restart to Update.
                
            }

        }
        private void DownloadFile(string url, string destPath)
        {
            WebClient webClient = new WebClient();
            webClient.DownloadProgressChanged += WebClient_DownloadProgressChanged;
            webClient.DownloadFile(url, destPath);
        }
        private void WebClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            mainViewModel.IsIndeterminate = false;
            mainViewModel.Percentage = e.ProgressPercentage;
            mainViewModel.Status = "Fetch: " + FetchedFileName + " (" + e.BytesReceived + "Bytes / " + e.TotalBytesToReceive + ")"; 
        }

        
    }
}
