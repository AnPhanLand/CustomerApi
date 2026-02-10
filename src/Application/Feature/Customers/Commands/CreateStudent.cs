public record CreateStudentCommand(StudentCreateDTO StudentDTO) : IRequest<Guid>;

public class CreateStudentHandler : IRequestHandler<CreateStudentCommand, Guid>
{
    private readonly IApplicationDbContext _context;
    public CreateStudentHandler(IApplicationDbContext context) => _context = context;

    public async Task<Guid> Handle(CreateStudentCommand request, CancellationToken ct)
    {
        var entity = new Student
        {
            Id = Guid.NewGuid(),
            FirstName = request.StudentDTO.FirstName,
            LastName = request.StudentDTO.LastName,
            ClassName = request.StudentDTO.ClassName
        };

        _context.Students.Add(entity);
        await _context.SaveChangesAsync(ct);
        
        return entity.Id;
    }
}