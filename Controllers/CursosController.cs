using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionAcademica.Models;
using GestionAcademica.Repositories;
using GestionAcademica.Services;

namespace GestionAcademica.Controllers
{
    public class CursosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CursosController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var cursos = await _unitOfWork.Cursos.GetAllAsync();
            foreach (var curso in cursos)
            {
                curso.Profesor = await _unitOfWork.Profesores.GetByIdAsync(curso.ProfesorId);
            }
            return View(cursos);
        }

        public async Task<IActionResult> Details(int id)
        {
            var curso = await _unitOfWork.Cursos.GetByIdAsync(id);
            if (curso == null) return NotFound();
            curso.Profesor = await _unitOfWork.Profesores.GetByIdAsync(curso.ProfesorId);
            return View(curso);
        }

        public async Task<IActionResult> Create()
        {
            await CargarProfesoresAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Curso curso)
        {
            if (!ModelState.IsValid)
            {
                await CargarProfesoresAsync(curso.ProfesorId);
                return View(curso);
            }

            await _unitOfWork.Cursos.AddAsync(curso);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Crear Curso", $"Se creó el curso {curso.Nombre}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var curso = await _unitOfWork.Cursos.GetByIdAsync(id);
            if (curso == null) return NotFound();
            await CargarProfesoresAsync(curso.ProfesorId);
            return View(curso);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Curso curso)
        {
            if (id != curso.CursoId) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarProfesoresAsync(curso.ProfesorId);
                return View(curso);
            }

            _unitOfWork.Cursos.Update(curso);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Editar Curso", $"Se actualizó el curso {curso.Nombre}");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _unitOfWork.Cursos.GetByIdAsync(id);
            if (curso == null) return NotFound();
            return View(curso);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var curso = await _unitOfWork.Cursos.GetByIdAsync(id);
            if (curso == null) return NotFound();

            _unitOfWork.Cursos.Delete(curso);
            await _unitOfWork.SaveChangesAsync();
            await MongoLogService.Instance.RegistrarAsync("Eliminar Curso", $"Se eliminó el curso {curso.Nombre}");

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarProfesoresAsync(int? seleccionado = null)
        {
            var profesores = await _unitOfWork.Profesores.GetAllAsync();
            ViewBag.ProfesorId = new SelectList(profesores, "ProfesorId", "Nombre", seleccionado);
        }
    }
}