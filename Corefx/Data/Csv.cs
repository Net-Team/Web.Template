using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data
{
    /// <summary>
    /// 表示Csv文件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Csv<T>
    {
        /// <summary>
        /// 字段
        /// </summary>
        private readonly List<IField> fields = new List<IField>();

        /// <summary>
        /// 获取数据模型
        /// </summary>
        public IEnumerable<T> Models { get; private set; }

        /// <summary>
        /// 数据转换为Excel
        /// </summary>
        /// <param name="models">数据</param>
        public Csv(IEnumerable<T> models)
        {
            this.Models = models ?? throw new ArgumentNullException(nameof(models));
        }

        /// <summary>
        /// 添加字段
        /// </summary>
        /// <typeparam name="TField">字段类型</typeparam>   
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        public Csv<T> AddField<TField>(string name, Func<T, TField> value)
        {
            var field = new Field<TField>(name, value);
            this.fields.Add(field);
            return this;
        }


        /// <summary>
        /// 保存为csv格式
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        public async Task SaveAsCsvAsync(string filePath, Encoding encoding = default)
        {
            var path = Path.GetDirectoryName(filePath);
            if (string.IsNullOrEmpty(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            using var stream = new FileStream(filePath, FileMode.Create);
            await this.SaveAsCsvAsync(stream, encoding);
        }

        /// <summary>
        /// 保存为csv格式
        /// </summary>
        /// <param name="stream">流</param>
        /// <param name="encoding">编码</param>
        public async Task SaveAsCsvAsync(Stream stream, Encoding encoding = default)
        {
            if (encoding == default)
            {
                encoding = Encoding.UTF8;
            }

            using var writer = new StreamWriter(stream, encoding, leaveOpen: true);
            var fieldItem = this.fields.Select(item => item.Name);
            var head = string.Join(",", fieldItem);
            await writer.WriteLineAsync(head);

            foreach (var item in this.Models)
            {
                var lineItems = this.fields.Select(f => f.GetValue(item)?.ToString());
                var line = string.Join(",", lineItems);
                await writer.WriteLineAsync(line);
            }
        }

        /// <summary>
        /// 字段接口
        /// </summary>
        private interface IField
        {
            string Name { get; }
            object GetValue(T model);
        }

        /// <summary>
        /// 表示字段信息
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        private class Field<TKey> : IField
        {
            private Func<T, TKey> valueFunc;

            public string Name { get; private set; }

            public Field(string name, Func<T, TKey> value)
            {
                this.Name = name;
                this.valueFunc = value;
            }

            public object GetValue(T model)
            {
                return this.valueFunc(model);
            }
        }
    }
}
