using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Factory.Models;

namespace Factory.Controllers {
  public class MachinesController : Controller {
    private readonly FactoryContext _db;
    public MachinesController(FactoryContext db) {
      _db = db;
    }
    public ActionResult Index() {
      ViewBag.Machines = _db.Machines.ToList();
      return View(_db.Machines.OrderBy(machine => machine.MakeModel).ToList());
    }
    public ActionResult Create() {
      return View();
    }
    [HttpPost]
    public ActionResult Create(Machine machine, int EngineerId) {
      _db.Machines.Add(machine);
      _db.SaveChanges();
      return RedirectToAction("Details", new {id= machine.MachineId});
      
    }
    public ActionResult Edit(int id) {
      var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
      ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Name");
      return View(thisMachine);
    }
    [HttpPost]
    public ActionResult Edit(Machine machine, int EngineerId)
    {
      if (EngineerId != 0) {
        _db.EngineerMachine.Add(new EngineerMachine() { EngineerId = EngineerId, MachineId = machine.MachineId });
      }
      _db.Entry(machine).State = EntityState.Modified;
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Delete(int id) {
      var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
      return View(thisMachine);
    }
    [HttpPost, ActionName("Delete")] // machine
    public ActionResult DeleteConfirmed(int id) {
      var thisMachine = _db.Machines.FirstOrDefault(machine => machine.MachineId == id);
      _db.Machines.Remove(thisMachine);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
    public ActionResult Details(int id) {
      ViewBag.EngineerId = new SelectList(_db.Engineers, "EngineerId", "Company");
      var thisMachine = _db.Machines
        .Include(machine => machine.JoinEntities)
        .ThenInclude(join => join.Engineer)
        .FirstOrDefault(machine => machine.MachineId == id);
      return View(thisMachine);
    }
    [HttpPost]
    public ActionResult AddEngineer(Machine machine, int EngineerId) {
      if (EngineerId != 0) {
        _db.EngineerMachine.Add(new EngineerMachine() { EngineerId = EngineerId, MachineId = machine.MachineId });
        _db.SaveChanges();
      }
      return RedirectToAction("Details", new {id= machine.MachineId});
    }
    [HttpPost]
    public ActionResult DeleteEngineer(int joinId) {
      var joinEntry = _db.EngineerMachine.FirstOrDefault(entry => entry.EngineerMachineId == joinId);
      _db.EngineerMachine.Remove(joinEntry);
      _db.SaveChanges();
      return RedirectToAction("Index");
    }
  }
}