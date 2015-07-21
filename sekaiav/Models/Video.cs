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

        public static Video[] GetVideoFromHtml(string html)
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
                        f_version = version
                    };
                    if (this.Factory != null)
                    {
                        entry.f_factory = this.Factory.Id;
                    }
                    else
                    {
                        entry.f_factory = -1;
                    }
                    if (this.Issuer != null)
                    {
                        entry.f_issuer = this.Issuer.Id;
                    }
                    else
                    {
                        entry.f_issuer = -1;
                    }
                    if (this.Director != null)
                    {
                        entry.f_director = this.Director.Id;
                    }
                    else
                    {
                        entry.f_director = -1;
                    }
                    db.t_video.InsertOnSubmit(entry);
                    db.SubmitChanges();
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
                        entry.f_factory = this.Factory.Id;
                    }
                    else
                    {
                        entry.f_factory = -1;
                    }
                    if (this.Issuer != null)
                    {
                        entry.f_issuer = this.Issuer.Id;
                    }
                    else
                    {
                        entry.f_issuer = -1;
                    }
                    if (this.Director != null)
                    {
                        entry.f_director = this.Director.Id;
                    }
                    else
                    {
                        entry.f_director = -1;
                    }
                    db.SubmitChanges();
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
