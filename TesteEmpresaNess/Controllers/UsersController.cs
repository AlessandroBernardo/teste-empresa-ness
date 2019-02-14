using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TesteNess.Models;

namespace TesteNess.Controllers
{
    public class UsersController : Controller
    {
        private readonly ClasseContexto _context;

        public UsersController(ClasseContexto context)
        {
            _context = context;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }


        [HttpGet]
        public async Task<IActionResult> GetLocFriends(double origLat, double origLong)
        {

            try
            {
                List<User> lstUsers = await _context.Users.ToListAsync();
                if (lstUsers == null)
                    return null;

                var lstDist = new List<UserDist>();

                foreach (var item in lstUsers)
                {
                    if (origLat.ToString() == item.Latitude)
                    {
                        origLat = origLat - 00.000001;
                    }

                    string destLat = item.Latitude;
                    string destLong = item.Longitude;

                    double dist = CalcDistance(origLat, origLong, destLat, destLong);
                    var adress = await GetAdressByLatLong(destLat.ToString(), destLong.ToString());

                    var userDist = new UserDist
                    {
                        Nome = item.Name,
                        Distancia = Convert.ToDouble(dist.ToString().Substring(0, 3)),
                        Endereco = adress.ToString()
                    };

                    lstDist.Add(userDist);
                }

                if (lstDist.Count < 3)
                {
                    return Json("Você tem poucos amigos! Vá fazer alguns depois retorne");
                }

                return View(lstDist.OrderBy(x => x.Distancia).Take(3));

            }
            catch (Exception ex)
            {
                return Json("");
            }

        }

        private double CalcDistance(double origem_lat, double origem_lng, string destino_lat, string destino_lng)
        {
            double x1 = origem_lat;
            string x2 = destino_lat;
            double y1 = origem_lng;
            string y2 = destino_lng;
                      
            double c = 90 - (Convert.ToDouble(y2));

            double b = 90 - (y1);
                        
            double a = Convert.ToDouble(x2) - x1;
                        
            double cos_a = Math.Cos(b) * Math.Cos(c) + Math.Sin(c) * Math.Sin(b) * Math.Cos(a);

            double arc_cos = Math.Acos(cos_a);

            double distancia = (40030 * arc_cos) / 360;

            return distancia;
        }     
               
        [HttpGet]
        public async Task<string> GetAdressByLatLong(string lat, string longt)
        {
            var key = "AIzaSyCouprQQ2PQ2d8Rb1F9Q7Qyf5FwAjpJJBs";

            //Adress _adress;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api/geocode/json?latlng=" + lat/*.ToString().Replace(",", ".")*/ + "," + longt/*.ToString().Replace(",", ".")*/ + "&key=" + key);
                if (response.IsSuccessStatusCode)
                {  
                    var adress = response.Content.ReadAsStringAsync().Result;
                    
                    var adrs = JsonConvert.DeserializeObject<Adress>(adress);

                    var endereco = adrs.results.Select(x => x.formatted_address).FirstOrDefault();

                    return endereco;                     
                }
                else
                {
                    return "";
                }
            }
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Name,Principal,Latitude,Longitude")] User user)
        {
            if (ModelState.IsValid)
            {
                //Convert.ToDouble(user.Latitude.ToString().Insert(3, ","));
                //user.Longitude.ToString().Insert(3, ",");

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,Principal,Longitude,Latitude")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        //public static void Main()
        //{
        //    var address = "Stavanger, Norway";

        //    var locationService = new GoogleLocationService();
        //    var point = locationService.GetLatLongFromAddress(address);

        //    var latitude = point.Latitude;
        //    var longitude = point.Longitude;

        //    Save lat/ long values to DB...
        //}
    }
}
