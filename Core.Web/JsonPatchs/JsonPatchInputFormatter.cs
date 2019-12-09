using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core.Web.JsonPatchs
{
    /// <summary>
    /// JsonPatch输入格式化工具
    /// </summary>
    public class JsonPatchInputFormatter : TextInputFormatter
    {
        /// <summary>
        /// JsonPatch输入格式化工具
        /// </summary>
        public JsonPatchInputFormatter()
        {
            this.SupportedEncodings.Add(UTF8EncodingWithoutBOM);
            this.SupportedEncodings.Add(UTF16EncodingLittleEndian);

            this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json-patch+json"));
        }

        /// <summary>
        /// 返回是否可以读取
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool CanRead(InputFormatterContext context)
        {
            return context.ModelType == typeof(JsonPatchDocument);
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
        {
            var stream = encoding.CodePage == Encoding.UTF8.CodePage ?
                context.HttpContext.Request.Body :
                new TranscodingReadStream(context.HttpContext.Request.Body, encoding);

            try
            {
                var options = context.HttpContext.RequestServices.GetService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;
                var operations = await JsonSerializer.DeserializeAsync<JsonPatchOperation[]>(stream, options);

                var model = new JsonPatchDocument(operations);
                return InputFormatterResult.Success(model);
            }
            catch (Exception ex)
            {
                var logger = context.HttpContext.RequestServices.GetService<ILogger<JsonPatchInputFormatter>>();
                logger.LogError(ex, ex.Message);
                return InputFormatterResult.Failure();
            }
            finally
            {
                if (stream is TranscodingReadStream transcoding)
                {
                    await transcoding.DisposeAsync();
                }
            }
        }
    }
}
