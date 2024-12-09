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

namespace HYDlgn.jobweb.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            //Test ZipperOne
            //var outpath = WK.exportoutputpath.GetAppKeyValue().Trim('\\');
            //var bwspath =Server.MapPath( WK.browsepath.GetAppKeyValue());
            //var files = new[] { "file1.pdf", "file2.pdf", "file3.pdf" }.Select(e => Path.Combine(bwspath, "Upload", e)).ToArray();

            //ZipperOne.Create(Path.Combine(outpath, "files.zip"),"", files);


            return View();
        }
    }
}