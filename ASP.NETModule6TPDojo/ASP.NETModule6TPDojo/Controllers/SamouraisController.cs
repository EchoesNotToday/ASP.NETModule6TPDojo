using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASP.NETModule6TPDojo.Data;
using ASP.NETModule6TPDojo.Models;
using BO;

namespace ASP.NETModule6TPDojo.Controllers
{
    public class SamouraisController : Controller
    {
        private DojoContext db = new DojoContext();

        // GET: Samourais
        public ActionResult Index()
        {
            return View(db.Samourais.ToList());
        }

        // GET: Samourais/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            ViewBag.Potentiel = GetPotentiel(samourai);
            return View(samourai);
        }

        // GET: Samourais/Create
        public ActionResult Create()
        {
            SamouraiVM samouraiVM = new SamouraiVM {
                Armes = GetArmesAvailable(),       
                ArtsMartiaux = db.ArtsMartiaux.ToList()
            };
            return View(samouraiVM);
        }

        // POST: Samourais/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SamouraiVM samouraiVM)
        {
            if (ModelState.IsValid)
            {
                if (samouraiVM.IdSelectedArme.HasValue)
                {
                    samouraiVM.Samourai.Arme = db.Armes.FirstOrDefault(a => a.Id == samouraiVM.IdSelectedArme);
                }
                foreach (var idArtMartial in samouraiVM.IdSelectedArtsMartiaux)
                {
                    samouraiVM.Samourai.ArtsMartiaux.Add(db.ArtsMartiaux.Find(idArtMartial));
                }
                db.Samourais.Add(samouraiVM.Samourai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            samouraiVM.Armes = GetArmesAvailable();
            return View(samouraiVM);
        }

        // GET: Samourais/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            SamouraiVM samouraiVM = new SamouraiVM {
                Samourai = samourai,
                Armes = GetArmesAvailable(),
                ArtsMartiaux = db.ArtsMartiaux.ToList(),
                IdSelectedArtsMartiaux = samourai.ArtsMartiaux.Select(a => a.Id).ToList()
            };
            if (samourai.Arme != null)
            {
                samouraiVM.IdSelectedArme = samourai.Arme.Id;
                samouraiVM.Armes.Add(samourai.Arme);
            }
            return View(samouraiVM);
        }

        // POST: Samourais/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SamouraiVM samouraiVM)
        {
            if (ModelState.IsValid)
            {
                Samourai samouraiDB = db.Samourais.Find(samouraiVM.Samourai.Id);
                if (samouraiDB.Arme != null)
                {
                    var arme = db.Armes.Find(samouraiDB.Arme.Id);
                    db.Entry(arme).State = EntityState.Modified;
                }

                samouraiDB.Arme = null;

                if (samouraiVM.IdSelectedArme.HasValue)
                {
                    samouraiDB.Arme = db.Armes.FirstOrDefault(a => a.Id == samouraiVM.IdSelectedArme);

                }
                samouraiDB.ArtsMartiaux.Clear();
                foreach (var idArtMartial in samouraiVM.IdSelectedArtsMartiaux)
                {
                    samouraiDB.ArtsMartiaux.Add(db.ArtsMartiaux.Find(idArtMartial));
                }

                samouraiDB.Nom = samouraiVM.Samourai.Nom;
                samouraiDB.Force = samouraiVM.Samourai.Force;

                db.Entry(samouraiDB).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            samouraiVM.Armes = GetArmesAvailable();
            samouraiVM.ArtsMartiaux = db.ArtsMartiaux.ToList();
            return View(samouraiVM);
        }

        // GET: Samourais/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            return View(samourai);
        }

        // POST: Samourais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Samourai samourai = db.Samourais.Find(id);
            db.Samourais.Remove(samourai);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private static int GetPotentiel(Samourai samourai)
        {
            int degatArme = 0;
            if (samourai.Arme != null)
            {
                degatArme = samourai.Arme.Degats;
            }
            return (samourai.Force + degatArme) * (samourai.ArtsMartiaux.Count() + 1);
         }

        private List<Arme> GetArmesAvailable()
        {
            List<int> armeUsed = db.Samourais.Where(s => s.Arme != null).Select(s => s.Arme.Id).ToList();
            return db.Armes.Where(a => !armeUsed.Contains(a.Id)).ToList();
        }
    }
}
