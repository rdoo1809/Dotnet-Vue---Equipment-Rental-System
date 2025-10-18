# AI Agent Reference Guide: Equipment Rental Management System

## 🎯 Project Overview

This document serves as a comprehensive reference for AI agents working with the Equipment Rental Management System. It outlines patterns, conventions, and implementation details to ensure consistent development practices.

## 🏗️ Architecture Patterns

### Repository Pattern Implementation

The codebase uses a generic repository pattern to abstract data access:

```csharp
// Interface Definition
public interface IRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity? GetById(int id);
    void Add(TEntity model);
    void Update(TEntity model);
    void Delete(TEntity model);
}

// Implementation Example
public class EquipmentRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public EquipmentRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<TEntity>();
    }
    
    public IEnumerable<TEntity> GetAll() => _dbSet.ToList();
    public TEntity? GetById(int id) => _dbSet.Find(id);
    public void Add(TEntity model) => _dbSet.Add(model);
    public void Update(TEntity model) => _dbSet.Update(model);
    public void Delete(TEntity model) => _dbSet.Remove(model);
}
```

**Key Points:**
- All repositories implement the same generic interface
- Use `_dbSet` for all database operations
- Never access `AppDbContext` directly from controllers
- Each entity type has its own repository class

### Unit of Work Pattern

The Unit of Work pattern manages multiple repositories and ensures consistent transactions:

```csharp
public interface IUnitOfWork
{
    IRepository<Equipment> Equipment { get; }
    IRepository<Customer> Customer { get; }
    IRepository<Rental> Rental { get; }
    int Complete();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    
    public IRepository<Equipment> Equipment { get; set; }
    public IRepository<Customer> Customer { get; set; }
    public IRepository<Rental> Rental { get; set; }

    public UnitOfWork(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        Equipment = new EquipmentRepository<Equipment>(_appDbContext);
        Customer = new CustomerRepository<Customer>(_appDbContext);
        Rental = new RentalRepository<Rental>(_appDbContext);
    }
    
    public int Complete() => _appDbContext.SaveChanges();
}
```

**Usage Pattern:**
```csharp
// In controllers
_unitOfWork.Equipment.Add(equipment);
_unitOfWork.Complete(); // Always call this after changes
```

## 🔐 Authentication & Authorization

### JWT Authentication Setup

```csharp
// Program.cs configuration
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes("YourSuperSecretKeyHere1234567890")),
        ClockSkew = TimeSpan.Zero
    };
});
```

### Role-Based Authorization

The system uses two roles:
- **Admin**: Full access to all operations
- **User**: Limited access to own data only

```csharp
// Controller-level authorization
[Authorize]
[Route("api/[controller]")]
public class EquipmentController : ControllerBase

// Method-level authorization
[Authorize(Roles = "Admin")]
[HttpPost]
public ActionResult<Equipment> CreateEquipment(Equipment equipment)

[Authorize(Roles = "Admin,User")]
[HttpGet]
public ActionResult<IEnumerable<Equipment>> ReadAllEquipment()
```

### User Claim Extraction

```csharp
// Service for extracting user information
public class CustomerService
{
    public (string? customerName, string? customerRole) GetUserNameAndRole(ClaimsPrincipal user)
    {
        var customerRole = user.FindFirstValue(ClaimTypes.Role);
        var customerName = user.FindFirstValue(ClaimTypes.Name);
        return (customerName, customerRole);
    }
}

// Usage in controllers
var (userName, userRole) = _customerService.GetUserNameAndRole(User);
if (userRole == "User" && !CanAccessResource(userName, resourceId))
    return Forbid();
```

## 🎮 Controller Patterns

### Standard Controller Structure

```csharp
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class EntityController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly EntityService _entityService;

    public EntityController(IUnitOfWork unitOfWork, EntityService entityService)
    {
        _unitOfWork = unitOfWork;
        _entityService = entityService;
    }

    // CRUD operations follow this pattern
}
```

### HTTP Method Patterns

