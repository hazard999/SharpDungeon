namespace sharpdungeon.utils
{
	public class Utils
	{
		public static string Capitalize(string str)
		{
			return char.ToUpper(str[0]) + str.Substring(1);
		}

		public static string Format(string format, params object[] args)
		{
			return string.Format(format, args);
		}

		public static string VOWELS = "aoeiu";

		public static string Indefinite(string noun)
		{
		    if (noun.Length == 0)
		        return "a";
		    
            return (VOWELS.IndexOf(char.ToLower(noun[0])) != -1 ? "an " : "a ") + noun;
		}
	}

}