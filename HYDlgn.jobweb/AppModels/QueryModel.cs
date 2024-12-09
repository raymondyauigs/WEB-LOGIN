using HYDlgn.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using HYDlgn.Abstraction;
using BootstrapTable.Pager;
using HYDlgn.Framework.AppModel;

namespace HYDlgn.jobweb.AppModels
{
    public class QySettingModel : QuerySettingModel
    {
        public IPager<IEditSettingModel> Records { get; set; }
    }

    public class QyUserModel: QueryUserModel
    {

        public IPager<IEditUserModel> Records { get; set; }

    }

    

}