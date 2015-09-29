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
    public partial class TargVersionEdit : Form
    {
        string[] individualData;
        int s1, s2, s3, s4, s5, s6, s7, s8, s9, s10, s11, s12, s13, s14, s15, s16, s17, s18, s19, s20, s21, s22, s23, s24, changeRequested, numberOfEvents;
        DateTime devEstCompDateTime = DateTime.Now;
        DateTime qaStartDateTime = DateTime.Now;
        DateTime qaCompDateTime = DateTime.Now;
        DateTime relDateTime = DateTime.Now;
        DateTime devCompDateTime = DateTime.Now;
        DateTime qaEstStartDateTime = DateTime.Now;
        DateTime qaEstCompDateTime = DateTime.Now;
        DateTime estRelDateTime = DateTime.Now;

        public TargVersionEdit()
        {
            InitializeComponent();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            this.Hide();
            new DisplayTargVersion().Show();
        }

        private void TargVersionEdit_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(Globals.path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.StartsWith(Globals.foundString))
                    {
                        individualData = line.Split('•');
                    }
                }
            }

            VersionName.Text = individualData[0];
            DevEstCompDate.Text = individualData[1];
            DevCompDate.Text = individualData[2];
            DevEstCompDur.Text = individualData[3];
            DevCompDur.Text = individualData[4];
            numberOfEvents = 0;

            if (individualData.Length >= 13)
            {
                Cust1Name.Show();
                Cust1EstDate.Show();
                Cust1Date.Show();
                pictureBox2.Show();
                pictureBox12.Show();
                Cust1EstDur.Show();
                Cust1Dur.Show();
                Cust1Name.Text = individualData[5];
                Cust1EstDate.Text = individualData[6];
                Cust1Date.Text = individualData[7];
                Cust1EstDur.Text = individualData[8];
                Cust1Dur.Text = individualData[9];
                button1.Show();
                numberOfEvents = 1;
            }

            if (individualData.Length >= 18)
            {
                Cust2Name.Show();
                Cust2EstDate.Show();
                Cust2Date.Show();
                pictureBox3.Show();
                pictureBox13.Show();
                Cust2EstDur.Show();
                Cust2Dur.Show();
                Cust2Name.Text = individualData[10];
                Cust2EstDate.Text = individualData[11];
                Cust2Date.Text = individualData[12];
                Cust2EstDur.Text = individualData[13];
                Cust2Dur.Text = individualData[14];
                button1.Hide();
                button2.Show();
                numberOfEvents = 2;
            }

            if (individualData.Length >= 23)
            {
                Cust3Name.Show();
                Cust3EstDate.Show();
                Cust3Date.Show();
                Cust3EstDur.Show();
                Cust3Dur.Show();
                pictureBox4.Show();
                pictureBox14.Show();
                Cust3Name.Text = individualData[15];
                Cust3EstDate.Text = individualData[16];
                Cust3Date.Text = individualData[17];
                Cust3EstDur.Text = individualData[18];
                Cust3Dur.Text = individualData[19];
                button2.Hide();
                button3.Show();
                numberOfEvents = 3;
            }

            if (individualData.Length >= 28)
            {
                Cust4Name.Show();
                Cust4EstDate.Show();
                Cust4Date.Show();
                Cust4EstDur.Show();
                Cust4Dur.Show();
                pictureBox5.Show();
                pictureBox15.Show();
                Cust4Name.Text = individualData[20];
                Cust4EstDate.Text = individualData[21];
                Cust4Date.Text = individualData[22];
                Cust4EstDur.Text = individualData[23];
                Cust4Dur.Text = individualData[24];
                button3.Hide();
                button4.Show();
                numberOfEvents = 4;
            }

            if (individualData.Length >= 33)
            {
                Cust5Name.Show();
                Cust5EstDate.Show();
                Cust5Date.Show();
                Cust5EstDur.Show();
                Cust5Dur.Show();
                pictureBox6.Show();
                pictureBox16.Show();
                Cust5Name.Text = individualData[25];
                Cust5EstDate.Text = individualData[26];
                Cust5Date.Text = individualData[27];
                Cust5EstDur.Text = individualData[28];
                Cust5Dur.Text = individualData[29];
                button4.Hide();
                button5.Show();
                numberOfEvents = 5;
            }

            if (individualData.Length == 38)
            {
                Cust6Name.Show();
                Cust6EstDate.Show();
                Cust6Date.Show();
                Cust6EstDur.Show();
                Cust6Dur.Show();
                pictureBox7.Show();
                pictureBox17.Show();
                Cust6Name.Text = individualData[30];
                Cust6EstDate.Text = individualData[31];
                Cust6Date.Text = individualData[32];
                Cust6EstDur.Text = individualData[33];
                Cust6Dur.Text = individualData[34];
                button5.Hide();
                button6.Show();
                AddNewEvent.Hide();
                numberOfEvents = 6;
            }

            EstRelDate.Text = individualData[individualData.Length - 3];
            RelDate.Text = individualData[individualData.Length - 2];
            DaysGainedOrLost.Text = individualData[individualData.Length - 1];
        }

        private void TargVersionEdit_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
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

        private void Updated_Click(object sender, EventArgs e)
        {
            IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
            var run = true;
            var run2 = true;
            bool b1, b2, b3, b4;
            int s1, s2;
            DateTime devEstCompDate = DateTime.Now;
            DateTime devCompDate = DateTime.Now;
            DateTime cust1EstDate = DateTime.Now; 
            DateTime cust1Date = DateTime.Now; 
            DateTime cust2EstDate = DateTime.Now; 
            DateTime cust2Date = DateTime.Now; 
            DateTime cust3EstDate = DateTime.Now; 
            DateTime cust3Date = DateTime.Now; 
            DateTime cust4EstDate = DateTime.Now; 
            DateTime cust4Date = DateTime.Now; 
            DateTime cust5EstDate = DateTime.Now; 
            DateTime cust5Date = DateTime.Now; 
            DateTime cust6EstDate = DateTime.Now; 
            DateTime cust6Date = DateTime.Now; 
            DateTime estRelDate = DateTime.Now;
            DateTime relDate = DateTime.Now;
            if (VersionName.Text.Trim() == "")
            {
                MessageBox.Show("Name Invalid.");
                run = false;
            }

            using (StreamReader sr = new StreamReader(Globals.path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.ToLower().StartsWith(VersionName.Text.Trim().ToLower() + "•") && (!Globals.foundString.ToLower().
                        StartsWith(VersionName.Text.Trim().ToLower() + "•")))
                    {
                        MessageBox.Show("There already exists a Target Version with that name");
                        run = false;
                    }
                }
                sr.Close();
            }
            if (run)
            {
                b1 = DateTime.TryParse(DevEstCompDate.Text, culture, DateTimeStyles.AssumeLocal, out devEstCompDate);
                b2 = DateTime.TryParse(DevCompDate.Text, culture, DateTimeStyles.AssumeLocal, out devCompDate);
                b3 = DateTime.TryParse(EstRelDate.Text, culture, DateTimeStyles.AssumeLocal, out estRelDate);
                b4 = DateTime.TryParse(RelDate.Text, culture, DateTimeStyles.AssumeLocal, out relDate);

                if (!(b1 && b2 && b3 && b4))
                {
                    MessageBox.Show("One or more of the dates do not exist.");
                    return;
                }

                if (devEstCompDate.DayOfWeek == 0 || (int) devEstCompDate.DayOfWeek == 6 || devCompDate.DayOfWeek == 0 || (int) devCompDate.DayOfWeek == 6 ||
                    estRelDate.DayOfWeek == 0 || (int) estRelDate.DayOfWeek == 6 || relDate.DayOfWeek == 0 || (int) relDate.DayOfWeek == 6)
                {
                    MessageBox.Show("One or more of the dates fall on a weekend.");
                    run = false;
                }

                if (!(int.TryParse(DevCompDur.Text, out s1) && int.TryParse(DevEstCompDur.Text, out s2) && s2 >= 0 && s1 >= 0))
                {
                    MessageBox.Show("One of the durations are not numbers.");
                    run = false;
                }

                if (Cust1Name.Visible && run)
                {
                    b1 = DateTime.TryParse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal, out cust1Date);
                    b2 = DateTime.TryParse(Cust1EstDate.Text, culture, DateTimeStyles.AssumeLocal, out cust1EstDate);

                    if (!(b1 && b2))
                    {
                        MessageBox.Show("One or more of the dates do not exist.");
                        return;
                    }

                    if (cust1Date.DayOfWeek == 0 || (int) cust1Date.DayOfWeek == 6 || cust1EstDate.DayOfWeek == 0 || (int) cust1EstDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        run = false;
                    }

                    if (!(int.TryParse(Cust1EstDur.Text, out s1) && int.TryParse(Cust1Dur.Text, out s2) && s2 >= 0 && s1 >= 0))
                    {
                        MessageBox.Show("One of the durations are not numbers.");
                        run = false;
                    }

                    if (Comparison(cust1Date, devCompDate) < 0)
                    {
                        MessageBox.Show("One or more of the dates are in incorrect order.");
                        return;
                    }
                }

                if (Cust2Name.Visible && run)
                {
                    b1 = DateTime.TryParse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal, out cust2Date);
                    b2 = DateTime.TryParse(Cust2EstDate.Text, culture, DateTimeStyles.AssumeLocal, out cust2EstDate);

                    if (!(b1 && b2))
                    {
                        MessageBox.Show("One or more of the dates do not exist.");
                        return;
                    }

                    if (cust2Date.DayOfWeek == 0 || (int)cust2Date.DayOfWeek == 6 || cust2EstDate.DayOfWeek == 0 || (int)cust2EstDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        run = false;
                    }

                    if (!(int.TryParse(Cust2EstDur.Text, out s1) && int.TryParse(Cust2Dur.Text, out s2) && s2 >= 0 && s1 >= 0))
                    {
                        MessageBox.Show("One of the durations are not numbers.");
                        run = false;
                    }

                    if (Comparison(cust2Date, cust1Date) < 0)
                    {
                        MessageBox.Show("One or more of the dates are in incorrect order.");
                        return;
                    }
                }

                if (Cust3Name.Visible && run)
                {
                    b1 = DateTime.TryParse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal, out cust3Date);
                    b2 = DateTime.TryParse(Cust3EstDate.Text, culture, DateTimeStyles.AssumeLocal, out cust3EstDate);

                    if (!(b1 && b2))
                    {
                        MessageBox.Show("One or more of the dates do not exist.");
                        return;
                    }

                    if (cust3Date.DayOfWeek == 0 || (int)cust3Date.DayOfWeek == 6 || cust3EstDate.DayOfWeek == 0 || (int)cust3EstDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        run = false;
                    }

                    if (!(int.TryParse(Cust3EstDur.Text, out s1) && int.TryParse(Cust3Dur.Text, out s2) && s2 >= 0 && s1 >= 0))
                    {
                        MessageBox.Show("One of the durations are not numbers.");
                        run = false;
                    } 
                    
                    if (Comparison(cust3Date, cust2Date) < 0)
                    {
                        MessageBox.Show("One or more of the dates are in incorrect order.");
                        return;
                    }
                }

                if (Cust4Name.Visible && run)
                {
                    b1 = DateTime.TryParse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal, out cust4Date);
                    b2 = DateTime.TryParse(Cust4EstDate.Text, culture, DateTimeStyles.AssumeLocal, out cust4EstDate);

                    if (!(b1 && b2))
                    {
                        MessageBox.Show("One or more of the dates do not exist.");
                        return;
                    }

                    if (cust4Date.DayOfWeek == 0 || (int)cust4Date.DayOfWeek == 6 || cust4EstDate.DayOfWeek == 0 || (int)cust4EstDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        run = false;
                    }

                    if (!(int.TryParse(Cust4EstDur.Text, out s1) && int.TryParse(Cust4Dur.Text, out s2) && s2 >= 0 && s1 >= 0))
                    {
                        MessageBox.Show("One of the durations are not numbers.");
                        run = false;
                    }

                    if (Comparison(cust4Date, cust3Date) < 0)
                    {
                        MessageBox.Show("One or more of the dates are in incorrect order.");
                        return;
                    }
                }

                if (Cust5Name.Visible && run)
                {
                    b1 = DateTime.TryParse(Cust5Date.Text, culture, DateTimeStyles.AssumeLocal, out cust5Date);
                    b2 = DateTime.TryParse(Cust5EstDate.Text, culture, DateTimeStyles.AssumeLocal, out cust5EstDate);

                    if (!(b1 && b2))
                    {
                        MessageBox.Show("One or more of the dates do not exist.");
                        return;
                    }

                    if (cust5Date.DayOfWeek == 0 || (int)cust5Date.DayOfWeek == 6 || cust5EstDate.DayOfWeek == 0 || (int)cust5EstDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        run = false;
                    }

                    if (!(int.TryParse(Cust5EstDur.Text, out s1) && int.TryParse(Cust5Dur.Text, out s2) && s2 >= 0 && s1 >= 0))
                    {
                        MessageBox.Show("One of the durations are not numbers.");
                        run = false;
                    }

                    if (Comparison(cust5Date, cust4Date) < 0)
                    {
                        MessageBox.Show("One or more of the dates are in incorrect order.");
                        return;
                    }
                }

                if (Cust6Name.Visible && run)
                {
                    b1 = DateTime.TryParse(Cust6Date.Text, culture, DateTimeStyles.AssumeLocal, out cust6Date);
                    b2 = DateTime.TryParse(Cust6EstDate.Text, culture, DateTimeStyles.AssumeLocal, out cust6EstDate);

                    if (!(b1 && b2))
                    {
                        MessageBox.Show("One or more of the dates do not exist.");
                        return;
                    }

                    if (cust6Date.DayOfWeek == 0 || (int)cust6Date.DayOfWeek == 6 || cust6EstDate.DayOfWeek == 0 || (int)cust6EstDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("One or more of the dates fall on a weekend.");
                        run = false;
                    }

                    if (!(int.TryParse(Cust6EstDur.Text, out s1) && int.TryParse(Cust6Dur.Text, out s2) && s2 >= 0 && s1 >= 0))
                    {
                        MessageBox.Show("One of the durations are not numbers.");
                        run = false;
                    }

                    if (Comparison(cust6Date, cust5Date) < 0)
                    {
                        MessageBox.Show("One or more of the dates are in incorrect order.");
                        return;
                    }
                }
            }
            //Now that all the error checking is done, we wil have to see if the order is alright, and then save the Target Version
            if (run)
            {
                var linesInPath = File.ReadAllLines(Globals.path);
                for (int i = 0; i < linesInPath.Length; i++)
                {
                    if (linesInPath[i].StartsWith(Globals.foundString))
                    {
                        linesInPath[i] = VersionName.Text + "•" + DevEstCompDate.Text + "•" + DevCompDate.Text + "•" +DevEstCompDur.Text + "•" + DevCompDur.Text;
                        if (Cust1Name.Visible)
                        {
                            linesInPath[i] += "•" + Cust1Name.Text + "•" + Cust1EstDate.Text + "•" + Cust1Date.Text + "•" + Cust1EstDur.Text + "•" + 
                                Cust1Dur.Text;
                        }
                        if (Cust2Name.Visible)
                        {
                            linesInPath[i] += "•" + Cust2Name.Text + "•" + Cust2EstDate.Text + "•" + Cust2Date.Text + "•" + Cust2EstDur.Text + "•" +
                                Cust2Dur.Text;
                        }
                        if (Cust3Name.Visible)
                        {
                            linesInPath[i] += "•" + Cust3Name.Text + "•" + Cust3EstDate.Text + "•" + Cust3Date.Text + "•" + Cust3EstDur.Text + "•" +
                                Cust3Dur.Text;
                        }
                        if (Cust4Name.Visible)
                        {
                            linesInPath[i] += "•" + Cust4Name.Text + "•" + Cust4EstDate.Text + "•" + Cust4Date.Text + "•" + Cust4EstDur.Text + "•" +
                                Cust4Dur.Text;
                        }
                        if (Cust5Name.Visible)
                        {
                            linesInPath[i] += "•" + Cust5Name.Text + "•" + Cust5EstDate.Text + "•" + Cust5Date.Text + "•" + Cust5EstDur.Text + "•" +
                                Cust5Dur.Text;
                        }
                        if (Cust6Name.Visible)
                        {
                            linesInPath[i] += "•" + Cust6Name.Text + "•" + Cust6EstDate.Text + "•" + Cust6Date.Text + "•" + Cust6EstDur.Text + "•" +
                                Cust6Dur.Text;
                        }
                        linesInPath[i] += "•" + EstRelDate.Text + "•" + RelDate.Text + "•" + DaysGainedOrLost.Text;
                    }
                }
                File.WriteAllLines(Globals.path, linesInPath);
                Globals.foundString = VersionName.Text + "•";
                MessageBox.Show("Target Version Successfully Saved");
            }
        }

        private void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (changeRequested == 1)
            {
                DevCompDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 2)
            {
                Cust1Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 3)
            {
                Cust2Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 4)
            {
                Cust3Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 5)
            {
                Cust4Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 6)
            {
                Cust5Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 7)
            {
                Cust6Date.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 8)
            {
                RelDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 11)
            {
                DevEstCompDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 12)
            {
                Cust1EstDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 13)
            {
                Cust2EstDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 14)
            {
                Cust3EstDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 15)
            {
                Cust4EstDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 16)
            {
                Cust5EstDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 17)
            {
                Cust6EstDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
            if (changeRequested == 18)
            {
                EstRelDate.Text = monthCalendar1.SelectionStart.ToString("d", CultureInfo.InvariantCulture);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 1;
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 2;
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 3;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 4;
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 5;
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 6;
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 7;
        }

        private void pictureBox8_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 8;
        }

        private void pictureBox11_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 11;
        }

        private void pictureBox12_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 12;
        }

        private void pictureBox13_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 13;
        }

        private void pictureBox14_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 14;
        }

        private void pictureBox15_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 15;
        }

        private void pictureBox16_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 16;
        }

        private void pictureBox17_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 17;
        }

        private void pictureBox18_Click(object sender, EventArgs e)
        {
            monthCalendar1.Hide();
            Thread.Sleep(50);
            monthCalendar1.Show();
            changeRequested = 18;
        }

        private void DevCompDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 1)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust1Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 2)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust2Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 3)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust3Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 4)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust4Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 5)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust5Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 6)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust6Date_Click(object sender, EventArgs e)
        {
            if (changeRequested != 7)
            {
                monthCalendar1.Hide();
            }
        }

        private void RelDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 8)
            {
                monthCalendar1.Hide();
            }
        }

        private void DevEstCompDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 11)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust1EstDate_TextChanged(object sender, EventArgs e)
        {
            if (changeRequested != 12)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust2EstDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 13)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust3EstDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 14)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust4EstDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 15)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust5EstDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 16)
            {
                monthCalendar1.Hide();
            }
        }

        private void Cust6EstDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 17)
            {
                monthCalendar1.Hide();
            }
        }

        private void EstRelDate_Click(object sender, EventArgs e)
        {
            if (changeRequested != 18)
            {
                monthCalendar1.Hide();
            }
        }

        private void AddNewEvent_Click(object sender, EventArgs e)
        {
            if (numberOfEvents == 0)
            {
                Cust1Name.Show();
                Cust1EstDate.Show();
                Cust1Date.Show();
                pictureBox2.Show();
                pictureBox12.Show();
                Cust1EstDur.Show();
                Cust1Dur.Show();
                button1.Show();
                numberOfEvents = 1;
            }
            else if (numberOfEvents == 1)
            {
                Cust2Name.Show();
                Cust2EstDate.Show();
                Cust2Date.Show();
                pictureBox3.Show();
                pictureBox13.Show();
                Cust2EstDur.Show();
                Cust2Dur.Show();
                button1.Hide();
                button2.Show();
                numberOfEvents = 2;
            }
            else if (numberOfEvents == 2)
            {
                Cust3Name.Show();
                Cust3EstDate.Show();
                Cust3Date.Show();
                Cust3EstDur.Show();
                Cust3Dur.Show();
                pictureBox4.Show();
                pictureBox14.Show();
                button2.Hide();
                button3.Show();
                numberOfEvents = 3;
            }
            else if (numberOfEvents == 3)
            {
                Cust4Name.Show();
                Cust4EstDate.Show();
                Cust4Date.Show();
                Cust4EstDur.Show();
                Cust4Dur.Show();
                pictureBox5.Show();
                pictureBox15.Show();
                button3.Hide();
                button4.Show();
                numberOfEvents = 4;
            }
            else if (numberOfEvents == 4)
            {
                Cust5Name.Show();
                Cust5EstDate.Show();
                Cust5Date.Show();
                Cust5EstDur.Show();
                Cust5Dur.Show();
                pictureBox6.Show();
                pictureBox16.Show();
                button4.Hide();
                button5.Show();
                AddNewEvent.Show();
                numberOfEvents = 5;
            }
            else if (numberOfEvents == 5)
            {
                Cust6Name.Show();
                Cust6EstDate.Show();
                Cust6Date.Show();
                Cust6EstDur.Show();
                Cust6Dur.Show();
                pictureBox7.Show();
                pictureBox17.Show();
                button5.Hide();
                button6.Show();
                AddNewEvent.Hide();
                numberOfEvents = 6;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cust1Name.Hide();
            Cust1EstDate.Hide();
            Cust1Date.Hide();
            pictureBox2.Hide();
            pictureBox12.Hide();
            Cust1EstDur.Hide();
            Cust1Dur.Hide();
            button1.Hide();
            numberOfEvents = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Cust2Name.Hide();
            Cust2EstDate.Hide();
            Cust2Date.Hide();
            pictureBox3.Hide();
            pictureBox13.Hide();
            Cust2EstDur.Hide();
            Cust2Dur.Hide();
            button1.Show();
            button2.Hide();
            numberOfEvents = 1;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Cust3Name.Hide();
            Cust3EstDate.Hide();
            Cust3Date.Hide();
            Cust3EstDur.Hide();
            Cust3Dur.Hide();
            pictureBox4.Hide();
            pictureBox14.Hide();
            button2.Show();
            button3.Hide();
            numberOfEvents = 2;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Cust4Name.Hide();
            Cust4EstDate.Hide();
            Cust4Date.Hide();
            Cust4EstDur.Hide();
            Cust4Dur.Hide();
            pictureBox5.Hide();
            pictureBox15.Hide();
            button3.Show();
            button4.Hide();
            numberOfEvents = 3;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Cust5Name.Hide();
            Cust5EstDate.Hide();
            Cust5Date.Hide();
            Cust5EstDur.Hide();
            Cust5Dur.Hide();
            pictureBox6.Hide();
            pictureBox16.Hide();
            button4.Show();
            button5.Hide();
            numberOfEvents = 4;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Cust6Name.Hide();
            Cust6EstDate.Hide();
            Cust6Date.Hide();
            Cust6EstDur.Hide();
            Cust6Dur.Hide();
            pictureBox7.Hide();
            pictureBox17.Hide();
            button5.Show();
            button6.Hide();
            AddNewEvent.Show();
            numberOfEvents = 5;
        }
    }
}
