using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class MeetingsController : ControllerBase
{
    private readonly AppDbContext _context;

    public MeetingsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMeeting([FromForm] MeetingDto dto)
    {
        var meeting = new Meeting
        {
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Description = dto.Description
        };

        if (dto.Document != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await dto.Document.CopyToAsync(memoryStream);
                meeting.DocumentData = memoryStream.ToArray();
                meeting.DocumentName = dto.Document.FileName;
                meeting.DocumentContentType = dto.Document.ContentType;
            }
        }

        _context.Meetings.Add(meeting);
        await _context.SaveChangesAsync();

        return Ok(meeting);
    }

    [HttpGet]
    public async Task<IActionResult> GetMeetings()
    {
        var meetings = await _context.Meetings.ToListAsync();
        return Ok(meetings);
    }

     [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMeeting(int id)
    {
        var meeting = await _context.Meetings.FindAsync(id);

        if (meeting == null)
        {
            return NotFound();
        }

        _context.Meetings.Remove(meeting);
        await _context.SaveChangesAsync();

        return Ok();
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetMeeting(int id)
    {
        var meeting = await _context.Meetings.FindAsync(id);

        if (meeting == null)
        {
            return NotFound();
        }

        return Ok(meeting);
    }

  
    [HttpPut("edit/{id}")]
       public async Task<IActionResult> UpdateMeeting(int id, [FromForm] MeetingDto dto)
    {
        var meeting = await _context.Meetings.FindAsync(id);

        if (meeting == null)
        {
            return NotFound();
        }

        meeting.Name = dto.Name;
        meeting.StartDate = dto.StartDate;
        meeting.EndDate = dto.EndDate;
        meeting.Description = dto.Description;

            if (dto.Document != null && dto.Document.Length > 0) 

        {
            using (var memoryStream = new MemoryStream())
            {
                await dto.Document.CopyToAsync(memoryStream);
                meeting.DocumentData = memoryStream.ToArray();
                meeting.DocumentName = dto.Document.FileName;
                meeting.DocumentContentType = dto.Document.ContentType;
            }
        }

        _context.Meetings.Update(meeting);
        await _context.SaveChangesAsync();

        return Ok(meeting);
    }


    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadDocument(int id)
    {
        var meeting = await _context.Meetings.FindAsync(id);

        if (meeting == null || meeting.DocumentData == null)
        {
            return NotFound();
        }

        return File(meeting.DocumentData, meeting.DocumentContentType, meeting.DocumentName);
    }
}
