namespace WebhookTester.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<WebhookTester.Models.WebhookTesterDatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(WebhookTester.Models.WebhookTesterDatabaseContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //context.Websites.AddOrUpdate(
            //  w => w.Id,
            //  new Website { Id = 1, Url = "http://www.aaa.com" },
            //  new Website { Id = 2, Url = "http://www.bbb.com" },
            //  new Website { Id = 3, Url = "http://www.ccc.com" }
            //);

            //context.Deliveries.AddOrUpdate(
            //    d => d.Data,
            //    new Delivery { WebsiteId = 1, Timestamp = DateTime.UtcNow.AddMinutes(-2), Data = "Test1" },
            //    new Delivery { WebsiteId = 1, Timestamp = DateTime.UtcNow.AddMinutes(-1), Data = "Test2" },
            //    new Delivery { WebsiteId = 1, Timestamp = DateTime.UtcNow.AddMinutes(-1), Data = "Test3" }
            //    );            //
        }
    }
}
