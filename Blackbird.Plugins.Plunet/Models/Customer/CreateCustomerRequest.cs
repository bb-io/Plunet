﻿namespace Blackbird.Plugins.Plunet.Models.Customer;

public class CreateCustomerRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Website { get; set; }
    public string Email { get; set; }
    public string HeadOfficePhone { get; set; }
    public string MobilePhone { get; set; }
    //public int FormOfAddress { get; set; }
    //public string CostCenter { get; set; }
}