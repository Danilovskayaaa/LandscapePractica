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
    
    public partial class Equipment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Equipment()
        {
            this.EquipmentUsage = new HashSet<EquipmentUsage>();
        }
    
        public int IDEquipment { get; set; }
        public Nullable<int> IDTypeEquipment { get; set; }
        public Nullable<int> IDStatusEquipment { get; set; }
        public string Name { get; set; }
    
        public virtual StatusesEquipment StatusesEquipment { get; set; }
        public virtual TypeEquipment TypeEquipment { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EquipmentUsage> EquipmentUsage { get; set; }
    }
}
