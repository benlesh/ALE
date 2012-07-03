using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ALE.Http
{
	public class Route
	{
		public static readonly Regex PathParser = new Regex(@"\:(\w+?)(\W|$)");
		public MethodInfo HandlerInfo;
		public readonly Dictionary<string, ParameterInfo> HandlerParameters;
		public readonly string Path;
		public readonly Regex PathTester;
		public readonly string[] Parameters;
		public readonly Type ControllerType;

		public Route(string path, string methodName, Type controllerType)
		{
			if (String.IsNullOrWhiteSpace(path)) throw new ArgumentNullException("path");
			if (String.IsNullOrWhiteSpace(methodName)) throw new ArgumentNullException("methodName");
			Path = path;
			Parameters = GetParameters(path);
			PathTester = CreatePathTester(path);
			ControllerType = controllerType;
			HandlerInfo = controllerType.GetMethod(methodName);
			HandlerParameters = HandlerInfo.GetParameters().ToDictionary(x => x.Name, x => x);
		}

		public static string[] GetParameters(string path)
		{
			var matches = PathParser.Matches(path);
			var results = new string[matches.Count];
			for (int i = 0; i < matches.Count; i++)
			{
				results[i] = matches[i].Groups[1].Value;
			}
			return results;
		}

		public static Regex CreatePathTester(string path)
		{
			var pattern = PathParser.Replace(path, m => "(?<" + m.Groups[1].Value + @">.+)" + m.Groups[2].Value);
			return new Regex(pattern);
		}

		public bool TryExecute(IContext context)
		{
		    var req = context.Request;
		    var res = context.Response;
			var path = req.Url.PathAndQuery;
			var isMatch = PathTester.IsMatch(path);
			if (isMatch)
			{
				var match = PathTester.Match(path);
				var args = new string[Parameters.Length];
				for (int i = 0; i < Parameters.Length; i++)
				{
					var parameterName = Parameters[i];
					args[i] = Uri.UnescapeDataString(match.Groups[parameterName].Value);
				}
				var controller = (IController) Activator.CreateInstance(ControllerType);
				controller.Request = req;
				controller.Response = res;
				controller.Context = context;
				var typedController = Convert.ChangeType(controller, ControllerType);
				var typedArgs = new object[args.Length];
				for (int i = 0; i < Parameters.Length; i++)
				{
					var paramKey = Parameters[i];
					var arg = args[i];
					var parameter = HandlerParameters[paramKey];
					typedArgs[i] = Convert.ChangeType(arg, parameter.ParameterType);
				}
				HandlerInfo.Invoke(typedController, typedArgs);
			}
			return isMatch;
		}
	}
}