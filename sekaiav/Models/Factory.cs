﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sekaiav.Models;

namespace sekaiav.Models
{
    public class Factory : BaseModel
    {
        public string Name { get; set; }

        public bool Save()
        {
            using (SekaiAVDataDataContext db = new SekaiAVDataDataContext())
            {
                if (this.JL_Id != null && this.Name != null && this.JL_Id != "" && this.Name != "")
                {
                    var q = (from m in db.t_factory
                             where m.f_javlib_id == JL_Id
                             select m).Count();
                    t_factory entry;
                    if (q == 0)
                    {
                        entry = new t_factory()
                        {
                            f_javlib_id = this.JL_Id,
                            f_name = this.Name,
                            f_create = DateTime.Now,
                            f_update = DateTime.Now
                        };
                        db.t_factory.InsertOnSubmit(entry);
                        db.SubmitChanges();
                        this.Id = entry.id;
                    }
                    else
                    {
                        entry = (from m in db.t_factory
                                 where m.f_javlib_id == JL_Id
                                 select m).First();
                        entry.f_javlib_id = JL_Id;
                        entry.f_name = Name;
                        entry.f_update = DateTime.Now;
                        db.SubmitChanges();
                        this.Id = entry.id;
                    }
                }
            }
            return true;
        }
    }
}
