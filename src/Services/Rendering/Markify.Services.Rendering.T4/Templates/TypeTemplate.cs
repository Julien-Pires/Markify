﻿// ------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version: 15.0.0.0
//  
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
// ------------------------------------------------------------------------------
namespace Markify.Services.Rendering.T4.Templates
{
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using static System.Globalization.CultureInfo;
    using Microsoft.VisualStudio.TextTemplating;
    using Markify.Domain.Compiler;
    using System;
    
    /// <summary>
    /// Class to produce the template output
    /// </summary>
    
    #line 1 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public partial class TypeTemplate : T4TemplateBase
    {
#line hidden
        /// <summary>
        /// Create the template output
        /// </summary>
        public override string TransformText()
        {
            
            #line 9 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
 var definition = (TypeDefinition)Session["Content"]; 
            
            #line default
            #line hidden
            this.Write("### **");
            
            #line 10 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(DefinitionFormatter.GetNameWithParameters(definition.Identity))));
            
            #line default
            #line hidden
            this.Write("** : ");
            
            #line 10 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefinitionFormatter.GetAccessModifiers(definition)));
            
            #line default
            #line hidden
            this.Write(" ");
            
            #line 10 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefinitionFormatter.GetKind(definition)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 11 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
 
	var summary = DefinitionFormatter.GetTypeComment(definition, c => c.Summary);
	if(summary.IsSome())
	{

            
            #line default
            #line hidden
            
            #line 16 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(summary.Value.GetText()));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 17 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}

	var remarks = DefinitionFormatter.GetTypeComment(definition, c => c.Remarks);
	if(remarks.IsSome())
	{

            
            #line default
            #line hidden
            this.Write(">**Remarks**\r\n>\r\n");
            
            #line 26 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.ToBlockquote(remarks.Value.GetText().Trim())));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 27 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}

            
            #line default
            #line hidden
            this.Write("***\r\n**Assembly**: ");
            
            #line 31 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefinitionFormatter.GetNamespace(definition)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 32 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	var modifiers = DefinitionFormatter.GetModifiers(definition);
	if(!string.IsNullOrWhiteSpace(modifiers))
	{

            
            #line default
            #line hidden
            this.Write("\r\n**Modifiers**: ");
            
            #line 38 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(modifiers));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 39 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}

	var parents = DefinitionFormatter.GetParents(definition);
	if(!string.IsNullOrWhiteSpace(parents))
	{

            
            #line default
            #line hidden
            this.Write("\r\n**Implements**: ");
            
            #line 47 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(parents)));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 48 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}

	var typeParameters = DefinitionFormatter.GetTypeComment(definition, c => c.TypeParameters);
	if(typeParameters.Any())
	{

            
            #line default
            #line hidden
            this.Write("\r\n**Type parameters**:\r\n");
            
            #line 57 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

		foreach(var param in typeParameters)
		{
			var name = param.Parameter.FirstOrDefault(c => c.Name == "name");
			if(name != null)
			{

            
            #line default
            #line hidden
            this.Write("* ");
            
            #line 64 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(name.Value.Value));
            
            #line default
            #line hidden
            this.Write(" : ");
            
            #line 64 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(param.GetText()));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 65 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

			}
		}
	}

	var returnType = DefinitionFormatter.GetReturnType(definition);
	if(returnType.HasValue)
	{

            
            #line default
            #line hidden
            this.Write("\r\n**Returns**: ");
            
            #line 75 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(returnType.ValueOr(string.Empty))));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 76 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}

/****** Display type fields ******/
var fields = DefinitionFormatter.GetFields(definition);
if(fields.Any())
{

            
            #line default
            #line hidden
            this.Write("***\r\n## **Fields**\r\n");
            
            #line 86 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
	
	foreach(var group in fields)
	{

            
            #line default
            #line hidden
            this.Write("### **");
            
            #line 90 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CurrentCulture.TextInfo.ToTitleCase(group.Key)));
            
            #line default
            #line hidden
            this.Write("**\r\n| Name  | Access |\r\n|---|---|\r\n");
            
            #line 93 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
		foreach(var field in group.OrderBy(c => c.Name))
		{
			var isReadOnly = field.Modifiers.Any(c => string.Equals(c, "readonly", StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(c, "const", StringComparison.CurrentCultureIgnoreCase));

            
            #line default
            #line hidden
            
            #line 98 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(field.Name));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 98 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(isReadOnly ? "Read-Only" : string.Empty));
            
            #line default
            #line hidden
            this.Write(" |\r\n");
            
            #line 99 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

		}
	}
}

/****** Display type properties ******/
var properties = DefinitionFormatter.GetProperties(definition);
if(properties.Any())
{

            
            #line default
            #line hidden
            this.Write("\r\n## **Properties**\r\n");
            
            #line 111 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
	
	foreach(var group in properties)
	{

            
            #line default
            #line hidden
            this.Write("### **");
            
            #line 115 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CurrentCulture.TextInfo.ToTitleCase(group.Key)));
            
            #line default
            #line hidden
            this.Write("**\r\n| Name  | Access |\r\n|---|---|\r\n");
            
            #line 118 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
		foreach(var property in group.OrderBy(c => c.Name))
		{

            
            #line default
            #line hidden
            
            #line 121 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(property.Name));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 121 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(DefinitionFormatter.GetPropertyAccess(property)));
            
            #line default
            #line hidden
            this.Write(" |\r\n");
            
            #line 122 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

		}
	}
}

/****** Display type events ******/
var events = DefinitionFormatter.GetEvents(definition);
if(events.Any())
{

            
            #line default
            #line hidden
            this.Write("\r\n## **Events**\r\n");
            
            #line 134 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
	
	foreach(var group in events)
	{

            
            #line default
            #line hidden
            this.Write("### **");
            
            #line 138 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CurrentCulture.TextInfo.ToTitleCase(group.Key)));
            
            #line default
            #line hidden
            this.Write("**\r\n| Name  | Type |\r\n|---|---|\r\n");
            
            #line 141 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
		foreach(var ev in group.OrderBy(c => c.Name))
		{

            
            #line default
            #line hidden
            
            #line 144 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(ev.Name));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 144 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(ev.Type)));
            
            #line default
            #line hidden
            this.Write(" |\r\n");
            
            #line 145 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

		}
	}
}

/****** Display type events ******/
var methods = DefinitionFormatter.GetMethods(definition);
if(methods.Any())
{

            
            #line default
            #line hidden
            this.Write("\r\n## **Methods**\r\n");
            
            #line 157 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
	
	foreach(var group in methods)
	{

            
            #line default
            #line hidden
            this.Write("### **");
            
            #line 161 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(CurrentCulture.TextInfo.ToTitleCase(group.Key)));
            
            #line default
            #line hidden
            this.Write("**\r\n| Name  | Parameters | Return\r\n|---|---|---|\r\n");
            
            #line 164 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
		
		var orderedMethods = group.OrderBy(c => c.Identity.Name).ThenBy(c => c.Parameters.Count());
		foreach(var method in orderedMethods)
		{
			var methodParameters = String.Join(", ", method.Parameters.Select(c => c.Type));

            
            #line default
            #line hidden
            
            #line 170 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(DefinitionFormatter.GetNameWithParameters(method.Identity))));
            
            #line default
            #line hidden
            this.Write(" | (");
            
            #line 170 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(methodParameters)));
            
            #line default
            #line hidden
            this.Write(") | ");
            
            #line 170 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(method.ReturnType)));
            
            #line default
            #line hidden
            this.Write(" |\r\n");
            
            #line 171 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

		}
	}
}

/****** Display type enum values ******/
var values = DefinitionFormatter.GetEnumValues(definition);
if(values.Any())
{

            
            #line default
            #line hidden
            this.Write("\r\n## **Values**\r\n| Name  | Value |\r\n|---|---|\r\n");
            
            #line 185 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	foreach(var enumValue in values)
	{

            
            #line default
            #line hidden
            
            #line 189 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(enumValue.Name));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 189 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(enumValue.Value.GetValueOrDefault(string.Empty)));
            
            #line default
            #line hidden
            this.Write(" |\r\n");
            
            #line 190 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}
}

/****** Display delegate parameters ******/
var parameters = DefinitionFormatter.GetParameters(definition);
if(parameters.Any())
{

            
            #line default
            #line hidden
            this.Write("\r\n## **Parameters**\r\n| Name  | Type | Modifier | Default |\r\n|---|---|---|---|\r\n");
            
            #line 203 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	foreach(var delegateParameters in parameters)
	{

            
            #line default
            #line hidden
            
            #line 207 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(delegateParameters.Name));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 207 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(MarkdownHelper.EscapeString(delegateParameters.Type)));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 207 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(delegateParameters.Modifier.GetValueOrDefault(string.Empty)));
            
            #line default
            #line hidden
            this.Write(" | ");
            
            #line 207 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(delegateParameters.DefaultValue.GetValueOrDefault(string.Empty)));
            
            #line default
            #line hidden
            this.Write(" |\r\n");
            
            #line 208 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}
}

var example = DefinitionFormatter.GetTypeComment(definition, c => c.Example);
if(example.IsSome())
{

            
            #line default
            #line hidden
            this.Write("\r\n## **Example**\r\n\r\n");
            
            #line 219 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"
            this.Write(this.ToStringHelper.ToStringWithCulture(example.Value.GetText()));
            
            #line default
            #line hidden
            this.Write("\r\n");
            
            #line 220 "D:\Users\Takumi\Documents\Projects\Markify\src\Services\Rendering\Markify.Services.Rendering.T4\Templates\TypeTemplate.tt"

	}

            
            #line default
            #line hidden
            return this.GenerationEnvironment.ToString();
        }
    }
    
    #line default
    #line hidden
}
