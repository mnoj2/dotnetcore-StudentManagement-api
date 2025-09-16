using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Data;
using StudentManagementApi.Entities.Models;
using StudentManagementApi.Entities;
using System.Security.Claims;

namespace StudentManagementApi.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase {

        private readonly AppDbContext _db;
        public StudentsController(AppDbContext db) { 
            _db = db; 
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile() {

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(id == null)
                return Unauthorized();

            var user = await _db.Users.FindAsync(Guid.Parse(id));

            if(user == null)
                return NotFound();

            return Ok(new { user.Id, user.Username, user.Role });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllStudents() {

            var students = await _db.Users
                .Where(u => u.Role == "Student")
                .Select(u => new { u.Id, u.Username, u.Role })
                .ToListAsync();

            return Ok(students);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("{studentId}/marks")]
        public async Task<IActionResult> AddMark(Guid studentId, AddMarkDto dto) {

            var student = await _db.Users.FindAsync(studentId);

            if(student == null || student.Role != "Student")
                return NotFound("Student not found");

            var mark = new Mark { StudentId = studentId, Subject = dto.Subject, Score = dto.Score };

            _db.Marks.Add(mark);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetMarks), new { studentId }, mark);
        }

        [Authorize]
        [HttpGet("{studentId}/marks")]
        public async Task<IActionResult> GetMarks(Guid studentId) {

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var role = User.FindFirstValue(ClaimTypes.Role);

            if(role != "Teacher" && role != "Admin" && userId != studentId)
                return Forbid();

            var marks = await _db.Marks.Where(m => m.StudentId == studentId).ToListAsync();

            return Ok(marks);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleDto dto) {

            var user = await _db.Users.FindAsync(id);

            if(user == null)
                return NotFound();

            user.Role = dto.Role;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id) {

            var user = await _db.Users.FindAsync(id);

            if(user == null)
                return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
