using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using sp_backend.Models;

namespace sp_backend.DTO
{
    public class ItemDTO
    {
        public int Item_number { get; set; }
        public string Type { get; set; }
        public int Order_number { get; set; }
        public int Product_group { get; set; }
        public int Packing_quantity { get; set; }
        public int Packing_unit { get; set; }
        public DateTime End_of_repair { get; set; }
        public int MaintenanceId { get; set; }
    }

}