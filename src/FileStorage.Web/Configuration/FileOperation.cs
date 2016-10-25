using Swashbuckle.Swagger.Model;
using Swashbuckle.SwaggerGen.Generator;

namespace FileStorage.Web.Configuration
{
    /// <summary>
    /// Utill for swagger documentation
    /// </summary>
    public class FileOperation : IOperationFilter
    {
        /// <summary>
        /// Allow to add files in swagger doc
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.OperationId.ToLower() == "apifileuploadpost")
            {
                operation.Parameters.Clear();//Clearing parameters
                operation.Parameters.Add(new NonBodyParameter
                {
                    Name = "File",
                    In = "formData",
                    Description = "Uplaod Image",
                    Required = true,
                    Type = "file"
                });
                operation.Consumes.Add("application/form-data");
            }
        }
    }
}
