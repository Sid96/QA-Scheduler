using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Calendar.NET;

namespace CalendarAppV4
{
    public partial class ViewCalendar : Form
    {
        int namesCount;
        public ViewCalendar()
        {
            InitializeComponent();
            //if the file doesn't exist or this is the first file (i.e. user just clicked the view calendar button):
            if (!File.Exists(NetGlobals.path) || NetGlobals.docCounter == 0)
            {
                File.Copy(Globals.path, NetGlobals.path, true);
                calendar1.CalendarDate = DateTime.Now;
                calendar1.CalendarView = CalendarViews.Month;
                var counter = 0;
                string liner;
                var reader = new StreamReader(Globals.path);
                while ((liner = reader.ReadLine()) != null)
                {
                    namesCount++;
                }
                reader.Close();
                NetGlobals.names = new string[namesCount];
                using (StreamReader sr = new StreamReader(Globals.path))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine().Trim().Split('•');
                        string[] color = { "white", "aqua", "burlywood", "chartreuse", "darkgray", "khaki", "deeppink", "gold",
                                             "red", "springgreen", "peru", "yellow", "yellowgreen", "tomato" };

                        if (line.Length == 8)
                        {
                            AddToCalendar0(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 13)
                        {
                            AddToCalendar1(line[0], line, Color.FromName(color[counter]));
                        }
                        
                        else if (line.Length == 18)
                        {
                            AddToCalendar2(line[0], line, Color.FromName(color[counter]));
                        }
                        
                        else if (line.Length == 23)
                        {
                            AddToCalendar3(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 28)
                        {
                            AddToCalendar4(line[0], line, Color.FromName(color[counter]));
                        }
                        
                        else if (line.Length == 33)
                        {
                            AddToCalendar5(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 38)
                        {
                            AddToCalendar6(line[0], line, Color.FromName(color[counter]));
                        }

                        NetGlobals.names[counter] = line[0];
                        counter++;

                        if (counter == 14)
                        {
                            counter = 0;
                        }
                    }
                }
            }
            //Otherwise...
            else
            {
                calendar1.CalendarDate = DateTime.Now;
                calendar1.CalendarView = CalendarViews.Month;
                int counter = 0;
                string liner;
                TextReader reader = new StreamReader(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                while ((liner = reader.ReadLine()) != null)
                {
                    namesCount++;
                }
                reader.Close();
                NetGlobals.names = new string[namesCount];
                using (StreamReader sr = new StreamReader(@"TEMPDATA•" + NetGlobals.docCounter + ".txt"))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine().Trim().Split('•');
                        string[] color = { "white", "aqua", "burlywood", "chartreuse", "darkgray", "khaki", "deeppink", "gold",
                                             "red", "springgreen", "peru", "yellow", "yellowgreen", "tomato" };

                        if (line.Length == 8)
                        {
                            AddToCalendar0(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 13)
                        {
                            AddToCalendar1(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 18)
                        {
                            AddToCalendar2(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 23)
                        {
                            AddToCalendar3(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 28)
                        {
                            AddToCalendar4(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 33)
                        {
                            AddToCalendar5(line[0], line, Color.FromName(color[counter]));
                        }

                        else if (line.Length == 38)
                        {
                            AddToCalendar6(line[0], line, Color.FromName(color[counter]));
                        }

                        NetGlobals.names[counter] = line[0];
                        counter++;

                        if (counter == 14)
                        {
                            counter = 0;
                        }
                    }
                }
            }
        }

        public void AddToCalendar0(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var relDate = line[6].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 0);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 0);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 0);
            NewEvent(name, "Release Date", relDate, color, 2, 0);
        }

        public void AddToCalendar1(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var cust1Date = line[7].Split('/');
            var relDate = line[11].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 1);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 1);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 1);
            NewEvent(name, line[5], cust1Date, color, 2, 1);
            NewEvent(name, "Release Date", relDate, color, 3, 1);
        }

