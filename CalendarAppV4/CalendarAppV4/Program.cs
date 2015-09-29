using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarAppV4
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (!File.Exists(Globals.path))
            {
                File.Create(Globals.path).Dispose();
                StreamWriter tw = new StreamWriter(Globals.path);
                tw.Close();
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
            string date = "01/08/2008";
            DateTime dt = DateTime.Parse(date, culture, System.Globalization.DateTimeStyles.AssumeLocal);
            Console.WriteLine("Year: {0}, Month: {1}, Day: {2}", dt.Year, dt.Month, dt.Day);

            ////This event deletes any target versions that have been released more than 90 days ago. 
            //using (StreamReader sr = new StreamReader(Globals.path))
            //{
            //    while (!sr.EndOfStream)
            //    {
            //        string line = sr.ReadLine();
            //        string[] relDate = line.Split('•')[9].Split('/');
            //        string foundString = line.Split('•')[0];
            //        DateTime relDateTime = new DateTime(int.Parse(relDate[2]), int.Parse(relDate[0]), int.Parse(relDate[1]));
            //        TimeSpan diff = DateTime.Today.Subtract(relDateTime);
            //        if (diff.Days > 90)
            //        {
            //            var oldLines = System.IO.File.ReadAllLines(Globals.path);
            //            var newLines = oldLines.Where(linek => !linek.Contains(foundString));
            //            System.IO.File.WriteAllLines(@"temptext.txt", newLines);
            //        }
            //    }
            //    sr.Close();
            //}
            ////If there are any versions to be deleted, the text file will exist, and it will replace the original database.
            //if (File.Exists(@"temptext.txt"))
            //{
            //    File.Copy(@"temptext.txt", Globals.path, true);
            //    File.Delete(@"temptext.txt");
            //}
            Application.Run(new Home());
        }
    }
}
