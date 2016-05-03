using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebhookTester.Models;

namespace WebhookTester.Controllers
{
	public class HomeController : Controller
	{
		private readonly WebhookTesterDatabaseContext db = new WebhookTesterDatabaseContext();

		//
		// GET: /Home/
		[AllowAnonymous]
		public ActionResult Index()
		{
			return View(db.Websites.ToList());
		}

		[AllowAnonymous]
		public ActionResult Details(int id)
		{
			var site = db.Websites.FirstOrDefault(w => w.Id == id);
			if (site == null)
			{
				return HttpNotFound();
			}
			site.Deliveries = db.Deliveries.Where(d => d.WebsiteId == site.Id).OrderByDescending(d => d.DeliveryDateUtc).ToList();
			return View(site);
		}

		[HttpPost]
		[AllowAnonymous]
		[ActionName("Index")]
		public ActionResult Callback()
		{
			try
			{
				string url = HttpContext.Request.Headers["X-Telligent-Webhook-Sender"];
				if (url == null)
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Missing required header 'X-Telligent-Webhook-Sender'.");

				string signature = HttpContext.Request.Headers["X-Telligent-Webhook-Signature"];

				var now = DateTime.UtcNow;

				string delivery;
				string raw;
				using (var reader = new System.IO.StreamReader(HttpContext.Request.InputStream))
				{
					raw = reader.ReadToEnd();
				}
				delivery = HttpUtility.UrlDecode(raw);

				var secretKey = System.Web.Configuration.WebConfigurationManager.AppSettings["webhookSecret"];
				string calculatedSignature = CalculateSignature(raw, secretKey);

				delivery = delivery + "\r\nSignatures:";
				delivery = delivery + "\r\n  From header : " + signature;
				delivery = delivery + "\r\n  Calculated  : " + calculatedSignature;

				var site = db.Websites.FirstOrDefault(w => w.Url == url);
				if (site != null)
				{
					site.LastDeliveryDateUtc = now;
					db.Deliveries.Add(new Delivery {WebsiteId = site.Id, DeliveryDateUtc = now, Data = delivery});
				}
				else
				{
					var newSite = db.Websites.Add(new Website {Url = url, LastDeliveryDateUtc = now});
					db.Deliveries.Add(new Delivery {WebsiteId = newSite.Id, DeliveryDateUtc = now, Data = delivery});
				}
				db.SaveChanges();

				return new HttpStatusCodeResult(200);
			}
			catch (Exception ex)
			{
				return new HttpStatusCodeResult(HttpStatusCode.Conflict, ex.ToString());
			}
		}

		[AllowAnonymous]
		public ActionResult Delete(int id)
		{
			var site = db.Websites.FirstOrDefault(w => w.Id == id);
			if (site == null)
			{
				return HttpNotFound();
			}

			db.Deliveries.RemoveRange(db.Deliveries.Where(d => d.WebsiteId == site.Id));
			db.Websites.Remove(site);
			db.SaveChanges();

			return Index();
		}

		private string CalculateSignature(string data, string secret)
		{
			var encoding = new System.Text.UTF8Encoding();
			byte[] secretByte = encoding.GetBytes(secret);
			HMACSHA256 hmacsha256 = new HMACSHA256(secretByte);

			byte[] dataBytes = encoding.GetBytes(data);
			byte[] dataHash = hmacsha256.ComputeHash(dataBytes);

			return Convert.ToBase64String(dataHash);
		}
	}
}
