using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Threading;

namespace CalendarAppV4
{
    public partial class CreateNewTargetVersion : Form
    {
        IFormatProvider culture = new CultureInfo("en-US", true);
        int changeRequested = 0;
        int s1, s2, s3, s4, s5, s6, calDateChange;
        DateTime devEstCompDateTime = DateTime.Today;
        DateTime customEvent1 = DateTime.Today;
        DateTime customEvent2 = DateTime.Today;
        DateTime customEvent3 = DateTime.Today;
        DateTime customEvent4 = DateTime.Today;
        DateTime customEvent5 = DateTime.Today;
        DateTime customEvent6 = DateTime.Today;
        DateTime relDateTime = DateTime.Today;
        DateTime manipulate = DateTime.Today;
        DateTime manipulate2 = DateTime.Today;
        DateTime manipulate3 = DateTime.Today;
        public CreateNewTargetVersion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (changeRequested == 0)
            {
                changeRequested++;
                groupBox2.Show();
            }
            else if (changeRequested == 1)
            {
                button2.Hide();
                changeRequested++;
                groupBox3.Show();
            }
            else if (changeRequested == 2)
            {
                button3.Hide();
                changeRequested++;
                groupBox4.Show();
            }
            else if (changeRequested == 3)
            {
                button4.Hide();
                changeRequested++;
                groupBox5.Show();
            }
            else if (changeRequested == 4)
            {
                button5.Hide();
                changeRequested++;
                groupBox6.Show();
            }
            else if (changeRequested == 5)
            {
                button6.Hide();
                changeRequested++;
                button1.Hide();
                label16.Hide();
                groupBox7.Show();
                groupBox2.Location = new Point(12, 114);
                groupBox3.Location = new Point(12, 158);
                groupBox4.Location = new Point(12, 202);
                groupBox5.Location = new Point(12, 246);
                groupBox6.Location = new Point(12, 290);
                groupBox7.Location = new Point(12, 334);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            groupBox2.Hide();
            changeRequested--;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            button2.Show();
            groupBox3.Hide();
            changeRequested--;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            button3.Show();
            groupBox4.Hide();
            changeRequested--;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            button4.Show();
            groupBox5.Hide();
            changeRequested--;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button5.Show();
            groupBox6.Hide();
            changeRequested--;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            button6.Show();
            groupBox7.Hide();
            changeRequested--;
            button1.Show();
            label16.Show();
            groupBox2.Location = new Point(12, 139);
            groupBox3.Location = new Point(12, 180);
            groupBox4.Location = new Point(12, 221);
            groupBox5.Location = new Point(12, 262);
            groupBox6.Location = new Point(12, 303);
        }

        private void Complete_Click(object sender, EventArgs e)
        {
            if (VersName.Text.Trim() == "")
            {
                MessageBox.Show("Name Invalid.");
                return;
            }
            using (StreamReader sr = new StreamReader(Globals.path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.ToLower().StartsWith(VersName.Text.Trim().ToLower() + "•"))
                    {
                        MessageBox.Show("There already exists a Target Version with that name");
                        return;
                    }
                }
                sr.Close();
            }
            string[] devEstCompDate = DevEstCompDate.Text.Split('/');
            string[] relDate = RelDate.Text.Split('/');

            if (!(devEstCompDate.Length == 3 && relDate.Length == 3))
            {
                MessageBox.Show("One or more of the dates are incorrect");
                return;
            }
            else if (!(int.TryParse(devEstCompDate[0], out s1) && int.TryParse(devEstCompDate[1], out s2) && int.TryParse(devEstCompDate[2], out s3)
                && int.TryParse(relDate[0], out s4) && int.TryParse(relDate[1], out s5) && int.TryParse(relDate[2], out s6)))
            {
                MessageBox.Show("One or more of the dates are incorrect.");
                return;
            }
            try
            {
                devEstCompDateTime = new DateTime(s3, s1, s2);
                relDateTime = new DateTime(s6, s4, s5);
                if ((int)devEstCompDateTime.DayOfWeek == 0 || (int)devEstCompDateTime.DayOfWeek == 6 || 
                    (int)relDateTime.DayOfWeek == 0 || (int)relDateTime.DayOfWeek == 6)
                {
                    MessageBox.Show("One or more of the dates fall on a weekend.");
                    return;
                }
            }
            catch
            {
                MessageBox.Show("One or more of the dates are incorrect.");
                return;
            }
            if (!(int.TryParse(DevEstCompDur.Text, out s1) && s1 >= 0))
            {
                MessageBox.Show("One of the durations are not valid numbers.");
                return;
            }

