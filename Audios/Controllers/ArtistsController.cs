using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Audios.Data;
using Audios.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Amazon.S3;
using Amazon;
using Amazon.S3.Transfer;

namespace Audios.Controllers
{
    [Authorize]
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Artists
        public async Task<IActionResult> Index()
        {
            return View(await _context.Artist.ToListAsync());
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImageUrl")] Artist artist, IFormFile file)
        {
            var artists = await _context.Artist.ToListAsync();
            Artist artistMatch = null;
            artistMatch = artists.FirstOrDefault(a => a.Name == artist.Name);

            if (artistMatch == null)
            {
                ModelState.Remove("ImageUrl");
                if (ModelState.IsValid)
                {
                    if (file == null)
                    {
                        artist.ImageUrl = "/images/default-image.png";
                    }
                    else
                    {
                        var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "images", file.FileName);
                        artist.ImageUrl = "/images/" + file.FileName;

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        //AWS
                        //try
                        //{
                        //    await UploadFileToS3(file);
                        //}
                        //catch(Exception ex)
                        //{
                        //    return NotFound();
                        //}

                        //artist.ImageUrl = $"https://{BucketInfo.Bucket}.s3.us-east-2.amazonaws.com/{file.FileName}";
                    }


                    _context.Add(artist);
                    await _context.SaveChangesAsync();

                    int id = artist.Id;
                    return RedirectToAction("Create", "Songs", new { Id = id });
                }
            }

            int matchId = artistMatch.Id;
            return RedirectToAction("Create", "Songs", new { Id = matchId });
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImageUrl")] Artist artist, IFormFile file)
        {
            if (id != artist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        var path = Path.Combine(
                        Directory.GetCurrentDirectory(), "wwwroot",
                        "images", file.FileName);
                        artist.ImageUrl = "/images/" + file.FileName;

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                    }

                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.Id))
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
            return View(artist);
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artist
                .FirstOrDefaultAsync(m => m.Id == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artist.FindAsync(id);
            _context.Artist.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArtistExists(int id)
        {
            return _context.Artist.Any(e => e.Id == id);
        }

        //public async Task UploadFileToS3(IFormFile file)
        //{
        //    using (var client = new AmazonS3Client(BucketInfo.AWSKey, BucketInfo.AWSSKey, RegionEndpoint.USEast2))
        //    {
        //        using (var newMemoryStream = new MemoryStream())
        //        {
        //            file.CopyTo(newMemoryStream);
        //            var uploadRequest = new TransferUtilityUploadRequest
        //            {
        //                InputStream = newMemoryStream,
        //                Key = file.FileName,
        //                BucketName = BucketInfo.Bucket,
        //                CannedACL = S3CannedACL.PublicRead
        //            };
        //            var fileTransferUtility = new TransferUtility(client);
        //            await fileTransferUtility.UploadAsync(uploadRequest);
        //        }
        //    }
        //}
    }
}
