using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DeliveryAgentManagementApp.Data;
using DeliveryAgentManagementApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.NetworkInformation;
using System.Collections;
using Microsoft.AspNetCore.Authorization;

namespace DeliveryAgentManagementApp.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DeliveryAgentManagementAppContext _context;

        public OrdersController(DeliveryAgentManagementAppContext context)
        {
            _context = context;
            
        }

        // GET: Orders

        [Authorize]
        public async Task<IActionResult> Index()
        {
            
            string userName = User.Identity.Name;
            ViewBag.UserName = userName;
            ApplicationUser user = _context.Users.FirstOrDefault(m => m.UserName == userName);
            var deliveryAgentManagementAppContext = _context.Order.Include(o => o.Courier).OrderBy(m => m.DeliveryStatus).ThenByDescending(m => m.Id);
            if (User.IsInRole("courier")) {
                Courier courier = _context.Courier.FirstOrDefault(m => m.UserId == user.Id);
                deliveryAgentManagementAppContext = _context.Order.Where(o => o.CourierId == courier.UserId).Include(o => o.Courier).OrderByDescending(m => m.DeliveryStatus).ThenByDescending(m => m.Id);
            }
            
            return View(await deliveryAgentManagementAppContext.ToListAsync());
        }

        // GET: Orders/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            ViewData["CourierId"] = new SelectList(_context.Courier, "UserId", "UserId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Create([Bind("Id,Product,CustomerName,Destination,TotalPrice,Retailer")] Order order)
        {
            if (ModelState.IsValid)
            {
                order.CourierId = null;
                order.Courier = null;
                order.DateOrdered = DateTime.Now;
                order.DeliveryStatus = Status.PENDING;
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourierId"] = new SelectList(_context.Courier, "UserId", "UserId", order.CourierId);
            return View(order);
        }

        // GET: Orders/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["CourierId"] = new SelectList(_context.Courier, "Id", "Id", order.CourierId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Product,CustomerName,Destination,TotalPrice,Retailer")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["CourierId"] = new SelectList(_context.Courier, "Id", "Id", order.CourierId);
            return View(order);
        }

        // GET: Orders/Delete/5
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Order == null)
            {
                return NotFound();
            }

            var order = await _context.Order
                .Include(o => o.Courier)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Order == null)
            {
                return Problem("Entity set 'DeliveryAgentManagementAppContext.Order'  is null.");
            }
            var order = await _context.Order.FindAsync(id);
            if (order != null)
            {
                _context.Order.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.Id == id);
        }

        //SELFMAKING FUNCTIONS
        [Authorize(Roles ="manager")]
        public IActionResult SendToCourierList(int id)
        {

            HttpContext.Session.SetInt32("OrderDeliveringId", id);
            return RedirectToAction("Index", "Couriers");
        }
        [Authorize]
        public async Task<IActionResult> CancelDelivery(int? id)
        {
            Order order = await _context.Order.FindAsync(id);
            Status status = Status.CANCELED;
            order.DeliveryStatus = status;
            order.CourierId = null;
            order.Courier = null;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { _context.Order });
        }

        [Authorize(Roles = "courier")]
        public async Task<IActionResult> CheckDelivered(int? id)
        {
            Order order = await _context.Order.FindAsync(id);
            order.DeliveryStatus = Status.DELIVERED;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { _context.Order });
        }

        [Authorize(Roles = "courier")]
        public async Task<IActionResult> Reschedule(int? id)
        {
            Order order = await _context.Order.FindAsync(id);

            Message message = new Message();

            message.CourierId = (int) order.CourierId;
            message.Courier = order.Courier;
            message.OrderId = order.Id;
            message.Order = order;
            message.DateSent = DateTime.Now;
            message.Content = "The Order ID " + order.Id + " has been postponed and need to be rescheduled";

            _context.Add(message);
            await _context.SaveChangesAsync();

            order.DeliveryStatus = Status.EXCEPTION;
            order.CourierId = null;
            order.Courier = null;

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Orders", _context.Order);
        }

        [Authorize(Roles = "manager")]
        public IActionResult ShowMessages(int? id)
        {
            return RedirectToAction("Index", "Messages", new { id });
        }
    }

    
}
