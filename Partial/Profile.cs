using System.ComponentModel.DataAnnotations.Schema;
using API.Profile;

namespace API.Database
{
    public partial class Profile
    {
        [NotMapped] public Store.Store Store { get; set; }
        [NotMapped] public DeathModel DeathModel { get; set; } = new DeathModel();
        [NotMapped] public HealthParams HealthParams { get; set; }

        public void Save()
        {
            using var context = new EntryContext();
            context.Update(this);
            context.SaveChanges();
        }
    }
}