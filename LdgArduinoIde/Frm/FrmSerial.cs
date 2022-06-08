using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace LdgArduinoIde
{
    public partial class FrmSerial : DockContent
    {

        string serialName = "";
        public FrmSerial(string serialName)
        {
            InitializeComponent();
            this.serialName = serialName;

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        private void FrmSerial_Load(object sender, EventArgs e)
        {
            
            //serialPort1.Encoding = Encoding.GetEncoding("GB2312");
            serialPort1.Encoding = Encoding.UTF8;
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();//获取电脑上可用串口号
            cmbPort.Items.AddRange(ports);//给comboBox1添加数据
            cmbPort.SelectedIndex = cmbPort.Items.Count > 0 ? 0 : -1;//如果里面有数据,显示第0个
            if (string.IsNullOrEmpty(serialName))
            {
                serialName = ArduinoIde.GetValue("serial.port");
            }
            
            cmbPort.Text = serialName;
            cmbDatabits.Text = ArduinoIde.GetValue("serial.databits");
            cmbBaudRate.Text = ArduinoIde.GetValue("serial.debug_rate");
            //int line = Convert.ToInt32( ArduinoIde.GetValue("serial.line_ending")); 
            cmbStop.SelectedIndex= cmbStop.Items.Count > 0 ? 0 : -1;
    
        }

        private void btnOpenOrClose_Click(object sender, EventArgs e)
        {
            OpenOrClose();
        }
        public  void WndProcNew(ref Message m)
        {
            if (m.Msg == 0x0219)
            {//设备改变
                if (m.WParam.ToInt32() == 0x8004)
                {//usb串口拔出
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    cmbPort.Items.Clear();//清除comboBox里面的数据
                    cmbPort.Items.AddRange(ports);//给comboBox1添加数据
                    if (btnOpenOrClose.Text == "关闭串口")
                    {//用户打开过串口
                        if (!serialPort1.IsOpen)
                        {//用户打开的串口被关闭:说明热插拔是用户打开的串口
                            btnOpenOrClose.Text = "打开串口";
                            serialPort1.Dispose();//释放掉原先的串口资源
                            cmbPort.SelectedIndex = cmbPort.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                            //cmbPort.Text = serialName;
                        }
                        else
                        {
                            cmbPort.Text = serialName;//显示用户打开的那个串口号
                        }
                    }
                    else
                    {//用户没有打开过串口
                        cmbPort.SelectedIndex = cmbPort.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                    }
                }
                else if (m.WParam.ToInt32() == 0x8000)
                {//usb串口连接上
                    string[] ports = System.IO.Ports.SerialPort.GetPortNames();//重新获取串口
                    cmbPort.Items.Clear();
                    cmbPort.Items.AddRange(ports);
                    if (btnOpenOrClose.Text == "关闭串口")
                    {//用户打开过一个串口
                        cmbPort.Text = serialName;//显示用户打开的那个串口号
                    }
                    else
                    {
                        cmbPort.SelectedIndex = cmbPort.Items.Count > 0 ? 0 : -1;//显示获取的第一个串口号
                    }
                }
            }
            //base.WndProc(ref m);
        }
    
        void OpenOrClose()
        {
            if (btnOpenOrClose.Text == "打开串口")
            {//如果按钮显示的是打开串口
                try
                {//防止意外错误
                    serialPort1.PortName = cmbPort.Text;//获取comboBox1要打开的串口号
                    serialName = cmbPort.Text;
                    serialPort1.BaudRate = int.Parse(cmbBaudRate.Text);//获取comboBox2选择的波特率
                    serialPort1.DataBits = int.Parse(cmbDatabits.Text);//设置数据位
                    ///*设置停止位*/
                    if (cmbStop.Text == "1") { serialPort1.StopBits = StopBits.One; }
                    else if (cmbStop.Text == "1.5") { serialPort1.StopBits = StopBits.OnePointFive; }
                    else if (cmbStop.Text == "2") { serialPort1.StopBits = StopBits.Two; }
                    ///*设置奇偶校验*/
                    //if (comboBox5.Text == "无") { serialPort1.Parity = Parity.None; }
                    //else if (comboBox5.Text == "奇校验") { serialPort1.Parity = Parity.Odd; }
                    //else if (comboBox5.Text == "偶校验") { serialPort1.Parity = Parity.Even; }

                    serialPort1.Open();//打开串口
                    btnOpenOrClose.Text = "关闭串口";//按钮显示关闭串口
                }
                catch (Exception err)
                {
                    MessageBox.Show("打开失败" + err.Message ,"提示!");//对话框显示打开失败
                }
            }
            else
            {//要关闭串口
                try
                {//防止意外错误
                    serialPort1.Close();//关闭串口
                }
                catch (Exception) { }
                btnOpenOrClose.Text = "打开串口";//按钮显示打开
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.Threading.Thread.Sleep(1000);
            try
            {
                int len = serialPort1.BytesToRead;//获取可以读取的字节数
                byte[] buff = new byte[len];//创建缓存数据数组
                serialPort1.Read(buff, 0, len);//把数据读取到buff数组
                string str = Encoding.UTF8.GetString(buff);//Byte值根据ASCII码表转为 String
                str = str.Replace("�", "");
                Invoke((new Action(() => //C# 3.0以后代替委托的新方法
                {
                    int start = txtLog.SelectionStart;
                    int end = txtLog.SelectionLength;
                    Point p= txtLog.AutoScrollOffset;
                    if (chkShowTime.Checked)
                    {
                        txtLog.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + " -> ");
                    }
                    txtLog.AppendText( str);//对话框追加显示数据
                    if (!ChkShowCurr.Checked)
                    {
                        txtLog.Select(start, end);//设置光标的位置到文本尾
                        txtLog.ScrollToCaret();

                        //txtLog.ScrollToCaret();

                    }
                })));
            }
            catch (Exception)
            {

            }
            
        }

        private void txtLog_TextChanged(object sender, EventArgs e)
        {
            //if (ChkShowCurr.Checked)
            //{
                
            //    txtLog.ScrollToCaret();
            //}
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string Str = txtContent.Text;//获取发送文本框里面的数据
            try
            {
                if (Str.Length > 0)
                {
                    serialPort1.Write(Str);//串口发送数据
                    txtContent.Text = "";
                }
            }
            catch (Exception) { }
        }
        public void CloseSerial()
        {
            if (btnOpenOrClose.Text == "关闭串口")
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    OpenOrClose();
                });
            }
        }
        public void OpenSerial()
        {
            if (btnOpenOrClose.Text == "打开串口")
            {
                this.Invoke((MethodInvoker)delegate ()
                {
                    OpenOrClose();
                });
                
            }
        }
        public bool IsOpen { get { return btnOpenOrClose.Text == "关闭串口"; } }
        private void txtContent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar=='\r')
            {
                btnSend_Click(null,null);
                e.Handled = true;
            }
        }

        private void FrmSerial_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                serialPort1.Close();//关闭串口
            }
            catch (Exception)
            {
            }
            
        }

        private void cmbPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (btnOpenOrClose.Text=="关闭串口")
            {
                OpenOrClose(); OpenOrClose();
            }
        }

        private void FrmSerial_Shown(object sender, EventArgs e)
        {
            if (cmbPort.Text==serialName)
            {
                OpenOrClose();
            }
        }

    }
}
