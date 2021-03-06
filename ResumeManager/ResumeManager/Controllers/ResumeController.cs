using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using ResumeManager.Data;
using ResumeManager.Models;

using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ResumeManager.Controllers
{
    public class ResumeController : Controller
    {
        private readonly ResumeDbContext _context;

        private readonly IWebHostEnvironment _webHost;


        public ResumeController(ResumeDbContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _webHost = webHost;

        }
        public IActionResult Index(string SearchText = "")
        {
            List<Applicant> applicants;

            if (SearchText != "" && SearchText != null)
            {
                applicants = _context.Applicants
                .Include(e => e.Experiences)
                .Where(a => a.Name.Contains(SearchText) || a.Qualification.Contains(SearchText) || a.Gender.Contains(SearchText))
                .ToList();
               
                 return View(applicants);    
            }
            else
            {
                applicants = _context.Applicants.ToList();
                return View(applicants);
            }
        }
        public IActionResult Loggers(string SearchText = "")
        {
            List<Applicant> applicants;

            if (SearchText != "" && SearchText != null)
            {
                applicants = _context.Applicants
                .Include(e => e.Experiences)
                .Where(a => a.Name.Contains(SearchText) || a.Qualification.Contains(SearchText) || a.Gender.Contains(SearchText))
                .ToList();

                return View(applicants);
            }
            else
            {
                applicants = _context.Applicants.ToList();
                return View(applicants);
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            Applicant applicant = new Applicant();
            applicant.Experiences.Add(new Experience() { ExperienceId = 1 });
        
            return View(applicant);
        }


        [HttpPost]
        public IActionResult Create(Applicant applicant)
        {
            applicant.Experiences.RemoveAll(n => n.YearsWorked == 0);

            string uniqueFileName = GetUploadedFileName(applicant);
            applicant.PhotoUrl = uniqueFileName;
            string unFileName=UploadedFileName(applicant);
            applicant.ResumeUrl = unFileName;

            _context.Add(applicant);
            _context.SaveChanges();
            return RedirectToAction("index");

        }


        private string GetUploadedFileName(Applicant applicant)
        {
            string uniqueFileName = null;

            if (applicant.ProfilePhoto != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + applicant.ProfilePhoto.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                { 
                    applicant.ProfilePhoto.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private string UploadedFileName(Applicant applicant)
        {
            string unFileName = null;

            if (applicant.ResumeFile != null)
            {
                string uploadsFolder = Path.Combine(_webHost.WebRootPath, "ResumeFile");
                unFileName = Guid.NewGuid().ToString() + "_" + applicant.ResumeFile.FileName;
                string filePath = Path.Combine(uploadsFolder, unFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    applicant.ResumeFile.CopyTo(fileStream);
                }
            }
            return unFileName;
        }


        public IActionResult Details(int Id)
        {
            Applicant applicant=_context.Applicants
                .Include(e =>  e.Experiences)
                .Where(a => a.Id==Id).FirstOrDefault();
           
            return View(applicant);
                
        }

        [HttpGet]
        public IActionResult Delete(int Id)
        {
            Applicant applicant = _context.Applicants
                .Include(e => e.Experiences)
                .Where(a => a.Id == Id).FirstOrDefault();

            return View(applicant);

        }
        [HttpPost]
        public IActionResult Delete(Applicant applicant)
        {
            _context.Attach(applicant);
            _context.Entry(applicant).State = EntityState.Deleted;

            _context.SaveChanges();

            return RedirectToAction("Loggers");
        }


     

        [HttpGet]
        public IActionResult Edit(int Id)
        {
            Applicant applicant = _context.Applicants
                .Include(e => e.Experiences)
                .Where(a => a.Id == Id).FirstOrDefault();

            return View(applicant);
            
        }
        [HttpPost]
        public IActionResult Edit(Applicant applicant)
        {
            
            _context.Entry(applicant).State = EntityState.Modified;

            
            _context.SaveChanges();

            return RedirectToAction("Loggers");
        }
        [Authorize]
        public IActionResult Secured()
        {
            return View();
        }
        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost("login")]
        public IActionResult Validate(string username, string password)
        {
            if (username == "bob" && password == "pizza")
            {
                return RedirectToAction("Loggers");
            }
            return BadRequest();
        }




    }
}


