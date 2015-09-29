using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalendarAppV4
{
    public partial class Home : Form
    {
        public Home()
        {
            InitializeComponent();
        }

        private void newTargetVersion_Click(object sender, EventArgs e)
        {
            Globals.createTargRedirect = 0;
            new CreateNewTargetVersion().Show();
            this.Hide();
        }

        private void Home_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void OpenTxt_Click(object sender, EventArgs e)
        {
            string[] linesInPath = File.ReadAllLines(Globals.path);
            string[] wordsInLine;
            if (!File.Exists(@"Text.txt"))
            {
                File.Create(@"Text.txt").Dispose();
                StreamWriter tw = new StreamWriter(@"Text.txt");
                tw.Close();
            }
            for (int i = 0; i < linesInPath.Length; i++)
            {
                wordsInLine = linesInPath[i].Split('•');
                linesInPath[i] = wordsInLine[0];
            }
            try
            {
                File.WriteAllLines(@"Text.txt", linesInPath);
                Process.Start(@"Text.txt");
            }
            catch
            {
                MessageBox.Show("Action Failed. Please close Text.txt and retry.");
            }
        }

        private void viewCalendar_Click(object sender, EventArgs e)
        {
            new ViewCalendar().Show();
            this.Hide();
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            string path = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                path = Directory.GetParent(path).FullName;
            }
            path += "\\Desktop\\DATAHERE.txt";
            File.Copy(Globals.path, @path, true);
            MessageBox.Show("Action Successful");
        }

        private void Append_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK && Path.GetExtension(openFileDialog1.FileName) == ".txt")
            {
                string path = @openFileDialog1.FileName;
                string[] lines = File.ReadAllLines(path);
                for (int i = 0; i < lines.Length; i++)
                {
                    var verification = File.ReadAllLines(Globals.path);
                    for (int j = 0; j < verification.Length; j++)
                    {
                        //if a duplicate occurs;
                        if (lines[i].Split('•')[0].ToLower().Equals(verification[j].Split('•')[0].ToLower()))
                        {
                            var oldLines = verification;
                            var newLines = oldLines.Where(linek => !linek.ToLower().Contains(lines[i].Split('•')[0].ToLower() + "•"));
                            File.WriteAllLines(Globals.path, newLines);
                        }
                    }
                    string appendText = lines[i] + Environment.NewLine;
                    File.AppendAllText(Globals.path, appendText);
                }
                MessageBox.Show("Action Successful");
            }
            else
            {
                MessageBox.Show("Action Failed");
            }
        }

        private void Replace_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK && Path.GetExtension(openFileDialog1.FileName) == ".txt")
            {
                File.Copy(@openFileDialog1.FileName, Globals.path, true);
                MessageBox.Show("Action Successful");
            }
            else if (result != DialogResult.Cancel)
            {
                MessageBox.Show("Action Failed");
            }
        }

        private void userInput_Click(object sender, EventArgs e)
        {
            string[] dataSource = new string[File.ReadLines(Globals.path).Count()];
            int len = dataSource.Length;
            Array.Clear(dataSource, 0, dataSource.Length);
            userInput.Items.Clear();
            int counter = 0;
            using (StreamReader sr = new StreamReader(Globals.path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    if (line != null)
                    {
                        //if the line starts with the substring in the textbox...
                        if (line.Split('•')[0].ToLower().StartsWith(userInput.Text))
                        {
                            dataSource[counter] = line.Split('•')[0];
                            counter++;
                        }
                    }
                }
            }
            for (int i = 0; i < File.ReadLines(Globals.path).Count(); i++)
            {
                //fills in the combobox with all lines starting with the substring
                if (dataSource[i] != null)
                {
                    userInput.Items.Add(dataSource[i]);
                }
            }
        }

        private void Done_Click(object sender, EventArgs e)
        {
            bool found = true;
            int counter = 0;
            //Reads the text file and looks for the queried text.
            using (StreamReader sr = new StreamReader(Globals.path))
            {
                while (!sr.EndOfStream && found)
                {
                    string line = sr.ReadLine();
                    string exact = userInput.Text.ToLower() + "•";
                    //This is for exact inputs
                    if (line.ToLower().StartsWith(exact))
                    {
                        string[] words = line.Split('•');
                        Globals.foundString = words[0] + "•";
                        found = false;
                    }
                    //This is for queries that look for target versions starting with the user input
                    else if (line.ToLower().StartsWith(userInput.Text.ToLower()))
                    {
                        string[] words = line.Split('•');
                        Globals.foundString = words[0] + "•";
                        counter++;
                    }
                }
                sr.Close();
                //If none are found, this message appears.
                if (found && counter > 1)
                {
                    MessageBox.Show(counter.ToString() + " Target Versions starting with that name.");
                }
                else if (found && counter == 0)
                {
                    MessageBox.Show("No Target Versions with that name.");
                    Globals.foundString = null;
                    userInput.Text = null;
                }
                //If only one is found, it launches the main display window.
                else
                {
                    new DisplayTargVersion().Show();
                    this.Hide();
                }
            }
        }
    }
}
