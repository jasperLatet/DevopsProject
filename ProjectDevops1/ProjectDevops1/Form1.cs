using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Dapper;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ProjectDevops1
{

    public partial class Form1 : Form
    {


        string connectionURL = @"SERVER=localhost;DATABASE=devops;UID=root;PASSWORD=;SslMode=None";



        MySqlConnection verbinding = null;
        MySqlDataReader rdr = null;

    
        const string Key = "71f11aa6cd5a5b808f39cdbf827d9e22";
      
        List<System.Windows.Forms.Label> lblList;

        List<System.Windows.Forms.Label> lblDatumList;
        List<System.Windows.Forms.Label> lblHuminity;
        List<System.Windows.Forms.Label> lblSpeed;
        List<System.Windows.Forms.PictureBox> pictureBoxList;

        List<WeatherInfo> currentInfo;

        public Form1()
        {
            InitializeComponent();
            lblList = new List<System.Windows.Forms.Label>
                    {
                        lblTemp2,
                        lblTemp3,
                        lblTemp4,
                        lblTemp5,
                        lblTemp6,
                    };
            lblDatumList = new List<System.Windows.Forms.Label>
                    {
                        lblDatum2,
                        lblDatum3,
                        lblDatum4,
                        lblDatum5,
                        lblDatum6,
                    };

            lblHuminity = new List<System.Windows.Forms.Label>
                    {
                         lblHum1,
                         lblHum2,
                         lblHum3,
                         lblHum4,
                         lblHum5,
                    };
            lblSpeed = new List<System.Windows.Forms.Label>
                    {
                         lblSpeed1,
                         lblSpeed2,
                         lblSpeed3,
                         lblSpeed4,
                         lblSpeed5,
                        
                    };
            pictureBoxList = new List<PictureBox> { 
                        pictureBox1,
                        pictureBox2,
                        pictureBox3,
                        pictureBox4,
                        pictureBox5,
                       
                            
            };
            currentInfo = new List<WeatherInfo>();
            lblDatumList.ForEach(x => x.Hide());
            lblList.ForEach(x => x.Hide());
            lblHuminity.ForEach(x => x.Hide());
            lblSpeed.ForEach(x => x.Hide());
            pictureBoxList.ForEach(x => x.Hide());

            lblNaam.Hide();
            lblTemp1.Hide();
            lblSpeed0.Hide();
            lblHum0.Hide();
            lblVoorspellingen.Hide();

            pictureBox6.Hide();
            btnToevoegen.Hide();
            //getForcast("Hoeselt")
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
                
        }


        void getWeather(string city)
        {
            using (WebClient web = new WebClient { Encoding = System.Text.Encoding.UTF8 })
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/weather?q=" + city + "&appid=71f11aa6cd5a5b808f39cdbf827d9e22&units=metric&cnt=6");
                if (txtStadsnaam.Text != "") {
                    var json = web.DownloadString(url); //Zorgt ervoor dat de JSON gelezen wordt en gedownload voor gebruik
                        var result = JsonConvert.DeserializeObject<Info.root>(json); //convert het in iets leesbaars voor het project


                    dynamic x = JObject.Parse(json);
                    if (((int)x.cod) != 200) { 
                        MessageBox.Show("City doesn't exist, please try another.");
                        return;
                    }
                        Info.root Output = result; //Zorgt ervoor dat de resultaten setters gebruikt worden

                        lblNaam.Text = string.Format("{0}", Output.name);
                        lblTemp1.Text = string.Format("{0}", Output.main.temp) + "°C"; //Haalt het resultaat uit main.temp
                        lblSpeed0.Text = string.Format("{0}", Output.wind.speed) + "m/s";
                        lblHum0.Text = string.Format("{0}", Output.main.humidity) + "%";
                        
                        lblNaam.Show();
                        lblTemp1.Show();
                        lblSpeed0.Show();
                        lblHum0.Show();
                        btnToevoegen.Show();
                        pictureBox6.Show();

                    currentInfo.Add(new WeatherInfo(DateTime.Today, Output.name, Output.main.temp, Output.main.humidity, Output.wind.speed, Output.sys.country));

                }
                else
                {
                    MessageBox.Show("Vul eerst de stads naam in aub!");
                }

            }


        }
        void toevoegenaandb(DateTime time, string stad, double temp, double humidity , double wind,  string land)
        {


            string datum = time.ToString("d");
            string sql = "insert into weer(land, stad, temp, speed, humidity, datum) VALUES " + "('" + land + "'," + "'"  + stad + "'," + "'" + temp.ToString() + "'," + "'" + wind.ToString() + "'," + "'" + humidity.ToString() + "'," + "'" + datum +"')";
 
            verbinding = new MySqlConnection(connectionURL);
            verbinding.Open();

            MySqlCommand command = new MySqlCommand(sql, verbinding);

            command.ExecuteNonQuery();

        }
        /*
        void toevoegenaandb(string stad, double temp, double humidity, double wind, string land)
        {

            DateTime thisDay = DateTime.Today;

            string datum = thisDay.ToString("d");
            string sql = "insert into weer(land, stad, temp, speed, humidity, datum) VALUES " + "('" + land + "'," + "'" + stad + "'," + "'" + temp.ToString() + "'," + "'" + wind.ToString() + "'," + "'" + humidity.ToString() + "'," + "'" + datum + "')";

            verbinding = new MySqlConnection(connectionURL);
            verbinding.Open();

            MySqlCommand command = new MySqlCommand(sql, verbinding);

            command.ExecuteNonQuery();

        }*/
        void getForcast(string city) //41:37
        {
            lblVoorspellingen.Show();
            using (WebClient web = new WebClient())
            {
                string url = string.Format("https://api.openweathermap.org/data/2.5/forecast?q=" + city + "&appid=71f11aa6cd5a5b808f39cdbf827d9e22&units=metric");
                var json = web.DownloadString(url);

                dynamic x = JObject.Parse(json);

                if (((int)x.cod) != 200)
                {

                    return;
                }

                JArray list = x.list;
                DateTime date = DateTime.Today;

                int day = 0;
                float gemTemp = 0;
                float gemHumidity = 0;
                float gemSpeed = 0;

                for (int i = 1; i < list.Count; i++)
                {
                    dynamic item = list[i];
                    Console.WriteLine(item.dt_txt);
                    DateTime currDate = Convert.ToDateTime(item.dt_txt, new CultureInfo("en-US"));
                    gemTemp += (float)item.main.temp;
                    gemHumidity += (float)item.main.humidity;
                    gemSpeed += (float)item.wind.speed;

                    if (!date.Day.Equals(currDate.Day))
                    {

                        gemTemp /= 7;
                        gemHumidity /= 7;
                        gemSpeed /= 7;


                        lblList[day].Text = gemTemp.ToString().Substring(0, 4) + "°C";
                        lblList[day].Show();

                        lblHuminity[day].Text = gemHumidity.ToString() + "%";
                        lblHuminity[day].Show();

                        lblSpeed[day].Text = gemSpeed.ToString().Substring(0, 4) + "m/s";
                        lblSpeed[day].Show();

                        pictureBoxList[day].Show();


                        date = date.AddDays(1);

                        lblDatumList[day].Text = date.ToString("dd" + "-" + "MM" + "-" + "yyyy");
                        lblDatumList[day++].Show();

                        currentInfo.Add(new WeatherInfo(date, (string)x.city.name, format(gemTemp), format(gemHumidity), format(gemSpeed), (string)x.city.country));
                        Console.WriteLine("Temp: " + gemTemp + " Humidity " + gemHumidity + " Gemspeed " + gemSpeed);
                        date = currDate;
                    }

                }
            }
        }
    

        public double format(float val)
        {
            return Double.Parse(string.Format("{0:0.##}", val));
        }
        private void lblNaam_Click(object sender, EventArgs e)
        {

        }

        private void btnToon_Click(object sender, EventArgs e)
        {
            
          
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnWeer_Click(object sender, EventArgs e)
        {
            string stad = txtStadsnaam.Text;
            currentInfo.Clear();
            getWeather(stad);
            getForcast(stad);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void lblHum1_Click(object sender, EventArgs e)
        {

        }

        private void lblSpeed2_Click(object sender, EventArgs e)
        {

        }

        private void btnToevoegen_Click(object sender, EventArgs e)
        {
            currentInfo.ForEach(x => toevoegenaandb(x.time, x.name, x.temp, x.humidity, x.speed, x.land));
            MessageBox.Show("Data toegevoegd aan de databank :)");
        }
    }


    public class WeatherInfo
    {
        public DateTime time;
        public string name;
        public double speed;
        public string country;

        public WeatherInfo(DateTime time, string name, double temp, double humidity, double speed, string country)
        {
            this.time = time;
            this.name = name;
            this.temp = temp;
            this.humidity = humidity;
            this.speed = speed;
            this.country = country;
        }

        public string stad { get; }
        public double temp { get; }
        public double humidity { get; }
        public double wind { get; }
        public string land { get; }
    }

}
