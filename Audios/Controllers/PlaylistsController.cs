using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Audios.Data;
using Audios.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Audios.Controllers
{
    //[Authorize]
    public class PlaylistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlaylistsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        [Authorize]
        // GET: Playlists
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Playlist.Include(p => p.ApplicationUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Playlists/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            var playlistSongs = await _context.PlaylistSong
                .Include(ps => ps.Song).ThenInclude(s => s.Artist)
                .Include(ps => ps.Playlist)
                .Where(ps => ps.PlaylistId == id).ToListAsync();

            var artists = new List<Artist>();
            
            foreach(var ps in playlistSongs)
            {
                artists = await _context.Artist.Where(a => a.Id == ps.Song.ArtistId).ToListAsync();
            }

            

            if (playlist == null)
            {
                return NotFound();
            }

            ViewBag.Playlist = playlist;
            return View(playlistSongs);
        }

        // GET: Playlists/Create
        [Authorize]
        public IActionResult Create()
        {
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: Playlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("Id,Name,ApplicationUserId")] Playlist playlist)
        {
            var user = await GetCurrentUserAsync();

            ModelState.Remove("UserId");
            if (ModelState.IsValid)
            {
                _context.Add(playlist);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
          
            return RedirectToAction("Index", "Songs");
        }

        // GET: Playlists/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist.FindAsync(id);
            if (playlist == null)
            {
                return NotFound();
            }
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", playlist.ApplicationUserId);
            return View(playlist);
        }

        // POST: Playlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ApplicationUserId")] Playlist playlist)
        {
            if (id != playlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaylistExists(playlist.Id))
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
            ViewData["ApplicationUserId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", playlist.ApplicationUserId);
            return View(playlist);
        }

        // GET: Playlists/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playlist == null)
            {
                return NotFound();
            }

            return View(playlist);
        }

        // POST: Playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playlist = await _context.Playlist.FindAsync(id);
            _context.Playlist.Remove(playlist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Playlists/AddToPlaylist/5, 13
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
            //CREATE NEW PLAYLIST SONG...
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> AddToPlaylist(string playlistId, int songId)
        {
            int playlist_id = Int32.Parse(playlistId);
            var playlistSongs = _context.PlaylistSong.Where(ps => ps.PlaylistId == playlist_id);
            int trackNum = playlistSongs.Count() + 1;

            var newPlaylistSong = new PlaylistSong()
            {
                PlaylistId = playlist_id,
                SongId = songId,
                TrackNumber = trackNum
            };

            _context.Add(newPlaylistSong);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Songs");
        }

        //UPDATE PLAYLISTSONG TRACKNUMBER...
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UpdateTrackNumbers(int playlistSongId)
        //{
        //var playlistSongs = _context.PlaylistSong.Where(ps => ps.Id == playlistSongId);
        //int trackNum = playlistSongs.Count() + 1;

        //var newPlaylistSong = new PlaylistSong()
        //{
        //    PlaylistId = playlistId,
        //    SongId = songId,
        //    TrackNumber = trackNum
        //};

        //_context.(newPlaylistSong);
        //await _context.SaveChangesAsync();
        //    return RedirectToAction("Index", "Songs");
        //}

        // POST: Playlists/RemovePlaylistSong/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> RemovePlaylistSong(int id)
        {
            var playlistSong = await _context.PlaylistSong.FindAsync(id);
            int detailId = playlistSong.PlaylistId;
            _context.PlaylistSong.Remove(playlistSong);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = detailId } );
        }

        // GET: Playlists/Share/5
        public async Task<IActionResult> Share(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playlist = await _context.Playlist
                .Include(p => p.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            var playlistSongs = await _context.PlaylistSong
                .Include(ps => ps.Song).ThenInclude(s => s.Artist)
                .Include(ps => ps.Playlist)
                .Where(ps => ps.PlaylistId == id).ToListAsync();

            var artists = new List<Artist>();

            foreach (var ps in playlistSongs)
            {
                artists = await _context.Artist.Where(a => a.Id == ps.Song.ArtistId).ToListAsync();
            }



            if (playlist == null)
            {
                return NotFound();
            }

            ViewBag.Playlist = playlist;
            return View(playlistSongs);
        }
        [Authorize]
        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
        [Authorize]
        private bool PlaylistExists(int id)
        {
            return _context.Playlist.Any(e => e.Id == id);
        }
    }
}
