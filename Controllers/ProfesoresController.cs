using Microsoft.AspNetCore.Mvc;
using GestionAcademica.Models;
using GestionAcademica.Repositories;
using GestionAcademica.Services;

namespace GestionAcademica.Controllers
{
    public class ProfesoresController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProfesoresController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var profesores = await _unitOfWork.Profesores.GetAllAsync();
            return View(profesores);
        }

        public async Task<IActionResult> Details(int id)
        {
            var profesor = await _unitOfWork.Profesores.GetByIdAsync(id);
            if (profesor == null) return NotFound();
            return View(profesor);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Profesor profesor)
        {
            if (!ModelState.IsValid) return View(profesor);

            await _unitOfWork.Profesores.AddAsync(profesor);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Crear Profesor", $"Se registró a {profesor.Nombre}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var profesor = await _unitOfWork.Profesores.GetByIdAsync(id);
            if (profesor == null) return NotFound();
            return View(profesor);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Profesor profesor)
        {
            if (id != profesor.ProfesorId) return NotFound();
            if (!ModelState.IsValid) return View(profesor);

            _unitOfWork.Profesores.Update(profesor);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Editar Profesor", $"Se actualizó a {profesor.Nombre}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var profesor = await _unitOfWork.Profesores.GetByIdAsync(id);
            if (profesor == null) return NotFound();
            return View(profesor);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profesor = await _unitOfWork.Profesores.GetByIdAsync(id);
            if (profesor == null) return NotFound();

            _unitOfWork.Profesores.Delete(profesor);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Eliminar Profesor", $"Se eliminó a {profesor.Nombre}");

            return RedirectToAction(nameof(Index));
        }
    }
}