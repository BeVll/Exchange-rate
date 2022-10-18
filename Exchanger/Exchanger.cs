using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Net;
using System.Globalization;
using System.Xml.Linq;

namespace Exchanger
{

    public class Exchanger
    {
        public List<Currency> currencies = new List<Currency>();
        public Exchanger()
        {

            string kurs = "<a class=\"dotted\" title=\".*\" href=\".*\">(\\w+)<";
            string kurs2 = "<div class=\"course\">([0-9]+\\.[0-9]+)<";

            string HtmlText = string.Empty;

            HttpWebRequest myHttwebrequest = (HttpWebRequest)HttpWebRequest.Create("https://kurs.com.ua/ru#main_table");
            HttpWebResponse myHttpWebresponse = (HttpWebResponse)myHttwebrequest.GetResponse();
            StreamReader strm = new StreamReader(myHttpWebresponse.GetResponseStream());
            HtmlText = strm.ReadToEnd();

            CultureInfo culture = CultureInfo.GetCultureInfo("en-En");
            Regex r = new Regex(kurs);
            MatchCollection mc1 = r.Matches(HtmlText);
            Regex r2 = new Regex(kurs2);
            MatchCollection mc2 = r2.Matches(HtmlText);
            for (int i = 0; i < mc1.Count; i++)
            {
                string name = mc1[i].Groups[1].Value;
                string value = mc2[i*3].Groups[1].Value;
                Currency currency = new Currency(name, Convert.ToDouble(value, culture));
                currencies.Add(currency);
              
            }
            currencies.Add(new Currency("UAH", 1));
        }
        public double GetCurrency(string cur1, string cur2)
        {
            Currency currency1 = currencies.Where(s => s.Name == cur1).FirstOrDefault();
            Currency currency2 = currencies.Where(s => s.Name == cur2).FirstOrDefault();
            double rate = Math.Round(currency1.value, 3) / Math.Round(currency2.value, 3);
            return Math.Round(rate, 3);
        }
    }
    
}
