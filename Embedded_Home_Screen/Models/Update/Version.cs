using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Embedded_Home_Screen.Models.Update
{
    public class Version
    {
        public int Major {  get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int SecondPatch { get; set; } = -1;
        public Release RelatedRelease { get; set; }

        public Version(int major, int minor, int patch, int secondPatch)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            SecondPatch = secondPatch;
            RelatedRelease = null;
        }
        public Version(int major, int minor, int patch, int secondPatch, Release relatedRelease)
        {
            Major = major;
            Minor = minor;
            Patch = patch;
            SecondPatch = secondPatch;
            RelatedRelease = relatedRelease;
        }

        public static Version getVersionFromString(string versionString)
        {
            string newString = versionString.Replace("v", "");
            int major = int.Parse(newString.Substring(0, newString.IndexOf(".")));
            newString = newString.Remove(0, newString.IndexOf(".") + 1);
            int minor = int.Parse(newString.Substring(0, newString.IndexOf(".")));
            newString = newString.Remove(0, newString.IndexOf(".") + 1);
            int patch = 0;
            int secondPatch = -1;
            if (newString.Length > 1)
            {
                // When there are 4 Version Numbers
                patch = int.Parse(newString.Substring(0, newString.IndexOf(".")));
                newString = newString.Remove(0, newString.IndexOf(".") + 1);
                secondPatch = int.Parse(newString.Substring(0, newString.IndexOf(".")));
            }
            else
            {
                patch = int.Parse(newString);
            }
            return new Version(major, minor, patch, secondPatch);
        }
        public static Version getVersionFromRelease(Release release)
        {
            Version ret = Version.getVersionFromString(release.Name);
            ret.RelatedRelease = release;
            return ret;
        }

        public bool isHigherThanThis(Version version)
        {
            return Major < version.Major || Minor < version.Minor || Patch < version.Patch || SecondPatch < version.SecondPatch;
        }
        public bool isLowerThanThis(Version version)
        {
            return Major > version.Major || Minor > version.Minor || Patch > version.Patch || SecondPatch > version.SecondPatch;
        }
        public override string ToString()
        {
            string ret = "v" + Major + "." + Minor + "." + Patch;
            if(SecondPatch != -1) ret += "." + SecondPatch;
            return ret;
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is Version)
            {
                return ((Version)obj).Major.Equals(Major) && ((Version)obj).Minor.Equals(Minor) && ((Version)obj).Patch.Equals(Patch) && ((Version)obj).SecondPatch.Equals(SecondPatch);
            }
            return base.Equals(obj);
        }
    }
}
