using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TColor = System.Drawing.Color;

namespace Helper
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            this.Width = Form.Width + 40;
            this.Height = Form.Height + 130;
            pictureBox1.Height = this.Height - 20;
            pictureBox1.Width = this.Width-20;
        }

        String sisfolder, String6;
        StringList FromStyle = new StringList(), Color_Style = new StringList();StringList Name_Style = new StringList(), FromFile = new StringList();
        public Form1 Form = new Form1();
        Colors  From_Style = new Colors ();
        Colors From_Style2 = new Colors();
        Colors From_Style3 = new Colors();


        public class StringList : List<string>
        {
            public void SaveToFile(string PatchFile, StringList list) //List.SaveToFile(@"F:\путь", List)
            {
                string[] mas = new string[list.Count];
                for (int i = 0; i < list.Count; i++)
                    mas[i] = list[i];
                    System.IO.File.WriteAllLines(PatchFile, mas, Encoding.UTF8);
            }
            public void LoadFromFile(string PatchFile, Colors list, Colors list2, Colors list3, StringList list4, Boolean stop) //List.SaveToFile(@"F:\путь", List)
            {
                string[] word = System.IO.File.ReadAllLines(PatchFile, Encoding.UTF8);
                list.Clear(); list2.Clear(); list3.Clear(); list4.Clear();
                string[] digit;

                for (int i = 0; i < word.Length; i++)
                    if (word[i] == "//")
                        if (i + 4 < word.Length)
                        {
                            digit = word[i + 2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            list.Add(TColor.FromArgb(int.Parse(digit[0]), int.Parse(digit[1]), int.Parse(digit[2])));
                            digit = word[i + 3].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            list2.Add(TColor.FromArgb(int.Parse(digit[0]), int.Parse(digit[1]), int.Parse(digit[2])));
                            digit = word[i + 4].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                            list3.Add(TColor.FromArgb(int.Parse(digit[0]), int.Parse(digit[1]), int.Parse(digit[2])));
                            list4.Add(word[i + 1]);

                        }        
                stop = false;   
            }   

            public void NormalLoadFromFile(string PatchFile, StringList list) //List.SaveToFile(@"F:\путь", List)
            {
                list.Clear();
                string[] word = System.IO.File.ReadAllLines(PatchFile, Encoding.UTF8);
                for (int i = 0; i < word.Length; i++) list.Add(word[i]);
            }         
        }



        public class Colors : List<TColor>
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            sisfolder = Environment.GetEnvironmentVariable("APPDATA");
            GoScreens();
            Form.Stop = false;
            while (true) { Application.DoEvents(); if (Form.Stop == false) break;  }
            if (File.Exists(sisfolder + @"\Helper\Options\FromStyle.txt"))
            {
                FromStyle.LoadFromFile(sisfolder + @"\Helper\Options\FromStyle.txt", From_Style, From_Style2, From_Style3, Name_Style, Form.Stop);
                while (true) { Application.DoEvents(); if (Form.Stop == false) break; }

                foreach (string str in Name_Style)
                    comboBox2.Items.Add(str);
                String6 = Form.ButtonsName[24];
                comboBox2.Items.Add(String6);
            }
            else
            {
                FromStyle.Add("//");
                FromStyle.Add("Style 1 (Standart)");
                FromStyle.Add("240, 248, 255");
                FromStyle.Add("240, 248, 255");
                FromStyle.Add("0, 0, 0");
                FromStyle.Add("//");
                FromStyle.Add("Style 2");
                FromStyle.Add("246, 245, 240");
                FromStyle.Add("239, 226, 207");
                FromStyle.Add("115, 99, 86");
                FromStyle.Add("//");
                FromStyle.Add("Style 3");
                FromStyle.Add("255,242,236");
                FromStyle.Add("236,205,203");
                FromStyle.Add("116,126,117");
                FromStyle.Add("//");
                FromStyle.Add("Style 4");
                FromStyle.Add("214,203,191");
                FromStyle.Add("190,179,165");
                FromStyle.Add("63,63,70");
                FromStyle.Add("//");
                FromStyle.Add("Style 5");
                FromStyle.Add("221,225,224");
                FromStyle.Add("191,197,193");
                FromStyle.Add("84,94,96");
                FromStyle.SaveToFile(sisfolder + @"\Helper\Options\FromStyle.txt", FromStyle);
                FromStyle.LoadFromFile(sisfolder + @"\Helper\Options\FromStyle.txt", From_Style, From_Style2, From_Style3, Name_Style, Form.Stop);
                while (true) { Application.DoEvents(); if (Form.Stop == false) break; }



            }
            comboBox2.Items.Clear();
            foreach (string str in Name_Style)
                comboBox2.Items.Add(str);
            String6 = Form.ButtonsName[24];
            comboBox2.Items.Add(String6);
            comboBox1.Items.Clear();
            comboBox1.Text = (Form.Style[1] + "   (" + Form.Style[2] + ")");
            comboBox1.Items.Add(Form.ButtonsName[25]);

            FromFile.NormalLoadFromFile(sisfolder + @"\Helper\Options\FromStyle.txt", FromFile);
            for (int x = 0; x < FromFile.Count; x++)
                if (Form.Style[4] == FromFile[x])
                    if (Form.Style[5] == FromFile[x + 1])
                        if (Form.Style[6] == FromFile[x + 2])
                            comboBox2.Text = FromFile[x - 1];



            saveToolStripMenuItem.Text = Form.ButtonsName[17];
            applyToolStripMenuItem.Text = Form.ButtonsName[18];
            cancelToolStripMenuItem.Text = Form.ButtonsName[19];
            label4.Text = Form.ButtonsName[20];
            label1.Text = Form.ButtonsName[21];
            this.Text = Form.ButtonsName[26];
        }

     

        public void GoScreens()
        {
            Bitmap ImageForm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Rectangle bounds = new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height);
            if (Form.listBox1.SelectedIndex > -1) Form.listBox1.SelectedIndex = (Application.OpenForms[0] as Form1).listBox1.SelectedIndex;
            this.BackColor = Form.BackColor;
            this.ForeColor = Form.ForeColor;
            menuStrip1.BackColor = Form.BackColor;
            menuStrip1.ForeColor = Form.ForeColor;
            Form.Show();
            
            Form.DrawToBitmap(ImageForm, bounds);
            Form.Hide();

            int i = 30;
            int j = 8;
            
            pictureBox1.Image = ImageForm;  
            Form.Stop = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
                if (comboBox2.Items[comboBox2.SelectedIndex].ToString() != String6)
                {
                    Form.StyleColor = From_Style[comboBox2.SelectedIndex];
                    Form.StyleColor2 = From_Style2[comboBox2.SelectedIndex];
                    Form.StyleColor3 = From_Style3[comboBox2.SelectedIndex];
                    Form.SetColorToForm();
                    GoScreens();

                }
                if (comboBox2.Items[comboBox2.SelectedIndex].ToString() == String6)
                {
                    colorDialog1.ShowDialog();
                    Form.StyleColor = TColor.FromArgb(colorDialog1.Color.A, colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                    colorDialog1.ShowDialog();
                    Form.StyleColor2 = TColor.FromArgb(colorDialog1.Color.A, colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                    colorDialog1.ShowDialog();
                    Form.StyleColor3 = TColor.FromArgb(colorDialog1.Color.A, colorDialog1.Color.R, colorDialog1.Color.G, colorDialog1.Color.B);
                    Form.SetColorToForm();
                    GoScreens();
                }
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            (Application.OpenForms[0] as Form1).StyleColor = From_Style[comboBox2.SelectedIndex];
            (Application.OpenForms[0] as Form1).StyleColor2 = From_Style2[comboBox2.SelectedIndex];
            (Application.OpenForms[0] as Form1).StyleColor3 = From_Style3[comboBox2.SelectedIndex];
            (Application.OpenForms[0] as Form1).SetColorToForm();
            (Application.OpenForms[0] as Form1).Style[4] = Form.StyleColor.R.ToString() + ", " + Form.StyleColor.G.ToString() + ", " + Form.StyleColor.B;
            (Application.OpenForms[0] as Form1).Style[5] = Form.StyleColor2.R.ToString() + ", " + Form.StyleColor2.G.ToString() + ", " + Form.StyleColor2.B;
            (Application.OpenForms[0] as Form1).Style[6] = Form.StyleColor3.R.ToString() + ", " + Form.StyleColor3.G.ToString() + ", " + Form.StyleColor3.B;


            (Application.OpenForms[0] as Form1).Style[1] = Form.Style[1];
            (Application.OpenForms[0] as Form1).Style[2] = Form.Style[2];
            (Application.OpenForms[0] as Form1).SetFont();

            (Application.OpenForms[0] as Form1).Style.SaveToFile(sisfolder + @"\Helper\Options\Style.dis", (Application.OpenForms[0] as Form1).Style);
            this.Close();
           
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
           Form.InputBox(Form.ButtonsName[22], Form.ButtonsName[23],  "");
           Form.NewName = "its_a_null"; 
           while (true)
           {
               Application.DoEvents();
               if (Form.NewName != "its_a_null") break;
           }
           if ((Form.NewName == "") || (Form.NewName == "null"))
           {
               Form.ShowMessage(Form.ButtonsName[14]);
               while (true)
               {
                   Application.DoEvents();
                   if (Form.Stop == false) break;
               }
           }
           else
           {
               
               FromFile.NormalLoadFromFile(sisfolder + @"\Helper\Options\FromStyle.txt", FromFile);
               FromFile.Add("//");
               FromFile.Add(Form.NewName);
               FromFile.Add(Form.StyleColor.R + ", " + Form.StyleColor.G + ", " + Form.StyleColor.B);
               FromFile.Add(Form.StyleColor2.R + ", " + Form.StyleColor2.G + ", " + Form.StyleColor2.B);
               FromFile.Add(Form.StyleColor3.R + ", " + Form.StyleColor3.G + ", " + Form.StyleColor3.B);
               FromFile.SaveToFile(sisfolder + @"\Helper\Options\FromStyle.txt", FromFile);
               comboBox2.Items.Clear();
               FromStyle.LoadFromFile(sisfolder + @"\Helper\Options\FromStyle.txt", From_Style, From_Style2, From_Style3, Name_Style, Form.Stop);
               while (true)
               {
                   Application.DoEvents();
                   if (Form.Stop == false) break;
               }

               foreach (string str in Name_Style)
                   comboBox2.Items.Add(str);
               String6 = "Свой стиль...";
               comboBox2.Items.Add(String6);

           }



        }

        private void timer1_Tick_1(object sender, EventArgs e)
        {
         
        }

        private void cancelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            (Application.OpenForms[0] as Form1).BringToFront();
                           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            Form.Style[1] = fontDialog1.Font.Name;
            Form.Style[2] = fontDialog1.Font.Size.ToString();
            Form.SetFont();
            GoScreens();
        }

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            (Application.OpenForms[0] as Form1).BringToFront();
        }
    }
}
