using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpectedObjects
{
	public static class ObjectExtensions
	{
		public static string ToUsefulString(this object obj)
		{
			if (obj is IExpectedDescription)
				return obj.ToString();

			string str;
			if (obj == null) return "[null]";
			if (obj.GetType() == typeof (string))
			{
				str = (string) obj;

				return "\"" + str.Replace("\n", "\\n") + "\"";
			}
			if (obj.GetType().IsValueType) return "[" + obj + "]";

			if (obj is IEnumerable)
			{
				IEnumerable<object> enumerable = ((IEnumerable) obj).Cast<object>();

				return obj.GetType() + ":\n" + enumerable.EachToUsefulString();
			}

			str = obj.ToString();

			if (str == null || str.Trim() == "")
			{
				return String.Format("{0}:[]", obj.GetType());
			}

			str = str.Trim();

			if (str.Contains("\n"))
			{
				return string.Format(@"{1}:[{0}]", str.Tab(), obj.GetType());
			}

			if (obj.GetType().ToString() == str)
				return obj.GetType().ToString();

			return string.Format("{0}:[{1}]", obj.GetType(), str);
		}

		static string EachToUsefulString<T>(this IEnumerable<T> enumerable)
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			sb.Append(String.Join(",\n", enumerable.Select(x => ToUsefulString(x).Tab()).Take(10).ToArray()));
			if (enumerable.Count() > 10)
			{
				if (enumerable.Count() > 11)
				{
					sb.AppendLine(String.Format(",\n  ...({0} more elements)", enumerable.Count() - 10));
				}
				else
				{
					sb.AppendLine(",\n" + enumerable.Last().ToUsefulString().Tab());
				}
			}
			else sb.AppendLine();
			sb.AppendLine("}");

			return sb.ToString();
		}

		static string Tab(this string str)
		{
			if (string.IsNullOrEmpty(str)) return "";

			string[] split = str.Split(new[] {"\r\n", "\n"}, StringSplitOptions.None);
			var sb = new StringBuilder();

			sb.Append("  " + split[0]);
			foreach (string part in split.Skip(1))
			{
				sb.AppendLine();
				sb.Append("  " + part);
			}

			return sb.ToString();
		}
	}
}