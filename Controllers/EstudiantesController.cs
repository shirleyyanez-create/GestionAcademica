using GestionAcademica.Models;
using GestionAcademica.Repositories;
using GestionAcademica.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionAcademica.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public EstudiantesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var estudiantes = await _unitOfWork.Estudiantes.GetAllAsync();
            return View(estudiantes);
        }

        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Estudiante estudiante)
        {
            if (!ModelState.IsValid) return View(estudiante);

            await _unitOfWork.Estudiantes.AddAsync(estudiante);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Crear Estudiante", $"Se registró a {estudiante.Nombre}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(id);
            if (estudiante == null) return NotFound();

            _unitOfWork.Estudiantes.Delete(estudiante);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Eliminar Estudiante", $"Se eliminó a {estudiante.Nombre}");

            return RedirectToAction(nameof(Index));
        }
    }
}