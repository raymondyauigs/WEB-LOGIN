using BootstrapTable.Pager;
using HYDlgn.Abstraction;
using HYDlgn.Framework.AppModel;
using HYDlgn.jobweb.AppModels;
using HYDlgn.jobweb.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HYDlgn.jobweb.Tools;
using HYDlgn.Framework.Tools;
using DocumentFormat.OpenXml.Bibliography;
using HYDlgn.Framework.Model;
using HYDlgn.Framework;
using System.Net.Http;
using HYDlgn.Service;
using DocumentFormat.OpenXml.Office2010.Excel;


namespace HYDlgn.jobweb.Controllers
{
    
    public class AccountController : BaseController
    {
        [JWTAuth(level = 0)]
        [HttpGet]
        public ActionResult Index()
        {
            return Index(new QyUserModel());
        }

        //[Authorize]
        [HttpPost]
        [JWTAuth(level = 0)]
        [ValidateAntiForgeryToken]
        public ActionResult Index(QyUserModel query)
        {
            ViewBag.LevelNames = userService.GetLevels().ToList();

            if (string.IsNullOrEmpty(query.AskUserName))
            {
                var rawquery = _db.CoreUsers.Select(x => new EditUserModel { Id = x.Id, UserName = x.UserName, Person = x.Person, Level = x.level, IsAdmin = x.IsAdmin, IsPowerUser = x.level ==9, CreatedAt = x.createdAt, UpdatedAt = x.updatedAt, Division = x.Division, Disabled = x.Disabled, IsReset = x.IsReset, Email = x.email, Post = x.post, IsVIP= x.level==6 }).ToList();

                

                if (!query.ShowAll)
                    rawquery = rawquery.Where(e => !e.Disabled).ToList();
                var dict = new List<JSort>();
                if (!string.IsNullOrEmpty(query.sort))
                {

                    var sort = new JSort { Order = query.sort.EndsWith("+") ? "asc" : "desc", Column = query.sort.Substring(0, query.sort.Length - 1) };
                    dict.Add(sort);
                    dict.Add(new JSort { Column = nameof(EditUserModel.UserName), Order = "asc" });

                }
                else
                {
                    dict.Add(new JSort { Column = nameof(EditUserModel.Division), Order = "asc" });
                    dict.Add(new JSort { Column = nameof(EditUserModel.Post), Order = "asc" });
                }
                query.totalRecords = rawquery.Count;
                if ((query.page - 1) * 10 > query.totalRecords)
                {
                    query.page = query.totalRecords / 10;
                }
                query.Records = rawquery.BuildOrderBys(JSONtoDynamicHelper.SaveDictionaryToJSON(dict)).ToPagerList(query.page);
            }
            else
            {

                var rawquery = _db.CoreUsers.Where(y => y.UserName.Contains(query.AskUserName) || y.Person.Contains(query.AskUserName) || y.UserId.Contains(query.AskUserName) || y.post.Contains(query.AskUserName)).Select(x => new EditUserModel { Id = x.Id, UserName = x.UserName, Person = x.Person, Level = x.level, IsAdmin = x.IsAdmin, IsPowerUser= x.level ==9 , CreatedAt = x.createdAt, UpdatedAt = x.updatedAt, Email = x.email, Division = x.Division, Disabled = x.Disabled, IsReset = x.IsReset, Post = x.post, IsVIP = x.level==6 || x.IsAdmin }).ToList();

                
                if (!query.ShowAll)
                    rawquery = rawquery.Where(e => !e.Disabled).ToList();

                var dict = new List<JSort>();
                if (!string.IsNullOrEmpty(query.sort))
                {

                    var sort = new JSort { Order = query.sort.EndsWith("+") ? "asc" : "desc", Column = query.sort.Substring(0, query.sort.Length - 1) };
                    dict.Add(sort);
                    dict.Add(new JSort { Column = nameof(EditUserModel.UserName), Order = "asc" });

                }
                else
                {
                    dict.Add(new JSort { Column = nameof(EditUserModel.Division), Order = "asc" });
                    dict.Add(new JSort { Column = nameof(EditUserModel.Post), Order = "asc" });
                }

                query.totalRecords = rawquery.Count;

                if ((query.page - 1) * 10 > query.totalRecords)
                {
                    query.page = query.totalRecords / 10;
                }

                query.Records = rawquery.BuildOrderBys(JSONtoDynamicHelper.SaveDictionaryToJSON(dict)).ToPagerList(query.page);



            }
            return View(query);

            
        }




        [HttpGet]
        //[Authorize]
        [JWTAuth(level = 0)]
        public ActionResult Delete(int Id)
        {
            var accItem = _db.CoreUsers.FirstOrDefault(y => y.Id == Id);
            if (accItem == null)
            {
                return Json(new { status = "failure", msg = $"The user id {Id} does not exist!" }, JsonRequestBehavior.AllowGet);
            }

            accItem.updatedAt = DateTime.Now;
            accItem.updatedBy = AppManager.UserState.UserID;

            _db.Entry(accItem).State = System.Data.Entity.EntityState.Modified;
            accItem.Disabled = !accItem.Disabled;
            //_db.DFAUsers.Remove(accItem);
            _db.SaveChanges();

            return Json(new { status = "success", msg = "", returnUrl = Url.Action("Index", "Account") }, JsonRequestBehavior.AllowGet);
        }





