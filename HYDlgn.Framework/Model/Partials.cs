using HYDlgn.Framework.metadata;
using System.ComponentModel.DataAnnotations;
using HYDlgn.Abstraction;
using System.ComponentModel.DataAnnotations.Schema;

namespace HYDlgn.Framework
{


    [MetadataType(typeof(SettingEdit_MetaData))]
    public partial class CoreSetting: IEditSettingModel
    {

    }



}
