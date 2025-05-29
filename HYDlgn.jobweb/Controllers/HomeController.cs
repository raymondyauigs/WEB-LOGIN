using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HYDlgn.Abstraction;
using WK = HYDlgn.Abstraction.Constants.WebKey;
using DT = HYDlgn.Abstraction.Constants.DT;
using DK = HYDlgn.Abstraction.Constants.DataKey;
using System.IO;
using HYDlgn.jobweb.Service;
using HYDlgn.Framework.AppModel;

namespace HYDlgn.jobweb.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
        [JWTAuth]
        public ActionResult Index()
        {
            //Test ZipperOne
            //var outpath = WK.exportoutputpath.GetAppKeyValue().Trim('\\');
            //var bwspath =Server.MapPath( WK.browsepath.GetAppKeyValue());
            //var files = new[] { "file1.pdf", "file2.pdf", "file3.pdf" }.Select(e => Path.Combine(bwspath, "Upload", e)).ToArray();

            //ZipperOne.Create(Path.Combine(outpath, "files.zip"),"", files);
            var model = new HomeModel();
            model.Groups = new[] { "B", "S", "T" };
            model.Links = new List<SystemLinkModel>();
            model.Links.AddRange(sttService.GetSettingFor(DK.SETT_COMMONPFX).Select(e => new SystemLinkModel { LinkName = e.Key, Url = e.Value, LinkGroup = "B", ImagePrefix="B" }));
            model.Links.AddRange(sttService.GetSettingFor(DK.SETT_SYSTEMPFX).Select(e => new SystemLinkModel { LinkName = e.Key, Url = e.Value, LinkGroup = "S", ImagePrefix="A" }));
            model.Links.AddRange(sttService.GetSettingFor(DK.SETT_PROJCTPFX).Select(e => new SystemLinkModel { LinkName = e.Key, Url = e.Value, LinkGroup = "T", ImagePrefix="S" }));

            return View(model);
        }
    }
}