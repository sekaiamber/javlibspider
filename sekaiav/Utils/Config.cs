using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sekaiav.Utils
{
    public class Config
    {
        public const int ThreadCount = 8;
        public const bool UseGivenJobVersion = true;
        public const string GivenJobVersion = "f30b337dd0644f6ab01eda830ed51ae6";

        public const string Actress_URL = "http://www.javlibrary.com/cn/star_list.php?prefix={0}&page={1}";
        public const string Actress_video_list_URL = "http://www.javlibrary.com/cn/vl_star.php?list&mode=2&s={0}&page={1}";
        public const string Actress_video_detail_URL = "http://www.javlibrary.com/cn/?v={0}";
    }
}
