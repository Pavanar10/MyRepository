using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data.OleDb;

namespace ProductionData
{
    public partial class Form1 : Form
    {
        string ConnectionString;
        SqlConnection cnn;
        int NoOfAbia = 0;
        int NoOfAsia = 0;
        string show = "daily";
        bool close = false;


        int NoOfAbianew = 0;
        int NoOfAsiaNew = 0;
        public Form1()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
            Daily.Visible = false;
            Date.Visible = false;
            Nokia.Focus();

            //Total and Daily display data grid font
            TotalandDaily.RowTemplate.MinimumHeight = 130;
            TotalandDaily.Rows[0].Height = 55;
            TotalandDaily.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 25F, FontStyle.Bold);
                     

            //populate datagrid
            ConnectToDb();

            // Create_Chart();
            Title Production = new Title();
            Production.Font = new Font("Calibri", 36, FontStyle.Bold);
            //Production.Text = "Nokia Production Chart";
            TotalProdChart.Titles.Add(Production);
            Production.Alignment = ContentAlignment.BottomRight;
            this.TotalProdChart.Refresh();
            TotalProdChart.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = false;
            TotalProdChart.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = false;
            TotalProdChart.ChartAreas["ChartArea1"].Area3DStyle.Enable3D = true;
            TotalProdChart.ChartAreas["ChartArea1"].Position.Height = 100;


            show_delivery();


