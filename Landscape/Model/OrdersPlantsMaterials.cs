//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Landscape.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrdersPlantsMaterials
    {
        public int IDOrderPlantsMaterials { get; set; }
        public Nullable<int> IDOrder { get; set; }
        public Nullable<int> IDPlant { get; set; }
        public Nullable<int> IDMaterial { get; set; }
        public Nullable<int> QuantityPlant { get; set; }
        public Nullable<int> QuantityMaterial { get; set; }
    
        public virtual Materials Materials { get; set; }
        public virtual Orders Orders { get; set; }
        public virtual Plants Plants { get; set; }
    }
}
