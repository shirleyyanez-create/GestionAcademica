using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionAcademica.Models;
using GestionAcademica.Repositories;
using GestionAcademica.Services;

namespace GestionAcademica.Controllers
{
    public class MatriculasController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MatriculasController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var matriculas = await _unitOfWork.Matriculas.GetAllAsync();
            foreach (var m in matriculas)
            {
                m.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(m.EstudianteId);
                m.Curso = await _unitOfWork.Cursos.GetByIdAsync(m.CursoId);
            }
            return View(matriculas);
        }

        public async Task<IActionResult> Details(int id)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(id);
            if (matricula == null) return NotFound();
            matricula.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(matricula.EstudianteId);
            matricula.Curso = await _unitOfWork.Cursos.GetByIdAsync(matricula.CursoId);
            return View(matricula);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Matricula matricula)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(matricula.EstudianteId, matricula.CursoId);
                return View(matricula);
            }

            await _unitOfWork.Matriculas.AddAsync(matricula);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await MongoLogService.Instance.RegistrarAsync("Crear Matrícula", $"Matrícula {matricula.MatriculaId} registrada");
            }
            catch
            {
                // El log es secundario: si Mongo falla, no debe tumbar la operación principal.
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(id);
            if (matricula == null) return NotFound();
            await CargarListasAsync(matricula.EstudianteId, matricula.CursoId);
            return View(matricula);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Matricula matricula)
        {
            if (id != matricula.MatriculaId) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(matricula.EstudianteId, matricula.CursoId);
                return View(matricula);
            }

            _unitOfWork.Matriculas.Update(matricula);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await MongoLogService.Instance.RegistrarAsync("Editar Matrícula", $"Matrícula {matricula.MatriculaId} actualizada");
            }
            catch
            {
                // El log es secundario: si Mongo falla, no debe tumbar la operación principal.
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(id);
            if (matricula == null) return NotFound();
            matricula.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(matricula.EstudianteId);
            matricula.Curso = await _unitOfWork.Cursos.GetByIdAsync(matricula.CursoId);
            return View(matricula);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var matricula = await _unitOfWork.Matriculas.GetByIdAsync(id);
            if (matricula == null) return NotFound();

            _unitOfWork.Matriculas.Delete(matricula);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await MongoLogService.Instance.RegistrarAsync("Eliminar Matrícula", $"Matrícula {matricula.MatriculaId} eliminada");
            }
            catch
            {
                // El log es secundario: si Mongo falla, no debe tumbar la operación principal.
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync(int? estudianteSel = null, int? cursoSel = null)
        {
            var estudiantes = await _unitOfWork.Estudiantes.GetAllAsync();
            var cursos = await _unitOfWork.Cursos.GetAllAsync();
            ViewBag.EstudianteId = new SelectList(estudiantes, "EstudianteId", "Nombre", estudianteSel);
            ViewBag.CursoId = new SelectList(cursos, "CursoId", "Nombre", cursoSel);
        }
    }
}