            //tableLayoutPanel1.Visible = false;
            //tableLayoutPanel2.Visible = false;
            //label5.Visible = false;
            //label6.Visible = false;

        }
        //Function to show Daily Production
        public void show_Daily()
        {
            TotalProdChart.Visible = false;
            Daily.Visible = true;
            Date.Visible = true;
            NokiaChart.Visible = false;
            Date.Text = DateTime.Now.ToString("dd-MM-yy");
            TotalandDaily.Columns[0].Visible = false;
            TotalandDaily.Columns[1].Visible = false;
            ConnectionString = @"Data Source=xxx;Initial Catalog=xxx;User ID=xxx;Password=xxx";
            cnn = new SqlConnection(ConnectionString);
            try
            {
                //read data from db for calculating ABIA and ASIA units
                cnn.Open();
                SqlCommand cmd = new SqlCommand("select count(distinct UnitID) from xyz where StationID in(4) and (CreationTime >= Cast(GETDATE() as date)) and Status ='PASS'", cnn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                NoOfAbia = dr.GetInt32(0);
                dr.Close();

                if (NoOfAbia == 0)
                    TotalandDaily.Rows[0].Cells[2].Value = null;
                else
                    TotalandDaily.Rows[0].Cells[2].Value = NoOfAbia;

               if (dr.IsClosed == true)
                {

                    SqlCommand cmd1 = new SqlCommand("select count(distinct UnitID) from xyztable where StationID in(16) and (CreationTime >=  Cast(GETDATE() as date)) and Status ='PASS'", cnn);
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    dr1.Read();
                    NoOfAsia = dr1.GetInt32(0);
                    if (NoOfAsia == 0)
                        TotalandDaily.Rows[0].Cells[3].Value = null;
                    else
                    TotalandDaily.Rows[0].Cells[3].Value = NoOfAsia;
                }

                //add abia and asia to calculate total production
                if (NoOfAbia == 0 && NoOfAsia == 0)
                    TotalandDaily.Rows[0].Cells[4].Value = null;
                else
                    TotalandDaily.Rows[0].Cells[4].Value = NoOfAbia + NoOfAsia;

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //Close sql connection
            cnn.Close();
        

        }


        //creating chart to display no of units produced
        public void Create_Chart()
        {
            TotalProdChart.Series.Clear();

            TotalProdChart.Series.Add("NokiaPR");
            TotalProdChart.Refresh();
            TotalProdChart.Update();
            string[] seriesArray = { "ABIA", "ASIA" };
            int abia = Convert.ToInt32(TotalandDaily.Rows[0].Cells[0].Value) + Convert.ToInt32(TotalandDaily.Rows[0].Cells[2].Value);
            string[] pointsArray = {abia.ToString(), TotalandDaily.Rows[0].Cells[1].Value.ToString() };
            try
            {
                //Add series and points
                this.TotalProdChart.Series["NokiaPR"].Points.AddXY(seriesArray[0], pointsArray[0]);
                this.TotalProdChart.Series["NokiaPR"].Points.AddXY(seriesArray[1], pointsArray[1]);

                TotalProdChart.Series["NokiaPR"].Points[0].Color = Color.OrangeRed;
                TotalProdChart.Series["NokiaPR"].Points[1].Color = Color.ForestGreen;

                TotalProdChart.Series["NokiaPR"].IsValueShownAsLabel = true;
                TotalProdChart.Series["NokiaPR"].Font = new Font("Arial", 22, FontStyle.Bold);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
        }


        //function to display total production
        public void ConnectToDb()
        {
            TotalProdChart.Visible = true;
            NokiaChart.Visible = true;
            Daily.Visible = false;
            Date.Visible = false;
            TotalandDaily.Columns[0].Visible = true;
            TotalandDaily.Columns[1].Visible = true;


            //for new ABIA and ASIA
            ConnectionString = @"Data Source=xxx;Initial Catalog=xxx;User ID=xxx;Password=xxx";
            cnn = new SqlConnection(ConnectionString);
            try
            {
                cnn.Open();

                //display ABIA old revision
                SqlCommand cmd = new SqlCommand("select count(distinct UnitID) from xyz where StationID in(4) and CreationTime between '2019-06-28 00:00:00' and '2019-09-05 00:00:00' and Status ='PASS'", cnn);
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                NoOfAbia = dr.GetInt32(0);
                TotalandDaily.Rows[0].Cells[0].Value = NoOfAbia;
                dr.Close();
                if (dr.IsClosed == true)
                {
                    //display ASIA old revision
                    SqlCommand cmd1 = new SqlCommand("select count(distinct UnitID) from xyz where StationID in(16) and CreationTime between '2019-06-28 00:00:00' and '2019-10-03 00:00:00'and Status ='PASS'", cnn);
                    SqlDataReader dr1 = cmd1.ExecuteReader();
                    dr1.Read();
                    NoOfAsia = dr1.GetInt32(0);
                    TotalandDaily.Rows[0].Cells[1].Value = NoOfAsia;
                    dr1.Close();

                    if (dr1.IsClosed == true)
                    {
                        //display ABIA new revision
                        SqlCommand cmd2 = new SqlCommand("select count(distinct UnitID) from xyz where StationID in(4) and CreationTime >='2019-09-05 00:00:00' and Status ='PASS'", cnn);
                        SqlDataReader dr2 = cmd2.ExecuteReader();
                        dr2.Read();
                        NoOfAbianew = dr2.GetInt32(0);
                        TotalandDaily.Rows[0].Cells[2].Value = NoOfAbianew;
                        dr2.Close();

                        if (dr2.IsClosed == true)
                        {
                            //display ASIA new revision
                            SqlCommand cmd3 = new SqlCommand("select count(distinct UnitID) from xyz where StationID in(16) and CreationTime >= '2019-10-03 00:00:00'and Status ='PASS'", cnn);
                            SqlDataReader dr3 = cmd3.ExecuteReader();
                            dr3.Read();
                            NoOfAsiaNew = dr3.GetInt32(0);
                            TotalandDaily.Rows[0].Cells[3].Value = NoOfAsiaNew;
                            dr3.Close();
                        }
                    }
                }
                              
                TotalandDaily.Rows[0].Cells[4].Value = NoOfAbia + NoOfAsia + NoOfAbianew + NoOfAsiaNew;
               
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Try Again");
            }
            cnn.Close();
            TotalProdChart.Series.Clear();

            TotalProdChart.Series.Add("NokiaPR");


            string[] seriesArray = { "ABIA", "ASIA" };
            int abia = Convert.ToInt32(TotalandDaily.Rows[0].Cells[0].Value) + Convert.ToInt32(TotalandDaily.Rows[0].Cells[2].Value);
            int asia = Convert.ToInt32(TotalandDaily.Rows[0].Cells[1].Value) + Convert.ToInt32(TotalandDaily.Rows[0].Cells[3].Value);
            string[] pointsArray = { abia.ToString(), asia.ToString() };
            try
            {
                //Add series and points
                this.TotalProdChart.Series["NokiaPR"].Points.AddXY(seriesArray[0], pointsArray[0]);
                this.TotalProdChart.Series["NokiaPR"].Points.AddXY(seriesArray[1], pointsArray[1]);

                TotalProdChart.Series["NokiaPR"].Points[0].Color = Color.OrangeRed;
                TotalProdChart.Series["NokiaPR"].Points[1].Color = Color.ForestGreen;

                TotalProdChart.Series["NokiaPR"].IsValueShownAsLabel = true;
                TotalProdChart.Series["NokiaPR"].Font = new Font("Arial", 48, FontStyle.Bold);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            this.TotalProdChart.Refresh();

        }
        //Show delivery..............
        public void show_delivery()
        {
            try
            {
              
                string constr = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source='DeliveryData.xlsx'; Extended Properties = 'Excel 12.0 xml;'";
                OleDbConnection con;
                OleDbDataAdapter adapter;

                con = new OleDbConnection(constr);
                string sql = "select * from [Sheet1$]";
                adapter = new OleDbDataAdapter(sql, con);
                DataSet ds = new DataSet();
                adapter.Fill(ds);

                //dataGridView3.DataSource = ds.Tables[0];
                DataRow dr = ds.Tables[0].Rows[0];
                AbiaDelivered.Text = dr["ABIA"].ToString();
                AsiaDelivered.Text = dr["ASIA"].ToString();
                int abia = Convert.ToInt32(dr["ABIA"]);
                int asia = Convert.ToInt32(dr["ASIA"]);
                TotalDelivered.Text = (abia + asia).ToString();

                //show finished goods stock
                AbiaFG.Text = (NoOfAbia + NoOfAbianew - abia).ToString();
                AbiaFG.Refresh();
                AbiaFG.Update();

                ASIAFG.Text = (NoOfAsia + NoOfAsiaNew - asia).ToString();
                ASIAFG.Refresh();
                ASIAFG.Update();

                TotalFG.Text = (Convert.ToInt32((NoOfAbia + NoOfAbianew - abia) + (NoOfAsia + NoOfAsiaNew - asia))).ToString();
                TotalFG.Refresh();
                TotalFG.Update();

                //close oledb con
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        //Add timer to display both Total and Daily Production
        private void Timer1_Tick(object sender, EventArgs e)
        {
            // Show daily production
            if (show == "daily")
            {
                show_Daily();
                show = "total";
            }
            else if (show == "total")
            {
               
               ConnectToDb();
               show_delivery();
               show = "daily";
                
            }
           
            
            
        }

       
    }
}
