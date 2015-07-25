using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sekaiav.Models
{
    public class VideoTag : BaseModel
    {
        public string Name { get; set; }

        public bool Save()
        {
            return Save(-1);
        }

        public bool Save(int video)
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                if (this.JL_Id != null && this.Name != null && this.JL_Id != "" && this.Name != "")
                {
                    var q = (from m in db.t_tag
                             where m.f_javlib_id == JL_Id
                             select m).Count();
                    t_tag entry;
                    if (q == 0)
                    {
                        entry = new t_tag()
                        {
                            f_javlib_id = this.JL_Id,
                            f_name = this.Name,
                            f_create = DateTime.Now,
                            f_update = DateTime.Now
                        };
                        db.t_tag.InsertOnSubmit(entry);
                        db.SubmitChanges();
                        this.Id = entry.id;
                    }
                    else
                    {
                        entry = (from m in db.t_tag
                                 where m.f_javlib_id == JL_Id
                                 select m).First();
                        entry.f_javlib_id = JL_Id;
                        entry.f_name = Name;
                        entry.f_update = DateTime.Now;
                        db.SubmitChanges();
                        this.Id = entry.id;
                    }
                    if (video > 0)
                    {
                        var rcount = (from m in db.t_tag_video
                                      where m.f_video == video && m.f_tag == entry.id
                                      select m).Count();
                        if (rcount == 0)
                        {
                            db.t_tag_video.InsertOnSubmit(new t_tag_video()
                            {
                                f_tag = entry.id,
                                f_video = video
                            });
                            db.SubmitChanges();
                        }
                    }
                }
            }
            return true;
        }

    }
}
