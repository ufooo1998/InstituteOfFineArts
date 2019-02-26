using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InstituteOfFineArts.Models;

namespace InstituteOfFineArts.Controllers
{
    public class CompetitionPostsController : Controller
    {
        private readonly InstituteOfFineArtsContext _context;

        public CompetitionPostsController(InstituteOfFineArtsContext context)
        {
            _context = context;
        }

        // GET: CompetitionPosts
        public async Task<IActionResult> Index()
        {
            var instituteOfFineArtsContext = _context.CompetitionPost.Include(c => c.Competition).Include(c => c.Post);
            return View(await instituteOfFineArtsContext.ToListAsync());
        }

        // GET: CompetitionPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competitionPost = await _context.CompetitionPost
                .Include(c => c.Competition)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (competitionPost == null)
            {
                return NotFound();
            }

            return View(competitionPost);
        }

        // GET: CompetitionPosts/Create
        public IActionResult Create()
        {
            ViewData["CompetitionID"] = new SelectList(_context.Competition, "ID", "ID");
            ViewData["PostID"] = new SelectList(_context.Post, "ID", "ID");
            return View();
        }

        // POST: CompetitionPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,CompetitionID,PostID,PostPoint")] CompetitionPost competitionPost)
        {
            if (ModelState.IsValid)
            {
                _context.Add(competitionPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CompetitionID"] = new SelectList(_context.Competition, "ID", "ID", competitionPost.CompetitionID);
            ViewData["PostID"] = new SelectList(_context.Post, "ID", "ID", competitionPost.PostID);
            return View(competitionPost);
        }

        // GET: CompetitionPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competitionPost = await _context.CompetitionPost.FindAsync(id);
            if (competitionPost == null)
            {
                return NotFound();
            }
            ViewData["CompetitionID"] = new SelectList(_context.Competition, "ID", "ID", competitionPost.CompetitionID);
            ViewData["PostID"] = new SelectList(_context.Post, "ID", "ID", competitionPost.PostID);
            return View(competitionPost);
        }

        // POST: CompetitionPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,CompetitionID,PostID,PostPoint")] CompetitionPost competitionPost)
        {
            if (id != competitionPost.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(competitionPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompetitionPostExists(competitionPost.ID))
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
            ViewData["CompetitionID"] = new SelectList(_context.Competition, "ID", "ID", competitionPost.CompetitionID);
            ViewData["PostID"] = new SelectList(_context.Post, "ID", "ID", competitionPost.PostID);
            return View(competitionPost);
        }

        // GET: CompetitionPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var competitionPost = await _context.CompetitionPost
                .Include(c => c.Competition)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (competitionPost == null)
            {
                return NotFound();
            }

            return View(competitionPost);
        }

        // POST: CompetitionPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var competitionPost = await _context.CompetitionPost.FindAsync(id);
            _context.CompetitionPost.Remove(competitionPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompetitionPostExists(int id)
        {
            return _context.CompetitionPost.Any(e => e.ID == id);
        }
    }
}
