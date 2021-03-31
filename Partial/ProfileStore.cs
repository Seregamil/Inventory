using System.ComponentModel.DataAnnotations.Schema;

namespace API.Database
{
    public partial class ProfileStore
    {
        [NotMapped]
        public object Object { get; set; }

        public void Save()
        {
            using var context = new EntryContext();
            context.Update(this);
            context.SaveChanges();
        }
    }
}