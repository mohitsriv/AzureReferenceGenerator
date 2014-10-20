using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGetBrowse.Models
{
    // Auto-generated using Paste JSON as Classes, available in Web projects!
    public class Rootobject
    {
        public D d { get; set; }
    }

    public class D
    {
        public Result[] results { get; set; }
        public string __next { get; set; }
    }

    public class Result
    {
        public __Metadata __metadata { get; set; }
        public string Id { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public string Description { get; set; }
        public string GalleryDetailsUrl { get; set; }
        public bool IsLatestVersion { get; set; }
        public bool IsAbsoluteLatestVersion { get; set; }
        public bool IsPrerelease { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ProjectUrl { get; set; }
        public string ReleaseNotes { get; set; }
        public string Tags { get; set; }
        public string Title { get; set; }
    }

    public class __Metadata
    {
        public string id { get; set; }
        public string uri { get; set; }
        public string type { get; set; }
        public string edit_media { get; set; }
        public string media_src { get; set; }
        public string content_type { get; set; }
    }
}
