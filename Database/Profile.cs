using System;
using System.Collections.Generic;

#nullable disable

namespace API.Database
{
    public partial class Profile
    {
        public Profile()
        {
            FriendProfiles = new HashSet<Friend>();
            FriendTargets = new HashSet<Friend>();
            ProfileFractions = new HashSet<ProfileFraction>();
            ProfileStores = new HashSet<ProfileStore>();
            Vehicles = new HashSet<Vehicle>();
        }

        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public decimal Money { get; set; }
        public decimal BankMoney { get; set; }
        public string CharacterData { get; set; }
        public int Dimension { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float Heading { get; set; }
        public int Level { get; set; }
        public int Exp { get; set; }
        public int Legend { get; set; }
        public int AccessLevel { get; set; }

        public virtual Account Account { get; set; }
        public virtual ICollection<Friend> FriendProfiles { get; set; }
        public virtual ICollection<Friend> FriendTargets { get; set; }
        public virtual ICollection<ProfileFraction> ProfileFractions { get; set; }
        public virtual ICollection<ProfileStore> ProfileStores { get; set; }
        public virtual ICollection<Vehicle> Vehicles { get; set; }
    }
}