#### GET Operations
```csharp
[Authorize(Roles = "Admin,User")]
[HttpGet]
public ActionResult<IEnumerable<Entity>> ReadAllEntities()
{
    return Ok(_unitOfWork.Entity.GetAll());
}

[Authorize(Roles = "Admin,User")]
[HttpGet("{id}")]
public ActionResult<Entity> ReadOneEntity(int id)
{
    var entity = _unitOfWork.Entity.GetById(id);
    if (entity is null) return NotFound();
    return Ok(entity);
}
```

#### POST Operations
```csharp
[Authorize(Roles = "Admin")]
[HttpPost]
public ActionResult<Entity> CreateEntity(Entity entity)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);
    _unitOfWork.Entity.Add(entity);
    _unitOfWork.Complete();
    return CreatedAtAction(nameof(ReadOneEntity), new { id = entity.Id }, entity);
}
```

#### PUT Operations
```csharp
[Authorize(Roles = "Admin")]
[HttpPut("{id}")]
public ActionResult<Entity> UpdateEntity(int id, Entity incoming)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);
    
    var entity = _unitOfWork.Entity.GetById(id);
    if (entity is null) return NotFound();
    
    // Map properties
    entity.Property1 = incoming.Property1;
    entity.Property2 = incoming.Property2;
    
    _unitOfWork.Entity.Update(entity);
    _unitOfWork.Complete();
    return Ok(entity);
}
```

#### DELETE Operations
```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public ActionResult<Entity> DeleteEntity(int id)
{
    var entity = _unitOfWork.Entity.GetById(id);
    if (entity is null) return NotFound();
    
    _unitOfWork.Entity.Delete(entity);
    _unitOfWork.Complete();
    return entity;
}
```

#### Special Rental Operations
```csharp
// Get active rentals
[Authorize(Roles = "Admin,User")]
[HttpGet("active")]
public ActionResult<IEnumerable<Rental>> ReadActiveRentals()
{
    var (userName, userRole) = _customerService.GetUserNameAndRole(User);
    var customer = _unitOfWork.Customer.GetAll()
        .FirstOrDefault(c => c.UserName == userName);
    if (customer is null) return NotFound();
    
    var activeRentals = _unitOfWork.Rental.GetAll()
        .Where(r => r.ReturnedAt == null);
    
    if (userRole == "User") 
        return Ok(activeRentals.Where(r => r.CustomerId == customer.Id).ToList());
    
    return Ok(activeRentals);
}

// Get overdue rentals
[Authorize(Roles = "Admin,User")]
[HttpGet("overdue")]
public ActionResult<IEnumerable<Rental>> ReadOverdueRentals()
{
    var (userName, userRole) = _customerService.GetUserNameAndRole(User);
    var customer = _unitOfWork.Customer.GetAll()
        .FirstOrDefault(c => c.UserName == userName);
    if (customer is null) return NotFound();
    
    var overdueRentals = _unitOfWork.Rental.GetAll()
        .Where(r => r.DueDate < DateTime.Now && r.ReturnedAt == null);
    
    if (userRole == "User") 
        return Ok(overdueRentals.Where(r => r.CustomerId == customer.Id).ToList());
    
    return Ok(overdueRentals);
}

// Issue rental
[Authorize(Roles = "Admin,User")]
[HttpPost("issue")]
public ActionResult<Equipment> CreateRentalIssue(Rental rental)
{
    // Implementation includes business rule validation
    // and automatic equipment availability updates
}

// Return rental
[Authorize(Roles = "Admin,User")]
[HttpPost("return")]
public ActionResult<Equipment> CreateRentalReturn(Rental rental)
{
    // Implementation includes equipment availability updates
    // and return timestamp recording
}
```

## 📊 Entity Models

### Model Structure

