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
    public class MessagesController : Controller
    {
        private readonly DeliveryAgentManagementAppContext _context;

        public MessagesController(DeliveryAgentManagementAppContext context)
        {
            _context = context;
        }

        // GET: Messages
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Index(int id)
        {
            var deliveryAgentManagementAppContext = _context.Message.Include(m => m.Courier).Include(m => m.Order).Where(m => m.OrderId == id);
            return View(await deliveryAgentManagementAppContext.ToListAsync());
        }

        // GET: Messages/Details/5
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Courier)
                .Include(m => m.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // GET: Messages/Create
        [Authorize(Roles = "manager")]
        public IActionResult Create()
        {
            ViewData["CourierId"] = new SelectList(_context.Courier, "Id", "Id");
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Create([Bind("Id,CourierId,OrderId,Content,DateSent")] Message message)
        {
            if (ModelState.IsValid)
            {
                _context.Add(message);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourierId"] = new SelectList(_context.Courier, "Id", "Id", message.CourierId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", message.OrderId);
            return View(message);
        }

        // GET: Messages/Edit/5
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }
            ViewData["CourierId"] = new SelectList(_context.Courier, "Id", "Id", message.CourierId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", message.OrderId);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourierId,OrderId,Content,DateSent")] Message message)
        {
            if (id != message.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(message);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MessageExists(message.Id))
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
            ViewData["CourierId"] = new SelectList(_context.Courier, "Id", "Id", message.CourierId);
            ViewData["OrderId"] = new SelectList(_context.Order, "Id", "Id", message.OrderId);
            return View(message);
        }

        // GET: Messages/Delete/5
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Message == null)
            {
                return NotFound();
            }

            var message = await _context.Message
                .Include(m => m.Courier)
                .Include(m => m.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (message == null)
            {
                return NotFound();
            }

            return View(message);
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Message == null)
            {
                return Problem("Entity set 'DeliveryAgentManagementAppContext.Message'  is null.");
            }
            var message = await _context.Message.FindAsync(id);
            if (message != null)
            {
                _context.Message.Remove(message);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MessageExists(int id)
        {
          return _context.Message.Any(e => e.Id == id);
        }
    }
}
