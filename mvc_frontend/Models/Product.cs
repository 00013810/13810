using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mvc_frontend.Models
{
    public class Product
    {
        // type of data for Product 
        public int ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public Category ProductCategory { get; set; }
    }
}