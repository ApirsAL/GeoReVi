using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoReVi
{
    /// <summary>
    /// Converting a column name to its alias
    /// </summary>
    public class AliasDbColumnConverter : BaseValueConverter<AliasDbColumnConverter>
    {
        //From column name to alias
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            using (var db = new ApirsDatabase())
            {
                try
                {
                    string alias = (from a in db.tblAlias
                                 where a.alColumnName == (string)value
                                 select a.alAlias).First();

                    return (alias != "") ?  alias : value;
                }
                catch
                {
                    return value;
                }
            }
        }

        //From column name to alias
        public string Convert(string value)
        {
            using (var db = new ApirsDatabase())
            {
                try
                {
                    string alias = (from a in db.tblAlias
                                    where a.alColumnName.Contains((string)value)
                                    select a.alAlias).First();

                    return (alias != "") ? alias.Replace("\r\n", "").Replace("\n", "").Replace("\r", "") : value;
                }
                catch(Exception e)
                {
                    return value;
                }
            }
        }

        //Converting back again
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            using (var db = new ApirsDatabase())
            {
                try
                {
                    string column = (from a in db.tblAlias
                                    where a.alAlias == (string)value
                                    select a.alColumnName).First();

                    return (column != "") ? column : value;
                }
                catch
                {
                    return value;
                }
            }
        }
    }

}
