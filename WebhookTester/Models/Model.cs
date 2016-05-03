using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace WebhookTester.Models
{
    public class Website
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Url { get; set; }
        [Required]
        public DateTime LastDeliveryDateUtc { get; set; }

        public ICollection<Delivery> Deliveries { get; set; }
    }

    public class Delivery
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime DeliveryDateUtc { get; set; }
        [Required]
        public string Data { get; set; }

        [ForeignKey("Website")]
        public int WebsiteId { get; set; }
        public Website Website { get; set; }
    }

    public class WebhookTesterDatabaseContext : DbContext
    {
        public WebhookTesterDatabaseContext()
        {
            Configuration.LazyLoadingEnabled = true;
        }

        public DbSet<Website> Websites { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }
    }
}