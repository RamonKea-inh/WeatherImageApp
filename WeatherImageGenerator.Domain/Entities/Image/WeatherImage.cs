using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherImageGenerator.Domain.Entities.Image
{
    public class WeatherImage
    {
        public Guid Id { get; set; }
        public byte[] ImageData { get; set; }
        public string BlobUrl { get; set; }
    }
}
