using Autofac;
using HYDlgn.Abstraction;
using HYDlgn.Service;
using HYDlgn.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using Microsoft.Extensions.Caching.Memory;
using HYDlgn.Framework.AppModel;
using HYDlgn.jobweb.Models;

namespace HYDlgn.jobweb.Service
{
    public class PersistentModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {


            builder.RegisterInstance(new MemoryCache(new MemoryCacheOptions())).As<IMemoryCache>().SingleInstance();
            builder.RegisterInstance(EditModels.Default).As<EditModels>().SingleInstance();
            builder.RegisterType<ErrorLog>().As<IErrorLog>().InstancePerLifetimeScope();
            builder.RegisterType<MiscLog>().As<IMiscLog>().InstancePerLifetimeScope();
            builder.RegisterType<StdbLog>().As<IStdbLog>().InstancePerLifetimeScope();            
            builder.RegisterType<HYDlgnEntities>().WithParameter(ConnectionStringParameter.Create()).InstancePerLifetimeScope();
            builder.RegisterType<SettingsService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();


        }
    }
}