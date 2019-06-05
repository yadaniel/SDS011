using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace sds011_viewer
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form {
		private SDS011 sds011;
		private int chartIdx = 0;
		private Chart chart;
		private Series series_pm25;
		private Series series_pm10;
		private string comPort = string.Empty;
		private System.Threading.Timer timer1000;
		private Random random;
		
		private static void TimerTick1000(object sender) {
			MainForm mainForm = (MainForm)sender;
			mainForm.Invoke(new Action(mainForm.updateChart));
		}
		
		private void updateChart() {
			//chart.Series["pm25"].Points.AddXY(chartIdx+=1, random.Next(0,10));
			//chart.Series["pm10"].Points.AddXY(chartIdx+=1, random.Next(0,10));
			//chart.Series["pm25"].Points.RemoveAt(0);
			//chart.ChartAreas["pm"].AxisX.Minimum = chartIdx-10;
			//chart.ChartAreas["pm"].AxisX.Maximum = chartIdx;
			if(sds011.pm_valid) {
				sds011.pm_valid = false;
				chart.Series["pm25"].Points.AddXY(chartIdx+=1, sds011.pm25);
				chart.Series["pm10"].Points.AddXY(chartIdx+=1, sds011.pm10);
				chart.ChartAreas["pm"].AxisX.Minimum = chartIdx-60;
				chart.ChartAreas["pm"].AxisX.Maximum = chartIdx;
			}
		}
		
		public MainForm() {
			InitializeComponent();
			random = new Random();
			sds011 = SDS011.getInstance();
			timer1000 = new System.Threading.Timer(TimerTick1000, this, 5000, 1000);
			createChart();
		}
		
		void MainFormDoubleClick(object sender, EventArgs e) {
        	sds011.Dispose();
        	timer1000.Dispose();
			Application.Exit();
		}
		
		void MainFormFormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
			sds011.Dispose();
        	timer1000.Dispose();
			Application.Exit();
		}
		
		private void createChart() {
			chart = new Chart() {
				Size = new Size(600, 300),
				Location = new Point() {X=10, Y=10},
			};
			series_pm25 = new Series() {
				Name = "pm25",
				//ChartType = SeriesChartType.Point,
				//MarkerStyle = MarkerStyle.Circle,
				//MarkerSize = 3,
				Color = Color.Red
			};
			series_pm10 = new Series() {
				Name = "pm10",
				//ChartType = SeriesChartType.Point,
				//MarkerStyle = MarkerStyle.Circle,
				//MarkerSize = 3,
				Color = Color.Green
			};
			Title title = new Title() {
				Text = "PM 2.5um 10um",
				Name = chart.Titles.NextUniqueName()
			};
			ChartArea area = new ChartArea() {
				Name = "pm",
				//Position = new ElementPosition(){Auto=false, X=50, Y=50, Width=10, Height=10},
				//InnerPlotPosition = new ElementPosition(){Auto=false, X=50, Y=50, Width=10, Height=10},
				BorderWidth = 5,
				AxisX = new Axis() {Title = "time [s]"},
				AxisY = new Axis() {Title = "pm 2.5 and pm 10 [uq/m^3]"},
			};
			chart.ChartAreas.Add(area);
			chart.Series.Add(series_pm25);
			chart.Series.Add(series_pm10);
			chart.Titles.Add(title);
			chart.Visible = true;
			Controls.Add(chart);
		}
		
		private void GroupBoxCOMEnter(object sender, EventArgs e) {
	        string[] listCOM = SerialPort.GetPortNames();
	        comboBoxSelectCOM.Items.Clear();
	        foreach (string com in listCOM) {
	            comboBoxSelectCOM.Items.Add(com);
	        }
		}
		
		void ConnectCOMClick(object sender, EventArgs e)
		{
			Button connect = (Button)sender;
        	if (connect.Text == "connect") {
            	// previously disconnected
            	string comPort = string.Empty;
            	try {
                	comPort = comboBoxSelectCOM.SelectedItem.ToString();
            	} catch {
                	MessageBox.Show(text: "connect SDS011 and select COM port", caption: "COM list error", buttons: MessageBoxButtons.OK);
            	}
            	if (comPort != string.Empty) {
                	bool connectionEstablished = sds011.tryConnectCOM(comPort);
                	if (connectionEstablished) {
                    	sds011.startThread();
                    	connect.Text = "disconnect";
                    	connect.BackColor = Color.LightGreen;
                	}
            	} else { /* something went wrong, refresh and connect again */ }
        	} else {
            	// previously connected
            	if (sds011.threadRunning)
            	{
                	sds011.stopThread();
                	connect.Text = "connect";
                	connect.BackColor = Control.DefaultBackColor;
            	}
        	}
		}
	}
}
