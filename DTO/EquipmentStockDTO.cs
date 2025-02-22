using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sp_backend.DTO
{
    public class EquipmentStockDTO
    {
        public int Id { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}