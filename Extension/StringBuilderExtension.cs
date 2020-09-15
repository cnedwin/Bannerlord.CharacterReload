using System;
using System.Text;

namespace CharacterReload
{
	public static class StringBuilderExtension
	{
		public static void AddItem(this StringBuilder sb, string key, int value)
		{
			sb.AppendLine(key + "=" + value);
		}

		public static void AddItem(this StringBuilder sb, string key, string value)
		{
			sb.AppendLine(key + "=" + value);
		}
	}
}
