using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherImageGenerator.Data.Configuration
{
    public class UnsplashSettings
    {
        public string AccessKey { get; set; } = string.Empty;
        public string BaseAddress { get; set; } = "https://api.unsplash.com/";
    }
}
