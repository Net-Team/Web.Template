using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Core.Xls
{
    /// <summary>
    /// 表示xls表格
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class XlsSheet<TModel> : IEnumerable<TModel> where TModel : class, new()
    {
        /// <summary>
        /// 模型的属性描述
        /// </summary>
        private static readonly PropertyDescriptor[] fieldDescriptors = PropertyDescriptor.CreateDescriptors<TModel>();

        /// <summary>
        /// 数据
        /// </summary>
        private readonly List<TModel> datas;

        /// <summary>
        /// 获取表名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// xls表格
        /// </summary>
        /// <param name="dataTable">数据源</param>
        /// <exception cref="ArgumentException"></exception>
        public XlsSheet([NotNull]DataTable dataTable)
        {
            this.CheckDataTable(dataTable);
            this.datas = this.Parse(dataTable).ToList();
            this.Name = dataTable.TableName;
        }

        /// <summary>
        /// 检查表格
        /// </summary>
        /// <param name="dataTable"></param>
        /// <exception cref="ArgumentException"></exception>
        private void CheckDataTable(DataTable dataTable)
        {
            var names = dataTable.Columns.Cast<DataColumn>().Select(item => item.ColumnName).ToHashSet();
            if (names.Count > 0)
            {
                foreach (var field in fieldDescriptors)
                {
                    if (names.Contains(field.Name) == false)
                    {
                        throw new ArgumentException($"表格{dataTable.TableName}未声明列{field.Name}");
                    }
                }
            }
        }

        /// <summary>
        /// dataTable 2 model
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        private IEnumerable<TModel> Parse(DataTable dataTable)
        {
            for (var i = 0; i < dataTable.Rows.Count; i++)
            {
                var model = new TModel();
                foreach (var field in fieldDescriptors)
                {
                    var raw = dataTable.Rows[i][field.Name];
                    var value = field.ColumnParser.Parse(raw, field.PropertyType);
                    field.SetValue(model, value);
                }
                yield return model;
            }
        }


        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TModel> GetEnumerator()
        {
            return this.datas.GetEnumerator();
        }


        /// <summary>
        /// 返回迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// 表示模型字段描述
        /// </summary>
        private class PropertyDescriptor
        {
            /// <summary>
            /// 属性设置器
            /// </summary>
            private readonly Action<object, object> setter;

            /// <summary>
            /// 字段名称
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 获取属性名称
            /// </summary>
            public Type PropertyType { get; }

            /// <summary>
            /// 字段的值转换器
            /// </summary>
            public XlsColumnParser ColumnParser { get; private set; }

            /// <summary>
            /// 模型字段描述
            /// </summary>
            /// <param name="property"></param>
            private PropertyDescriptor(PropertyInfo property)
            {
                var xlsColumn = property.GetCustomAttribute<XlsColumnAttribute>();
                if (xlsColumn == null)
                {
                    this.Name = property.Name;
                    this.ColumnParser = new XlsColumnParser();
                }
                else
                {
                    this.Name = xlsColumn.Name ?? property.Name;
                    this.ColumnParser = Activator.CreateInstance(xlsColumn.ParserType) as XlsColumnParser;
                }

                this.PropertyType = property.PropertyType;
                this.setter = Lambda.CreateSetAction<object, object>(property);
            }

            /// <summary>
            /// 设置属性值
            /// </summary>
            /// <param name="instance">实例</param>
            /// <param name="value">属性值</param>
            public void SetValue(object instance, object value)
            {
                this.setter.Invoke(instance, value);
            }

            /// <summary>
            /// 创建成员描述
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static PropertyDescriptor[] CreateDescriptors<T>() where T : class
            {
                return typeof(T).GetProperties().Select(p => new PropertyDescriptor(p)).ToArray();
            }
        }
    }
}
