//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HYDlgn.Framework
{
    using System;
    using System.Collections.Generic;
    
    public partial class CoreUser
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Person { get; set; }
        public string EncPassword { get; set; }
        public bool Disabled { get; set; }
        public Nullable<System.DateTime> loginedAt { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsReset { get; set; }
        public System.DateTime updatedAt { get; set; }
        public string updatedBy { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
        public int level { get; set; }
        public string post { get; set; }
        public string Division { get; set; }
        public string AdminScope { get; set; }
        public string email { get; set; }
    }
}
