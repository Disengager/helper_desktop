using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;

namespace Helper
{
    public partial class HelperAddSite : Form
    {
        public HelperAddSite()
        {
            InitializeComponent();
        }

        #region Получить код
        System.Net.WebClient webClient = new System.Net.WebClient();
        public string GetCodeInSite(string link)
        {
            string Val = "";
            try
            {
                Stream st = webClient.OpenRead(link);
                StreamReader sr = new StreamReader(st);
                Val = sr.ReadToEnd();
                st.Close();
            }
            catch{};
            return Val;
        }
        #endregion

        #region Узнать информацию по тегам

        class GetInformation
        {
           public string RefreshTeg1 = "null", RefreshTeg2 = "null";
           public int RefreshResult = 0;

        }
        GetInformation NowInform = new GetInformation();

        public string GetInfo(Control teg1,Control teg2)
        {
            try
            {

                if (NowInform.RefreshTeg1 != teg1.Text) { NowInform.RefreshTeg1 = teg1.Text; NowInform.RefreshTeg2 = teg2.Text; NowInform.RefreshResult = 1; }
                else { NowInform.RefreshResult++; }
                string st = richTextBox1.Text.Split(new string[] { teg1.Text }, StringSplitOptions.RemoveEmptyEntries)[NowInform.RefreshResult];
                string finalinfo = st.Substring(0, st.IndexOf(teg2.Text));
                if (teg1.Name == "textBox7")
                {
                    string result = "";
                    for (int i = 0; i < finalinfo.Length; i++)
                        foreach (string st1 in new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" })
                            result += finalinfo[i].ToString() == st1? st1: String.Empty;
                    finalinfo = result;
                }
                return finalinfo;
            }
            catch { return "Ничего не найдено"; };
        }
        #endregion

        #region Объеденение всех трёх вкладок
        private string ftext;

        public string text
        {
            get { return ftext; }
            set
            {
                ftext = value;
                richTextBox2.Text = richTextBox3.Text = richTextBox1.Text = ftext;
            }
        }

        private string fEdit;

        public string Edit
        {
            get { return fEdit;  }
            set
            {
                fEdit = value;
                textBox1.Text = fEdit;
            }
        }

        #endregion

        private void HelperAddSite_Load(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        #region кнопки

        private void button1_Click(object sender, EventArgs e)
        {
            Edit = textBox1.Text;
            text = GetCodeInSite(Edit);
        }

        private void button6_Click(object sender, EventArgs e)  { textBox5.Text = GetInfo(textBox7, textBox8); }

        private void button2_Click(object sender, EventArgs e) { textBox4.Text = GetInfo(textBox2, textBox3); }

        private void textBox1_TextChanged(object sender, EventArgs e)  { Edit = textBox1.Text; }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            if ((textBox4.Text == "") | (textBox4.Text == "")) { MessageBox.Show("Вкладки 'Название' и 'Обновление' - обязательные к заполнению"); return; };


            string sisfolder = (Application.OpenForms[0] as Form1).sisfolder;
            string[] st = textBox1.Text.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> StringList = new List<string>();
            string[] mas = File.ReadAllLines(sisfolder + @"\Helper\SiteInfo.dis");
            foreach (string s in mas)
                StringList.Add(s);
            StringList.Add(st[1] + "[=_=]" + textBox2.Text + "[=_=]" + textBox3.Text + "[=_=]" + textBox7.Text + "[=_=]" + textBox8.Text + "[=_=]" + textBox11.Text + "[=_=]" + textBox12.Text);
            File.WriteAllLines(sisfolder + @"\Helper\SiteInfo.dis", StringList.ToArray());
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e) { this.Hide(); }

        private void button8_Click(object sender, EventArgs e) { textBox9.Text = GetInfo(textBox11, textBox12); }


    }
}
