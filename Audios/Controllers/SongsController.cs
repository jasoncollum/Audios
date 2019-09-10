using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Audios.Data;
using Audios.Models;

namespace Audios.Controllers
{
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;
        //private readonly IHostingEnvironment _env;

        public SongsController(
            ApplicationDbContext context
            //UserManager<ApplicationUser> userManager,
            //IHostingEnvironment env
            )
        {
            _context = context;
            //_env = env;
            //_userManager = userManager;
        }
        //private readonly UserManager<ApplicationUser> _userManager;

        // GET: Songs
        public async Task<IActionResult> Index(string searchInput)
        {
            //List search results
            if (!String.IsNullOrEmpty(searchInput))
            {
                var songs = from s in _context.Song
                            .Include(s => s.Artist)
                            .Include(s => s.Vocal)
                               select s;

                var filteredSongs = songs.Where(s => s.Title.Contains(searchInput) 
                                                  || s.Artist.Name.Contains(searchInput)
                                                  || s.Vocal.Type.Contains(searchInput)
                                                  || s.SearchWords.Contains(searchInput));

                return View(await filteredSongs.ToListAsync());
            }
            else
            {
                // List songs
                var songs = _context.Song
                .Include(s => s.Artist)
                .Include(s => s.Vocal);
                return View(await songs.ToListAsync());
            }
        }

        // GET: Songs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Artist)
                .Include(s => s.Vocal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // GET: Songs/Create
        public IActionResult Create()
        {
            ViewData["VocalId"] = new SelectList(_context.Vocal, "Id", "Type");
            return View();
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Bpm,SearchWords,Lyrics,isOneStop,VocalId,ArtistId")] Song song)
        {
            //ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                _context.Add(song);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VocalId"] = new SelectList(_context.Vocal, "Id", "Type", song.VocalId);
            return View(song);
        }

        // GET: Songs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song.FindAsync(id);
            if (song == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "ImageUrl", song.ArtistId);
            return View(song);
        }

        // POST: Songs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Bpm,SearchWords,Lyrics,isOneStop,AudioUrl,VocalId,ArtistId")] Song song)
        {
            if (id != song.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(song);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SongExists(song.Id))
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
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "ImageUrl", song.ArtistId);
            return View(song);
        }

        // GET: Songs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var song = await _context.Song
                .Include(s => s.Artist)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (song == null)
            {
                return NotFound();
            }

            return View(song);
        }

        // POST: Songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var song = await _context.Song.FindAsync(id);
            _context.Song.Remove(song);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SongExists(int id)
        {
            return _context.Song.Any(e => e.Id == id);
        }
    }
}
