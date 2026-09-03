using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GestionAcademica.Models;
using GestionAcademica.Repositories;
using GestionAcademica.Services;

namespace GestionAcademica.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CalificacionesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var calificaciones = await _unitOfWork.Calificaciones.GetAllAsync();
            foreach (var c in calificaciones)
            {
                c.Matricula = await _unitOfWork.Matriculas.GetByIdAsync(c.MatriculaId);
                if (c.Matricula != null)
                {
                    c.Matricula.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(c.Matricula.EstudianteId);
                    c.Matricula.Curso = await _unitOfWork.Cursos.GetByIdAsync(c.Matricula.CursoId);
                }
            }
            return View(calificaciones);
        }

        public async Task<IActionResult> Details(int id)
        {
            var calificacion = await _unitOfWork.Calificaciones.GetByIdAsync(id);
            if (calificacion == null) return NotFound();

            calificacion.Matricula = await _unitOfWork.Matriculas.GetByIdAsync(calificacion.MatriculaId);
            if (calificacion.Matricula != null)
            {
                calificacion.Matricula.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(calificacion.Matricula.EstudianteId);
                calificacion.Matricula.Curso = await _unitOfWork.Cursos.GetByIdAsync(calificacion.Matricula.CursoId);
            }
            return View(calificacion);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Calificacion calificacion)
        {
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(calificacion.MatriculaId);
                return View(calificacion);
            }

            await _unitOfWork.Calificaciones.AddAsync(calificacion);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await MongoLogService.Instance.RegistrarAsync("Crear Calificación", $"Calificación {calificacion.CalificacionId} registrada");
            }
            catch
            {
                // El log es secundario: si Mongo falla, no debe tumbar la operación principal.
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var calificacion = await _unitOfWork.Calificaciones.GetByIdAsync(id);
            if (calificacion == null) return NotFound();
            await CargarListasAsync(calificacion.MatriculaId);
            return View(calificacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Calificacion calificacion)
        {
            if (id != calificacion.CalificacionId) return NotFound();
            if (!ModelState.IsValid)
            {
                await CargarListasAsync(calificacion.MatriculaId);
                return View(calificacion);
            }

            _unitOfWork.Calificaciones.Update(calificacion);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await MongoLogService.Instance.RegistrarAsync("Editar Calificación", $"Calificación {calificacion.CalificacionId} actualizada");
            }
            catch
            {
                // El log es secundario: si Mongo falla, no debe tumbar la operación principal.
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var calificacion = await _unitOfWork.Calificaciones.GetByIdAsync(id);
            if (calificacion == null) return NotFound();

            calificacion.Matricula = await _unitOfWork.Matriculas.GetByIdAsync(calificacion.MatriculaId);
            if (calificacion.Matricula != null)
            {
                calificacion.Matricula.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(calificacion.Matricula.EstudianteId);
                calificacion.Matricula.Curso = await _unitOfWork.Cursos.GetByIdAsync(calificacion.Matricula.CursoId);
            }
            return View(calificacion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calificacion = await _unitOfWork.Calificaciones.GetByIdAsync(id);
            if (calificacion == null) return NotFound();

            _unitOfWork.Calificaciones.Delete(calificacion);
            await _unitOfWork.SaveChangesAsync();

            try
            {
                await MongoLogService.Instance.RegistrarAsync("Eliminar Calificación", $"Calificación {calificacion.CalificacionId} eliminada");
            }
            catch
            {
                // El log es secundario: si Mongo falla, no debe tumbar la operación principal.
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync(int? matriculaSel = null)
        {
            var matriculas = await _unitOfWork.Matriculas.GetAllAsync();
            foreach (var m in matriculas)
            {
                m.Estudiante = await _unitOfWork.Estudiantes.GetByIdAsync(m.EstudianteId);
                m.Curso = await _unitOfWork.Cursos.GetByIdAsync(m.CursoId);
            }

            var lista = matriculas.Select(m => new
            {
                m.MatriculaId,
                Texto = $"{m.Estudiante?.Nombre} - {m.Curso?.Nombre}"
            });

            ViewBag.MatriculaId = new SelectList(lista, "MatriculaId", "Texto", matriculaSel);
        }
    }
}