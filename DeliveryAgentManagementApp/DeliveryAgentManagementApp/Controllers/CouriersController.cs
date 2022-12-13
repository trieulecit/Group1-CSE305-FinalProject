using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryAgentManagementApp.Data;
using DeliveryAgentManagementApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace DeliveryAgentManagementApp.Controllers
{

    public class CouriersController : Controller
    {
        private readonly DeliveryAgentManagementAppContext _context;

        public CouriersController(DeliveryAgentManagementAppContext context)
        {
            _context = context;
        }

        // GET: Couriers
        public async Task<IActionResult> Index()
        {
            string userName = User.Identity.Name;
            ViewBag.UserName = userName;
            ApplicationUser user = _context.Users.FirstOrDefault(m => m.UserName == userName);
            var context = await _context.Courier.ToListAsync();

            if (User.IsInRole("courier"))
            {
                Courier courier = _context.Courier.FirstOrDefault(m => m.UserId == user.Id);
                context = await _context.Courier.Where(o => o.UserId == courier.UserId).ToListAsync();
            }
            return View(context);
        }

        // GET: Couriers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Courier == null)
            {
                return NotFound();
            }

            var courier = await _context.Courier
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (courier == null)
            {
                return NotFound();
            }

            return View(courier);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Couriers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Name,ShippingFee")] Courier courier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(courier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(courier);
        }

        // GET: Couriers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Courier == null)
            {
                return NotFound();
            }

            var courier = await _context.Courier.FindAsync(id);
            if (courier == null)
            {
                return NotFound();
            }
            return View(courier);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Name,ShippingFee")] Courier courier)
        {
            if (id != courier.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourierExists(courier.UserId))
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
            return View(courier);
        }

        // GET: Couriers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courier == null)
            {
                return NotFound();
            }

            var courier = await _context.Courier
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (courier == null)
            {
                return NotFound();
            }

            return View(courier);
        }

        // POST: Couriers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Courier == null)
            {
                return Problem("Entity set 'DeliveryAgentManagementAppContext.Courier'  is null.");
            }
            var courier = await _context.Courier.FindAsync(id);
            if (courier != null)
            {
                _context.Courier.Remove(courier);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourierExists(int id)
        {
          return _context.Courier.Any(e => e.UserId == id);
        }

        //SELFMAKING
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Choose(int? id)
        {
 
            var orderId = HttpContext.Session.GetInt32("OrderDeliveringId");
            if(orderId == null || orderId == 0)
            {
                return RedirectToAction("AccessDenied", "Accounts");
            }
            Order order = await _context.Order.FindAsync(orderId);
            order.CourierId = id;
            order.Courier = await _context.Courier.FindAsync(id);

            Status status = Status.OUTFORDELIVERY;
            order.DeliveryStatus = status;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Orders");
        }

        
    }
}
