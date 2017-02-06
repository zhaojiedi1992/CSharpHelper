using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace CSharpHelperLib.ConvertExt
{
    public static class DataSetHelper
    {
        public static List<T> ToList<T>(this DataSet ds, int tableNum = 0) where T : class, new()
        {
            DataTable dataTable = ds.Tables[tableNum];
            List<T> list = new List<T>();
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                T t = (T)((object)Activator.CreateInstance(typeof(T)));
                PropertyInfo[] properties = t.GetType().GetProperties();
                PropertyInfo[] array = properties;
                for (int j = 0; j < array.Length; j++)
                {
                    PropertyInfo propertyInfo = array[j];
                    for (int k = 0; k < dataTable.Columns.Count; k++)
                    {
                        if (propertyInfo.Name.Equals(dataTable.Columns[k].ColumnName))
                        {
                            if (dataTable.Rows[i][k] != DBNull.Value)
                            {
                                propertyInfo.SetValue(t, dataTable.Rows[i][k], null);
                            }
                            else
                            {
                                propertyInfo.SetValue(t, null, null);
                            }
                            break;
                        }
                    }
                }
                list.Add(t);
            }
            return list;
        }
    }
}
