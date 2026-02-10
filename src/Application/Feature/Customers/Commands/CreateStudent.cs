public record CreateStudentCommand(StudentCreateDTO StudentDTO) : IRequest<Guid>;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateStudentHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var entity = new Student
        {
            FirstName = request.StudentDTO.FirstName,
            LastName = request.StudentDTO.LastName,
            ClassName = request.StudentDTO.ClassName,
            Status = request.StudentDTO.Status,
            Address = request.StudentDTO.Address,
            Gender = request.StudentDTO.Gender,
            DateOfBirth = request.StudentDTO.DateOfBirth,
            ClassID = request.StudentDTO.ClassID,
            Grade = request.StudentDTO.Grade,
            PhuHuynhId = request.StudentDTO.PhuHuynhId
        };

        _context.Students.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.Id;
    }
}