        [JWTAuth(level =99999)]
        [HttpGet]
        public ActionResult ChangePassword(string ReturnUrl = null)
        {
            ViewBag.ContentWidth = "";
            var userid = AppManager.UserState.UserID;
            var changeuser = _db.CoreUsers.FirstOrDefault(y => y.UserId == userid && !y.Disabled);
            ViewBag.RedirectUrl = changeuser.IsAdmin ? Url.Action("Index", "Account") : Url.Action("Index", "Home");

            var model = new ChangePasswordModel { Id = changeuser.Id, UserName = changeuser.UserId, ReturnUrl = ReturnUrl ?? Request.UrlReferrer.PathAndQuery };
            return View(model);
        }

        [JWTAuth(level = 99999)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            ViewBag.ContentWidth = "";
            if (ModelState.IsValid)
            {
                var changeuser = _db.CoreUsers.FirstOrDefault(y => y.Id == model.Id);



                var passwed = userService.ChangeUserPassword(changeuser.UserId, model.Confirmpwd, User.Identity.Name, model.OldPwd);
                if (passwed)
                    return RedirectToAction("Index","Home");
                if (changeuser.Disabled)
                {
                    ModelState.AddModelError("OldPwd", "The account is disabled");
                }
                else
                {
                    ModelState.AddModelError("OldPwd", "Old password does not match");
                }

                return View(model);
            }
            else
            {

                return View(model);
            }


        }


        //[Authorize]
        [JWTAuth( level = 0)]
        [HttpGet]
        public ActionResult ChangePwd(int Id)
        {
            ViewBag.ContentWidth = "";
            var changeuser = _db.CoreUsers.FirstOrDefault(y => y.Id == Id);

            var model = new ChangePwdModel { Id = changeuser.Id, UserName = changeuser.UserName };
            return View(model);
        }

        //[Authorize]
        [JWTAuth( level = 0)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePwd(ChangePwdModel model)
        {
            ViewBag.ContentWidth = "";
            var changeuser = _db.CoreUsers.FirstOrDefault(y => y.Id == model.Id);

            if (ModelState.IsValid)
            {
                if (model.Pwd != model.Confirmpwd)
                {
                    ModelState.AddModelError("Confirmpwd", "The password is not matched!");

                    return View(model);
                }


                userService.TransactionNow(()=> userService.ChangeUserPassword(changeuser.UserId, model.Confirmpwd, User.Identity.Name),"Reset Password");



            }
            else
            {
                return View(model);
            }

            return Redirect("/Account");
            
        }


        [HttpGet]
        [JWTAuth(false)]
        public ActionResult Login(string returnUrl=null,bool needClear=false)
        {
            ViewBag.ContentWidth = "";

            if (needClear)
            {
                if(AppManager.UserState!=null)
                {
                    //messageService.SeenFor(AppManager.UserState.UserID, AppManager.UserState.UserName);

                    //bool found = false;
                    //foreach(var foundmsg in _db.DFAMessages.Where(e => e.UserId == AppManager.UserState.UserID))
                    //{
                    //    if (foundmsg != null)
                    //    {
                    //        found = true;
                    //        foundmsg.Seen = true;
                    //        foundmsg.UpdatedBy = AppManager.UserState.UserID;
                    //        foundmsg.UpdatedAt = DateTime.Now;
                    //        _db.Entry(foundmsg).State = System.Data.Entity.EntityState.Modified;

                            
                    //    }
                    //}
                    //if (found)
                    //    _db.SaveChanges();

                }

                var cookie = new HttpCookie(Constants.Setting.AuthorizeCookieKey, "");
                HttpContext.Response.Cookies.Add(cookie);
                Session[Constants.Session.UserName] = null;
                Session[Constants.Session.UserId] = null;
                Session[Constants.Session.UserLevel] = null;
                Session[Constants.Session.Message] = null;
                Session[Constants.Session.MessageCount] = null;
                Session[Constants.Session.IsBSGuy] = null;
                Session[Constants.Session.UserEmail] = null;
                

            }


            return View(new LoginModel { returnUrl=returnUrl });
        }

        [HttpPost]
        [JWTAuth(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string account, string password,string returnUrl)
        {
            //var user = userService.EditUser("001", account,AppManager.WindowUser, password);


            var user = _db.CoreUsers.FirstOrDefault(y => (y.UserName == account || y.UserId == account || y.post == account) && !y.Disabled);

            if (userService.LoginUser(account, password, AppManager.WindowUser))
            {

                var cookie = new HttpCookie(Constants.Setting.AuthorizeCookieKey, JWTHelper.GenerateToken(user.UserId, user.UserName, user.level, user.post, user.IsAdmin, user.Division, user.email));
                HttpContext.Response.Cookies.Add(cookie);



                if (user.IsReset)
                    return Json(new { returnUrl = Url.Action("ChangePassword", "Account", new { ReturnUrl = returnUrl }) }, JsonRequestBehavior.AllowGet);
                if (!user.IsAdmin || (returnUrl != null && returnUrl.Contains("/Login")))
                {
                    return Json(new { returnUrl = Url.Action("Index", "Home") }, JsonRequestBehavior.AllowGet);
                }



                return Json(new { returnUrl = returnUrl }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var error = user == null ? "User not found!" : (user.Disabled ? "The account is disabled. Please contact your Division Administrator!" : "Password is not correct!");
                var cookie = new HttpCookie(Constants.Setting.AuthorizeCookieKey, "");
                HttpContext.Response.Cookies.Add(cookie);
                return Json(error, JsonRequestBehavior.AllowGet);
            }
        }
    }
}