using System;
using System.Collections.Generic;

#nullable disable

namespace API.Database
{
    public partial class ProfileStore
    {
        public int Id { get; set; }
        public int ProfileId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public double Weight { get; set; }
        public int Type { get; set; }
        public string ObjectData { get; set; }
        public int? EntityId { get; set; }
        public int? EntityType { get; set; }
        public int StorageId { get; set; }
        public int SlotId { get; set; }

        public virtual Profile Profile { get; set; }
    }
}
