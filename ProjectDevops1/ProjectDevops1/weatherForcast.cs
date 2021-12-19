using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectDevops1
{
    class weatherForcast
    {   
        public city city { get; set; }
        public List<list> list { get; set; }

    }
    public class temp
    {
        public double day { get; set; }
       
    }
    public class main
    {
        public double temp { get; set; }
        public double pressure { get; set; }
        public double humidity { get; set; }
        public double temp_min { get; set; }
        public double temp_max { get; set; }

    }
    public class weather
    {
        public string main { get; set; }
        public string description { get; set; }    
    }
    public class city
    {
        public string name { get; set; }

    }
    public class list
    {
        public double dt { get; set; } //milliseconds dagen
        public double pressure { get; set; } 
        public double humidity { get; set; } 
        public double speed { get; set; } 
         

    }



}





