using CustomerAPI.Data;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerAPI.Controllers
{
    [ApiController]
    [Route("api/customers")]
    public class CustomerController : Controller
    {
        private readonly CustomerAPIDbContext dbContext;

        public CustomerController(CustomerAPIDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            return Ok( await dbContext.Customers.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetCustomer([FromRoute] Guid id)
        {
            var customer = await dbContext.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);

        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Models.AddCustomerRequest addCustomerRequest)
        {
            var today = DateTime.Now;
            var customer = new Customer()
            {
                CustomerId = Guid.NewGuid(),
                FirstName = addCustomerRequest.FirstName,
                LastName = addCustomerRequest.LastName,
                UserName = addCustomerRequest.FirstName + " " + addCustomerRequest.LastName,
                EmailAddress = addCustomerRequest.EmailAddress,
                DateOfBirth = addCustomerRequest.DateOfBirth,
                Age = DateTime.Now.Year - DateTime.Parse( addCustomerRequest.DateOfBirth).Year,
                DateCreated = today.ToString(),
                DateEdited = today.ToString(),
                IsDeleted = addCustomerRequest.IsDeleted
            };

            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();

            return Ok(customer);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateCustomer([FromRoute] Guid id,Models.UpdateCustomerRequest updateCustomerRequest)
        {
           var customer =  await dbContext.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.FirstName = updateCustomerRequest.FirstName;   
                customer.LastName = updateCustomerRequest.LastName;
                customer.EmailAddress = updateCustomerRequest.EmailAddress;
                customer.DateOfBirth = updateCustomerRequest.DateOfBirth;
                customer.Age = updateCustomerRequest.Age;
                customer.DateEdited = updateCustomerRequest.DateEdited;

                await dbContext.SaveChangesAsync();

                return Ok(customer);
            };

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] Guid id)
        {
            var customer = await dbContext.Customers.FindAsync(id);

            if (customer != null)
            {
                dbContext.Remove(customer);
                await dbContext.SaveChangesAsync();

                return Ok(customer);
            }

            return NotFound();
        }
    }
}