            if (Comparison(relDateTime, devEstCompDateTime) < 0)
            {
                MessageBox.Show("One or more of the dates are in incorrect order.");
                return;
            }

            if (groupBox2.Visible)
            {
                if (Cust1Name.Text.Trim() == "")
                {
                    MessageBox.Show("You do not have a name for a custom event");
                    return;
                }
                string[] cust1Date = Cust1Date.Text.Split('/');
                if (!(cust1Date.Length == 3))
                {
                    MessageBox.Show("One or more of the dates are incorrect");
                    return;
                }
                else if (!(int.TryParse(cust1Date[0], out s1) && int.TryParse(cust1Date[1], out s2) && int.TryParse(cust1Date[2], out s3)))
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                try
                {
                    customEvent1 = new DateTime(s3, s1, s2);
                    if ((int)customEvent1.DayOfWeek == 0 || (int)customEvent1.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                if (!(int.TryParse(Cust1Dur.Text, out s1) && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not valid numbers.");
                    return;
                }
                if (Comparison(customEvent1, devEstCompDateTime) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
                if (Comparison(relDateTime, customEvent1) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
            }

            if (groupBox3.Visible)
            {
                if (Cust2Name.Text.Trim() == "")
                {
                    MessageBox.Show("You do not have a name for a custom event");
                    return;
                }
                if (Cust2Name.Text.Trim() == Cust1Name.Text.Trim())
                {
                    MessageBox.Show("One of the event names are not unique");
                    return;
                }
                string[] cust2Date = Cust2Date.Text.Split('/');
                if (!(cust2Date.Length == 3))
                {
                    MessageBox.Show("One or more of the dates are incorrect");
                    return;
                }
                else if (!(int.TryParse(cust2Date[0], out s1) && int.TryParse(cust2Date[1], out s2) && int.TryParse(cust2Date[2], out s3)))
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                try
                {
                    customEvent2 = new DateTime(s3, s1, s2);
                    if ((int)customEvent2.DayOfWeek == 0 || (int)customEvent2.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                if (!(int.TryParse(Cust2Dur.Text, out s1) && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not valid numbers.");
                    return;
                }
                if (Comparison(customEvent2, customEvent1) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
                if (Comparison(relDateTime, customEvent2) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
            }

            if (groupBox4.Visible)
            {
                if (Cust3Name.Text.Trim() == "")
                {
                    MessageBox.Show("You do not have a name for a custom event");
                    return;
                }
                if (Cust3Name.Text.Trim() == Cust1Name.Text.Trim() || Cust3Name.Text.Trim() == Cust2Name.Text.Trim())
                {
                    MessageBox.Show("One of the event names are not unique");
                    return;
                }
                string[] cust3Date = Cust3Date.Text.Split('/');
                if (!(cust3Date.Length == 3))
                {
                    MessageBox.Show("One or more of the dates are incorrect");
                    return;
                }
                else if (!(int.TryParse(cust3Date[0], out s1) && int.TryParse(cust3Date[1], out s2) && int.TryParse(cust3Date[2], out s3)))
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                try
                {
                    customEvent3 = new DateTime(s3, s1, s2);
                    if ((int)customEvent3.DayOfWeek == 0 || (int)customEvent3.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                if (!(int.TryParse(Cust3Dur.Text, out s1) && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not valid numbers.");
                    return;
                }
                if (Comparison(customEvent3, customEvent2) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
                if (Comparison(relDateTime, customEvent3) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
            }

            if (groupBox5.Visible)
            {
                if (Cust4Name.Text.Trim() == "")
                {
                    MessageBox.Show("You do not have a name for a custom event");
                    return;
                }
                if (Cust4Name.Text.Trim() == Cust1Name.Text.Trim() || Cust4Name.Text.Trim() == Cust2Name.Text.Trim() ||
                    Cust4Name.Text.Trim() == Cust3Name.Text.Trim())
                {
                    MessageBox.Show("One of the event names are not unique");
                    return;
                }
                string[] cust4Date = Cust4Date.Text.Split('/');
                if (!(cust4Date.Length == 3))
                {
                    MessageBox.Show("One or more of the dates are incorrect");
                    return;
                }
                else if (!(int.TryParse(cust4Date[0], out s1) && int.TryParse(cust4Date[1], out s2) && int.TryParse(cust4Date[2], out s3)))
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                try
                {
                    customEvent4 = new DateTime(s3, s1, s2);
                    if ((int)customEvent4.DayOfWeek == 0 || (int)customEvent4.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                if (!(int.TryParse(Cust4Dur.Text, out s1) && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not valid numbers.");
                    return;
                }
                if (Comparison(customEvent4, customEvent3) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
                if (Comparison(relDateTime, customEvent4) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
            }

            if (groupBox6.Visible)
            {
                if (Cust5Name.Text.Trim() == "")
                {
                    MessageBox.Show("You do not have a name for a custom event");
                    return;
                }
                if (Cust5Name.Text.Trim() == Cust1Name.Text.Trim() || Cust5Name.Text.Trim() == Cust2Name.Text.Trim() ||
                    Cust5Name.Text.Trim() == Cust3Name.Text.Trim() || Cust5Name.Text.Trim() == Cust4Name.Text.Trim())
                {
                    MessageBox.Show("One of the event names are not unique");
                    return;
                }
                string[] cust5Date = Cust5Date.Text.Split('/');
                if (!(cust5Date.Length == 3))
                {
                    MessageBox.Show("One or more of the dates are incorrect");
                    return;
                }
                else if (!(int.TryParse(cust5Date[0], out s1) && int.TryParse(cust5Date[1], out s2) && int.TryParse(cust5Date[2], out s3)))
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                try
                {
                    customEvent5 = new DateTime(s3, s1, s2);
                    if ((int)customEvent5.DayOfWeek == 0 || (int)customEvent5.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                if (!(int.TryParse(Cust5Dur.Text, out s1) && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not valid numbers.");
                    return;
                }
                if (Comparison(customEvent5, customEvent4) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
                if (Comparison(relDateTime, customEvent5) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
            }

            if (groupBox7.Visible)
            {
                if (Cust6Name.Text.Trim() == "")
                {
                    MessageBox.Show("You do not have a name for a custom event");
                    return;
                }
                if (Cust6Name.Text.Trim() == Cust1Name.Text.Trim() || Cust6Name.Text.Trim() == Cust2Name.Text.Trim() ||
                    Cust6Name.Text.Trim() == Cust3Name.Text.Trim() || Cust6Name.Text.Trim() == Cust4Name.Text.Trim() || 
                    Cust6Name.Text.Trim() == Cust5Name.Text.Trim())
                {
                    MessageBox.Show("One of the event names are not unique");
                    return;
                }
                string[] cust6Date = Cust6Date.Text.Split('/');
                if (!(cust6Date.Length == 3))
                {
                    MessageBox.Show("One or more of the dates are incorrect");
                    return;
                }
                else if (!(int.TryParse(cust6Date[0], out s1) && int.TryParse(cust6Date[1], out s2) && int.TryParse(cust6Date[2], out s3)))
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                try
                {
                    customEvent6 = new DateTime(s3, s1, s2);
                    if ((int)customEvent6.DayOfWeek == 0 || (int)customEvent6.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        return;
                    }
                }
                catch
                {
                    MessageBox.Show("One or more of the dates are incorrect.");
                    return;
                }
                if (!(int.TryParse(Cust6Dur.Text, out s1) && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not valid numbers.");
                    return;
                }
                if (Comparison(customEvent6, customEvent5) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
                if (Comparison(relDateTime, customEvent6) < 0)
                {
                    MessageBox.Show("One or more of the dates are in incorrect order.");
                    return;
                }
            }

            using (StreamWriter outStream = File.AppendText(Globals.path))
            {
                outStream.Write(VersName.Text.Trim());
                outStream.Write("•" + DevEstCompDate.Text);
                outStream.Write("•" + DevEstCompDate.Text);
                outStream.Write("•" + DevEstCompDur.Text);
                outStream.Write("•" + DevEstCompDur.Text);
                outStream.Close();
            }

            if (groupBox2.Visible)
            {
                using (StreamWriter outStream = File.AppendText(Globals.path))
                {
                    outStream.Write("•" + Cust1Name.Text.Trim());
                    outStream.Write("•" + Cust1Date.Text);
                    outStream.Write("•" + Cust1Date.Text);
                    outStream.Write("•" + Cust1Dur.Text);
                    outStream.Write("•" + Cust1Dur.Text);
                    outStream.Close();
                }
            }
            if (groupBox3.Visible)
            {
                using (StreamWriter outStream = File.AppendText(Globals.path))
                {
                    outStream.Write("•" + Cust2Name.Text.Trim());
                    outStream.Write("•" + Cust2Date.Text);
                    outStream.Write("•" + Cust2Date.Text);
                    outStream.Write("•" + Cust2Dur.Text);
                    outStream.Write("•" + Cust2Dur.Text);
                    outStream.Close();
                }
            }
            if (groupBox4.Visible)
            {
                using (StreamWriter outStream = File.AppendText(Globals.path))
                {
                    outStream.Write("•" + Cust3Name.Text.Trim());
                    outStream.Write("•" + Cust3Date.Text);
                    outStream.Write("•" + Cust3Date.Text);
                    outStream.Write("•" + Cust3Dur.Text);
                    outStream.Write("•" + Cust3Dur.Text);
                    outStream.Close();
                }
            }
            if (groupBox5.Visible)
            {
                using (StreamWriter outStream = File.AppendText(Globals.path))
                {
                    outStream.Write("•" + Cust4Name.Text.Trim());
                    outStream.Write("•" + Cust4Date.Text);
                    outStream.Write("•" + Cust4Date.Text);
                    outStream.Write("•" + Cust4Dur.Text);
                    outStream.Write("•" + Cust4Dur.Text);
                    outStream.Close();
                }
            }
            if (groupBox6.Visible)
            {
                using (StreamWriter outStream = File.AppendText(Globals.path))
                {
                    outStream.Write("•" + Cust5Name.Text.Trim());
                    outStream.Write("•" + Cust5Date.Text);
                    outStream.Write("•" + Cust5Date.Text);
                    outStream.Write("•" + Cust5Dur.Text);
                    outStream.Write("•" + Cust5Dur.Text);
                    outStream.Close();
                }
            }
            if (groupBox7.Visible)
            {
                using (StreamWriter outStream = File.AppendText(Globals.path))
                {
                    outStream.Write("•" + Cust6Name.Text.Trim());
                    outStream.Write("•" + Cust6Date.Text);
                    outStream.Write("•" + Cust6Date.Text);
                    outStream.Write("•" + Cust6Dur.Text);
                    outStream.Write("•" + Cust6Dur.Text);
                    outStream.Close();
                }
            }

            using (StreamWriter outStream = File.AppendText(Globals.path))
            {
                outStream.Write("•" + RelDate.Text);
                outStream.Write("•" + RelDate.Text);
                outStream.WriteLine("•0");
                outStream.Close();
            }

            MessageBoxManager.Yes = "View Calendar";
            MessageBoxManager.No = "Home Page";
            MessageBoxManager.Cancel = "Back";
            MessageBoxManager.Register();

            //Lets the user choose where to go.
            var result = MessageBox.Show("Target Version Saved. Where would you like to go?",
            "Important Question",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.None,
            MessageBoxDefaultButton.Button3);
            MessageBoxManager.Unregister();
            if (result == DialogResult.Yes)
            {
                new ViewCalendar().Show();
                this.Hide();
            }
            if (result == DialogResult.No)
            {
                new Home().Show();
                this.Hide();
            } 
        }

        private void Back_Click(object sender, EventArgs e)
        {
            if (Globals.createTargRedirect == 0)
            {
                new Home().Show();
            }
            else if (Globals.createTargRedirect == 1)
            {
                new ViewCalendar().Show();
            }
            this.Hide();
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (calDateChange == 1)
            {
                DevEstCompDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 2)
            {
                Cust1Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 3)
            {
                Cust2Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 4)
            {
                Cust3Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 5)
            {
                Cust4Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 6)
            {
                Cust5Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 7)
            {
                Cust6Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (calDateChange == 8)
            {
                RelDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 1;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 2;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 3;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 4;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 5;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 6;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 7;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            groupBox1.Hide();
            Thread.Sleep(50);
            groupBox1.Show();
            calDateChange = 8;
        }



        private void CreateNewTargetVersion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private static int Comparison(DateTime endD, DateTime startD)
        {
            var calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 -
                (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;
            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return (int)(calcBusinessDays - 1.0);
        }

        private DateTime AddBusinessDays(DateTime date, int workingDays)
        {
            try
            {
                int direction = workingDays < 0 ? -1 : 1;
                DateTime newDate = date;
                while (workingDays != 0)
                {
                    newDate = newDate.AddDays(direction);
                    if (newDate.DayOfWeek != DayOfWeek.Saturday && newDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        workingDays -= direction;
                    }
                }
                return newDate;
            }
            catch
            {
                MessageBox.Show("Duration is too big");
                return DateTime.Now;
            }
        }

        private void DevEstCompDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 1)
            {
                groupBox1.Hide();
            }
        }

        private void Cust1Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 2)
            {
                groupBox1.Hide();
            }
        }

        private void Cust2Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 3)
            {
                groupBox1.Hide();
            }
        }

        private void Cust3Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 4)
            {
                groupBox1.Hide();
            }
        }

        private void Cust4Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 5)
            {
                groupBox1.Hide();
            }
        }

        private void Cust5Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 6)
            {
                groupBox1.Hide();
            }
        }

        private void Cust6Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 7)
            {
                groupBox1.Hide();
            }
        }

        private void RelDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 8)
            {
                groupBox1.Hide();
            }
        }

        private void DevEstCompDate_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(DevEstCompDate.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (!int.TryParse(DevEstCompDur.Text, out s1))
            {
                return;
            }

            if (groupBox2.Visible)
            {
                Cust1Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }

            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void DevEstCompDur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(DevEstCompDur.Text, out s1))
            {
                return;
            }

            if (!DateTime.TryParse(DevEstCompDate.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox2.Visible)
            {
                Cust1Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }

            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust1Date_TextChanged(object sender, EventArgs e) 
        {
            if (!DateTime.TryParse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (DateTime.TryParse(DevEstCompDate.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
            {
                DevEstCompDur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            if (!int.TryParse(Cust1Dur.Text, out s1))
            {
                return;
            }

            if (groupBox3.Visible)
            {
                Cust2Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust1Dur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Cust1Dur.Text, out s1))
            {
                return;
            }

            if (!DateTime.TryParse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox3.Visible)
            {
                Cust2Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust2Date_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (DateTime.TryParse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
            {
                Cust1Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            if (!int.TryParse(Cust2Dur.Text, out s1))
            {
                return;
            }

            if (groupBox4.Visible)
            {
                Cust3Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }

            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust2Dur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Cust2Dur.Text, out s1))
            {
                return;
            }

            if (!DateTime.TryParse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox4.Visible)
            {
                Cust3Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust3Date_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (DateTime.TryParse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
            {
                Cust2Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            if (!int.TryParse(Cust3Dur.Text, out s1))
            {
                return;
            }

            if (groupBox5.Visible)
            {
                Cust4Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }

            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust3Dur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Cust3Dur.Text, out s1))
            {
                return;
            }

            if (!DateTime.TryParse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox5.Visible)
            {
                Cust4Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust4Date_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (DateTime.TryParse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
            {
                Cust3Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            if (!int.TryParse(Cust4Dur.Text, out s1))
            {
                return;
            }

            if (groupBox6.Visible)
            {
                Cust5Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }

            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust4Dur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Cust4Dur.Text, out s1))
            {
                return;
            }

            if (!DateTime.TryParse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox6.Visible)
            {
                Cust5Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust5Date_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(Cust5Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (DateTime.TryParse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
            {
                Cust4Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            if (!int.TryParse(Cust5Dur.Text, out s1))
            {
                return;
            }

            if (groupBox7.Visible)
            {
                Cust6Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }

            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust5Dur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Cust5Dur.Text, out s1))
            {
                return;
            }

            if (!DateTime.TryParse(Cust5Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox7.Visible)
            {
                Cust6Date.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
            else
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void Cust6Date_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(Cust6Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (DateTime.TryParse(Cust5Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
            {
                Cust5Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            if (!int.TryParse(Cust6Dur.Text, out s1))
            {
                return;
            }

            RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
        }

        private void Cust6Dur_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(Cust5Dur.Text, out s1))
            {
                return;
            }

            if (DateTime.TryParse(Cust6Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                RelDate.Text = AddBusinessDays(manipulate, s1).ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void RelDate_TextChanged(object sender, EventArgs e)
        {
            if (!DateTime.TryParse(RelDate.Text, culture, DateTimeStyles.AssumeLocal, out manipulate))
            {
                return;
            }

            if (groupBox7.Visible)
            {
                if (!DateTime.TryParse(Cust6Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                Cust6Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            else if (groupBox6.Visible)
            {
                if (!DateTime.TryParse(Cust5Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                Cust5Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            else if (groupBox5.Visible)
            {
                if (!DateTime.TryParse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                Cust4Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            else if (groupBox4.Visible)
            {
                if (!DateTime.TryParse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                Cust3Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            else if (groupBox3.Visible)
            {
                if (!DateTime.TryParse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                Cust2Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            else if (groupBox2.Visible)
            {
                if (!DateTime.TryParse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                Cust1Dur.Text = Comparison(manipulate, manipulate2).ToString();
            }

            else
            {
                if (!DateTime.TryParse(DevEstCompDate.Text, culture, DateTimeStyles.AssumeLocal, out manipulate2))
                {
                    return;
                }
                DevEstCompDur.Text = Comparison(manipulate, manipulate2).ToString();
            }
        }
    }
}
