namespace CustomerAPI.Models
{
    public class UpdateCustomerRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public string DateOfBirth { get; set; }
        public int Age { get; set; }

        public string DateCreated { get; set; }

        public string DateEdited { get; set; }

        public int IsDeleted { get; set; }

    }


}
