using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sekaiav.Models;

namespace sekaiav.Models
{
    public static class SekaiAVDataExtensions
    {
        public static Actress GetModel(this t_actress entry)
        {
            return new Actress
            {
                Id = entry.id,
                JL_Id = entry.f_javlib_id,
                Name = entry.f_name,
                Update = entry.f_update,
                Prefix = entry.f_prefix,
                Version = entry.f_version
            };
        }

        public static Director GetModel(this t_director entry)
        {
            return new Director
            {
                Id = entry.id,
                JL_Id = entry.f_javlib_id,
                Name = entry.f_name,
                Update = entry.f_update
            };
        }

        public static Factory GetModel(this t_factory entry)
        {
            return new Factory
            {
                Id = entry.id,
                JL_Id = entry.f_javlib_id,
                Name = entry.f_name,
                Update = entry.f_update
            };
        }

        public static Issuer GetModel(this t_issuer entry)
        {
            return new Issuer
            {
                Id = entry.id,
                JL_Id = entry.f_javlib_id,
                Name = entry.f_name,
                Update = entry.f_update
            };
        }

        public static Video GetModel(this v_video entry)
        {
            Video v = new Video
            {
                Code = entry.f_code,
                Id = entry.id,
                IssueDate = entry.f_issuer_date,
                JL_Id = entry.f_javlib_id,
                Length = entry.f_length,
                Name = entry.f_name,
                Update = entry.f_update
            };
            if (entry.f_director_id.HasValue)
            {
                v.Factory = new Factory
                {
                    Id = entry.f_director_id.Value,
                    JL_Id = entry.f_director_javlib_id,
                    Name = entry.f_director_name
                };
            }
            v.Issuer = new Issuer
            {
                Id = entry.f_issuer_id,
                JL_Id = entry.f_issuer_javlib_id,
                Name = entry.f_issuer_name
            };
            v.Factory = new Factory
            {
                Id = entry.f_factory_id,
                JL_Id = entry.f_factory_javlib_id,
                Name = entry.f_factory_name
            };
            return v;
        }
    }
}
