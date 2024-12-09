﻿using HYDlgn.Abstraction;
using HYDlgn.Framework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace HYDlgn.Service
{
    public abstract class BaseDbService
    {
        protected HYDlgnEntities db;
        protected IMiscLog log;


        public virtual bool TransactionNow(Func<bool> doIt,string label=null)
        {
            DbContextTransaction scope = null;
            try
            {
                scope= db.Database.BeginTransaction();
                if(!doIt())
                {
                    log.LogMisc($"the intended transaction does not work out for {label}!");
                    scope.Rollback();
                    return false;
                }
                scope.Commit();
                return true;
            }
            catch(DbUpdateException ex) {
                log.LogMisc(ex.Message, ex);
                if(scope != null)
                {
                    scope.Rollback();
                }
                return false;
            }
        }
    }
}
