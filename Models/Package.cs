using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGetBrowse.Models
{
    class Package
    {
        public int Count { get; set; }
        public bool IsPrerelease { get; set; }
        public string LatestVerion { get; set; }
        public string GalleryDetailsUrl { get; set; }
        public string ReleaseNotes { get; set; }
    }
}
