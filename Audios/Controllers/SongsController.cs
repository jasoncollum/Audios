using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Microsoft.AspNetCore.Http;
using Audios.Data;
using Audios.Models;
using Microsoft.AspNetCore.Authorization;

namespace Audios.Controllers
{
    [Authorize]
    public class SongsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SongsController(
            ApplicationDbContext context
            )
        {
            _context = context;

        }

        // GET: Songs
        public async Task<IActionResult> Index(string searchInput, bool exists, string message)
        {
            //List search results
            if (!String.IsNullOrEmpty(searchInput))
            {
                var songs = _context.Song
                            .Include(s => s.Artist)
                            .Include(s => s.Vocal);

                if (searchInput.ToLower() == "male")
                {
                    var maleVocalSongs = songs.Where(s => s.Vocal.Type == "Male");
                    return View(await maleVocalSongs.ToListAsync());
                }

                if (searchInput.ToLower() == "male/female" || searchInput.ToLower() == "male female")
                {
                    var maleFemaleVocalSongs = songs.Where(s => s.Vocal.Type == "Male/Female");
                    if (maleFemaleVocalSongs.Any())
                    {
                        return View(await maleFemaleVocalSongs.ToListAsync());
                    }
                    else
                    {
                        ViewBag.noMaleFemaleVocals = true;
                        ViewBag.mfMessage = "There are currently no songs that feature both Male and Female vocals in the Audios database";
                        return View(await songs.ToListAsync());
                    }
                }

                if (searchInput == "instrumental" || searchInput == "instrumentals" || searchInput == "inst")
                {
                    var instrumentalSongs = songs.Where(s => s.Vocal.Type == "None");
                    if (instrumentalSongs.Any())
                    {
                        return View(await instrumentalSongs.ToListAsync());
                    }
                    else
                    {
                        ViewBag.noInstrumentals = true;
                        ViewBag.instMessage = "There are currently no Instrumentals in the Audios database";
                        return View(await songs.ToListAsync());
                    }
                }

                var filteredSongs = songs.Where(s => s.Title.Contains(searchInput) 
                                                  || s.Artist.Name.Contains(searchInput)
                                                  || s.Vocal.Type.Contains(searchInput)
                                                  || s.SearchWords.Contains(searchInput));

                //ViewData["Playlist"] = new SelectList(_context.Playlist, "Id", "Name");
                return View(await filteredSongs.ToListAsync());
            }
            else
            {
                // List songs
                var songs = _context.Song
                .Include(s => s.Artist)
                .Include(s => s.Vocal);

                ViewData["exists"] = exists;
                ViewData["message"] = message;

                var pl = _context.Playlist;
                ViewData["Playlist"] = new SelectList(_context.Playlist, "Id", "Name");
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
        public IActionResult Create(int Id)
        {
            var song = new Song() { ArtistId = Id };
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "Name", "ImageUrl");
            ViewData["VocalId"] = new SelectList(_context.Vocal, "Id", "Type");
            return View(song);
        }

        // POST: Songs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Bpm,SearchWords,Lyrics,isOneStop,AudioUrl,ImageUrl,VocalId,ArtistId")]Song song, IFormFile file)
        {

            if (_context.Song.Any(s => s.Title == song.Title && s.ArtistId == song.ArtistId))
            {
                Artist Artist = _context.Artist.FirstOrDefault(a => a.Id == song.ArtistId);
                //bool exists = true;
                //string message = $"{song.Title} by {Artist.Name} is already in Audios";
                return RedirectToAction("Index", "Songs", new { exists = true, message = $"{song.Title} by {Artist.Name} is already in Audios" } );
            }
            else
            {
                var path = Path.Combine(
                  Directory.GetCurrentDirectory(), "wwwroot",
                  "audio-data", file.FileName);

                song.AudioUrl = "/audio-data/" + file.FileName;
                ModelState.Remove("AudioUrl");

                if (ModelState.IsValid)
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _context.Add(song);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["VocalId"] = new SelectList(_context.Vocal, "Id", "Type", song.VocalId);
                    return View(song);
                }
            }
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
            ViewData["ArtistId"] = new SelectList(_context.Artist, "Id", "Name", song.ArtistId);
            ViewData["VocalId"] = new SelectList(_context.Vocal, "Id", "Type", song.VocalId);
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
