using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using TwinCAT.Ads;
using System.IO;
using System.Timers; 

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private int hbool1;
        private int hint1;
        private int hstr1;
        private int hlreal1;
        private TcAdsClient adsClient;
        private ArrayList notificationHandles;
        private int hcomplexStruct;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //read by handle
                //the second parameter specifies the type of the variable

                textBox1.Text = adsClient.ReadAny(hbool1, typeof(Boolean)).ToString();
                textBox2.Text = adsClient.ReadAny(hint1, typeof(int)).ToString();
                textBox4.Text = adsClient.ReadAny(hlreal1, typeof(Double)).ToString();
                textBox3.Text = adsClient.ReadAny(hstr1, typeof(String), new int[] { 5 }).ToString();
                FillStructControls((ComplexStruct)adsClient.ReadAny(hcomplexStruct, typeof(ComplexStruct)));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            adsClient = new TcAdsClient();
            notificationHandles = new ArrayList();
            try
            {
               
                adsClient.AdsNotificationEx += new AdsNotificationExEventHandler(adsClient_AdsNotificationEx);
                button4.Enabled = false;
                button6.Enabled = false;
                adsClient.Connect(801);
                hbool1 = adsClient.CreateVariableHandle("MAIN.bool1");
                hint1 = adsClient.CreateVariableHandle("MAIN.int1");
                hstr1 = adsClient.CreateVariableHandle("MAIN.str1");
                hlreal1 = adsClient.CreateVariableHandle("MAIN.lreal1");
                hcomplexStruct = adsClient.CreateVariableHandle("MAIN.ComplexStruct1");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private  void TimeEvent(object source, ElapsedEventArgs e)
        {
            

        }
        private void adsClient_AdsNotificationEx(object sender, AdsNotificationExEventArgs e)
        {
            TextBox textBox = (TextBox)e.UserData;
            Type type = e.Value.GetType();
            if (type == typeof(string) || type.IsPrimitive)
                textBox.Text = e.Value.ToString();
            else if (type == typeof(ComplexStruct))
                FillStructControls((ComplexStruct)e.Value);     
        }
        
       
      
        private void button2_Click(object sender, EventArgs e)
        {

            try
            {

                adsClient.WriteAny(hbool1, Boolean.Parse(textBox1.Text));
                adsClient.WriteAny(hint1, byte.Parse(textBox2.Text));
                adsClient.WriteAny(hstr1, textBox3.Text, new int[] { 5 });
                adsClient.WriteAny(hcomplexStruct, GetStructFromControls());
                adsClient.WriteAny(hlreal1, Double.Parse(textBox4.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            button3.Enabled = false;
            button4.Enabled = true;
        }

        private void FillStructControls(ComplexStruct structure)
        {

          
            
            textBox5.Text= structure.boolVal.ToString();
            textBox6.Text = structure.stringVal;
            textBox7.Text = String.Format("{0:d}, {1:d}, {2:d}, {3:d}", structure.dintArr[0],
                structure.dintArr[1], structure.dintArr[2], structure.dintArr[3]);
           
          
        
        }
        private ComplexStruct GetStructFromControls()
        {

            ComplexStruct structure = new ComplexStruct();
            String[] stringArr = textBox7.Text.Split(new char[] { ',' });
            
            for (int i = 0; i < stringArr.Length; i++)
                structure.dintArr[i] = int.Parse(stringArr[i]);

            structure.boolVal = Boolean.Parse(textBox5.Text);

            structure.stringVal = textBox6.Text;
           
            return structure;
        }

       
        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            button3.Enabled = true;
            button4.Enabled = false;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            textBox1.Text = adsClient.ReadAny(hbool1, typeof(Boolean)).ToString();
            textBox2.Text = adsClient.ReadAny(hint1, typeof(int)).ToString();
            textBox4.Text = adsClient.ReadAny(hlreal1, typeof(Double)).ToString();
            textBox3.Text = adsClient.ReadAny(hstr1, typeof(String), new int[] { 5 }).ToString();
            FillStructControls((ComplexStruct)adsClient.ReadAny(hcomplexStruct, typeof(ComplexStruct)));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            notificationHandles.Clear();
            try
            {
                //register notification            
                notificationHandles.Add(adsClient.AddDeviceNotificationEx("MAIN.int1", AdsTransMode.OnChange, 100, 0, textBox2, typeof(int)));

                notificationHandles.Add(adsClient.AddDeviceNotificationEx("MAIN.bool1", AdsTransMode.OnChange, 100, 0, textBox1, typeof(Boolean)));
                notificationHandles.Add(adsClient.AddDeviceNotificationEx("MAIN.lreal1", AdsTransMode.OnChange, 100, 0, textBox4, typeof(Double)));
                notificationHandles.Add(adsClient.AddDeviceNotificationEx("MAIN.str1", AdsTransMode.OnChange, 100, 0, textBox3, typeof(String), new int[] { 5 }));
                notificationHandles.Add(adsClient.AddDeviceNotificationEx("MAIN.complexStruct1", AdsTransMode.OnChange, 100, 0, textBox5, typeof(ComplexStruct)));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            button6.Enabled = true;
            button5.Enabled = false;
         
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //delete registered notifications.
            try
            {
                foreach (int handle in notificationHandles)
                    adsClient.DeleteDeviceNotification(handle);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            notificationHandles.Clear();
            button5.Enabled = true;
            button6.Enabled = false;
            
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {

        }
        
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class ComplexStruct
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool boolVal;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 6)]
        public string stringVal = "";
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public int[] dintArr = new int[4];
       
	
    }
}