```csharp
public class Entity
{
    public int Id { get; set; }
    public required String Property1 { get; set; }
    public required String Property2 { get; set; }
    public String OptionalProperty { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### Key Conventions
- Use `required` keyword for non-nullable properties
- Use `String` instead of `string` (project preference)
- Use `DateTime` for timestamps
- Use `bool` for flags
- Follow PascalCase naming

### Rental Model Structure
```csharp
public class Rental
{
    public int Id { get; set; }
    public int EquipmentId { get; set; }
    public int CustomerId { get; set; }
    public DateTime IssuedAt { get; set; }
    public DateTime DueDate { get; set; }        // 7 days from issue date
    public DateTime? ReturnedAt { get; set; }    // Nullable - set when returned
}
```

**Key Properties:**
- `IssuedAt`: When the rental was created
- `DueDate`: When the rental is due (7 days from issue by default)
- `ReturnedAt`: When the rental was returned (null if still active)

## 🗄️ Database Patterns

### Entity Framework Configuration

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; } = null!;
    public DbSet<Customer> Customer { get; set; } = null!;
    public DbSet<Rental> Rental { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Seed data
        modelBuilder.Entity<Equipment>().HasData(
            new Equipment { Id = 1, Name = "Item", /* ... */ }
        );
    }
}
```

### Migration Patterns
- Always use Entity Framework migrations for schema changes
- Include seed data in migrations when appropriate
- Use descriptive migration names

## 🔄 Business Logic Patterns

### Rental System Logic

```csharp
// Issue rental
public ActionResult<Equipment> CreateRentalIssue(Rental rental)
{
    // 1. Validate user permissions
    var (userName, userRole) = _customerService.GetUserNameAndRole(User);
    
    // 2. Check business rules
    var hasActiveRental = HasActiveRental(customer);
    var equipmentAvailable = CheckEquipmentAvailability(rental);
    
    if (hasActiveRental) return BadRequest("Customer has an active rental.");
    if (!equipmentAvailable) return BadRequest("Equipment is not available.");
    
    // 3. Update equipment availability
    SetEquipmentAvailability(rental, false);
    
    // 4. Create rental record
    rental.IssuedAt = DateTime.UtcNow;
    rental.DueDate = DateTime.UtcNow.AddDays(7); // Set due date to 7 days from issue
    rental.ReturnedAt = null;
    _unitOfWork.Rental.Add(rental);
    _unitOfWork.Complete();
    
    return CreatedAtAction(nameof(ReadOneRental), new { id = rental.CustomerId }, rental);
}
```

### Business Rules
1. **One Active Rental**: Customers can only have one active rental at a time
2. **Equipment Availability**: Equipment must be available to be rented
3. **User Access**: Users can only access their own data (unless Admin)
4. **Availability Updates**: Equipment availability must be updated when renting/returning
5. **Due Date Management**: Rentals have a 7-day default due date from issue date
6. **Overdue Tracking**: System tracks overdue rentals based on DueDate vs current time

## 🛠️ Service Layer Patterns

### Service Structure

```csharp
public class EntityService
{
    public (string? userName, string? userRole) GetUserNameAndRole(ClaimsPrincipal user)
    {
        var userRole = user.FindFirstValue(ClaimTypes.Role);
        var userName = user.FindFirstValue(ClaimTypes.Name);
        return (userName, userRole);
    }
    
    // Other business logic methods
}
```

### Service Registration

```csharp
// Program.cs
builder.Services.AddScoped<EntityService>();
```

## 📁 File Organization

```
Controllers/     - API controllers with [Authorize] attributes
├── AuthController.cs
├── CustomerController.cs
├── EquipmentController.cs
├── RentalController.cs
└── HomeController.cs

Models/         - Entity models
├── Customer.cs
├── Equipment.cs
├── Rental.cs
└── ErrorViewModel.cs

Repositories/   - Repository pattern implementation
├── IRepository.cs
├── IUnitOfWork.cs
├── UnitOfWork.cs
├── CustomerRepository.cs
├── EquipmentRepository.cs
└── RentalRepository.cs

Services/       - Business logic services
└── CustomerService.cs

Data/          - Database context
└── AppDbContext.cs

Views/         - MVC views (minimal)
├── Home/
└── Shared/

Migrations/    - Entity Framework migrations
```

## 🔧 Common Helper Methods

### Equipment Availability Management

