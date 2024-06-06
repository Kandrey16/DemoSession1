using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ООО_Ткани_Сессия1_2.Models
{
    internal class Product
    {
        public int ID_PRODUCT { get; set; }
        public string NAME_PRODUCT { get; set; }
        public string DESCRIPTION_PRODUCT { get; set; }
        public string CREATOR_PRODUCT { get; set; }
        public float PRICE_PRODUCT { get; set; }
        public string IMG_PRODUCT { get; set; }
        public Boolean IS_ENABLED { get; set; }

        public string Is_EnabledText => IS_ENABLED ? "В наличии" : "Нет в наличии";

    }
}
