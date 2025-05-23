﻿using HYDlgn.Abstraction;
using HYDlgn.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DK= HYDlgn.Abstraction.Constants.DataKey;
using DT = HYDlgn.Abstraction.Constants.DT;
using HYDlgn.Framework.AppModel;
using NPOI.OpenXmlFormats.Encryption;
using NPOI.OpenXmlFormats.Dml.Diagram;

namespace HYDlgn.Service
{
    public class SettingsService : ISettingService
    {
        IMiscLog log;
        HYDlgnEntities db;
        public SettingsService(IMiscLog mlog,HYDlgnEntities mdb) { 
            log = mlog;
            db = mdb;
        }

        public IEnumerable<KeyValuePair<string, string>> GetSettingFor(string type,int target=0)
        {
            if(new[] { DK.SETT_COMMONPFX,DK.SETT_SYSTEMPFX ,DK.SETT_PROJCTPFX}.Contains(type))
            {
                var links = db.CoreSettings.Where(e => e.SettingId.StartsWith(type)).OrderBy(e=> e.SettingId).Select(e => e.SettingValue).ToArray();
                foreach(var l in links)
                {
                    var lvalue = l.ItSplit("|").ToArray();
                    yield return new KeyValuePair<string, string>(lvalue[0], lvalue[1]);

                }

            }
            else if(new[] { DK.SETT_LAYOUTNAME ,DK.SETT_BGNAME, DK.SETT_BGSUBNAME}.Contains(type))
            {
                var layout = db.CoreSettings.Where(e => e.SettingId == type).Select(e => e.SettingValue).FirstOrDefault();
                if (layout != null)
                {
                    yield return new KeyValuePair<string, string>(layout, layout);
                }
            }
            else if(type == DK.SETT_LOCATION)
            {
                var locations = db.CoreSettings.Where(e => e.SettingId == DK.SETT_LOCATION).Select(e=> e.SettingValue).FirstOrDefault();
                if(locations!=null)
                {
                    foreach(var l in locations.ItSplit("|").OrderBy(e=>e))
                    {
                        yield return new KeyValuePair<string, string>(l, l);
                    }
                }
            }
            else if(type==DK.SETT_YESNO)
            {
                yield return new KeyValuePair<string, string>(DT.YES,DT.YES);
                yield return new KeyValuePair<string, string>(DT.NO,DT.NO);

            }
            else if(type==DK.SETT_UPOSTTYPE)
            {
                var posts = db.CoreSettings.Where(e => e.SettingId == DK.SETT_UPOSTTYPE).Select(e => e.SettingValue).FirstOrDefault();
                if(posts!=null)
                {
                    foreach (var p in posts.ItSplit(",").OrderBy(e=>e))
                        yield return new KeyValuePair<string, string>(p, p);
                }

            }

            

        }
    }
}
