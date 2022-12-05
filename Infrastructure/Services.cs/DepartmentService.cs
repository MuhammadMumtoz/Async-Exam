using System.Net;
using Dapper;

public class DepartmentService
{
    private readonly DapperContext _context;
    public DepartmentService(DapperContext context)
    {
        _context = context;
    }

    public async Task<Response<List<DepartmentDto>>> GetDepartments()
    {
        using (var connection = _context.CreateConnection())
        {
            var sql = "Select id as Id, department_name as DepartmentName, manager_id as ManadgerId, location_id as LocationId from departments";
            var response = await connection.QueryAsync<DepartmentDto>(sql);
            return new Response<List<DepartmentDto>>(response.ToList());
        }
    }

    public async Task<Response<DepartmentDto>> InsertDepartment(DepartmentDto department)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = $"Insert into departments(department_name,manager_id,location_id) values('{department.DepartmentName}',{department.ManagerId},{department.LocationId}) returning id";
                var response = await connection.ExecuteScalarAsync<int>(sql);
                department.Id = response;
                return new Response<DepartmentDto>(department);
            }
        }
        catch (Exception ex)
        {
            return new Response<DepartmentDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }
    public async Task<Response<DepartmentDto>> UpdateDepartment(DepartmentDto department)
    {
        try
        {
            using (var connection = _context.CreateConnection())
            {
                var sql = $"Update departments set department_name = '{department.DepartmentName}', manager_id = {department.ManagerId}, location_id = {department.LocationId} where id = {department.Id} returning id";
                var response = await connection.ExecuteScalarAsync<int>(sql);
                department.Id = response;
                return new Response<DepartmentDto>(department);
            }
        }
        catch (Exception ex)
        {
            return new Response<DepartmentDto>(HttpStatusCode.InternalServerError, ex.Message);
        }
    }


    public async Task<Response<string>> DeleteDepartment(int id)
    {
        using (var connection = _context.CreateConnection())
        {
            var sql = $"Delete from departments where id = {id}";
            var response = await connection.ExecuteAsync(sql);
            if(response>0)
                return new Response<string>("Department deleted successfully");
                return new Response<string>(HttpStatusCode.BadRequest,"Category not found");
        }
    }

}