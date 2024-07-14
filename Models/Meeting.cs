// Models/Meeting.cs
public class Meeting
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Description { get; set; }
    public byte[] DocumentData { get; set; } 
    public string DocumentName { get; set; } 
    public string DocumentContentType { get; set; } 
}
