using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Jep.Hardware.CardReader;
using System.Threading;
namespace Test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.comboBox1.Text.Equals("IDR210"))
            {
                bool Flag = IDR210.Open();
                IDCard idcard = IDR210.Read();
                if (idcard != null)
                {
                    this.lalName.Text = idcard.Name;
                    this.lalSex.Text = idcard.Sex == 1 ? "男" : "女";
                    this.lalTime.Text = idcard.ValidityPeriod.ToString();
                    this.lalFolk.Text = idcard.EthnicGroup.ToString();
                    this.lalCode.Text = idcard.IDCardNumber.ToString();
                    this.lalBirthday.Text = idcard.Birthday.ToString("yyyy年MM月dd日");
                    this.lalAgree.Text = idcard.IssueUnit.ToString();
                    this.lalAddress.Text = idcard.Address.ToString();
                    this.pictureBox1.Load("photo//photo.bmp");
                    //this.pictureBox1.Image = idcard.Photo;
                }
            }
            else
            {
                ReadCard read = new ReadCard(1);
                bool Flag = read.Open();
                IDCard idcard = null;
                if (Flag)
                {
                    idcard = read.ReadIDCard();
                    if (idcard != null)
                    {
                        this.lalName.Text = idcard.Name;
                        this.lalSex.Text = idcard.Sex == 1 ? "男" : "女";
                        this.lalTime.Text = idcard.ValidityPeriod.ToString();
                        this.lalFolk.Text = idcard.EthnicGroup.ToString();
                        this.lalCode.Text = idcard.IDCardNumber.ToString();
                        this.lalBirthday.Text = idcard.Birthday.ToString("yyyy年MM月dd日");
                        this.lalAgree.Text = idcard.IssueUnit.ToString();
                        this.lalAddress.Text = idcard.Address.ToString();
                        this.pictureBox1.Load("photo//photo.bmp");
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool Flag = false;
            while (true)
            {
                string cardid = "";
                Thread.Sleep(2000);
                IDR210.Beep(100);
                Flag = IDR210.FindCard(out cardid);
                if (!Flag)
                {
                    Console.WriteLine("寻卡失败");
                }
                else
                {
                    string strs = string.Format("寻到{0}卡",cardid);
                    Console.WriteLine(strs);
                }
            }

        }
    }
}