        public void AddToCalendar2(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var cust1Date = line[7].Split('/');
            var cust2Date = line[12].Split('/');
            var relDate = line[16].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 2);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 2);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 2);
            NewEvent(name, line[5], cust1Date, color, 2, 2);
            NewEvent(name, line[10], cust2Date, color, 3, 2);
            NewEvent(name, "Release Date", relDate, color, 4, 2);
        }

        public void AddToCalendar3(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var cust1Date = line[7].Split('/');
            var cust2Date = line[12].Split('/');
            var cust3Date = line[17].Split('/');
            var relDate = line[21].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 3);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 3);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 3);
            NewEvent(name, line[5], cust1Date, color, 2, 3);
            NewEvent(name, line[10], cust2Date, color, 3, 3);
            NewEvent(name, line[15], cust3Date, color, 4, 3);
            NewEvent(name, "Release Date", relDate, color, 5, 3);
        }

        public void AddToCalendar4(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var cust1Date = line[7].Split('/');
            var cust2Date = line[12].Split('/');
            var cust3Date = line[17].Split('/');
            var cust4Date = line[22].Split('/');
            var relDate = line[26].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 4);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 4);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 4);
            NewEvent(name, line[5], cust1Date, color, 2, 4);
            NewEvent(name, line[10], cust2Date, color, 3, 4);
            NewEvent(name, line[15], cust3Date, color, 4, 4);
            NewEvent(name, line[20], cust4Date, color, 5, 4);
            NewEvent(name, "Release Date", relDate, color, 6, 4);
        }

        public void AddToCalendar5(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var cust1Date = line[7].Split('/');
            var cust2Date = line[12].Split('/');
            var cust3Date = line[17].Split('/');
            var cust4Date = line[22].Split('/');
            var cust5Date = line[27].Split('/');
            var relDate = line[31].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 5);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 5);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 5);
            NewEvent(name, line[5], cust1Date, color, 2, 5);
            NewEvent(name, line[10], cust2Date, color, 3, 5);
            NewEvent(name, line[15], cust3Date, color, 4, 5);
            NewEvent(name, line[20], cust4Date, color, 5, 5);
            NewEvent(name, line[25], cust5Date, color, 6, 5);
            NewEvent(name, "Release Date", relDate, color, 7, 5);
        }

        public void AddToCalendar6(string name, string[] line, Color color)
        {
            var devEstCompDate = line[1].Split('/');
            var devCompDate = line[2].Split('/');
            var cust1Date = line[7].Split('/');
            var cust2Date = line[12].Split('/');
            var cust3Date = line[17].Split('/');
            var cust4Date = line[22].Split('/');
            var cust5Date = line[27].Split('/');
            var cust6Date = line[32].Split('/');
            var relDate = line[36].Split('/');

            //if the Dev Est. Complete date is the same as the actual complete date, the estimated date will be disabled, to show a difference between the 2.
            if (devEstCompDate[0].Trim() != devCompDate[0].Trim() || devEstCompDate[1].Trim() != devCompDate[1].Trim() || devEstCompDate[2].Trim() != devCompDate[2].Trim())
            {
                NewEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 6);
            }
            else
            {
                NewDisabledEvent(name, "Dev Est Comp Date", devEstCompDate, color, 0, 6);
            }
            NewEvent(name, "Dev Comp Date", devCompDate, color, 1, 6);
            NewEvent(name, line[5], cust1Date, color, 2, 6);
            NewEvent(name, line[10], cust2Date, color, 3, 6);
            NewEvent(name, line[15], cust3Date, color, 4, 6);
            NewEvent(name, line[20], cust4Date, color, 5, 6);
            NewEvent(name, line[25], cust5Date, color, 6, 6);
            NewEvent(name, line[30], cust6Date, color, 7, 6);
            NewEvent(name, "Release Date", relDate, color, 8, 6);
        }

        public void NewEvent(string name, string type, string[] date, Color color, int order, int numberOfEvents)
        {
            var newEvent = new CustomEvent
            {

                Name = name,
                EventText = name + " " + type,
                IgnoreTimeComponent = true,
                Date = new DateTime(int.Parse(date[2]), int.Parse(date[0]), int.Parse(date[1])),
                EventColor = color,
                Order = order,
                NumberOfEvents = numberOfEvents,
            };
            Calendar.NET.NetGlobals.customEvents.Add(newEvent);
            calendar1.AddEvent(newEvent);
        }

        //Does the same thing as above, but makes the event disabled.

        public void NewDisabledEvent(string name, string type, string[] date, Color color, int order, int numberOfEvents)
        {
            var newEvent = new CustomEvent
            {
                Name = name,
                EventText = name + " " + type,
                IgnoreTimeComponent = true,
                Date = new DateTime(int.Parse(date[2]), int.Parse(date[0]), int.Parse(date[1])),
                EventColor = color,
                Enabled = false,
                Order = order,
                NumberOfEvents = numberOfEvents,
            };
            Calendar.NET.NetGlobals.customEvents.Add(newEvent);
            calendar1.AddEvent(newEvent);
        }

        private void Back_Click(object sender, EventArgs e)
        {
            File.Copy(Globals.path, NetGlobals.path, true);
            NetGlobals.customEvents.Clear();
            while (NetGlobals.docCounter != 0)
            {
                File.Delete(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                NetGlobals.docCounter--;
            }
            File.Delete(@"TEMPDATA•0.txt");
            this.Hide();
            new Home().Show();
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            Calendar.NET.NetGlobals.customEvents.Clear();
            Array.Clear(Calendar.NET.NetGlobals.names, 0, Calendar.NET.NetGlobals.names.Length);
            while (NetGlobals.docCounter != 0)
            {
                File.Delete(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                NetGlobals.docCounter--;
            }
            File.Delete(NetGlobals.path);
            new ViewCalendar().Show();
            this.Hide();
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            if (NetGlobals.docCounter != 0)
            {
                NetGlobals.customEvents.Clear();
                File.Delete(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                NetGlobals.docCounter--;
                new ViewCalendar().Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Cannot Undo Further!");
            }
        }

        private void CreateNewTargVers_Click(object sender, EventArgs e)
        {
            Globals.createTargRedirect = 1;
            Calendar.NET.NetGlobals.customEvents.Clear();
            Array.Clear(Calendar.NET.NetGlobals.names, 0, Calendar.NET.NetGlobals.names.Length);
            while (NetGlobals.docCounter != 0)
            {
                File.Delete(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                NetGlobals.docCounter--;
            }
            File.Delete(NetGlobals.path);
            new CreateNewTargetVersion().Show();
            this.Hide();
        }

        private void Save_Click(object sender, EventArgs e)
        {
            File.Copy(@"TEMPDATA•" + NetGlobals.docCounter + ".txt", Globals.path, true);
            MessageBox.Show("Save Successful");
            while (NetGlobals.docCounter != 0)
            {
                File.Delete(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                NetGlobals.docCounter--;
            }
            File.Delete(@"TEMPDATA•0.txt");
            File.Copy(Globals.path, NetGlobals.path, true);      
        }

        public int Comparison(DateTime endD, DateTime startD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;
            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return (int)(calcBusinessDays - 1.0);
        }

        private void ViewCalendar_FormClosed(object sender, FormClosedEventArgs e)
        {
            while (NetGlobals.docCounter != 0)
            {
                File.Delete(@"TEMPDATA•" + NetGlobals.docCounter + ".txt");
                NetGlobals.docCounter--;
            }
            File.Delete(NetGlobals.path);
            Application.Exit();
        }

    }
}
