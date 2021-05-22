using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Text.RegularExpressions;

namespace ChartsMicroservice
{
    public class SwaggerDefaultValues : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			var apiDescription = context.ApiDescription;
			operation.Deprecated |= apiDescription.IsDeprecated();
            operation.Description = Formatted(operation.Description);
            operation.Summary = Formatted(operation.Summary);

            if (operation.Parameters == null)
				return;

			// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
			// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
			foreach (var parameter in operation.Parameters)
			{
				var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
				if (parameter.Description == null)
				{
					parameter.Description = description.ModelMetadata?.Description;
				}

				if (parameter.Schema.Default == null && description.DefaultValue != null)
				{
					parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
				}

				parameter.Required |= description.IsRequired;
			}
		}

        private string Formatted(string text)
        {
            if (text == null) return null;

            // Strip out the whitespace that messes up the markdown in the xml comments,
            // but don't touch the whitespace in <code> blocks. Those get fixed below.
            string resultString = Regex.Replace(text, @"(^[ \t]+)(?![^<]*>|[^>]*<\/)", "", RegexOptions.Multiline);
            resultString = Regex.Replace(resultString, @"<code[^>]*>", "<pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            resultString = Regex.Replace(resultString, @"</code[^>]*>", "</pre>", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
            resultString = Regex.Replace(resultString, @"<!--", "", RegexOptions.Multiline);
            resultString = Regex.Replace(resultString, @"-->", "", RegexOptions.Multiline);

            try
            {
                string pattern = @"<pre\b[^>]*>(.*?)</pre>";

                foreach (Match match in Regex.Matches(resultString, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline))
                {
                    var formattedPreBlock = FormatPreBlock(match.Value);
                    resultString = resultString.Replace(match.Value, formattedPreBlock);
                }
                return resultString;
            }
            catch
            {
                // Something went wrong so just return the original resultString
                return resultString;
            }
        }

        private string FormatPreBlock(string preBlock)
        {
            // Split the <pre> block into multiple lines
            var linesArray = preBlock.Split('\n');
            if (linesArray.Length < 2)
            {
                return preBlock;
            }
            else
            {
                // Get the 1st line after the <pre>
                string line = linesArray[1];
                int lineLength = line.Length;
                string formattedLine = line.TrimStart(' ', '\t');
                int paddingLength = lineLength - formattedLine.Length;

                // Remove the padding from all of the lines in the <pre> block
                for (int i = 1; i < linesArray.Length - 1; i++)
                {
                    linesArray[i] = linesArray[i].Substring(paddingLength);
                }

                var formattedPreBlock = string.Join("", linesArray);
                return formattedPreBlock;
            }
        }
    }
}
