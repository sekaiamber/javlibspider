using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sekaiav.Models;
using System.Text.RegularExpressions;

namespace sekaiav.Models
{
    public class Actress : BaseModel
    {
        public string Name { get; set; }
        public string Prefix { get; set; }

        public static Actress[] GetActressFromHtml(string html)
        {
            List<Actress> ret = new List<Actress>();
            Regex reg = new Regex("<div +class=\"starbox\">.+</div>");
            var m = reg.Match(html);
            if (m.Success)
            {
                string v = m.Value;
                Regex reg_actress = new Regex("<div id=\".{1,6}\" class=\"searchitem\">.+?</div>");
                var ms = reg_actress.Matches(v);
                foreach (Match match in ms)
                {
                    Actress ac = new Actress();
                    string actress_html = match.Value;
                    Regex _reg = new Regex("(?<=id=\").+?(?=\")");
                    var mid = _reg.Match(actress_html);
                    ac.JL_Id = mid.Value;
                    _reg = new Regex("(?<=<a.+>).+(?=</a>)");
                    var mname = _reg.Match(actress_html);
                    ac.Name = mname.Value;

                    ret.Add(ac);
                }
            }
            return ret.ToArray();
        }

        public static Actress GetActressNotVersion(string version, bool overwrite, string[] fixture)
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                var ret = (from m in db.t_actress
                           where m.f_version != version && fixture.Contains(m.f_prefix)
                           select m).FirstOrDefault();
                if (ret == null)
                {
                    return null;
                }
                else
                {
                    if (overwrite)
                    {
                        ret.f_version = version;
                        db.SubmitChanges();
                    }
                    return ret.GetModel();
                }
            }
        }

        public bool Save(string version)
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                var count = (from m in db.t_actress
                             where m.f_javlib_id == this.JL_Id
                             select m).Count();
                t_actress entry;
                if (count == 0)
                {
                    entry = new t_actress()
                    {
                        f_javlib_id = this.JL_Id,
                        f_name = this.Name,
                        f_update = DateTime.Now,
                        f_version = version
                    };
                    db.t_actress.InsertOnSubmit(entry);
                    db.SubmitChanges();
                }
                else
                {
                    entry = (from m in db.t_actress
                             where m.f_javlib_id == this.JL_Id
                             select m).First();
                    entry.f_name = this.Name;
                    entry.f_prefix = this.Prefix;
                    entry.f_update = DateTime.Now;
                    entry.f_version = version;
                    db.SubmitChanges();
                }
            }
            return true;
        }
    }
}
