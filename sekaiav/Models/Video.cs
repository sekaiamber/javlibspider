using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sekaiav.Models;
using System.Text.RegularExpressions;

namespace sekaiav.Models
{
    public class Video : BaseModel
    {
        public Director Director { get; set; }
        public double Length { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public DateTime IssueDate { get; set; }
        public Issuer Issuer { get; set; }
        public Factory Factory { get; set; }
        public VideoTag[] Tags { get; set; }

        public static Video[] GetVideoListFromHtml(string html)
        {
            List<Video> ret = new List<Video>();
            RegexOptions regexOptions = RegexOptions.Singleline;
            Regex reg = new Regex("<table class=\"videotextlist\">.+</table>(?=<!-- end of videotextlist -->)", regexOptions);
            var m = reg.Match(html);
            if (m.Success)
            {
                string table = m.Value;
                Regex reg_actress = new Regex("<tr( class=\"dimrow\")?>.+?</tr>", regexOptions);
                var ms = reg_actress.Matches(table);
                foreach (Match match in ms)
                {
                    Video v = new Video();
                    string tr_html = match.Value;
                    Regex _reg = new Regex("<a href=\".+\".+?>");
                    var _title_all = _reg.Match(tr_html).Value;
                    _reg = new Regex("(?<=href=\"\\./\\?v=).+?(?=\")");
                    v.JL_Id = _reg.Match(_title_all).Value;
                    _reg = new Regex("(?<=title=\").+?(?=\")");
                    var _code_name = _reg.Match(_title_all).Value;
                    _reg = new Regex("[A-Z0-9]{2,6}-[A-Z0-9]{2,6}");
                    v.Code = _reg.Match(_code_name).Value;
                    v.Name = _code_name.Substring(v.Code.Length + 1);
                    _reg = new Regex("(?<=<td>)\\d{4}-\\d{2}-\\d{2}");
                    v.IssueDate = DateTime.Parse(_reg.Match(tr_html).Value);

                    ret.Add(v);
                }
            }
            return ret.ToArray();
        }

        public static Video GetActressNotVersion(string version, int thcount, int thindex)
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                var entry = (from m in db.t_video
                           where m.f_version != version && m.id % thcount == thindex
                           select m).FirstOrDefault();
                if (entry == null)
                {
                    return null;
                }
                Video ret = new Video
                {
                    Id = entry.id,
                    Code = entry.f_code,
                    Create = entry.f_create,
                    IssueDate = entry.f_issuer_date,
                    JL_Id = entry.f_javlib_id,
                    Length = entry.f_length,
                    Name = entry.f_name,
                    Update = entry.f_update,
                    Version = entry.f_version,
                    Factory = null,
                    Director = null,
                    Issuer = null,
                    Tags = new VideoTag[0]
                };
                if (entry.f_director > 0)
                {
                    var _d = (from m in db.t_director where m.id == entry.f_director select m).FirstOrDefault();
                    if (_d != null)
                    {
                        ret.Director = _d.GetModel();
                    }
                }
                if (entry.f_factory > 0)
                {
                    var _d = (from m in db.t_factory where  m.id == entry.f_factory select m).FirstOrDefault();
                    if (_d != null)
                    {
                        ret.Factory = _d.GetModel();
                    }
                }
                if (entry.f_issuer > 0)
                {
                    var _d = (from m in db.t_issuer where m.id == entry.f_issuer select m).FirstOrDefault();
                    if (_d != null)
                    {
                        ret.Issuer = _d.GetModel();
                    }
                }
                ret.Tags = (from m in db.t_tag
                             join n in db.t_tag_video on m.id equals n.f_tag
                             where n.f_video == ret.Id
                             select m.GetModel()).ToArray();
                return ret;
            }
        }

