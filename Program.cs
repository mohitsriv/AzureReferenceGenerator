using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NuGetBrowse.Models;

namespace NuGetBrowse
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0 && String.Equals(args[0], "dbg", StringComparison.OrdinalIgnoreCase))
            {
                args = args.Skip(1).ToArray();
            }

            AsyncMain(args).Wait();
        }

        private static async Task AsyncMain(string[] args)
        {
            var ids = new Dictionary<string, Package>();

            // ...Target page.
            // string page = "http://nuget.org/api/v2";
            string next = "http://nuget.org/api/v2/Packages()?$select=IsPrerelease,ReleaseNotes,Authors,Id,Tags,Title,Description,ProjectUrl,GalleryDetailsUrl,Version,IsLatestVersion,IsAbsoluteLatestVersion,LastUpdated&$filter=substringof('azureofficial',Tags)";

            // ... Use HttpClient.
            var userProfile = Environment.GetEnvironmentVariable("USERPROFILE");           
            // This is a raw dump of the NuGet feed, for debugging
            using (var outfile = new StreamWriter(Path.Combine(userProfile, "Desktop", "NuGetFeed.txt")))
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                while (!String.IsNullOrWhiteSpace(next))
                {
                    using (HttpResponseMessage response = await client.GetAsync(next))
                    using (HttpContent content = response.Content)
                    {
                        // ... Read the string.
                        string result = await content.ReadAsStringAsync();

                        // Write the stream contents to a new file
                        outfile.WriteLine(result.ToString());

                        var nuGetJson = JsonConvert.DeserializeObject<Rootobject>(result.ToString());
                        // We build a dictionary to help construct the view we want on the reference page.
                        foreach (var entry in nuGetJson.d.results)
                        {
                            var id = entry.Id;
                            var version = entry.Version;
                            if ((bool)entry.IsPrerelease)
                            {
                                id += " (Prerelease)";
                            }
                            if (!ids.ContainsKey(id))
                            {
                                ids.Add(id, new Package { Count = 0 });
                            }
                            ids[id].Count++;
                            ids[id].IsPrerelease = (bool)entry.IsPrerelease;
                            ids[id].LatestVerion = (String.Compare(ids[id].LatestVerion, version) > 0) ? ids[id].LatestVerion : version;
                            //Console.WriteLine(id + ":" + version + ":" + (bool)entry["IsLatestVersion"] + ":" + (bool)entry["IsAbsoluteLatestVersion"]);
                            ids[id].GalleryDetailsUrl = entry.GalleryDetailsUrl;
                            ids[id].ReleaseNotes = entry.ReleaseNotes;
                        }
                        next = (nuGetJson.d.__next != null) ? nuGetJson.d.__next : null;
                    }
                }
            }
            // Write it out as a tab-delimited file for easy import into Excel
            using (var outfile = new StreamWriter(Path.Combine(userProfile, "Desktop", "AzureReferencePage.tsv")))
            {
                outfile.WriteLine("Package" + "\t" + "Document?\t" + "IsOneSDK?\t" + "Releases" + "\t" + "IsPrerelease" + "\t" + "LatestVerion" + "\t" + "GalleryDetailsUrl" + "\t" + "GalleryDetailsUrl as Link");
                foreach (string key in ids.Keys.OrderBy(m => m))
                {
                    bool document = true;
                    var entry = ids[key];
                    if (entry.IsPrerelease)
                    {
                        var releaseKey = key.Substring(0, key.IndexOf(" (Prerelease)"));
                        if (ids.ContainsKey(releaseKey))
                        {
                            if (String.Compare(ids[releaseKey].LatestVerion, ids[key].LatestVerion) > 0)
                            {
                                document = false;
                            }
                        }
                    }
                    outfile.WriteLine(key + "\t" + (document ? "" : "FALSE") + "\t\t" + entry.Count + "\t" + entry.IsPrerelease + "\t" + entry.LatestVerion + "\t" + entry.GalleryDetailsUrl);
                }
            }
        }
    }
}
