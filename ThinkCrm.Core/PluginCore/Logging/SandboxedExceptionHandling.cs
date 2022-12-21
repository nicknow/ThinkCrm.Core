using System;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace ThinkCrm.Core.PluginCore.Logging
{
    public class SandboxedExceptionHandling
    {
        /// <summary>
        /// This utility method can be used for retrieving details of exeception objects when Reflection is prohibited by Sandbox.
        /// </summary>
        /// <param name="e">Exception.</param>        
        public static string GetExtendedExceptionDetails(object e, string indent = null)
        {            

            try
            {
                if (e == null) throw new ArgumentNullException(nameof(e));

                var sb = new StringBuilder(indent);
                sb.AppendLine($"{indent}SandboxedExceptionHandling");
                sb.AppendLine($"{indent}Type: {e.GetType().FullName}");

                if (e is AggregateException)
                {
                    var ex = (AggregateException) e;
                    sb.Append($"{indent}AggregateException.").AppendLine();
                    sb.Append($"{indent}Type: {ex.GetType().Name}").AppendLine();
                    sb.Append($"{indent}Message: {ex.Message}").AppendLine();
                    sb.Append($"{indent}Stack Trace: {ex.StackTrace}").AppendLine();
                    sb.Append($"{indent}Aggregate Exceptions ({ex.InnerExceptions?.Count}").AppendLine();
                    if (ex.InnerExceptions != null && ex.InnerExceptions.Any()) ex.InnerExceptions.ToList().ForEach(x => GetExtendedExceptionDetails(x, "   " + indent));
                }
                else if (e is FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>)
                {
                    var ex = (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>) e;
                    sb.Append($"{indent}FaultException").AppendLine();
                    sb.Append($"{indent}Timestamp: {ex.Detail.Timestamp}").AppendLine();
                    sb.Append($"{indent}Code: {ex.Detail.ErrorCode}").AppendLine();
                    sb.Append($"{indent}Message: {ex.Detail.Message}").AppendLine();
                    sb.Append($"{indent}Inner Fault: {(ex.Detail.InnerFault != null ? ex.Detail.InnerFault.Message : "(No Inner Exception)")}").AppendLine();
                    if (ex.Detail.InnerFault != null)
                        sb.Append(GetExtendedExceptionDetails(ex.Detail.InnerFault, "   " + indent)).AppendLine();

                }
                else if (e is TimeoutException)
                {
                    var ex = (TimeoutException) e;
                    sb.Append($"{indent}Timeout Exception").AppendLine();
                    sb.Append($"{indent}Message: {ex.Message}").AppendLine();
                    sb.Append($"{indent}Stack Trace: {ex.StackTrace}").AppendLine();
                    sb.Append($"{indent}Inner Fault: {(ex.InnerException?.Message ?? "(No Inner Exception)")}").AppendLine();
                    if (ex.InnerException != null)
                        sb.Append(GetExtendedExceptionDetails(ex.InnerException, "  " + indent)).AppendLine();
                }
                else if (e is Exception)
                {
                    var ex = (Exception) e;
                    sb.Append($"{indent}Exception.").AppendLine();
                    sb.Append($"{indent}Type: {ex.GetType().Name}");
                    sb.Append($"{indent}Message: {ex.Message}").AppendLine();
                    sb.Append($"{indent}Stack Trace: {ex.StackTrace}").AppendLine();
                    sb.Append($"{indent}Inner Fault: {(ex.InnerException?.Message ?? "(No Inner Exception)")}").AppendLine();
                    if (ex.InnerException != null)
                        sb.Append(GetExtendedExceptionDetails(ex.InnerException, "  " + indent)).AppendLine();
                }
                else
                {
                    sb.Append(
                        $"SandboxExceptionHandling.GetExtendedExceptionDetails: Object type is not an Exception ({e.GetType()} / {e})");
                }

                return sb.ToString();
            }
            catch (Exception exception)
            {
                //log or swallow here
                return
                    $"GetExtendedExceptionDetails: Exception during logging of Exception message: {exception.Message}";
            }
        }
    }
}

