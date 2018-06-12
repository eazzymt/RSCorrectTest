using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClsRS;

namespace RSCorrectTest
{
    public partial class Form1 : Form
    {
        int rsLen = 9;
        int parityLen = 4;
        RS objRS;
        byte[] parityData;

        public Form1()
        {
            InitializeComponent();
            objRS = new RS(rsLen, parityLen);
        }

        private void BtnEncode_Click(object sender, EventArgs e)
        {
            txtCorrData.Text = string.Empty;
            txtCorrParity.Text = string.Empty;

            if (txtInData.Text == string.Empty) return;
            byte[] encData = new byte[rsLen - parityLen];
            parityData = new byte[parityLen];

            for (int cnt = 0; cnt < txtInData.Text.Length; cnt++)
            {
                encData[cnt] = (byte)txtInData.Text[cnt];
            }
            encData = objRS.Encode(encData);
            for (int cnt = 0; cnt < txtInData.Text.Length; cnt++)
            {
                txtErrData.Text += (char)encData[cnt];
            }
            for (int cnt = rsLen - parityLen; cnt < rsLen; cnt++)
            {
                parityData[cnt - rsLen + parityLen] = encData[cnt];
                txtErrParity.Text += encData[cnt].ToString("X2") + "h ";
            }
        }

        private void BtnDecode_Click(object sender, EventArgs e)
        {
            txtCorrData.Text = string.Empty;
            txtCorrParity.Text = string.Empty;

            RS objRS = new RS(rsLen, parityLen);
            byte[] encData = new byte[rsLen];
            for (int cnt = 0; cnt < txtErrData.Text.Length; cnt++)
            {
                encData[cnt] = (byte)txtErrData.Text[cnt];
            }
            for (int cnt = rsLen - parityLen; cnt < rsLen; cnt++)
            {
                encData[cnt] = parityData[cnt - (rsLen - parityLen)];
            }
            byte[] decData = objRS.Decode(encData);
            for (int cnt = 0; cnt < txtErrData.Text.Length; cnt++)
            {
                txtCorrData.Text += (char)decData[cnt];
            }
            for (int cnt = txtErrData.Text.Length; cnt < decData.Length; cnt++)
            {
                txtCorrParity.Text += decData[cnt].ToString("X2") + "h ";
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            txtInData.Text = "ABCDE";
        }
    }
}
