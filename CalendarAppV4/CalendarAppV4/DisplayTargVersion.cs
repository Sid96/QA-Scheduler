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
    public partial class DisplayTargVersion : Form
    {
        IFormatProvider culture = new System.Globalization.CultureInfo("en-US", true);
        int daysLost, changeRequested;
        string[] individualData;
        Color curBackColor;
        Color curForeColor;
        bool storeColor = true;
        bool updateClick = false;
        bool successfulCheck;
        private bool unchanged;
        List<string> undoLines = new List<string>();

        public DisplayTargVersion()
        {
            InitializeComponent();
        }

        private void TargVersEdit_Click(object sender, EventArgs e)
        {
            undoLines.Clear();
            new TargVersionEdit().Show();
            this.Hide();
        }

        private void Back_Click(object sender, EventArgs e)
        {
            undoLines.Clear();
            new Home().Show();
            this.Hide();
        }

        private void DisplayTargVersion_Load(object sender, EventArgs e)
        {
            using (StreamReader sr = new StreamReader(Globals.path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line.StartsWith(Globals.foundString))
                    {
                        individualData = line.Split('•');
                        undoLines.Add(line);
                    }
                }
            }

            VersionName.Text = individualData[0];
            DevEstCompDate.Text = individualData[1];
            DevCompDate.Text = individualData[2];
            DevEstCompDur.Text = individualData[3];
            DevCompDur.Text = individualData[4];

            if (individualData.Length >= 13)
            {
                Cust1Name.Show();
                Cust1EstDate.Show();
                Cust1Date.Show();
                pictureBox2.Show();
                Cust1EstDur.Show();
                Cust1Dur.Show();
                Cust1Name.Text = individualData[5];
                Cust1EstDate.Text = individualData[6];
                Cust1Date.Text = individualData[7];
                Cust1EstDur.Text = individualData[8];
                Cust1Dur.Text = individualData[9];
            }            

            if (individualData.Length>= 18)
            {
                Cust2Name.Show();
                Cust2EstDate.Show();
                Cust2Date.Show();
                pictureBox3.Show();
                Cust2EstDur.Show();
                Cust2Dur.Show();
                Cust2Name.Text = individualData[10];
                Cust2EstDate.Text = individualData[11];
                Cust2Date.Text = individualData[12];
                Cust2EstDur.Text = individualData[13];
                Cust2Dur.Text = individualData[14];
            }

            if (individualData.Length >= 23)
            {
                Cust3Name.Show();
                Cust3EstDate.Show();
                Cust3Date.Show();                
                Cust3EstDur.Show();
                Cust3Dur.Show();
                pictureBox4.Show();
                Cust3Name.Text = individualData[15];
                Cust3EstDate.Text = individualData[16];
                Cust3Date.Text = individualData[17];
                Cust3EstDur.Text = individualData[18];
                Cust3Dur.Text = individualData[19];
            }

            if (individualData.Length >= 28)
            {
                Cust4Name.Show();
                Cust4EstDate.Show();
                Cust4Date.Show();
                Cust4EstDur.Show();
                Cust4Dur.Show();
                pictureBox5.Show();
                Cust4Name.Text = individualData[20];
                Cust4EstDate.Text = individualData[21];
                Cust4Date.Text = individualData[22];
                Cust4EstDur.Text = individualData[23];
                Cust4Dur.Text = individualData[24];
            }

            if (individualData.Length >= 33)
            {
                Cust5Name.Show();
                Cust5EstDate.Show();
                Cust5Date.Show();
                Cust5EstDur.Show();
                Cust5Dur.Show();
                pictureBox6.Show();
                Cust5Name.Text = individualData[25];
                Cust5EstDate.Text = individualData[26];
                Cust5Date.Text = individualData[27];
                Cust5EstDur.Text = individualData[28];
                Cust5Dur.Text = individualData[29];
            }

            if (individualData.Length == 38)
            {
                Cust6Name.Show();
                Cust6EstDate.Show();
                Cust6Date.Show();
                Cust6EstDur.Show();
                Cust6Dur.Show();
                pictureBox7.Show();
                Cust6Name.Text = individualData[30];
                Cust6EstDate.Text = individualData[31];
                Cust6Date.Text = individualData[32];
                Cust6EstDur.Text = individualData[33];
                Cust6Dur.Text = individualData[34];
            }

            EstRelDate.Text = individualData[individualData.Length - 3];
            RelDate.Text = individualData[individualData.Length - 2];

            if (string.IsNullOrEmpty(ChangeInDays.Text))
            {
                ChangeInDays.Text = "0";
            }
            if (individualData[individualData.Length-1] == "0")
            {
                DaysGainedOrLostSoFar.Text = "0";
            }
            else
            {
                DaysGainedOrLostSoFar.Text = individualData[individualData.Length-1];
            }
        }

        private int Comparison(DateTime endD, DateTime startD)
        {
            double calcBusinessDays =
                1 + ((endD - startD).TotalDays * 5 - (startD.DayOfWeek - endD.DayOfWeek) * 2) / 7;

            if ((int)endD.DayOfWeek == 6) calcBusinessDays--;
            if ((int)startD.DayOfWeek == 0) calcBusinessDays--;

            return (int)(calcBusinessDays - 1.0);
        }

        private DateTime AddBusinessDays(DateTime date, int workingDays)
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

        private void Check_Click(object sender, EventArgs e)
        {
            unchanged = true;
            //Assigning dates to the information
            DateTime relDate;
            int devCompSpan, nDay, nMonth, nYear;
            var cust1CompSpan = 0;
            var cust2CompSpan = 0;
            var cust3CompSpan = 0;
            var cust4CompSpan = 0;
            var cust5CompSpan = 0;
            var cust6CompSpan = 0;
            string[] newdmy;
            var devDate = DateTime.Parse(individualData[2], culture, DateTimeStyles.AssumeLocal);
            bool run = int.TryParse(DevCompDur.Text, out devCompSpan);
            if (run && devCompSpan >= 0)
            {
                devCompSpan -= int.Parse(individualData[4]);
            }
            else
            {
                MessageBox.Show("The duration is incorrect. Please try again.");
                DevCompDur.Text = individualData[4];
                return;
            }
            if (Cust1Name.Visible)
            {
                
                run = int.TryParse(Cust1Dur.Text, out cust1CompSpan);
                if (run && cust1CompSpan >= 0)
                {
                    cust1CompSpan -= int.Parse(individualData[9]);
                }
                else
                {
                    MessageBox.Show("The duration is incorrect. Please try again.");
                    Cust1Dur.Text = individualData[9];
                    return;
                }
            }
            if (Cust2Name.Visible)
            {
                
                run = int.TryParse(Cust2Dur.Text, out cust2CompSpan);
                if (run && cust2CompSpan >= 0)
                {
                    cust2CompSpan -= int.Parse(individualData[14]);
                }
                else
                {
                    MessageBox.Show("The duration is incorrect. Please try again.");
                    Cust2Dur.Text = individualData[14];
                    return;
                }
            }
            if (Cust3Name.Visible)
            {
                run = int.TryParse(Cust3Dur.Text, out cust3CompSpan);
                if (run && cust3CompSpan >= 0)
                {
                    cust3CompSpan -= int.Parse(individualData[19]);
                }
                else
                {
                    MessageBox.Show("The duration is incorrect. Please try again.");
                    Cust3Dur.Text = individualData[19];
                    return;
                }
            }
            if (Cust4Name.Visible)
            {
                run = int.TryParse(Cust4Dur.Text, out cust4CompSpan);
                if (run && cust4CompSpan >= 0)
                {
                    cust4CompSpan -= int.Parse(individualData[24]);
                }
                else
                {
                    MessageBox.Show("The duration is incorrect. Please try again.");
                    Cust4Dur.Text = individualData[24];
                    return;
                }
            }
            if (Cust5Name.Visible)
            {
                run = int.TryParse(Cust5Dur.Text, out cust5CompSpan);
                if (run && cust5CompSpan >= 0)
                {
                    cust5CompSpan -= int.Parse(individualData[29]);
                }
                else
                {
                    MessageBox.Show("The duration is incorrect. Please try again.");
                    Cust5Dur.Text = individualData[29];
                    return;
                }
            }
            if (Cust6Name.Visible)
            {
                run = int.TryParse(Cust6Dur.Text, out cust6CompSpan);
                if (run && cust6CompSpan >= 0)
                {
                    cust6CompSpan -= int.Parse(individualData[34]);
                }
                else
                {
                    MessageBox.Show("The duration is incorrect. Please try again.");
                    Cust6Dur.Text = individualData[34];
                    return;
                }
            }

            relDate = DateTime.Parse(individualData[individualData.Length - 2], culture, DateTimeStyles.AssumeLocal);

            if (DevCompDate.Text != individualData[2] ^ devCompSpan != 0)
            {
                newdmy = DevCompDate.Text.Split('/');
                if (newdmy.Length != 3)
                {
                    MessageBox.Show("The new date stored is not a valid date. Please verify new date");
                    DevCompDate.Text = individualData[2];
                    return;
                }
                if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
                      int.TryParse(newdmy[2], out nYear)))
                {
                    MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
                    DevCompDate.Text = individualData[2];
                    return;
                }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        DevCompDate.Text = individualData[2];
                        return;
                    }
                    individualData[2] = DevCompDate.Text;
                    individualData[4] = DevCompDur.Text;
                    if (Cust1Name.Visible)
                    {
                        DateTime newCust1Date = AddBusinessDays(newDate, int.Parse(individualData[4]));
                        Cust1Date.Text = (newCust1Date.ToString("d", CultureInfo.InvariantCulture));
                        individualData[7] = Cust1Date.Text;
                        if (Cust2Name.Visible)
                        {
                            DateTime newCust2Date = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                            Cust2Date.Text = (newCust2Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[12] = Cust2Date.Text;
                            if (Cust3Name.Visible)
                            {
                                DateTime newCust3Date = AddBusinessDays(newCust2Date,
                                    int.Parse(individualData[14]));
                                Cust3Date.Text = (newCust3Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[17] = Cust3Date.Text;
                                if (Cust4Name.Visible)
                                {
                                    DateTime newCust4Date = AddBusinessDays(newCust3Date,
                                        int.Parse(individualData[19]));
                                    Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[22] = Cust4Date.Text;
                                    if (Cust5Name.Visible)
                                    {
                                        DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                            int.Parse(individualData[24]));
                                        Cust5Date.Text =
                                            (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                        individualData[27] = Cust5Date.Text;
                                        if (Cust6Name.Visible)
                                        {
                                            DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                                int.Parse(individualData[29]));
                                            Cust6Date.Text =
                                                (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                            individualData[32] = Cust6Date.Text;
                                            DateTime newRelDate = AddBusinessDays(newCust6Date,
                                                int.Parse(individualData[34]));
                                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                newRelDate = AddBusinessDays(newRelDate, 1);
                                            }
                                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                            individualData[36] = RelDate.Text;
                                            daysLost += Comparison(newRelDate, relDate);
                                            ChangeInDays.Text = (-daysLost).ToString();
                                            unchanged = false;
                                        }
                                        else
                                        {
                                            DateTime newRelDate = AddBusinessDays(newCust5Date,
                                                int.Parse(individualData[29]));
                                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                newRelDate = AddBusinessDays(newRelDate, 1);
                                            }
                                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                            individualData[31] = RelDate.Text;
                                            daysLost += Comparison(newRelDate, relDate);
                                            ChangeInDays.Text = (-daysLost).ToString();
                                            unchanged = false;
                                        }
                                    }
                                    else
                                    {
                                        DateTime newRelDate = AddBusinessDays(newCust4Date,
                                            int.Parse(individualData[24]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[26] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust3Date,
                                        int.Parse(individualData[19]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[21] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust2Date,
                                    int.Parse(individualData[14]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[16] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newCust1Date, int.Parse(individualData[9]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[11] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                    else
                    {
                        DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[4]));
                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                        {
                            newRelDate = AddBusinessDays(newRelDate, 1);
                        }
                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                        individualData[6] = RelDate.Text;
                        daysLost += Comparison(newRelDate, relDate);
                        ChangeInDays.Text = (-daysLost).ToString();
                        unchanged = false;
                    }
                }
                catch
                {
                    DevCompDate.Text = individualData[2];
                    MessageBox.Show("The new date input is not a valid date. Please verify the new date.");
                }
            }
            if (Cust1Name.Visible)
            {
                newdmy = Cust1Date.Text.Split('/');
                if (newdmy.Length != 3)
                {
                    MessageBox.Show("The new date stored is not a valid date. Please verify new date");
                    Cust1Date.Text = individualData[7];
                    return;
                }
                if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
                int.TryParse(newdmy[2], out nYear)))
                {
                    MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
                    Cust1Date.Text = individualData[7];
                    return;
                }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        Cust1Date.Text = individualData[7];
                        return;
                    }
                    if (Cust1Date.Text != individualData[7])
                    {
                        if (Comparison(newDate, devDate) < 0)
                        {
                            MessageBox.Show("The new date provided results in a negative duration.");
                            Cust1Date.Text = individualData[7];
                            return;
                        }
                        individualData[7] = Cust1Date.Text;
                        DevCompDur.Text = Comparison(newDate, devDate).ToString();
                        individualData[4] = DevCompDur.Text;
                        if (Cust2Name.Visible)
                        {
                            DateTime newCust2Date = AddBusinessDays(newDate, int.Parse(individualData[9]));
                            Cust2Date.Text = (newCust2Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[12] = Cust2Date.Text;
                            if (Cust3Name.Visible)
                            {
                                DateTime newCust3Date = AddBusinessDays(newCust2Date, int.Parse(individualData[14]));
                                Cust3Date.Text = (newCust3Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[17] = Cust3Date.Text;
                                if (Cust4Name.Visible)
                                {
                                    DateTime newCust4Date = AddBusinessDays(newCust3Date, int.Parse(individualData[19]));
                                    Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[22] = Cust4Date.Text;
                                    if (Cust5Name.Visible)
                                    {
                                        DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                            int.Parse(individualData[24]));
                                        Cust5Date.Text = (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                        individualData[27] = Cust5Date.Text;
                                        if (Cust6Name.Visible)
                                        {
                                            DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                                int.Parse(individualData[29]));
                                            Cust6Date.Text =
                                                (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                            individualData[32] = Cust6Date.Text;
                                            DateTime newRelDate = AddBusinessDays(newCust6Date,
                                                int.Parse(individualData[34]));
                                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                newRelDate = AddBusinessDays(newRelDate, 1);
                                            }
                                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                            individualData[36] = RelDate.Text;
                                            daysLost += Comparison(newRelDate, relDate);
                                            ChangeInDays.Text = (-daysLost).ToString();
                                            unchanged = false;
                                        }
                                        else
                                        {
                                            DateTime newRelDate = AddBusinessDays(newCust5Date,
                                                int.Parse(individualData[29]));
                                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                newRelDate = AddBusinessDays(newRelDate, 1);
                                            }
                                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                            individualData[31] = RelDate.Text;
                                            daysLost += Comparison(newRelDate, relDate);
                                            ChangeInDays.Text = (-daysLost).ToString();
                                            unchanged = false;
                                        }
                                    }
                                    else
                                    {
                                        DateTime newRelDate = AddBusinessDays(newCust4Date,
                                            int.Parse(individualData[24]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[26] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust3Date,
                                        int.Parse(individualData[19]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[21] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust2Date,
                                    int.Parse(individualData[14]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[16] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[9]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[11] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                    else if (cust1CompSpan != 0)
                    {
                        individualData[9] = Cust1Dur.Text;
                        if (Cust2Name.Visible)
                        {
                            DateTime newCust2Date = AddBusinessDays(newDate, int.Parse(individualData[9]));
                            Cust2Date.Text = (newCust2Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[12] = Cust2Date.Text;
                            if (Cust3Name.Visible)
                            {
                                DateTime newCust3Date = AddBusinessDays(newCust2Date,
                                    int.Parse(individualData[14]));
                                Cust3Date.Text = (newCust3Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[17] = Cust3Date.Text;
                                if (Cust4Name.Visible)
                                {
                                    DateTime newCust4Date = AddBusinessDays(newCust3Date,
                                        int.Parse(individualData[19]));
                                    Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[22] = Cust4Date.Text;
                                    if (Cust5Name.Visible)
                                    {
                                        DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                            int.Parse(individualData[24]));
                                        Cust5Date.Text =
                                            (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                        individualData[27] = Cust5Date.Text;
                                        if (Cust6Name.Visible)
                                        {
                                            DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                                int.Parse(individualData[29]));
                                            Cust6Date.Text =
                                                (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                            individualData[32] = Cust6Date.Text;
                                            DateTime newRelDate = AddBusinessDays(newCust6Date,
                                                int.Parse(individualData[34]));
                                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                newRelDate = AddBusinessDays(newRelDate, 1);
                                            }
                                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                            individualData[36] = RelDate.Text;
                                            daysLost += Comparison(newRelDate, relDate);
                                            ChangeInDays.Text = (-daysLost).ToString();
                                            unchanged = false;
                                        }
                                        else
                                        {
                                            DateTime newRelDate = AddBusinessDays(newCust5Date,
                                                int.Parse(individualData[29]));
                                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                            {
                                                newRelDate = AddBusinessDays(newRelDate, 1);
                                            }
                                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                            individualData[31] = RelDate.Text;
                                            daysLost += Comparison(newRelDate, relDate);
                                            ChangeInDays.Text = (-daysLost).ToString();
                                            unchanged = false;
                                        }
                                    }
                                    else
                                    {
                                        DateTime newRelDate = AddBusinessDays(newCust4Date,
                                            int.Parse(individualData[24]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[26] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust3Date,
                                        int.Parse(individualData[19]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[21] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust2Date,
                                    int.Parse(individualData[14]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[16] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[9]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[11] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                }
                catch
                {
                    Cust1Date.Text = individualData[7];
                    MessageBox.Show("The new date input is not a valid date. Please verify the new date.");
                }
            }
            if (Cust2Name.Visible)
            {
                newdmy = Cust2Date.Text.Split('/');
                if (newdmy.Length != 3)
                {
                    MessageBox.Show("The new date stored is not a valid date. Please verify new date");
                    Cust2Date.Text = individualData[12];
                    return;
                }
                if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
                int.TryParse(newdmy[2], out nYear)))
                {
                    MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
                    Cust2Date.Text = individualData[12];
                    return;
                }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        Cust2Date.Text = individualData[12];
                        return;
                    }
                    if (Cust2Date.Text != individualData[12])
                    {
                        if (Comparison(newDate, DateTime.Parse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal)) < 0)
                        {
                            MessageBox.Show("The new date provided results in a negative duration.");
                            Cust2Date.Text = individualData[12];
                            return;
                        }
                        individualData[12] = Cust2Date.Text;
                        Cust1Dur.Text = Comparison(newDate, DateTime.Parse(Cust1Date.Text, culture, DateTimeStyles.AssumeLocal)).ToString();
                        individualData[9] = Cust1Dur.Text;
                        if (Cust3Name.Visible)
                        {
                            DateTime newCust3Date = AddBusinessDays(newDate, int.Parse(individualData[14]));
                            Cust3Date.Text = (newCust3Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[17] = Cust3Date.Text;
                            if (Cust4Name.Visible)
                            {
                                DateTime newCust4Date = AddBusinessDays(newCust3Date, int.Parse(individualData[19]));
                                Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[22] = Cust4Date.Text;
                                if (Cust5Name.Visible)
                                {
                                    DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                        int.Parse(individualData[24]));
                                    Cust5Date.Text = (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[27] = Cust5Date.Text;
                                    if (Cust6Name.Visible)
                                    {
                                        DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                            int.Parse(individualData[29]));
                                        Cust6Date.Text =
                                            (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                        individualData[32] = Cust6Date.Text;
                                        DateTime newRelDate = AddBusinessDays(newCust6Date,
                                            int.Parse(individualData[34]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[36] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                    else
                                    {
                                        DateTime newRelDate = AddBusinessDays(newCust5Date,
                                            int.Parse(individualData[29]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[31] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust4Date,
                                        int.Parse(individualData[24]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[26] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust3Date,
                                    int.Parse(individualData[19]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[21] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[14]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday && newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[16] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                    else if (cust2CompSpan != 0)
                    {
                        individualData[14] = Cust2Dur.Text;
                        if (Cust3Name.Visible)
                        {
                            DateTime newCust3Date = AddBusinessDays(newDate,
                                int.Parse(individualData[14]));
                            Cust3Date.Text = (newCust3Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[17] = Cust3Date.Text;
                            if (Cust4Name.Visible)
                            {
                                DateTime newCust4Date = AddBusinessDays(newCust3Date,
                                    int.Parse(individualData[19]));
                                Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[22] = Cust4Date.Text;
                                if (Cust5Name.Visible)
                                {
                                    DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                        int.Parse(individualData[24]));
                                    Cust5Date.Text =
                                        (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[27] = Cust5Date.Text;
                                    if (Cust6Name.Visible)
                                    {
                                        DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                            int.Parse(individualData[29]));
                                        Cust6Date.Text =
                                            (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                        individualData[32] = Cust6Date.Text;
                                        DateTime newRelDate = AddBusinessDays(newCust6Date,
                                            int.Parse(individualData[34]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[36] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                    else
                                    {
                                        DateTime newRelDate = AddBusinessDays(newCust5Date,
                                            int.Parse(individualData[29]));
                                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                               newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                        {
                                            newRelDate = AddBusinessDays(newRelDate, 1);
                                        }
                                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                        individualData[31] = RelDate.Text;
                                        daysLost += Comparison(newRelDate, relDate);
                                        ChangeInDays.Text = (-daysLost).ToString();
                                        unchanged = false;
                                    }
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust4Date,
                                        int.Parse(individualData[24]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[26] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust3Date,
                                    int.Parse(individualData[19]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[21] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[14]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[16] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                }
                catch
                {
                    Cust2Date.Text = individualData[12];
                    MessageBox.Show("The new date input is not a valid date. Please verify the new date.");
                }
            }
            if (Cust3Name.Visible)
            {
                newdmy = Cust3Date.Text.Split('/');
	            if (newdmy.Length != 3)
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date");
		            Cust3Date.Text = individualData[17];
		            return;
	            }
	            if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
	            int.TryParse(newdmy[2], out nYear)))
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
		            Cust3Date.Text = individualData[17];
		            return;
	            }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        Cust3Date.Text = individualData[17];
                        return;
                    }
                    if (Cust3Date.Text != individualData[17])
                    {
                        if (Comparison(newDate, DateTime.Parse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal)) < 0)
                        {
                            MessageBox.Show("The new date provided results in a negative duration.");
                            Cust3Date.Text = individualData[17];
                            return;
                        }
                        individualData[17] = Cust3Date.Text;
                        Cust2Dur.Text = Comparison(newDate, DateTime.Parse(Cust2Date.Text, culture, DateTimeStyles.AssumeLocal)).ToString();
                        individualData[14] = Cust2Dur.Text;
                        if (Cust4Name.Visible)
                        {
                            DateTime newCust4Date = AddBusinessDays(newDate, int.Parse(individualData[19]));
                            Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[22] = Cust4Date.Text;
                            if (Cust5Name.Visible)
                            {
                                DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                    int.Parse(individualData[24]));
                                Cust5Date.Text = (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[27] = Cust5Date.Text;
                                if (Cust6Name.Visible)
                                {
                                    DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                        int.Parse(individualData[29]));
                                    Cust6Date.Text =
                                        (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[32] = Cust6Date.Text;
                                    DateTime newRelDate = AddBusinessDays(newCust6Date,
                                        int.Parse(individualData[34]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[36] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust5Date,
                                        int.Parse(individualData[29]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[31] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust4Date,
                                    int.Parse(individualData[24]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[26] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate,
                                int.Parse(individualData[19]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[21] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                    else if (cust3CompSpan != 0)
                    {
                        individualData[19] = Cust3Dur.Text;
                        if (Cust4Name.Visible)
                        {
                            DateTime newCust4Date = AddBusinessDays(newDate, int.Parse(individualData[19]));
                            Cust4Date.Text = (newCust4Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[22] = Cust4Date.Text;
                            if (Cust5Name.Visible)
                            {
                                DateTime newCust5Date = AddBusinessDays(newCust4Date,
                                    int.Parse(individualData[24]));
                                Cust5Date.Text =
                                    (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[27] = Cust5Date.Text;
                                if (Cust6Name.Visible)
                                {
                                    DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                        int.Parse(individualData[29]));
                                    Cust6Date.Text =
                                        (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                    individualData[32] = Cust6Date.Text;
                                    DateTime newRelDate = AddBusinessDays(newCust6Date,
                                        int.Parse(individualData[34]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[36] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                                else
                                {
                                    DateTime newRelDate = AddBusinessDays(newCust5Date,
                                        int.Parse(individualData[29]));
                                    while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                    {
                                        newRelDate = AddBusinessDays(newRelDate, 1);
                                    }
                                    RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                    individualData[31] = RelDate.Text;
                                    daysLost += Comparison(newRelDate, relDate);
                                    ChangeInDays.Text = (-daysLost).ToString();
                                    unchanged = false;
                                }
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust4Date,
                                    int.Parse(individualData[24]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[26] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate,
                                int.Parse(individualData[19]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[21] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                }
                catch
                {
                    Cust3Date.Text = individualData[17];
                    MessageBox.Show("The new date input is not a valid date. Please verify the new date.");
                }
            }
            if (Cust4Name.Visible)
            {
                newdmy = Cust4Date.Text.Split('/');
	            if (newdmy.Length != 3)
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date");
		            Cust4Date.Text = individualData[22];
		            return;
	            }
	            if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
	            int.TryParse(newdmy[2], out nYear)))
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
		            Cust4Date.Text = individualData[22];
		            return;
	            }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        Cust4Date.Text = individualData[22];
                        return;
                    }
                    if (Cust4Date.Text != individualData[22])
                    {
                        if (Comparison(newDate, DateTime.Parse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal)) < 0)
                        {
                            MessageBox.Show("The new date provided results in a negative duration.");
                            Cust4Date.Text = individualData[22];
                            return;
                        }
                        individualData[22] = Cust4Date.Text;
                        Cust3Dur.Text = Comparison(newDate, DateTime.Parse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal)).ToString();
                        individualData[19] = Cust3Dur.Text;
                        if (Cust5Name.Visible)
                        {
                            DateTime newCust5Date = AddBusinessDays(newDate,
                                int.Parse(individualData[24]));
                            Cust5Date.Text = (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[27] = Cust5Date.Text;
                            if (Cust6Name.Visible)
                            {
                                DateTime newCust6Date = AddBusinessDays(newCust5Date,
                                    int.Parse(individualData[29]));
                                Cust6Date.Text =
                                    (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[32] = Cust6Date.Text;
                                DateTime newRelDate = AddBusinessDays(newCust6Date,
                                    int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                        newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                        newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[36] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust5Date,
                                    int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                        newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                        newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[31] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate,
                                int.Parse(individualData[24]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                    newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                    newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[26] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                    else if (cust4CompSpan != 0)
                    {
                        individualData[24] = Cust4Dur.Text;
                        if (Cust5Name.Visible)
                        {
                            DateTime newCust5Date = AddBusinessDays(newDate, int.Parse(individualData[24]));
                            Cust5Date.Text =
                                (newCust5Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[27] = Cust5Date.Text;
                            if (Cust6Name.Visible)
                            {
                                DateTime newCust6Date = AddBusinessDays(newCust5Date, int.Parse(individualData[29]));
                                Cust6Date.Text = (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                                individualData[32] = Cust6Date.Text;
                                DateTime newRelDate = AddBusinessDays(newCust6Date, int.Parse(individualData[34]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[36] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                            else
                            {
                                DateTime newRelDate = AddBusinessDays(newCust5Date, int.Parse(individualData[29]));
                                while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                       newRelDate.DayOfWeek != DayOfWeek.Thursday)
                                {
                                    newRelDate = AddBusinessDays(newRelDate, 1);
                                }
                                RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                                individualData[31] = RelDate.Text;
                                daysLost += Comparison(newRelDate, relDate);
                                ChangeInDays.Text = (-daysLost).ToString();
                                unchanged = false;
                            }
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[24]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[26] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                }
                catch
                {
                    Cust4Date.Text = individualData[22];
                    MessageBox.Show("The new date input is not a valid date. Please verify the new date.");
                }
            }
            if (Cust5Name.Visible)
            {
                newdmy = Cust5Date.Text.Split('/');
	            if (newdmy.Length != 3)
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date");
		            Cust5Date.Text = individualData[27];
		            return;
	            } 
	            if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
	            int.TryParse(newdmy[2], out nYear)))
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
		            Cust5Date.Text = individualData[27];
		            return;
	            }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        Cust5Date.Text = individualData[27];
                        return;
                    }
                    if (Cust5Date.Text != individualData[27])
                    {
                        if (Comparison(newDate, DateTime.Parse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal)) < 0)
                        {
                            MessageBox.Show("The new date provided results in a negative duration.");
                            Cust5Date.Text = individualData[27];
                            return;
                        }
                        individualData[27] = Cust5Date.Text;
                        Cust4Dur.Text = Comparison(newDate, DateTime.Parse(Cust4Date.Text, culture, DateTimeStyles.AssumeLocal)).ToString();
                        individualData[24] = Cust4Dur.Text;
                        if (Cust6Name.Visible)
                        {
                            DateTime newCust6Date = AddBusinessDays(newDate,
                                int.Parse(individualData[29]));
                            Cust6Date.Text =
                                (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[32] = Cust6Date.Text;
                            DateTime newRelDate = AddBusinessDays(newCust6Date,
                                int.Parse(individualData[34]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                    newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                    newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[36] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate,
                                int.Parse(individualData[29]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                    newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                    newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[31] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                    else if (cust5CompSpan != 0)
                    {
                        individualData[29] = Cust5Dur.Text;
                        if (Cust6Name.Visible)
                        {
                            DateTime newCust6Date = AddBusinessDays(newDate, int.Parse(individualData[29]));
                            Cust6Date.Text = (newCust6Date.ToString("d", CultureInfo.InvariantCulture));
                            individualData[32] = Cust6Date.Text;
                            DateTime newRelDate = AddBusinessDays(newCust6Date, int.Parse(individualData[34]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[36] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                        else
                        {
                            DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[29]));
                            while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                                   newRelDate.DayOfWeek != DayOfWeek.Thursday)
                            {
                                newRelDate = AddBusinessDays(newRelDate, 1);
                            }
                            RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                            individualData[31] = RelDate.Text;
                            daysLost += Comparison(newRelDate, relDate);
                            ChangeInDays.Text = (-daysLost).ToString();
                            unchanged = false;
                        }
                    }
                }
                catch
                {
                    Cust5Date.Text = individualData[27];
                    MessageBox.Show(("The new date input is not a valid date. Please verify the new date."));
                }
            }
            if (Cust6Name.Visible)
            {
                newdmy = Cust6Date.Text.Split('/');
	            if (newdmy.Length != 3)
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date");
		            Cust6Date.Text = individualData[32];
		            return;
	            }
	            if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) &&
	            int.TryParse(newdmy[2], out nYear)))
	            {
		            MessageBox.Show("The new date stored is not a valid date. Please verify new date.");
		            Cust6Date.Text = individualData[32];
		            return;
	            }
                try
                {
                    DateTime newDate = new DateTime(nYear, nMonth, nDay);
                    if (newDate.DayOfWeek == 0 || (int) newDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        Cust6Date.Text = individualData[32];
                        return;
                    }
                    if (Cust6Date.Text != individualData[32])
                    {
                        if (Comparison(newDate, DateTime.Parse(Cust5Date.Text, culture, DateTimeStyles.AssumeLocal)) < 0)
                        {
                            MessageBox.Show("The new date provided results in a negative duration.");
                            Cust6Date.Text = individualData[32];
                            return;
                        }
                        individualData[32] = Cust6Date.Text;
                        Cust5Dur.Text = Comparison(newDate, DateTime.Parse(Cust3Date.Text, culture, DateTimeStyles.AssumeLocal)).ToString();
                        individualData[29] = Cust5Date.Text;
                        DateTime newRelDate = AddBusinessDays(newDate,int.Parse(individualData[34]));
                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday && newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                            newRelDate.DayOfWeek != DayOfWeek.Thursday)
                        {
                            newRelDate = AddBusinessDays(newRelDate, 1);
                        }
                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                        individualData[36] = RelDate.Text;
                        daysLost += Comparison(newRelDate, relDate);
                        ChangeInDays.Text = (-daysLost).ToString();
                        unchanged = false;
                    }
                    else if (cust6CompSpan != 0)
                    {
                        individualData[34] = Cust6Dur.Text;
                        DateTime newRelDate = AddBusinessDays(newDate, int.Parse(individualData[34]));
                        while (newRelDate.DayOfWeek != DayOfWeek.Tuesday &&
                           newRelDate.DayOfWeek != DayOfWeek.Wednesday &&
                           newRelDate.DayOfWeek != DayOfWeek.Thursday)
                        {
                            newRelDate = AddBusinessDays(newRelDate, 1);
                        }
                        RelDate.Text = newRelDate.ToString("d", CultureInfo.InvariantCulture);
                        individualData[36] = RelDate.Text;
                        daysLost += Comparison(newRelDate, relDate);
                        ChangeInDays.Text = (-daysLost).ToString();
                        unchanged = false;
                    }
                }
                catch
                {
                    Cust6Date.Text = individualData[32];
                    MessageBox.Show("The new date input is not a valid date. Please verify the new date.");
                }
            }
            else if (RelDate.Text != individualData[individualData.Length-2])
            {
                newdmy = RelDate.Text.Split('/');
                if (newdmy.Length != 3)
                {
                    MessageBox.Show("The new date stored is not an actual date. Please verify new date.");
                    RelDate.Text = individualData[individualData.Length-2];
                    return;
                }
                if (!(int.TryParse(newdmy[1], out nDay) && int.TryParse(newdmy[0], out nMonth) && int.TryParse(newdmy[2], out nYear)))
                {
                    MessageBox.Show("The new date stored is not an actual date. Please verify new date.");
                    RelDate.Text = individualData[individualData.Length - 2];
                    return;
                }
                try
                {
                    DateTime newRelDate = new DateTime(nYear, nMonth, nDay);
                    if (newRelDate.DayOfWeek == 0 || (int) newRelDate.DayOfWeek == 6)
                    {
                        MessageBox.Show("This date is invalid, as it falls on a weekend.");
                        RelDate.Text = individualData[individualData.Length - 2];
                        return;
                    }
                    DateTime previousDate = DateTime.Parse(individualData[individualData.Length - 6], culture, DateTimeStyles.AssumeLocal);
                    if (Comparison(newRelDate, previousDate) < 0)
                    {
                        MessageBox.Show("This date is invalid, since the release date will be before the event preceding it. Please try again.");
                        RelDate.Text = individualData[individualData.Length - 2];
                        return;
                    }
                    individualData[individualData.Length - 2] = RelDate.Text;
                    daysLost += Comparison(newRelDate, relDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    unchanged = false;
                }
                catch
                {
                    MessageBox.Show("The new date stored is not an actual date. Please verify new date.");
                    RelDate.Text = individualData[individualData.Length - 2];
                    return;
                }
            }
            if (unchanged)
            {
                //If nothing has happened, it goes to this part of the loop
                if (!updateClick)
                {
                    MessageBox.Show("Nothing has changed!");
                    return;
                }
            }

            //If the release date has been changed by more than 2 days, the box's background color will become red (as an alert to the user)

            if (int.Parse(ChangeInDays.Text) < -2)
            {
                if (storeColor)
                {

                    curBackColor = ChangeInDays.BackColor;
                    curForeColor = ChangeInDays.ForeColor;
                    storeColor = false;
                }
                ChangeInDays.BackColor = Color.Red;
                ChangeInDays.ForeColor = Color.White;
            }
            else
            {
                ChangeInDays.BackColor = curBackColor;
                ChangeInDays.ForeColor = curForeColor;
                storeColor = true;
            }
            //Next thing to do is store all this information in temporary storage and update it every time something is changed, so it is possible to undo it.
            string line = VersionName.Text + "•" + DevEstCompDate.Text + "•" + DevCompDate.Text + "•" +
                            DevEstCompDur.Text + "•" + DevCompDur.Text + "•";
            if (individualData.Length > 8)
            {
                line += Cust1Name.Text + "•" + Cust1EstDate.Text + "•" + Cust1Date.Text + "•" + Cust1EstDur.Text +
                        "•" + Cust1Dur.Text + "•";
            }
            if (individualData.Length > 13)
            {
                line += Cust2Name.Text + "•" + Cust2EstDate.Text + "•" + Cust2Date.Text + "•" + Cust2EstDur.Text +
                        "•" + Cust2Dur.Text + "•";
            }
            if (individualData.Length > 18)
            {
                line += Cust3Name.Text + "•" + Cust3EstDate.Text + "•" + Cust3Date.Text + "•" + Cust3EstDur.Text +
                        "•" + Cust3Dur.Text + "•";
            }
            if (individualData.Length > 23)
            {
                line += Cust4Name.Text + "•" + Cust4EstDate.Text + "•" + Cust4Date.Text + "•" + Cust4EstDur.Text +
                        "•" + Cust4Dur.Text + "•";
            }
            if (individualData.Length > 28)
            {
                line += Cust5Name.Text + "•" + Cust5EstDate.Text + "•" + Cust5Date.Text + "•" + Cust5EstDur.Text +
                        "•" + Cust5Dur.Text + "•";
            }
            if (individualData.Length > 33)
            {
                line += Cust6Name.Text + "•" + Cust6EstDate.Text + "•" + Cust6Date.Text + "•" + Cust6EstDur.Text +
                        "•" + Cust6Dur.Text + "•";
            }
            line += EstRelDate.Text + "•" + RelDate.Text + "•" + individualData[individualData.Length - 1];
            undoLines.Add(line);
            
            //Lastly we report that the check has gotten through and we're all good.
            successfulCheck = true;
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            undoLines.Clear();
            ChangeInDays.BackColor = curBackColor;
            ChangeInDays.ForeColor = curForeColor;
            DisplayTargVersion_Load(sender, e);
            ChangeInDays.Text = "0";
            daysLost = 0;
        }

        private void Updates_Click(object sender, EventArgs e)
        {
            undoLines.Clear();
            updateClick = true;
            successfulCheck = false;
            Check_Click(sender, e);
            if (!successfulCheck)
            {
                MessageBox.Show("Update unsuccessful");
                return;
            }
            updateClick = false;
            string[] linesInPath = File.ReadAllLines(Globals.path);
            for (int i = 0; i < linesInPath.Length; i++)
            {
                if (linesInPath[i].StartsWith(Globals.foundString))
                {
                    int updateDGOLSF;
                    if (int.TryParse(ChangeInDays.Text, out updateDGOLSF))
                    {
                        updateDGOLSF += int.Parse(individualData[individualData.Length-1]);
                        linesInPath[i] = VersionName.Text + "•" + DevEstCompDate.Text + "•" + DevCompDate.Text + "•" + DevEstCompDur.Text + "•" + DevCompDur.Text;
                        if (individualData.Length >= 13)
                        {
                            linesInPath[i] += "•" + Cust1Name.Text + "•" + Cust1EstDate.Text + "•" + Cust1Date.Text + "•" + Cust1EstDur.Text + "•" +Cust1Dur.Text;
                        }
                        if (individualData.Length >= 18)
                        {
                            linesInPath[i] += "•" + Cust2Name.Text + "•" + Cust2EstDate.Text + "•" + Cust2Date.Text + "•" + Cust2EstDur.Text + "•" +Cust2Dur.Text;
                        }
                        if (individualData.Length >= 23)
                        {
                            linesInPath[i] += "•" + Cust3Name.Text + "•" + Cust3EstDate.Text + "•" + Cust3Date.Text + "•" + Cust3EstDur.Text + "•" +Cust3Dur.Text;
                        }
                        if (individualData.Length >= 28)
                        {
                            linesInPath[i] += "•" + Cust4Name.Text + "•" + Cust4EstDate.Text + "•" + Cust4Date.Text + "•" + Cust4EstDur.Text + "•" +Cust4Dur.Text;
                        }
                        if (individualData.Length >= 33)
                        {
                            linesInPath[i] += "•" + Cust5Name.Text + "•" + Cust5EstDate.Text + "•" + Cust5Date.Text + "•" + Cust5EstDur.Text + "•" +Cust5Dur.Text;
                        }
                        if (individualData.Length >= 38)
                        {
                            linesInPath[i] += "•" + Cust6Name.Text + "•" + Cust6EstDate.Text + "•" + Cust6Date.Text + "•" + Cust6EstDur.Text + "•" +Cust6Dur.Text;
                        }
                        linesInPath[i] += "•" + EstRelDate.Text + "•" + RelDate.Text + "•" + updateDGOLSF.ToString();
                    }
                    else
                    {
                        linesInPath[i] = VersionName.Text + "•" + DevEstCompDate.Text + "•" + DevCompDate.Text + "•" + DevEstCompDur.Text + "•" + DevCompDur.Text;
                        if (individualData.Length >= 13)
                        {
                            linesInPath[i] += "•" + Cust1Name.Text + "•" + Cust1EstDate.Text + "•" + Cust1Date.Text + "•" + Cust1EstDur.Text + "•" +Cust1Dur.Text;
                        }
                        if (individualData.Length >= 18)
                        {
                            linesInPath[i] += "•" + Cust2Name.Text + "•" + Cust2EstDate.Text + "•" + Cust2Date.Text + "•" + Cust2EstDur.Text + "•" +Cust2Dur.Text;
                        }
                        if (individualData.Length >= 23)
                        {
                            linesInPath[i] += "•" + Cust3Name.Text + "•" + Cust3EstDate.Text + "•" + Cust3Date.Text + "•" + Cust3EstDur.Text + "•" +Cust3Dur.Text;
                        }
                        if (individualData.Length >= 28)
                        {
                            linesInPath[i] += "•" + Cust4Name.Text + "•" + Cust4EstDate.Text + "•" + Cust4Date.Text + "•" + Cust4EstDur.Text + "•" +Cust4Dur.Text;
                        }
                        if (individualData.Length >= 33)
                        {
                            linesInPath[i] += "•" + Cust5Name.Text + "•" + Cust5EstDate.Text + "•" + Cust5Date.Text + "•" + Cust5EstDur.Text + "•" +Cust5Dur.Text;
                        }
                        if (individualData.Length >= 38)
                        {
                            linesInPath[i] += "•" + Cust6Name.Text + "•" + Cust6EstDate.Text + "•" + Cust6Date.Text + "•" + Cust6EstDur.Text + "•" +Cust6Dur.Text;
                        }
                        linesInPath[i] += "•" + EstRelDate.Text + "•" + RelDate.Text + "•" + individualData[15];
                    }
                }
            }
            ChangeInDays.Text = null;
            File.WriteAllLines(Globals.path, linesInPath);
            Reset_Click(sender, e);
            MessageBox.Show("Target Version Successfully Updated");
        }

        private void DisplayTargVersion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            undoLines.Clear();
            DialogResult result1 = MessageBox.Show("This will delete the Target Version. Are you sure " +
                       "you want to proceed?", "Delete Target Version?", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                new Home().Show();
                var oldLines = System.IO.File.ReadAllLines(Globals.path);
                var newLines = oldLines.Where(line => !line.Contains(Globals.foundString));
                System.IO.File.WriteAllLines(Globals.path, newLines);
                this.Hide();
            }
        }

        private void Undo_Click(object sender, EventArgs e)
        {
            if (individualData.Length == 8)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] || RelDate.Text != individualData[6])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    RelDate.Text = individualData[6];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[6] = tempArray[6];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        RelDate.Text = individualData[14];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]), int.Parse(RelDate.Text.Split('/')[0]), 
                        int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
            }
            else if (individualData.Length == 13)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] || Cust1Date.Text != individualData[7] ||
                    Cust1Dur.Text != individualData[9] || RelDate.Text != individualData[11])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    Cust1Date.Text = individualData[7];
                    Cust1Dur.Text = individualData[9];
                    RelDate.Text = individualData[11];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[7] = tempArray[7];
                        individualData[9] = tempArray[9];
                        individualData[11] = tempArray[11];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        Cust1Date.Text = individualData[7];
                        Cust1Dur.Text = individualData[9];
                        RelDate.Text = individualData[14];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]), int.Parse(RelDate.Text.Split('/')[0]), int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
            }
            else if (individualData.Length == 18)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] || Cust1Date.Text != individualData[7] ||
                    Cust1Dur.Text != individualData[9] || Cust2Date.Text != individualData[12] || Cust2Dur.Text != individualData[14] || 
                    RelDate.Text != individualData[16])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    Cust1Date.Text = individualData[7];
                    Cust1Dur.Text = individualData[9];
                    Cust2Date.Text = individualData[12];
                    Cust2Dur.Text = individualData[14];
                    RelDate.Text = individualData[16];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[7] = tempArray[7];
                        individualData[9] = tempArray[9];
                        individualData[12] = tempArray[12];
                        individualData[14] = tempArray[14];
                        individualData[16] = tempArray[16];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        Cust1Date.Text = individualData[7];
                        Cust1Dur.Text = individualData[9];
                        Cust2Date.Text = individualData[12];
                        Cust2Dur.Text = individualData[14];
                        RelDate.Text = individualData[16];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]), int.Parse(RelDate.Text.Split('/')[0]), int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
            }
            else if (individualData.Length == 23)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] || Cust1Date.Text != individualData[7] ||
                     Cust1Dur.Text != individualData[9] || Cust2Date.Text != individualData[12] || Cust2Dur.Text != individualData[14] ||
                     Cust3Date.Text != individualData[17] || Cust3Dur.Text != individualData[19] || RelDate.Text != individualData[21])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    Cust1Date.Text = individualData[7];
                    Cust1Dur.Text = individualData[9];
                    Cust2Date.Text = individualData[12];
                    Cust2Dur.Text = individualData[14];
                    Cust3Date.Text = individualData[17];
                    Cust3Dur.Text = individualData[19];
                    RelDate.Text = individualData[21];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[7] = tempArray[7];
                        individualData[9] = tempArray[9];
                        individualData[12] = tempArray[12];
                        individualData[14] = tempArray[14];
                        individualData[17] = tempArray[17]; 
                        individualData[19] = tempArray[19];
                        individualData[21] = tempArray[21];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        Cust1Date.Text = individualData[7];
                        Cust1Dur.Text = individualData[9];
                        Cust2Date.Text = individualData[12];
                        Cust2Dur.Text = individualData[14];
                        Cust3Date.Text = individualData[17];
                        Cust3Dur.Text = individualData[19];
                        RelDate.Text = individualData[21];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]), int.Parse(RelDate.Text.Split('/')[0]), int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
            }
            else if (individualData.Length == 28)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] || Cust1Date.Text != individualData[7] ||
                     Cust1Dur.Text != individualData[9] || Cust2Date.Text != individualData[12] || Cust2Dur.Text != individualData[14] ||
                     Cust3Date.Text != individualData[17] || Cust3Dur.Text != individualData[19] || Cust4Date.Text != individualData[22] ||
                     Cust4Dur.Text != individualData[24] || RelDate.Text != individualData[26])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    Cust1Date.Text = individualData[7];
                    Cust1Dur.Text = individualData[9];
                    Cust2Date.Text = individualData[12];
                    Cust2Dur.Text = individualData[14];
                    Cust3Date.Text = individualData[17];
                    Cust3Dur.Text = individualData[19];
                    Cust4Date.Text = individualData[22];
                    Cust4Dur.Text = individualData[24];
                    RelDate.Text = individualData[26];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[7] = tempArray[7];
                        individualData[9] = tempArray[9];
                        individualData[12] = tempArray[12];
                        individualData[14] = tempArray[14];
                        individualData[17] = tempArray[17];
                        individualData[19] = tempArray[19];
                        individualData[22] = tempArray[22];
                        individualData[24] = tempArray[24];
                        individualData[26] = tempArray[26];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        Cust1Date.Text = individualData[7];
                        Cust1Dur.Text = individualData[9];
                        Cust2Date.Text = individualData[12];
                        Cust2Dur.Text = individualData[14];
                        Cust3Date.Text = individualData[17];
                        Cust3Dur.Text = individualData[19];
                        Cust4Date.Text = individualData[22];
                        Cust4Dur.Text = individualData[24];
                        RelDate.Text = individualData[26];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]), int.Parse(RelDate.Text.Split('/')[0]), int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
            }
            else if (individualData.Length == 33)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] || Cust1Date.Text != individualData[7] ||
                      Cust1Dur.Text != individualData[9] || Cust2Date.Text != individualData[12] || Cust2Dur.Text != individualData[14] ||
                      Cust3Date.Text != individualData[17] || Cust3Dur.Text != individualData[19] || Cust4Date.Text != individualData[22] ||
                      Cust4Dur.Text != individualData[24] || Cust5Date.Text != individualData[27] || Cust5Dur.Text != individualData[29] ||
                      RelDate.Text != individualData[31])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    Cust1Date.Text = individualData[7];
                    Cust1Dur.Text = individualData[9];
                    Cust2Date.Text = individualData[12];
                    Cust2Dur.Text = individualData[14];
                    Cust3Date.Text = individualData[17];
                    Cust3Dur.Text = individualData[19];
                    Cust4Date.Text = individualData[22];
                    Cust4Dur.Text = individualData[24];
                    Cust5Date.Text = individualData[27];
                    Cust5Dur.Text = individualData[29];
                    RelDate.Text = individualData[31];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[7] = tempArray[7];
                        individualData[9] = tempArray[9];
                        individualData[12] = tempArray[12];
                        individualData[14] = tempArray[14];
                        individualData[17] = tempArray[17];
                        individualData[19] = tempArray[19];
                        individualData[22] = tempArray[22];
                        individualData[24] = tempArray[24];
                        individualData[27] = tempArray[27]; 
                        individualData[29] = tempArray[29];
                        individualData[31] = tempArray[31];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        Cust1Date.Text = individualData[7];
                        Cust1Dur.Text = individualData[9];
                        Cust2Date.Text = individualData[12];
                        Cust2Dur.Text = individualData[14];
                        Cust3Date.Text = individualData[17];
                        Cust3Dur.Text = individualData[19];
                        Cust4Date.Text = individualData[22];
                        Cust4Dur.Text = individualData[24];
                        Cust5Date.Text = individualData[27];
                        Cust5Dur.Text = individualData[29];
                        RelDate.Text = individualData[31];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]), int.Parse(RelDate.Text.Split('/')[0]), int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
            }
            else if (individualData.Length == 38)
            {
                if (DevCompDate.Text != individualData[2] || DevCompDur.Text != individualData[4] ||
                    Cust1Date.Text != individualData[7] ||
                    Cust1Dur.Text != individualData[9] || Cust2Date.Text != individualData[12] ||
                    Cust2Dur.Text != individualData[14] ||
                    Cust3Date.Text != individualData[17] || Cust3Dur.Text != individualData[19] ||
                    Cust4Date.Text != individualData[22] ||
                    Cust4Dur.Text != individualData[24] || Cust5Date.Text != individualData[27] ||
                    Cust5Dur.Text != individualData[29] ||
                    Cust6Date.Text != individualData[32] || Cust6Dur.Text != individualData[34] ||
                    RelDate.Text != individualData[36])
                {
                    DevCompDate.Text = individualData[2];
                    DevCompDur.Text = individualData[4];
                    Cust1Date.Text = individualData[7];
                    Cust1Dur.Text = individualData[9];
                    Cust2Date.Text = individualData[12];
                    Cust2Dur.Text = individualData[14];
                    Cust3Date.Text = individualData[17];
                    Cust3Dur.Text = individualData[19];
                    Cust4Date.Text = individualData[22];
                    Cust4Dur.Text = individualData[24];
                    Cust5Date.Text = individualData[27];
                    Cust5Dur.Text = individualData[29];
                    Cust6Date.Text = individualData[32];
                    Cust6Dur.Text = individualData[34];
                    RelDate.Text = individualData[36];
                }
                else if (undoLines.Count > 1)
                {
                    undoLines.RemoveAt((undoLines.Count() - 1));
                    List<string> range = undoLines.GetRange(undoLines.Count - 1, 1);
                    var oldRelDate = DateTime.Parse(RelDate.Text, culture, DateTimeStyles.AssumeLocal);
                    foreach (string line in range)
                    {
                        string[] tempArray = line.Split('•');
                        individualData[2] = tempArray[2];
                        individualData[4] = tempArray[4];
                        individualData[7] = tempArray[7];
                        individualData[9] = tempArray[9];
                        individualData[12] = tempArray[12];
                        individualData[14] = tempArray[14];
                        individualData[17] = tempArray[17];
                        individualData[19] = tempArray[19];
                        individualData[22] = tempArray[22];
                        individualData[24] = tempArray[24];
                        individualData[27] = tempArray[27];
                        individualData[29] = tempArray[29];
                        individualData[32] = tempArray[32];
                        individualData[34] = tempArray[34];
                        individualData[36] = tempArray[36];

                        DevCompDate.Text = individualData[2];
                        DevCompDur.Text = individualData[4];
                        Cust1Date.Text = individualData[7];
                        Cust1Dur.Text = individualData[9];
                        Cust2Date.Text = individualData[12];
                        Cust2Dur.Text = individualData[14];
                        Cust3Date.Text = individualData[17];
                        Cust3Dur.Text = individualData[19];
                        Cust4Date.Text = individualData[22];
                        Cust4Dur.Text = individualData[24];
                        Cust5Date.Text = individualData[27];
                        Cust5Dur.Text = individualData[29];
                        Cust6Date.Text = individualData[32];
                        Cust6Dur.Text = individualData[34];
                        RelDate.Text = individualData[36];
                    }
                    DateTime newRelDate = new DateTime(int.Parse(RelDate.Text.Split('/')[2]),
                        int.Parse(RelDate.Text.Split('/')[0]), int.Parse(RelDate.Text.Split('/')[1]));
                    daysLost += Comparison(newRelDate, oldRelDate);
                    ChangeInDays.Text = (-daysLost).ToString();
                    if (int.Parse(ChangeInDays.Text) < -2)
                    {
                        if (storeColor)
                        {

                            curBackColor = ChangeInDays.BackColor;
                            curForeColor = ChangeInDays.ForeColor;
                            storeColor = false;
                        }
                        ChangeInDays.BackColor = Color.Red;
                        ChangeInDays.ForeColor = Color.White;
                    }
                    else
                    {
                        ChangeInDays.BackColor = curBackColor;
                        ChangeInDays.ForeColor = curForeColor;
                        storeColor = true;
                    }
                }
                else
                {
                    MessageBox.Show("Cannot Undo Further!");
                }
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
    }
}