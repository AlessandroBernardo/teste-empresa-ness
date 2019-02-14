using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                    double destLat = Convert.ToDouble(item.Latitude);
                    double destLong = Convert.ToDouble(item.Longitude);

                    var dist = await CalcDist(origLat, origLong, destLat, destLong);
                    var adress = await GetAdressByLatLong(destLat, destLong);

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


        public async Task<double> CalcDist(double origLat, double origLong, double destLat, double destLong)
        {
            double x1 = origLat;
            double x2 = origLong;
            double y1 = destLat;
            double y2 = destLong;

            // Distancia entre os 2 pontos no plano cartesiano ( pitagoras )
            //double distancia = System.Math.Sqrt( System.Math.Pow( (x2 - x1), 2 ) + System.Math.Pow( (y2 - y1), 2 ) );

            // ARCO AB = c 
            double c = 90 - (y2);

            // ARCO AC = b 
            double b = 90 - (y1);

            // Arco ABC = a 
            // Diferença das longitudes: 
            double a = x2 - x1;

            // Formula: cos(a) = cos(b) * cos(c) + sen(b)* sen(c) * cos(A) 
            double cos_a = Math.Cos(b) * Math.Cos(c) + Math.Sin(c) * Math.Sin(b) * Math.Cos(a);

            double arc_cos = Math.Acos(cos_a);

            // 2 * pi * Raio da Terra = 6,28 * 6.371 = 40.030 Km 
            // 360 graus = 40.030 Km 
            // 3,2169287 = x 
            // x = (40.030 * 3,2169287)/360 = 357,68 Km 

            double distancia = (40030 * arc_cos) / 360;

            return distancia;
        }

        [HttpGet]
        public async Task<JsonResult> GetAdressByLatLong(double lat, double longt)
        {
            var key = "AIzaSyCouprQQ2PQ2d8Rb1F9Q7Qyf5FwAjpJJBs";

            //Adress _adress;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://maps.googleapis.com/maps/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync("api/geocode/json?latlng=" + lat.ToString().Replace(",", ".") + "," + longt.ToString().Replace(",", ".") + "&key=" + key);
                if (response.IsSuccessStatusCode)
                {  //GET
                    var adress = response.Content.ReadAsStringAsync().Result;
                    //var _adress = JObject.Parse(adress).ToString();
                    var adrs = JsonConvert.DeserializeObject<Adress>(adress);

                    var endereco = adrs.results.Select(x => x.formatted_address).FirstOrDefault();

                    return Json(endereco);
                }
                else
                {
                    return Json("");
                }
            }
        }


        //public async Task<JsonResult> GetAdressByLatLong(double lat, double longt)
        //{

        //    var api = "https://maps.googleapis.com/maps/api/geocode/json?latlng=";
        //    var key = "AIzaSyCouprQQ2PQ2d8Rb1F9Q7Qyf5FwAjpJJBs";


        //    try
        //    {
        //        var adress =  api + lat + "," + longt + "&" + key;
        //        if (adress != null)
        //        {
        //            Adress _adress = adress;
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //    return Json("");
        //}


        // GET: Users/Details/5
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

                Convert.ToDouble(user.Latitude.ToString().Insert(3, ","));
                user.Longitude.ToString().Insert(3, ",");

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