```csharp
private void SetEquipmentAvailability(Rental rental, bool isAvailable)
{
    var equipment = _unitOfWork.Equipment.GetById(rental.EquipmentId);
    if (equipment == null) return;
    equipment.IsAvailable = isAvailable;
    _unitOfWork.Equipment.Update(equipment);
}

private bool HasActiveRental(Customer customer)
{
    return _unitOfWork.Rental.GetAll()
        .Any(r => r.CustomerId == customer.Id && r.ReturnedAt == null);
}

private bool CheckEquipmentAvailability(Rental rental)
{
    var equipment = _unitOfWork.Equipment.GetById(rental.EquipmentId);
    if (equipment == null) return false;
    return equipment.IsAvailable;
}
```

## 🚨 Error Handling Patterns

### Standard Error Responses

```csharp
// Not found
if (entity is null) return NotFound();

// Validation errors
if (!ModelState.IsValid) return BadRequest(ModelState);

// Authorization failures
if (userRole == "User" && !CanAccessResource(userName, resourceId))
    return Forbid();

// Business rule violations
if (hasActiveRental) return BadRequest("Customer has an active rental.");
```

## 🔍 Testing Considerations

### Controller Testing
- Mock `IUnitOfWork` and services
- Test both success and failure scenarios
- Verify proper HTTP status codes
- Test authorization rules

### Repository Testing
- Use in-memory database for testing
- Test all CRUD operations
- Verify data integrity

## 📝 Naming Conventions

### Controllers
- `{Entity}Controller` (e.g., `EquipmentController`)
- Methods: `Read{Entity}`, `Create{Entity}`, `Update{Entity}`, `Delete{Entity}`
- Special methods: `CreateRentalIssue`, `CreateRentalReturn`

### Repositories
- `{Entity}Repository<TEntity>`
- Always implement `IRepository<TEntity>`

### Services
- `{Entity}Service`
- Use descriptive method names

### Variables
- Use camelCase for local variables
- Use descriptive names
- Use `_` prefix for private fields

## 🎯 Key Principles

1. **Separation of Concerns**: Controllers handle HTTP, services handle business logic, repositories handle data
2. **Dependency Injection**: All dependencies injected through constructor
3. **Authorization First**: Always check permissions before processing
4. **Consistent Patterns**: Follow established patterns throughout the codebase
5. **Error Handling**: Return appropriate HTTP status codes
6. **Business Rules**: Enforce business rules consistently
7. **Data Integrity**: Always call `Complete()` after changes

## 📋 Rental System Endpoints

### Standard CRUD Operations
- `GET /api/rental` → List all rentals (filtered by user role)
- `GET /api/rental/{id}` → Get specific rental (own data only for users)
- `PUT /api/rental/{id}` → Update rental due date (Admin only)
- `DELETE /api/rental/{id}` → Delete rental (Admin only)

### Specialized Rental Operations
- `GET /api/rental/active` → List active rentals (where ReturnedAt is null)
- `GET /api/rental/completed` → List completed rentals (where ReturnedAt is not null)
- `GET /api/rental/overdue` → List overdue rentals (DueDate < now AND ReturnedAt is null)
- `POST /api/rental/issue` → Create new rental (with business rule validation)
- `POST /api/rental/return` → Return existing rental (updates ReturnedAt)

### Business Logic in Endpoints
- **Issue Rental**: Validates one active rental rule, checks equipment availability, sets 7-day due date
- **Return Rental**: Updates equipment availability, sets return timestamp
- **Overdue Check**: Compares DueDate with current DateTime
- **Role Filtering**: Users only see their own rentals, Admins see all

## 🚀 Development Workflow

1. **Create Entity**: Define model with proper annotations
2. **Create Repository**: Implement `IRepository<TEntity>`
3. **Update Unit of Work**: Add repository to `IUnitOfWork`
4. **Create Service**: Add business logic if needed
5. **Create Controller**: Implement CRUD operations with proper authorization
6. **Add Migration**: Update database schema if needed
7. **Test**: Verify all operations work correctly

This reference guide should be used by AI agents to maintain consistency with the existing codebase patterns and conventions.
