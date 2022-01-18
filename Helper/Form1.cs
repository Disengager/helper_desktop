using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using TColor = System.Drawing.Color;
using System.Net.NetworkInformation;
using System.Net;
using System.Runtime.InteropServices;
using System.Linq;



namespace Helper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            #region настройка внашнего вида программы
            this.Size = new Size(Properties.Settings.Default.Size);
            splitter1.SplitPosition = Properties.Settings.Default.SplitPosition;
            #endregion

            #region подготовка директорий в аппдате
            sisfolder = Environment.GetEnvironmentVariable("APPDATA");
            if (!Directory.Exists(sisfolder + @"\Helper")) Directory.CreateDirectory(sisfolder + @"\Helper");
            if (!Directory.Exists(sisfolder + @"\Helper\Image")) Directory.CreateDirectory(sisfolder + @"\Helper\Image");
            if (!Directory.Exists(sisfolder + @"\Helper\Options")) Directory.CreateDirectory(sisfolder + @"\Helper\Options");
            #endregion

            #region заполнение стринг листов

            //конфиг

            config = File.Exists(sisfolder + @"\Helper\Config.dis") ? config.LoadFromFile(sisfolder + @"\Helper\Config.dis", config) : GetStringList(Properties.Settings.Default.Config);
            config.SaveToFile(sisfolder + @"\Helper\Co14nfig.dis", config);
            timer_autorefresh.Interval = double.Parse(config[0]) > 0? int.Parse(config[0]) * 100 : 10000000;

            //пункты листбокса

            info = File.Exists(sisfolder + @"\Helper\info.dis") ? info.LoadFromFile(sisfolder + @"\Helper\info.dis", info) : info;
            info.SaveToFile(sisfolder + @"\Helper\info.dis", info);
            if (info.Count > 0)
            {
                for (int i = 0; i < info.Count; i++)
                    if (info[i] != "") listBox1.Items.Add(info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                LastIndex = listBox1.SelectedIndex = 0;
            }

            //названия кнопок

            ButtonsName = File.Exists(sisfolder + @"\Helper\Options\ButtonNames.dis")?ButtonsName.LoadFromFile(sisfolder + @"\Helper\Options\ButtonNames.dis", ButtonsName):GetStringList(Properties.Settings.Default.ButtonsName);
            ButtonsName.SaveToFile(sisfolder + @"\Helper\Options\ButtonNames.dis", ButtonsName);
            #region изменение названий в соотвествии с пунктами в стринг листе
            this.Text = ButtonsName[0];
            смотретьToolStripMenuItem.Text = ButtonsName[1];
            добавитьToolStripMenuItem.Text = ButtonsName[2];
            удалитьToolStripMenuItem.Text = ButtonsName[3];
            редактироватьДанныеToolStripMenuItem.Text = ButtonsName[4];
            названиеToolStripMenuItem.Text = ButtonsName[5];
            ссылкуToolStripMenuItem.Text = ButtonsName[6];
            label1.Text = ButtonsName[7];
            label2.Text = ButtonsName[8];
            label3.Text = ButtonsName[9];
            InputBox_ButtonOk = ButtonsName[10];
            InputBox_ButtonCancel = ButtonsName[11];
            toolStripMenuItem1.Text = ButtonsName[26];
            обновитьToolStripMenuItem.Text = ButtonsName[27];
            скопНазваниеToolStripMenuItem.Text = ButtonsName[28];
            допИнфоToolStripMenuItem.Text = ButtonsName[29];
            автообновлениеToolStripMenuItem.Text = ButtonsName[30];
            элементToolStripMenuItem.Text = ButtonsName[31];
            сайтToolStripMenuItem.Text = ButtonsName[32];
            картинкуToolStripMenuItem.Text = ButtonsName[37];
            изСсылкиToolStripMenuItem.Text = ButtonsName[38];
            изФайлаToolStripMenuItem.Text = ButtonsName[39];
            загрузитьРезервнуюБазуToolStripMenuItem.Text = ButtonsName[41];
            #endregion

            //Стиль программы

            Style = File.Exists(sisfolder + @"\Helper\Options\Style.dis") ? Style.LoadFromFile(sisfolder + @"\Helper\Options\Style.dis", Style) : GetStringList(Properties.Settings.Default.Style);
            Style.SaveToFile(sisfolder + @"\Helper\Options\Style.dis", Style);
            #region изменение цветов в соотвествии с пунктами стринг листа
            SetFont();
            StyleColor = GetColor(Style[4]);
            StyleColor2 = GetColor(Style[5]);
            StyleColor3 = GetColor(Style[6]);
            SetColorToForm();
            #endregion 

            //заполнение второго листбокса

            foreach (string st in listBox1.Items)
            {
                listBox2.Items.Add("");
                status.Add("");
            }

            //заполнение информации о сайтах

            StringList stl = GetStringList(Properties.Settings.Default.SiteInfo);
            if (!File.Exists(sisfolder + @"\Helper\SiteInfo.dis")) stl.SaveToFile(sisfolder + @"\Helper\SiteInfo.dis", stl);

            timer_synchronize.Enabled = true;

            #endregion
        }
       
        #region ListBox без скрул бара
        public class NotScroolBarListBox : System.Windows.Forms.ListBox
        {
            private bool mShowScroll;
            protected override System.Windows.Forms.CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    if (!mShowScroll)
                        cp.Style = cp.Style & ~0x200000;
                    return cp;
                }
            }
            public bool ShowScrollbar
            {
                get { return mShowScroll; }
                set
                {
                    if (value != mShowScroll)
                    {
                        mShowScroll = value;
                        if (IsHandleCreated)
                            RecreateHandle();
                    }
                }
            }
        }
        #endregion

        #region Переменные
        public String sisfolder, ssilka, InputBox_ButtonOk, InputBox_ButtonCancel,  NewSerias = "", NewName, DopName = "";
        int LastIndex;
        public Boolean Delete_Time, Stop, transport = false;
        String[] Saves = new String [6];
        public StringList config = new StringList(), info = new StringList(), sost = new StringList(), status = new StringList(),  
            ButtonsName = new StringList(), Style = new StringList();
        Input_Box  inputbox_form = new Input_Box();
        MessageBoxClass showmessage_form = new MessageBoxClass();
        LoadBarBox loadbar_form = new LoadBarBox();
        HelperAddSite helper_add_site = new HelperAddSite();
        public TColor StyleColor = new TColor(), StyleColor2 = new TColor(),StyleColor3 = new TColor();
        System.Net.WebClient webClient = new System.Net.WebClient();
        #endregion

        #region Свойства стринг листа
        public class StringList : List<string>
        {
            public void SaveToFile(string PatchFile, StringList list) //List.SaveToFile(@"F:\путь", List)
            {
                string[] mas = list.ToArray();
                System.IO.File.WriteAllLines(PatchFile, mas, Encoding.UTF8);
            }
            public StringList LoadFromFile(string PatchFile, StringList list) //List.SaveToFile(@"F:\путь", List)
            {
                list.Clear();
                string[] word = System.IO.File.ReadAllLines(PatchFile, Encoding.UTF8);
                for (int i = 0; i < word.Length; i++) list.Add(word[i]);
                return list;
            }            
        }

        public StringList GetStringList(System.Collections.Specialized.StringCollection listst)
        {
            StringList lst = new StringList();
            foreach (string st in listst) lst.Add(st);
            return lst;
        }

        public System.Collections.Specialized.StringCollection GetSpecializedStringCollection(StringList listst)
        {
            var lst = new System.Collections.Specialized.StringCollection();
            foreach (string st in listst) lst.Add(st);
            return lst;
        }
        #endregion

        #region ShowMessage
        public void ShowMessage(string text)
        {
            Stop = true;
            showmessage_form.label1.Text = text;
            showmessage_form.label1.TextAlign = ContentAlignment.MiddleCenter;
            showmessage_form.button1.Text = ButtonsName[10];
            showmessage_form.BackColor = this.BackColor;
            showmessage_form.ForeColor = this.ForeColor;
            showmessage_form.button1.BackColor = this.BackColor;
            showmessage_form.Show();
        }
        #endregion

        #region InputBox
        public string InputBox(string title, string Caption, string TextInTextBox)
        {
            Stop = true;
            inputbox_form.Text = title;  
            inputbox_form.textBox1.Text = TextInTextBox;
            inputbox_form.label1.Text = Caption;
            inputbox_form.button1.Text = ButtonsName[10];
            inputbox_form.button2.Text = ButtonsName[11];
            inputbox_form.textBox1.BackColor = listBox1.BackColor;
            inputbox_form.ForeColor = this.ForeColor;
            inputbox_form.BackColor = inputbox_form.button1.BackColor = inputbox_form.button2.BackColor = this.BackColor;
            inputbox_form.Show();
            return TextInTextBox;
        }
        #endregion

        #region LoadBar
        string title = "";
        private void LoadBar()
        {
            Invoke((MethodInvoker)delegate
            {             
                Stop = true;
                loadbar_form.FormBorderStyle = FormBorderStyle.FixedToolWindow;
                loadbar_form.label1.Text = title;    
                loadbar_form.progressBar1.Maximum = listBox1.Items.Count-1;
                loadbar_form.progressBar1.BackColor = loadbar_form.BackColor = this.BackColor;   
                loadbar_form.ForeColor = this.ForeColor;
                loadbar_form.Show();
            });
        }
        #endregion

        #region HelperAddSiteBox

        public string HelperAddSiteBox() { helper_add_site.Show(); return ""; }

        #endregion

        #region Получение информации о пунктах
        public void GetParam(String Line, String Razdel)
        {
            try
            {
                String[] word = Saves = Line.Split(new string[] { Razdel }, StringSplitOptions.RemoveEmptyEntries);
                DopName = word.Length > 5 ? word[5] : "";

                ssilka = word[4];
                numericUpDown1.Value = int.Parse(word[1]);
                textBox1.Text = word[2];
                comboBox1.SelectedIndex = int.Parse(word[3]);

                string[] mass = { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };
                string result = ""; Boolean bol;
                for (int i = 0; i < word[0].Length; i++)
                {
                    bol = true;
                    foreach (string st2 in mass) if (word[0][i].ToString() == st2) bol = false;
                    if (bol != false) result += word[0][i];
                }

                word[0] = result;
                if (File.Exists(sisfolder + @"\Helper\Image\" + word[0] + ".jpg")) pictureBox1.Load(sisfolder + @"\Helper\Image\" + word[0] + ".jpg");
                else pictureBox1.Image = pictureBox2.Image;
                Delete_Time = false;
            }
            catch { };
        }
        #endregion 

        #region Сохранения информации о пунктах
        public void SaveParam(StringList list, String Razdel)
        {
            String[] word = list[LastIndex].Split(new string[] { Razdel }, StringSplitOptions.RemoveEmptyEntries);
            string text = listBox1.Items[LastIndex].ToString();
            for (int i = 1; i < word.Length; i++) text += word[i];
            list[LastIndex] = text;
            list.SaveToFile(sisfolder + @"\Helper\info.dis", info);  
        }
        #endregion

        #region Обновление
        public void Refresh(string link, int ItemIndex)
        {
                try
                {


                    int index = 0, index2 = 0; Boolean bol = false, bol2 = false; string[] PaswwordFile;

                    string[] txt = File.ReadAllLines(sisfolder + @"\Helper\SiteInfo.dis", Encoding.UTF8);

                    for (int j = 0; j < txt.Length; j++)
                      { 
                        string[] txt1 = txt[j].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                        if (link.IndexOf(txt1[0]) > 0)
                        {  bol = link.IndexOf(txt1[0]) > 0;index = link.IndexOf(txt1[0]) > 0? j: index; }
                    }
                    if (File.Exists(sisfolder + @"\Helper\Password.dis"))
                    {
                        PaswwordFile = File.ReadAllLines(sisfolder + @"\Helper\Password.dis", Encoding.UTF8);

                        for (int j = 0; j < PaswwordFile.Length; j++)
                        {
                            string[] PasswordFile1 = PaswwordFile[j].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                            if (link.IndexOf(PasswordFile1[0]) > 0)
                            { bol2 = link.IndexOf(PasswordFile1[0]) > 0; index2 = link.IndexOf(PasswordFile1[0]) > 0 ? j : index2; }
                        }
                    }
                    if (bol)
                    {
                        string text = ""; 

                        string[] txt1 = txt[index].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                        if (bol2)
                        {
                            PaswwordFile = File.ReadAllLines(sisfolder + @"\Helper\Password.dis", Encoding.UTF8);

                            string[] PasswordFile1 = PaswwordFile[index2].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                            string Login = PasswordFile1[1], Password = PasswordFile1[2];

                            MessageBox.Show("");

                            MessageBox.Show("");
                            
                            //webBrowser1.Navigate(link);
                            //text = webBrowser1.DocumentText;
                            //MessageBox.Show(text);

                        }
                        else
                        {
                            Stream st = webClient.OpenRead(link); StreamReader sr = new StreamReader(st);

                            text = sr.ReadToEnd(); st.Close();
                        }

                        string str = text.Split(new string[] { txt1[3] }, StringSplitOptions.RemoveEmptyEntries)[1];

                        str = str.Substring(0, str.IndexOf(txt1[4]));
                        string[] mass = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" }; string result = "";

                        for (int i = 0; i < str.Length; i++)
                            foreach (string st1 in mass) if (str[i].ToString() == st1) result += st1;

                        str = result;

                        String[] word = info[ItemIndex].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);
                        if (str != "")

                            if (int.Parse(str) > int.Parse(word[1])) {  NewSerias += word[0] + "\n"; status[ItemIndex] = " [New]"; }
                            else status[ItemIndex] = " ";

                       
                    }
                }
                catch { };
        }
        #endregion

        #region Сворачивание и разворачивание второго листбокса
        public void TwoListBox(Boolean pos)
        {
            listBox2.Visible = допИнфоToolStripMenuItem.Checked = pos;
            foreach (Control c in this.Controls)
                c.Left += pos && c.Name != "listBox2" ? 30 : -30; 
            this.Width += pos?30 : -30;
        }
        #endregion

        #region Работа со стилями
        public TColor GetColor(string st)
        {
            string[] digit = st.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return TColor.FromArgb(int.Parse(digit[0]), int.Parse(digit[1]), int.Parse(digit[2]));
        }

        public void SetFont()
        {
            foreach (Control c in this.Controls) 
                c.Font = new Font(Style[1], float.Parse(Style[2]), this.Font.Style, this.Font.Unit);
            обновитьToolStripMenuItem.Font = скопНазваниеToolStripMenuItem.Font = автообновлениеToolStripMenuItem.Font = допИнфоToolStripMenuItem.Font = 
                new Font(Style[1], float.Parse(Style[2]), this.Font.Style, this.Font.Unit);
        }

        public void SetColorToForm()
        {
            //первый цвет
            this.BackColor = menuStrip1.BackColor = contextMenuStrip1.BackColor = StyleColor;
            //второй цвет
            listBox1.BackColor = listBox2.BackColor = rectangleShape1.BackColor = label1.BackColor =
            label2.BackColor = label3.BackColor = numericUpDown1.BackColor =  textBox1.BackColor =  comboBox1.BackColor = StyleColor2;
            //третий цвет
            this.ForeColor = listBox1.ForeColor = listBox2.ForeColor = menuStrip1.ForeColor = contextMenuStrip1.ForeColor = StyleColor3;
        }
        #endregion

        #region Смотреть
        private void смотретьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (listBox1.Items.Count != 0)
                    if (DopName == "")System.Diagnostics.Process.Start(ssilka);
                    else System.Diagnostics.Process.Start(DopName);
            }
            catch { };
        }
        #endregion

        #region ListBox
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (transport == false)
            {
                try { GetParam(info[listBox1.SelectedIndex], "[=_=]"); LastIndex = listBox1.SelectedIndex; }
                catch { };
            }
        }
        #endregion

        #region Удалить
        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0) return;
            if (listBox1.SelectedIndex < -1) return;

                    Delete_Time = true; timer_synchronize.Enabled = false; string result = ""; Boolean bol;

                    string Patch = info[listBox1.SelectedIndex].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    
                    for (int i = 0; i < Patch.Length; i++)
                    {
                        bol = true;
                        foreach (string st2 in new string[] { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" })  bol = Patch[i].ToString() != st2;
                        result += bol != false?Patch[i].ToString():"";
                    }

                    Patch = result;
                    info.RemoveAt(listBox1.SelectedIndex);

                    listBox1.Items.Clear();
                    for (int i = 0; i < info.Count; i++)
                    { listBox1.Items.Add(info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0]); listBox2.Items.Add(""); }

                    if (listBox1.Items.Count > 1) listBox1.SelectedIndex = 0;
                    else { pictureBox1.Image = pictureBox2.Image; numericUpDown1.Value = 0; comboBox1.Text = textBox1.Text = String.Empty; };

                    timer_synchronize.Enabled = true;

                    if (File.Exists(sisfolder + @"\Helper\Image\" + Patch + ".jpg")) File.Delete(sisfolder + @"\Helper\Image\" + Patch + ".jpg");           
        }
        #endregion

        #region Закрытие формы 
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Delete_Time = true;  info.SaveToFile(sisfolder + @"\Helper\info.dis", info);  Delete_Time = false;

            Properties.Settings.Default.Size = new Point(this.Size); Properties.Settings.Default.SplitPosition = splitter1.SplitPosition;

            if (MessageBox.Show(this, ButtonsName[40], this.Text, MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                Properties.Settings.Default.ReserveCopy = GetSpecializedStringCollection(info);

            Properties.Settings.Default.Save();
        }
        #endregion

        #region Редактировать название
        private void названиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                NewName = InputBox(ButtonsName[12], ButtonsName[13], listBox1.Items[listBox1.SelectedIndex].ToString());
                while (true) { Application.DoEvents(); if (!Stop) break; } 
                if (NewName == "")
                {
                    ShowMessage(ButtonsName[14]);
                    while (true) { Application.DoEvents(); if (!Stop) break; }
                }
                else if (NewName != "null")
                {
                    int newselectedindex = listBox1.SelectedIndex;
                    String[] word = info[listBox1.SelectedIndex].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < 4; i++) Saves[i] = word[i]; Saves[4] = ssilka = word[4]; Saves[0] = NewName;

                    info[listBox1.SelectedIndex] = Saves[0] + "[=_=]" + Saves[1] + "[=_=]" + Saves[2] + "[=_=]" + Saves[3] + "[=_=]" + Saves[4] + (DopName != "" ? "[=_=]" + DopName : "");

                    Delete_Time = true;
                    listBox1.Items.Clear();

                    for (int i = 0; i < info.Count; i++)
                        if (info[i] != "") listBox1.Items.Add(info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    Delete_Time = false;
                    listBox1.SelectedIndex = newselectedindex;
                }
            }
        }
        #endregion

        #region Изменение номера текущей серии
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
                if (!Delete_Time)
                {
                    String Line = info[listBox1.SelectedIndex];
                    String[] word = Line.Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 4; i++) Saves[i] = word[i]; Saves[4] = ssilka = word[4]; Saves[1] = numericUpDown1.Value.ToString();

                    info[listBox1.SelectedIndex] = Saves[0] + "[=_=]" + Saves[1] + "[=_=]" + Saves[2] + "[=_=]" + Saves[3] + "[=_=]" + Saves[4] + (DopName != "" ? "[=_=]" + DopName : "");

                    System.Threading.Thread.Sleep(5);
                }
        }
        #endregion

        #region Изменение сколько всего серий
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
           if (listBox1.Items.Count != 0)
                if (!Delete_Time)
                {
                    String Line = info[listBox1.SelectedIndex];
                    String[] word = Line.Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 4; i++) Saves[i] = word[i]; Saves[4] = ssilka = word[4]; Saves[2] = textBox1.Text;

                    info[listBox1.SelectedIndex] = Saves[0] + "[=_=]" + Saves[1] + "[=_=]" + Saves[2] + "[=_=]" + Saves[3] + "[=_=]" + Saves[4] + (DopName != "" ? "[=_=]" + DopName : "");

                    System.Threading.Thread.Sleep(5);
                }
        }
        #endregion

        #region Изменение оценки
        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
                if (!Delete_Time)
                {
                    String Line = info[listBox1.SelectedIndex];
                    String[] word = Line.Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 4; i++) Saves[i] = word[i]; Saves[4] = ssilka = word[4]; Saves[3] = comboBox1.Text;

                    info[listBox1.SelectedIndex] = Saves[0] + "[=_=]" + Saves[1] + "[=_=]" + Saves[2] + "[=_=]" + Saves[3] + "[=_=]" + Saves[4] + (DopName != "" ? "[=_=]" + DopName : "");

                    System.Threading.Thread.Sleep(5);
                }
        }
        #endregion

        #region Изменение ссылки
        private void ссылкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                InputBox(ButtonsName[15], ButtonsName[16], ssilka);
                while (true) { Application.DoEvents(); if (!Stop) break; }

                if (NewName == "")
                {
                    ShowMessage(ButtonsName[14]);
                    while (true) { Application.DoEvents(); if (!Stop) break; }
                }
                else if (NewName != "null")
                {
                    ssilka = Saves[4] = NewName;
                    String[] word = info[listBox1.SelectedIndex].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 4; i++) Saves[i] = word[i];

                    info[listBox1.SelectedIndex] = Saves[0] + "[=_=]" + Saves[1] + "[=_=]" + Saves[2] + "[=_=]" + Saves[3] + "[=_=]" + Saves[4] + (DopName != "" ? "[=_=]" + DopName : "");
                }
            }
        }
        #endregion

        #region Изменение дополнительной ссылки
        private void допСсылкуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
            {
                InputBox(ButtonsName[15], ButtonsName[16], DopName);
                while (true) { Application.DoEvents(); if (!Stop) break; }

                if (NewName == "")
                {
                    ShowMessage(ButtonsName[14]);
                    while (true) { Application.DoEvents(); if (!Stop) break; }
                }
                else if (NewName != "null")
                {
                    DopName = NewName;
                    String[] word = info[listBox1.SelectedIndex].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < 4; i++) Saves[i] = word[i];

                    info[listBox1.SelectedIndex] = Saves[0] + "[=_=]" + Saves[1] + "[=_=]" + Saves[2] + "[=_=]" + Saves[3] + "[=_=]" + Saves[4] + (DopName != "" ? "[=_=]" + DopName : "");
                }
            }
        }
        #endregion

        #region Автосохранение
        private void timer_autosave_Tick(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
                if (!Delete_Time)
                { Delete_Time = true; info.SaveToFile(sisfolder + @"\Helper\info.dis", info);  Delete_Time = false; }
        }
        #endregion

        #region Синхронизация элементов между собой
        private void timer_synchronize_Tick(object sender, EventArgs e)
        {
            listBox2.SelectedIndex = listBox1.SelectedIndex;
            listBox2.TopIndex = listBox1.TopIndex;

            if (loadbar_form.progressBar1.Value == loadbar_form.progressBar1.Maximum)
            { loadbar_form.Hide(); loadbar_form.progressBar1.Value = 0; }

            tableLayoutPanel2.BackColor = this.BackColor = rectangleShape1.BackColor;
            
        }
        #endregion

        #region Открытие формы отвечающий за дизайн
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Refresh(); Form2 form2 = new Form2(); form2.Show(); form2.Form = new Form1() ;
        }
        #endregion

        #region Копирование названия
        private void скопНазваниеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(listBox1.Items[listBox1.SelectedIndex]);   
        }
        #endregion

        #region Показать/Скрыть второй Listbox
        private void допИнфоToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (допИнфоToolStripMenuItem.Checked) 
                TwoListBox(допИнфоToolStripMenuItem.Checked = false);
            else
                TwoListBox(допИнфоToolStripMenuItem.Checked = true);
        }
        #endregion

        #region Автообновление
        private void автообновлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (автообновлениеToolStripMenuItem.Checked)
                автообновлениеToolStripMenuItem.Checked = timer_autorefresh.Enabled = false;
            else
            {
                автообновлениеToolStripMenuItem.Checked = timer_autorefresh.Enabled = true;
                timer_autorefresh.Interval = int.Parse(config[0]) * 60000;
                обновитьВсёToolStripMenuItem_Click(sender, e);    
            }
        }

        private void timer_autorefresh_Tick(object sender, EventArgs e) { обновитьВсёToolStripMenuItem_Click(sender, e); }

        #endregion

        #region Добавление элемента
        private void элементToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string st = InputBox(ButtonsName[33], ButtonsName[34], "");
                while (true) { Application.DoEvents(); if (!Stop) break; }

                st = NewName;
                if (st == "")
                {
                    ShowMessage(ButtonsName[14]);
                    while (true) { Application.DoEvents(); if (!Stop) break; }
                }
                else if (st != "null")
                {
                    try
                    {
                        int index = 0;  Boolean bol = false;
                        string[] txt = File.ReadAllLines(sisfolder + @"\Helper\SiteInfo.dis", Encoding.Default);

                        for (int j = 0; j < txt.Length; j++)
                        {
                            string[] txt1 = txt[j].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                            if (st.IndexOf(txt1[0]) > 0) { bol = true; index = j; }
                        }

                        if (bol)
                        {
                            Delete_Time = true; timer_synchronize.Enabled = false;  string NewElement = ""; Stream st1 = webClient.OpenRead(st); 
                            StreamReader sr = new StreamReader(st1);  string text = sr.ReadToEnd(); st1.Close();

                            string[] txt1 = txt[index].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);
                            string[] str1 = text.Split(new string[] { txt1[1] }, StringSplitOptions.RemoveEmptyEntries);

                            string str = str1[1].Substring(0, str1[1].IndexOf(txt1[2])); string Patch = str;

                            NewElement += str + "[=_=]0[=_=]0[=_=]0[=_=]" + st;  //новый эелемент в списке
                            info.Add(NewElement);

                            txt1 = txt[index].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);
                            str1 = text.Split(new string[] { txt1[5] }, StringSplitOptions.RemoveEmptyEntries);
                            str = str1[1].Substring(0, str1[1].IndexOf(txt1[6]));

                            string[] mass = { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" }; string result = "";

                            for (int i = 0; i < Patch.Length; i++)
                            {
                                bol = true;
                                foreach (string st2 in mass) if (Patch[i].ToString() == st2) bol = false;
                                if (bol)  result += Patch[i];
                            }
                            Patch = result;

                            if (str.IndexOf("://") > 0)  webClient.DownloadFile(str, sisfolder + @"\Helper\Image\" + Patch + ".jpg");
                            else  webClient.DownloadFile("http://" + txt1[0] + str, sisfolder + @"\Helper\Image\" + Patch + ".jpg");

                            listBox1.Items.Clear();
                            for (int i = 0; i < info.Count; i++)
                                listBox1.Items.Add(info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                            listBox1.SelectedIndex = 0; listBox2.Items.Add("");
                            timer_synchronize.Enabled = true;

                        }
                        else
                            if (st.IndexOf(@":\") > 0)
                            {
                                string Pathch = new DirectoryInfo(st).Name;
                                string NewElement = Pathch + "[=_=]0[=_=]0[=_=]0[=_=]" + st;  //новый эелемент в списке
                                info.Add(NewElement);

                                listBox1.Items.Clear();
                                for (int i = 0; i < info.Count; i++)
                                    listBox1.Items.Add( info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                                listBox1.SelectedIndex = 0; listBox2.Items.Add("");
                                timer_synchronize.Enabled = true;
                            }
                            else ShowMessage(ButtonsName[35]);
                    }
                    catch { };
                }
            }
            catch { };
        }
        #endregion

        #region Добавление сайта

        private void сайтToolStripMenuItem_Click(object sender, EventArgs e) {  HelperAddSiteBox(); }

        #endregion

        #region Добавить картинку
        //Из ссылки
        private void изСсылкиToolStripMenuItem_Click(object sender, EventArgs e)
        {

                NewName = InputBox("Смена картинки", ButtonsName[34], "");
                while (true) { Application.DoEvents(); if (!Stop) break; }
                if (NewName == "")
                {
                    ShowMessage(ButtonsName[14]);
                    while (true) { Application.DoEvents(); if (!Stop) break; }
                }
                else if (NewName != "null")
                {
                    string Patch = GetNoramlNamePath();

                    pictureBox1.Load(NewName);
                    try
                    {
                        webClient.DownloadFile(NewName, sisfolder + @"\Helper\Image\" + Patch + ".jpg");
                        pictureBox1.Load(sisfolder + @"\Helper\Image\" + Patch + ".jpg");
                    }
                    catch { }
                }
        }
       
        //Из Файла
        private void изФайлаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "")
            {
                   string Patch = GetNoramlNamePath();

                    pictureBox1.Load(openFileDialog1.FileName);
                    if (File.Exists(sisfolder + @"\Helper\Image\" + Patch + ".jpg"))
                        File.Delete(sisfolder + @"\Helper\Image\" + Patch + ".jpg");
                    File.Copy(openFileDialog1.FileName, sisfolder + @"\Helper\Image\" + Patch + ".jpg");
                    
            }
        }
        //Исправление пути
        private string GetNoramlNamePath()
        {
            Boolean bol = true; string result1 = "";

            string Patch = listBox1.Items[listBox1.SelectedIndex].ToString();
            string[] mass = { @"\", "/", ":", "*", "?", "\"", "<", ">", "|" };

            for (int i = 0; i < Patch.Length; i++)
            {
                bol = true;

                foreach (string st2 in mass) if (Patch[i].ToString() == st2) bol = false;

                if (bol != false) result1 += Patch[i];
            }
            return Patch = result1;
        }
        #endregion

        #region Работа с формой

        private void Form1_Resize(object sender, EventArgs e)  {  if (WindowState == FormWindowState.Minimized)  Hide(); }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e) {  Show();  WindowState = FormWindowState.Normal;  }

        private void развернутьToolStripMenuItem_Click(object sender, EventArgs e) { Show();  WindowState = FormWindowState.Normal;}

        private void выходToolStripMenuItem_Click(object sender, EventArgs e) { this.Close(); }

        #endregion

        #region Обновление всех элементов в ListBox
        private void обновитьВсёToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
                if (ConnectionAvailable("http://www.google.ru"))
                {
                        NewSerias = ""; status.Clear();

                        for (int i = 0; i < listBox2.Items.Count; i++) status.Add("");

                        title = ButtonsName[27]; LoadBar();

                        for (int i = 0; i < listBox1.Items.Count; i++)
                        {
                            try
                            {
                                loadbar_form.progressBar1.Value = i;

                                String[] word = info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                                System.Threading.Thread MyTrhead = new System.Threading.Thread(delegate() { Refresh(word[4], i); });

                                MyTrhead.Start(); MyTrhead.Join();
                            }
                            catch { };
                        }
                        if (!допИнфоToolStripMenuItem.Checked) if (listBox1.Left < 16) TwoListBox(true);

                        for (int i = 0; i < listBox2.Items.Count; i++) listBox2.Items[i] = status[i];

                        if (NewSerias != "")  notifyIcon1.ShowBalloonTip(8500, "New", NewSerias, ToolTipIcon.None);
                    }
        }
        #endregion

        #region Проверка работоспособности интернета
        public bool ConnectionAvailable(string strServer)
        {
            try
            {
                HttpWebRequest reqFP = (HttpWebRequest)HttpWebRequest.Create(strServer);
                HttpWebResponse rspFP = (HttpWebResponse)reqFP.GetResponse();

                if (HttpStatusCode.OK == rspFP.StatusCode)
                {  rspFP.Close(); return true; }
                else
                { rspFP.Close(); return false; }
            }
            catch (WebException)
            { return false; }
        }
        #endregion

        #region Обновление одного элемента в ListBox
        private void обновитьToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (listBox1.Items.Count != 0)
                if (ConnectionAvailable("http://www.google.ru"))
                {
                    NewSerias = ""; title = ButtonsName[27]; LoadBar();
                        try
                        {
                            int i = listBox1.SelectedIndex;
                            loadbar_form.progressBar1.Value = loadbar_form.progressBar1.Maximum;

                            String[] word = info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries);

                            System.Threading.Thread MyTrhead = new System.Threading.Thread(delegate() { Refresh(word[4], i); });

                            MyTrhead.Start();  MyTrhead.Join();
                        }
                        catch { };

                    if (!допИнфоToolStripMenuItem.Checked)
                        if (listBox1.Left < 16) TwoListBox(true);

                    for (int i = 0; i < listBox2.Items.Count; i++) listBox2.Items[i] = status[i];
                }
        }
        #endregion

        #region Открытие папки с файлами конфигураций
        private void папкаСНастройкамиToolStripMenuItem_Click(object sender, EventArgs e)  { System.Diagnostics.Process.Start(sisfolder + @"\Helper"); }
        #endregion

        #region Загрузка резервной копии данных
        private void загрузитьРезервнуюБазуToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete_Time = true;

            listBox1.Items.Clear(); listBox2.Items.Clear(); status.Clear();
            info = GetStringList(Properties.Settings.Default.ReserveCopy);

            if (info.Count > 0)
            {
                for (int i = 0; i < info.Count; i++)
                    if (info[i] != "") listBox1.Items.Add(info[i].Split(new string[] { "[=_=]" }, StringSplitOptions.RemoveEmptyEntries)[0]);

                LastIndex = listBox1.SelectedIndex = 0;
            }

            foreach (string st in listBox1.Items)
            { listBox2.Items.Add(""); status.Add(""); }

            Delete_Time = false;
        }
        #endregion

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void тестToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser1.Navigate("https://login.vk.com/?act=login&email=79193517220&pass=------");
            webBrowser1.Navigate("https://vk.com/videos-6020992");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string text = webBrowser1.DocumentText.Split(new string[] { "Nostalgia Critic - Jack Frost" }, StringSplitOptions.RemoveEmptyEntries)[1];
            MessageBox.Show(text);
        }

    }
}
