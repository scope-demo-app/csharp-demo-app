using System;
using System.ComponentModel.DataAnnotations;

namespace csharp_demo_app
{
    public class ImagesEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public string ContentType { get; set; }
        public byte[] ContentData { get; set; }
    }
}