        public void UpdateDetailFromHtml(string html)
        {
            RegexOptions regexOptions = RegexOptions.Singleline;
            Regex reg = new Regex("<div id=\"video_info\">.+(?=<!-- end of video_info -->)", regexOptions);
            var m = reg.Match(html);
            if (m.Success)
            {
                string table = m.Value;
                // length
                reg = new Regex("(?<=长度).+(?=<!-- end of video_length -->)", regexOptions);
                var _m = reg.Match(table);
                if (_m.Success)
                {
                    reg = new Regex("(?<=>)\\d+(?=<)", regexOptions);
                    _m = reg.Match(_m.Value);
                    if (_m.Success)
                    {
                        this.Length = Int32.Parse(_m.Value);
                    }
                }
                // director
                reg = new Regex("(?<=导演).+(?=<!-- end of video_director -->)", regexOptions);
                _m = reg.Match(table);
                if (_m.Success)
                {
                    reg = new Regex("<a .+>.+</a>", regexOptions);
                    _m = reg.Match(_m.Value);
                    if (_m.Success)
                    {
                        this.Director = new Director();
                        reg = new Regex("(?<=href=\"vl_director.php\\?d=).+(?=\" rel)", regexOptions);
                        var _m2 = reg.Match(_m.Value);
                        if (_m2.Success)
                        {
                            this.Director.JL_Id = _m2.Value;
                        }
                        reg = new Regex("(?<=>).+(?=</a>)", regexOptions);
                        _m2 = reg.Match(_m.Value);
                        if (_m2.Success)
                        {
                            this.Director.Name = _m2.Value;
                        }
                    }
                }
                // factory
                reg = new Regex("(?<=制作商).+(?=<!-- end of video_maker -->)", regexOptions);
                _m = reg.Match(table);
                if (_m.Success)
                {
                    reg = new Regex("<a .+>.+</a>", regexOptions);
                    _m = reg.Match(_m.Value);
                    if (_m.Success)
                    {
                        this.Factory = new Factory();
                        reg = new Regex("(?<=href=\"vl_maker.php\\?m=).+(?=\" rel)", regexOptions);
                        var _m2 = reg.Match(_m.Value);
                        if (_m2.Success)
                        {
                            this.Factory.JL_Id = _m2.Value;
                        }
                        reg = new Regex("(?<=>).+(?=</a>)", regexOptions);
                        _m2 = reg.Match(_m.Value);
                        if (_m2.Success)
                        {
                            this.Factory.Name = _m2.Value;
                        }
                    }
                }
                // issuer
                reg = new Regex("(?<=发行商).+(?=<!-- end of video_label -->)", regexOptions);
                _m = reg.Match(table);
                if (_m.Success)
                {
                    reg = new Regex("<a .+>.+</a>", regexOptions);
                    _m = reg.Match(_m.Value);
                    if (_m.Success)
                    {
                        this.Issuer = new Issuer();
                        reg = new Regex("(?<=href=\"vl_label.php\\?l=).+(?=\" rel)", regexOptions);
                        var _m2 = reg.Match(_m.Value);
                        if (_m2.Success)
                        {
                            this.Issuer.JL_Id = _m2.Value;
                        }
                        reg = new Regex("(?<=>).+(?=</a>)", regexOptions);
                        _m2 = reg.Match(_m.Value);
                        if (_m2.Success)
                        {
                            this.Issuer.Name = _m2.Value;
                        }
                    }
                }
                // tags
                reg = new Regex("(?<=类别).+(?=<!-- end of video_genres -->)", regexOptions);
                _m = reg.Match(table);
                if (_m.Success)
                {
                    reg = new Regex("<span.+?/span>", regexOptions);
                    List<VideoTag> lst = new List<VideoTag>();
                    foreach (Match _m2 in reg.Matches(_m.Value))
                    {
                        reg = new Regex("<a .+>.+</a>", regexOptions);
                        var _m3 = reg.Match(_m2.Value);
                        if (_m3.Success)
                        {
                            VideoTag _t = new VideoTag();
                            reg = new Regex("(?<=href=\"vl_genre.php\\?g=).+(?=\" rel)", regexOptions);
                            var _m4 = reg.Match(_m3.Value);
                            if (_m4.Success)
                            {
                                _t.JL_Id = _m4.Value;
                            }
                            reg = new Regex("(?<=>).+(?=</a>)", regexOptions);
                            _m4 = reg.Match(_m3.Value);
                            if (_m4.Success)
                            {
                                _t.Name = _m4.Value;
                            }
                            lst.Add(_t);
                        }
                    }
                    this.Tags = lst.ToArray();
                }
            }
        }

        public bool Save(string version)
        {
            return this.Save(version, -1);
        }

        public bool Save(string version, int actress)
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                var count = (from m in db.t_video
                             where m.f_javlib_id == this.JL_Id
                             select m).Count();
                t_video entry;
                if (count == 0)
                {
                    entry = new t_video()
                    {
                        f_code = this.Code,
                        f_issuer_date = this.IssueDate,
                        f_javlib_id = this.JL_Id,
                        f_length = this.Length,
                        f_name = this.Name,
                        f_update = DateTime.Now,
                        f_version = version,
                        f_create = DateTime.Now
                    };
                    if (this.Factory != null)
                    {
                        this.Factory.Save();
                        entry.f_factory = this.Factory.Id;
                    }
                    else
                    {
                        entry.f_factory = -1;
                    }
                    if (this.Issuer != null)
                    {
                        this.Issuer.Save();
                        entry.f_issuer = this.Issuer.Id;
                    }
                    else
                    {
                        entry.f_issuer = -1;
                    }
                    if (this.Director != null)
                    {
                        this.Director.Save();
                        entry.f_director = this.Director.Id;
                    }
                    else
                    {
                        entry.f_director = -1;
                    }
                    db.t_video.InsertOnSubmit(entry);
                    db.SubmitChanges();
                    if (this.Tags != null)
                    {
                        foreach (var tag in this.Tags)
                        {
                            tag.Save(this.Id);
                        }
                    }
                }
                else
                {
                    entry = (from m in db.t_video
                             where m.f_javlib_id == this.JL_Id
                             select m).First();
                    entry.f_code = this.Code;
                    entry.f_issuer_date = this.IssueDate;
                    entry.f_javlib_id = this.JL_Id;
                    entry.f_length = this.Length;
                    entry.f_name = this.Name;
                    entry.f_update = DateTime.Now;
                    entry.f_version = version;
                    if (this.Factory != null)
                    {
                        this.Factory.Save();
                        entry.f_factory = this.Factory.Id;
                    }
                    else
                    {
                        entry.f_factory = -1;
                    }
                    if (this.Issuer != null)
                    {
                        this.Issuer.Save();
                        entry.f_issuer = this.Issuer.Id;
                    }
                    else
                    {
                        entry.f_issuer = -1;
                    }
                    if (this.Director != null)
                    {
                        this.Director.Save();
                        entry.f_director = this.Director.Id;
                    }
                    else
                    {
                        entry.f_director = -1;
                    }
                    db.SubmitChanges();
                    if (this.Tags != null)
                    {
                        foreach (var tag in this.Tags)
                        {
                            tag.Save(this.Id);
                        }
                    }
                }
                if (actress > 0)
                {
                    var rcount = (from m in db.t_actress_video
                                  where m.f_actress == actress && m.f_video == entry.id
                                  select m).Count();
                    if (rcount == 0)
                    {
                        db.t_actress_video.InsertOnSubmit(new t_actress_video()
                        {
                            f_actress = actress,
                            f_video = entry.id
                        });
                        db.SubmitChanges();
                    }
                }
            }
            return true;
        }
    }
}
