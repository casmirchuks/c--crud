using CustomerAPI.Data;
using CustomerAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

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

        public static string IsValidEmail(string email)
        {
            try
            {
                MailAddress validEmail = new MailAddress(email);
                return validEmail.Address;
            }
            catch (FormatException)
            {
                //if the email address is not valid, show an error message
                return "Incorrect email. Please try again";
            }
            catch (Exception)
            {
                return "Incorrect email. Please try again";
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(Models.AddCustomerRequest addCustomerRequest)
        {
            var today = DateTime.Now;
            DateTime userBirth = DateTime.Parse(addCustomerRequest.DateOfBirth);
            int age = DateTime.Now.Year - userBirth.Year;

            if (DateTime.Now.Month < userBirth.Month ||
                (DateTime.Now.Month == userBirth.Month && DateTime.Now.Day < userBirth.Day))
            {
                age--;
            }

            Console.WriteLine("Your age is: " + age);


            var customer = new Customer() {
                CustomerId = Guid.NewGuid(),
                FirstName = addCustomerRequest.FirstName,
                LastName = addCustomerRequest.LastName,
                UserName = addCustomerRequest.FirstName + " " + addCustomerRequest.LastName,
                EmailAddress = IsValidEmail(addCustomerRequest.EmailAddress),
                DateOfBirth = addCustomerRequest.DateOfBirth,
                //Age = DateTime.Now.Year - DateTime.Parse( addCustomerRequest.DateOfBirth).Year,
                Age = age,
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
            var today = DateTime.Now;
            var customer =  await dbContext.Customers.FindAsync(id);
            if (customer != null)
            {
                customer.FirstName = updateCustomerRequest.FirstName;   
                customer.LastName = updateCustomerRequest.LastName;
                customer.UserName = updateCustomerRequest.FirstName + " " + updateCustomerRequest.LastName;
                customer.EmailAddress = IsValidEmail(updateCustomerRequest.EmailAddress);
                customer.DateOfBirth = updateCustomerRequest.DateOfBirth;
                customer.Age = DateTime.Now.Year - DateTime.Parse(updateCustomerRequest.DateOfBirth).Year;
                customer.DateEdited = today.ToString();
                customer.IsDeleted = updateCustomerRequest.IsDeleted;


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
