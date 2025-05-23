﻿using HYDlgn.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Ux = HYDlgn.Abstraction.UtilExtensions;
using HYDlgn.Framework.metadata;

namespace HYDlgn.Framework.AppModel
{
    public class EditModels
    {
        public static EditModels _model;
        public static EditModels Default
        {
            get
            {
                if( _model == null )
                    _model = new EditModels();
                return _model;
            }
        }



        public static Dictionary<string, string> _KEYVALS = new Dictionary<string, string>()
        {
            { "Yes / N.A.","YesNA" },
            { "Yes, but exit solely connects to at-grade non-BFA footpath or staircase","YesExit" },
            { "Full Day","FULL" },
            { "Specific Time","CUSTOM" },
            {"AM","AM" },
            {"PM","PM" },
        };
        static EditModels()
        {
         

            var bkdsettingmap = new[] { 
                Ux.AsPairExpr<EditSettingModel,CoreSetting>(e=> e.updatedAt,null), 
                Ux.AsPairExpr<EditSettingModel, CoreSetting>(e => e.updatedBy, null) 
            };
            Ux.GiveBind(bkdsettingmap);
            Ux.GiveBind<CoreSetting, EditSettingModel>();



        }
        public static  string RevertValue(string desc)
        {
            var foundpair = new KeyValuePair<string, string>(desc, desc);
            if (_KEYVALS.ContainsKey(desc) || _KEYVALS.ContainsValue(desc))
            {
                foundpair = _KEYVALS.First(e => e.Key == desc || e.Value == desc);
            }
            return foundpair.Value;


        }




    }

    public class LGNPasswordInput
    {
        public string EncPassword { get; set; }
        public string UserId { get; set; }
    }

    
    public class HomeModel
    {
        public List<SystemLinkModel> Links { get; set; }
        public string[] Groups { get; set; }



    }

    public class SystemLinkModel
    {
        public string LinkGroup { get; set; }
        public string LinkName { get; set; }

        public string ImagePrefix { get; set; }

        public int iconindex { get; set; }
        public string Url { get; set; }
    }
    

    [MetadataType(typeof(SettingEdit_MetaData))]
    public class EditSettingModel: IEditSettingModel
    {
        public int Id { get; set; }
        public string SettingId { get; set; }
        public string SettingValue { get; set; }

        public bool CanEdit { get; set; }

        public Nullable<System.DateTime> updatedAt { get; set; }
        public string updatedBy { get; set; }


    }



    public class TabHeaderModel : ITabModel
    {
        public int MaxCount { get; set; }
        public string[] Titles { get; set; }
    }



    public class UrlModel : IUrlModel
    {
        public string UrlTitle { get; set; }

        public int MaxCount { get; set; }

        public string BaseUrl { get; set; }
        public IUrl[] Urls { get; set; }

        public int InitStart { get; set; }

        public IUrlModel ExtractModel(int init, int len)
        {

            return new UrlModel
            {
                Urls = this.Urls,
                BaseUrl = this.BaseUrl,
                InitStart = init,
                MaxCount = len,
                UrlTitle = this.UrlTitle
            };
        }

    }

    public class UrlItem: IUrl
    {
        public string Url { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public int Thumb { get; set; }
    }

}
